// 常數設定
const INITIAL_DELAY = 3000; // 初始重試延遲（毫秒）
const MAX_DELAY = 5000; // 最大重試延遲（毫秒）
const MAX_RETRIES = 3; // 最大重試次數

class TruckMap {
  constructor() {
    try {
      // 檢查 Leaflet 是否可用
      if (typeof L === "undefined") {
        console.error("等待 Leaflet 載入...");
        setTimeout(() => this.constructor(), 500);
        return;
      }

      // 初始化地圖設定
      this.mapConfig = {
        center: [22.6273, 120.3014],
        zoom: 13,
        minZoom: 10,
        maxZoom: 18,
        zoomControl: true,
        attributionControl: true,
      };

      // 初始化其他屬性
      this.map = null;
      this.markers = new Map();
      this.selectedTruckId = null;
      this.isPolling = false;
      this.previousPositions = new Map();

      // 修改 API 基礎路徑判斷邏輯
      const currentPath = window.location.pathname;
      const isWebSocketServer = window.location.hostname.includes("somee.com");

      // 設定 API 基礎路徑 - 根據是否在正式環境調整
      this.apiBaseUrl = isWebSocketServer
        ? "http://king.somee.com/WebSocketServer/api"
        : "/api";

      // 設定圖示路徑
      this.iconBasePath = isWebSocketServer
        ? "http://king.somee.com/WebSocketServer/images"
        : currentPath === "/"
        ? "/images"
        : currentPath + "images";

      console.log("當前主機:", window.location.hostname);
      console.log("當前路徑:", currentPath);
      console.log("API 基礎路徑:", this.apiBaseUrl);
      console.log("圖示基礎路徑:", this.iconBasePath);

      // 確保圖示路徑正確
      this.validateIconPath();

      // 建立八個方向的圖示
      this.directionIcons = {
        N: this.createTruckIcon("o01"), // 北
        NE: this.createTruckIcon("o02"), // 東北
        E: this.createTruckIcon("o03"), // 東
        SE: this.createTruckIcon("o04"), // 東南
        S: this.createTruckIcon("o05"), // 南
        SW: this.createTruckIcon("o06"), // 西南
        W: this.createTruckIcon("o07"), // 西
        NW: this.createTruckIcon("o08"), // 西北
      };

      // 初始化搜尋相關變數
      this.searchInput = document.getElementById("searchInput");
      this.allTrucksData = []; // 儲存所有車輛資料
      this.filteredTrucks = []; // 儲存過濾後的車輛資料

      // 綁定事件處理器
      this.handleMapClick = this.handleMapClick.bind(this);
      this.handleResize = this.handleResize.bind(this);
      this.toggleSidebar = this.toggleSidebar.bind(this);

      // 綁定搜尋事件
      if (this.searchInput) {
        this.searchInput.addEventListener("input", () => this.handleSearch());
      }

      // 綁定事件處理器到 window 物件
      window.handleTruckItemClick = (car) => this.handleTruckItemClick(car);

      // 立即初始化地圖
      this.initMap().catch((error) => {
        console.error("地圖初始化失敗:", error);
        this.updateStatus("地圖載入失敗，請重新整理頁面", true);
      });
    } catch (error) {
      console.error("初始化地圖時發生錯誤:", error);
      this.updateStatus("無法初始化地圖，請重新整理頁面", true);
    }
  }

  // 新增驗證圖示路徑的方法
  async validateIconPath() {
    try {
      const testImagePath = `${this.iconBasePath}/noGarbage_truck_o01.png`;
      const response = await fetch(testImagePath, { method: "HEAD" });
      if (!response.ok) {
        console.error(`無法載入圖示: ${testImagePath}`);
        // 如果路徑無效，嘗試使用備用路徑
        this.iconBasePath = "/images";
        console.log("使用備用圖示路徑:", this.iconBasePath);
      }
    } catch (error) {
      console.error("驗證圖示路徑時發生錯誤:", error);
      // 發生錯誤時使用備用路徑
      this.iconBasePath = "/images";
    }
  }

  // 建立垃圾車圖示
  createTruckIcon(direction) {
    return L.divIcon({
      className: `truck-icon direction-${direction}`,
      html: `<img src="${this.iconBasePath}/noGarbage_truck_${direction}.png" alt="垃圾車" />`,
      iconSize: [48, 48],
      iconAnchor: [24, 24],
      popupAnchor: [0, -24],
    });
  }

  // 取得方向圖示
  getMarkerIcon(direction = "N", isSelected = false) {
    const icon = this.directionIcons[direction] || this.directionIcons.N;
    if (isSelected) {
      const dirNum = this.getDirectionNumber(direction);
      return L.divIcon({
        className: `truck-icon direction-o${dirNum
          .toString()
          .padStart(2, "0")} selected`,
        html: `<img src="${this.iconBasePath}/noGarbage_truck_o${dirNum
          .toString()
          .padStart(2, "0")}.png" alt="垃圾車" />`,
        iconSize: [56, 56], // 選中時放大
        iconAnchor: [28, 28],
        popupAnchor: [0, -28],
      });
    }
    return icon;
  }

  // 取得方向對應的數字
  getDirectionNumber(direction) {
    const dirMap = {
      N: 1,
      NE: 2,
      E: 3,
      SE: 4,
      S: 5,
      SW: 6,
      W: 7,
      NW: 8,
    };
    return dirMap[direction] || 1;
  }

  // 修改 API 路徑處理
  async fetchTruckLocations() {
    let retryCount = 0;
    let currentDelay = INITIAL_DELAY;

    while (retryCount < MAX_RETRIES) {
      try {
        const response = await fetch(`${this.apiBaseUrl}/trucks`, {
          method: "GET",
          headers: {
            Accept: "application/json",
            "Cache-Control": "no-cache",
          },
          credentials: "omit",
          mode: "cors",
        });

        if (!response.ok) {
          const errorText = await response.text();
          console.error("API 回應錯誤:", {
            status: response.status,
            statusText: response.statusText,
            errorText: errorText,
          });

          this.updateStatus("資料更新中，請稍候...", true);

          // 如果是 500 錯誤，等待後重試
          if (response.status === 500) {
            await new Promise((resolve) => setTimeout(resolve, 5000));
            retryCount++;
            continue;
          }

          throw new Error(
            `伺服器錯誤 (${response.status}): ${response.statusText}`
          );
        }

        const result = await response.json();
        console.log("API 回應資料:", result);

        if (!result || !result.success) {
          if (result?.message?.includes("沒有垃圾車資料")) {
            return { success: true, message: "目前沒有垃圾車資料", data: [] };
          }
          throw new Error(result?.message || "無效的回應格式");
        }

        return result;
      } catch (error) {
        retryCount++;
        console.error("取得垃圾車資料時發生錯誤:", error);

        this.updateStatus("資料更新發生錯誤，正在重試...", true);

        await new Promise((resolve) => setTimeout(resolve, currentDelay));
        currentDelay = Math.min(currentDelay * 2, MAX_DELAY);
      }
    }

    this.updateStatus("資料更新失敗，請檢查網路連線", true);
    throw new Error("資料更新失敗");
  }

  async initMap() {
    try {
      // 檢查地圖容器
      const mapContainer = document.getElementById("map");
      if (!mapContainer) {
        throw new Error("找不到地圖容器元素");
      }

      // 確保容器可見
      mapContainer.style.height = "100%";
      mapContainer.style.width = "100%";

      // 建立地圖實例
      this.map = L.map("map", this.mapConfig);

      // 加入圖層
      L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
        attribution: " OpenStreetMap contributors",
        maxZoom: this.mapConfig.maxZoom,
      }).addTo(this.map);

      // 設定事件監聽
      this.setupEventListeners();

      // 開始輪詢資料
      await this.startPolling();

      console.log("地圖初始化完成");
      return true;
    } catch (error) {
      console.error("初始化地圖時發生錯誤:", error);
      throw error;
    }
  }

  // 設定事件監聽器
  setupEventListeners() {
    this.map.on("click", this.handleMapClick);
    window.addEventListener("resize", this.handleResize);
    document
      .getElementById("toggle-sidebar")
      .addEventListener("click", this.toggleSidebar);
    window.addEventListener("beforeunload", () => {
      this.isPolling = false;
    });
  }

  // 計算方向角度（0-360度）
  calculateDirection(prevLat, prevLng, currentLat, currentLng) {
    if (!prevLat || !prevLng) return 0;
    const dy = currentLat - prevLat;
    const dx = currentLng - prevLng;
    let angle = Math.atan2(dy, dx) * (180 / Math.PI);
    angle = (angle + 360) % 360;
    return angle;
  }

  // 更新標記
  updateMarkers(data) {
    try {
      const newMarkers = new Map();
      const currentTime = new Date();

      data.forEach(({ car, time, location, latitude, longitude }) => {
        const position = [latitude, longitude];
        const direction = this.calculateDirection(
          car,
          position,
          this.previousPositions.get(car)
        );

        // 檢查是否為已存在的標記
        let marker = this.markers.get(car);
        const isSelected = car === this.selectedTruckId;

        if (marker) {
          // 更新現有標記
          marker.setLatLng(position);
          marker.setIcon(this.getMarkerIcon(direction, isSelected));
          marker.setPopupContent(this.createPopupContent(car, time, location));
        } else {
          // 建立新標記
          marker = L.marker(position, {
            icon: this.getMarkerIcon(direction, isSelected),
          }).addTo(this.map);

          marker.bindPopup(this.createPopupContent(car, time, location));

          // 綁定點擊事件
          marker.on("click", () => this.handleMarkerClick(car));
        }

        // 更新標記集合
        newMarkers.set(car, marker);

        // 更新位置歷史
        this.previousPositions.set(car, position);
      });

      // 只移除不再出現的標記
      this.markers.forEach((marker, car) => {
        if (!newMarkers.has(car)) {
          marker.remove();
        }
      });

      // 更新標記集合
      this.markers = newMarkers;

      // 更新側邊欄
      this.updateTruckList(data);
    } catch (error) {
      console.error("更新標記時發生錯誤:", error);
    }
  }

  // 更新卡車清單
  updateTruckList(data) {
    try {
      const listContainer = document.getElementById("truckList");
      if (!listContainer) {
        console.error("找不到清單容器元素");
        return;
      }

      // 建立新的清單內容
      const listContent = data
        .sort((a, b) => a.car.localeCompare(b.car))
        .map(({ car, time, location }) => {
          const isSelected = car === this.selectedTruckId;
          return `
            <div class="truck-item${isSelected ? " selected" : ""}" 
                 data-car="${car}"
                 onclick="handleTruckItemClick('${car}')">
                <div class="truck-info">
                    <span class="truck-id">${car}</span>
                    <span class="truck-time">${time}</span>
                </div>
                <div class="truck-location">${location}</div>
            </div>
          `;
        })
        .join("");

      // 更新清單內容
      listContainer.innerHTML = listContent;

      // 如果有選中的項目，確保它可見
      if (this.selectedTruckId) {
        const selectedElement = listContainer.querySelector(
          `[data-car="${this.selectedTruckId}"]`
        );
        if (selectedElement) {
          selectedElement.scrollIntoView({
            behavior: "smooth",
            block: "nearest",
          });
        }
      }
    } catch (error) {
      console.error("更新卡車清單時發生錯誤:", error);
    }
  }

  // 切換側邊欄顯示/隱藏
  toggleSidebar() {
    const sidebar = document.getElementById("sidebar");
    const isHidden = sidebar.style.transform === "translateX(0px)";
    sidebar.style.transform = isHidden
      ? "translateX(300px)"
      : "translateX(0px)";
  }

  // 點擊地圖時自動隱藏側邊欄（僅在移動設備上）
  handleMapClick() {
    if (window.innerWidth <= 768) {
      const sidebar = document.getElementById("sidebar");
      sidebar.style.transform = "translateX(300px)";
    }
  }

  // 處理視窗大小變化
  handleResize() {
    this.map.invalidateSize();
  }

  // 清除所有標記
  clearMarkers() {
    this.markers.forEach((marker) => marker.remove());
    this.markers.clear();
    this.previousPositions.clear();
    this.allTrucksData = [];
    this.filteredTrucks = [];
    const listContainer = document.getElementById("truckList");
    if (listContainer) {
      listContainer.innerHTML = "";
    }
  }

  // 開始輪詢
  async startPolling() {
    let retryCount = 0;
    let currentDelay = INITIAL_DELAY;
    this.isPolling = true;

    while (this.isPolling) {
      try {
        this.updateStatus("正在更新資料...");
        const result = await this.fetchTruckLocations();

        if (result?.data?.length > 0) {
          // 更新所有車輛資料
          this.allTrucksData = result.data;

          // 重新執行搜尋過濾
          this.handleSearch();

          this.updateStatus(`已更新 ${result.data.length} 輛垃圾車位置`);

          // 重設重試計數器
          retryCount = 0;
          currentDelay = INITIAL_DELAY;
        } else {
          this.clearMarkers();
          this.updateStatus("目前沒有垃圾車資料");
        }
      } catch (error) {
        retryCount++;
        const isNetworkError =
          error instanceof TypeError || error.name === "TypeError";

        if (retryCount > MAX_RETRIES) {
          this.updateStatus(
            isNetworkError
              ? "網路連線失敗，請檢查網路設定"
              : `無法連線到伺服器，${Math.floor(MAX_DELAY / 1000)} 秒後重試...`,
            true
          );
          currentDelay = MAX_DELAY;
        } else {
          const nextRetrySeconds = Math.floor(currentDelay / 1000);
          this.updateStatus(
            `更新失敗: ${error.message}，${nextRetrySeconds} 秒後重試...`,
            true
          );
          currentDelay = Math.min(currentDelay * 2, MAX_DELAY);
        }
      }

      await new Promise((resolve) => setTimeout(resolve, currentDelay));
    }
  }

  // 更新狀態顯示
  updateStatus(message, isError = false) {
    const statusElement = document.getElementById("status");
    const statusTextElement = statusElement.querySelector(".status-text");
    const loadingElement = statusElement.querySelector(".loading");

    statusElement.className = `status${isError ? " status-error" : ""}`;
    loadingElement.style.display = isError ? "none" : "inline-block";
    statusTextElement.textContent = message;
  }

  // 建立彈出視窗內容
  createPopupContent(car, time, location) {
    return `
      <div class="popup-content">
        <h3>${car}</h3>
        <p>位置：${location}</p>
        <p>更新時間：${time}</p>
      </div>
    `;
  }

  // 處理標記點擊事件
  handleMarkerClick(car) {
    this.selectedTruckId = this.selectedTruckId === car ? null : car;

    // 更新所有標記的圖示
    this.markers.forEach((marker, id) => {
      marker.setIcon(
        this.getMarkerIcon(
          this.calculateDirection(
            id,
            marker.getLatLng(),
            this.previousPositions.get(id)
          ),
          id === this.selectedTruckId
        )
      );
    });

    // 如果有選中的車輛，將地圖中心移動到該位置
    if (this.selectedTruckId) {
      const marker = this.markers.get(this.selectedTruckId);
      if (marker) {
        this.map.setView(marker.getLatLng(), this.map.getZoom());
      }
    }
  }

  // 處理清單項目點擊事件
  handleTruckItemClick(car) {
    // 更新選中狀態
    this.selectedTruckId = this.selectedTruckId === car ? null : car;

    // 更新所有標記的圖示
    this.markers.forEach((marker, id) => {
      marker.setIcon(
        this.getMarkerIcon(
          this.calculateDirection(
            id,
            marker.getLatLng(),
            this.previousPositions.get(id)
          ),
          id === this.selectedTruckId
        )
      );

      // 更新彈出視窗內容
      if (id === this.selectedTruckId) {
        const truckData = this.allTrucksData.find((t) => t.car === id);
        if (truckData) {
          marker.openPopup();
          marker.setPopupContent(`
                    <div class="popup-content">
                        <h3>垃圾車 ${truckData.car}</h3>
                        <p>位置：${truckData.location}</p>
                        <p>更新時間：${truckData.time}</p>
                    </div>
                `);
        }
      } else {
        marker.closePopup();
      }
    });

    // 更新清單項目的樣式
    const listContainer = document.getElementById("truckList");
    if (listContainer) {
      const items = listContainer.getElementsByClassName("truck-item");
      Array.from(items).forEach((item) => {
        const itemCar = item.getAttribute("data-car");
        if (itemCar === this.selectedTruckId) {
          item.classList.add("selected");
          // 滾動到選中的項目
          item.scrollIntoView({ behavior: "smooth", block: "nearest" });
        } else {
          item.classList.remove("selected");
        }
      });
    }

    // 如果有選中的車輛，將地圖中心移動到該位置
    if (this.selectedTruckId) {
      const marker = this.markers.get(this.selectedTruckId);
      if (marker) {
        this.map.setView(marker.getLatLng(), this.map.getZoom());
      }
    }
  }

  // 處理搜尋
  handleSearch() {
    const searchText = (this.searchInput?.value || "").trim().toLowerCase();

    // 如果搜尋欄為空，顯示所有車輛
    if (!searchText) {
      this.filteredTrucks = [...this.allTrucksData];
    } else {
      // 否則進行過濾
      this.filteredTrucks = this.allTrucksData.filter(
        (truck) =>
          truck.car.toLowerCase().includes(searchText) ||
          truck.location.toLowerCase().includes(searchText)
      );
    }

    // 更新地圖和清單
    this.updateMarkers(this.filteredTrucks);
    this.updateTruckList(this.filteredTrucks);
  }
}

// 確保全域變數可用
window.TruckMap = TruckMap;
