// 常數設定
const INITIAL_DELAY = 3000; // 初始重試延遲（毫秒）
const MAX_DELAY = 5000; // 最大重試延遲（毫秒）
const MAX_RETRIES = 3; // 最大重試次數

class TruckMap {
  constructor() {
    try {
      // 檢查 Leaflet 是否可用
      if (typeof L === "undefined") {
        throw new Error("Leaflet 函式庫未載入");
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

      // 設定 API 基礎路徑
      this.apiBaseUrl = getApiBaseUrl();

      // 設定圖示路徑
      const iconPath = this.getIconPath();
      L.Icon.Default.imagePath = iconPath;

      // 明確設定每個圖示
      L.Icon.Default.mergeOptions({
        iconRetinaUrl: iconPath + "marker-icon-2x.png",
        iconUrl: iconPath + "marker-icon.png",
        shadowUrl: iconPath + "marker-shadow.png",
      });

      // 綁定事件處理器
      this.handleMapClick = this.handleMapClick.bind(this);
      this.handleResize = this.handleResize.bind(this);
      this.toggleSidebar = this.toggleSidebar.bind(this);
    } catch (error) {
      throw error;
    }
  }

  // 修改圖示路徑處理
  getIconPath() {
    try {
      const baseUrl = window.location.pathname.includes("WebSocketServer")
        ? "/WebSocketServer/lib/leaflet"
        : "/lib/leaflet";
      return baseUrl;
    } catch (error) {
      return "/lib/leaflet";
    }
  }

  // 修改 API 路徑處理
  async fetchTruckLocations() {
    try {
      const apiUrl = `${getApiBaseUrl()}/trucks`;
      const response = await fetch(apiUrl);

      if (!response.ok) {
        throw new Error(`HTTP 錯誤! 狀態: ${response.status}`);
      }

      const data = await response.json();
      if (!data.success) {
        throw new Error(data.message || "API 回應失敗");
      }

      return data.data;
    } catch (error) {
      throw error;
    }
  }

  async init() {
    try {
      // 等待 DOM 完全載入
      if (document.readyState !== "complete") {
        await new Promise((resolve) => {
          window.addEventListener("load", resolve);
        });
      }

      // 檢查地圖容器
      const mapContainer = document.getElementById("map");
      if (!mapContainer) {
        throw new Error("找不到地圖容器元素");
      }

      // 檢查容器尺寸
      const containerStyle = window.getComputedStyle(mapContainer);

      if (containerStyle.height === "0px" || containerStyle.width === "0px") {
        throw new Error("地圖容器尺寸為 0");
      }

      await this.initMap();

      this.setupEventListeners();

      await this.startPolling();
    } catch (error) {
      throw error;
    }
  }

  async initMap() {
    try {
      // 建立地圖實例
      this.map = L.map("map", this.mapConfig);

      // 加入圖層
      const tileLayer = L.tileLayer(
        "https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png",
        {
          attribution: "© OpenStreetMap contributors",
          maxZoom: 19,
        }
      );

      await new Promise((resolve, reject) => {
        tileLayer.on("load", resolve);
        tileLayer.on("error", reject);
        tileLayer.addTo(this.map);
      });

      // 設定地圖邊界（高雄市範圍）
      const bounds = L.latLngBounds(
        L.latLng(22.3, 120.1), // 西南角
        L.latLng(23.0, 120.5) // 東北角
      );
      this.map.setMaxBounds(bounds);

      // 確保地圖在邊界內
      this.map.on("drag", () => {
        this.map.panInsideBounds(bounds, { animate: false });
      });

      // 強制重新計算地圖大小
      this.map.invalidateSize();
    } catch (error) {
      throw new Error(`地圖初始化失敗: ${error.message}`);
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

  // 根據方向獲取圖標
  getMarkerIcon(direction, isSelected = false) {
    // 使用 getStaticPath 處理圖片路徑
    const iconUrl = getStaticPath("/truck.png");
    return L.divIcon({
      className: `truck-marker${isSelected ? " selected" : ""}`,
      html: `<img src="${iconUrl}" style="transform: rotate(${direction}deg);" width="32" height="32">`,
      iconSize: [32, 32],
      iconAnchor: [16, 16],
    });
  }

  // 更新卡車清單
  updateTruckList(trucks) {
    const truckList = document.getElementById("truck-list");
    truckList.innerHTML = "";

    trucks.forEach((truck) => {
      const truckItem = document.createElement("div");
      truckItem.className = `truck-item${
        truck.car === this.selectedTruckId ? " selected" : ""
      }`;
      truckItem.innerHTML = `
                <h3>${truck.car}</h3>
                <p>位置: ${truck.location}</p>
                <p>更新時間: ${truck.time}</p>
            `;

      truckItem.addEventListener("click", () => {
        this.selectedTruckId =
          this.selectedTruckId === truck.car ? null : truck.car;
        this.updateMarkers(trucks);
        this.updateTruckList(trucks);
      });

      truckList.appendChild(truckItem);
    });
  }

  // 更新標記
  updateMarkers(trucks) {
    const newMarkers = new Map();

    trucks.forEach((truck) => {
      const { car, location, time, longitude, latitude } = truck;
      const position = [latitude, longitude];
      const prevPosition = this.previousPositions.get(car);
      const direction = this.calculateDirection(
        prevPosition?.[0],
        prevPosition?.[1],
        latitude,
        longitude
      );

      let marker = this.markers.get(car);
      const isSelected = car === this.selectedTruckId;

      if (marker) {
        marker.setLatLng(position);
        marker.setIcon(this.getMarkerIcon(direction, isSelected));
      } else {
        marker = L.marker(position, {
          icon: this.getMarkerIcon(direction, isSelected),
        }).addTo(this.map);
      }

      marker.bindPopup(`
                <div class="popup-content">
                    <h3>${car}</h3>
                    <p>位置: ${location}</p>
                    <p>更新時間: ${time}</p>
                </div>
            `);

      if (isSelected) {
        this.map.setView(position, this.map.getZoom());
      }

      newMarkers.set(car, marker);
      this.previousPositions.set(car, position);
    });

    // 移除舊的標記
    this.markers.forEach((marker, id) => {
      if (!newMarkers.has(id)) {
        this.map.removeLayer(marker);
      }
    });

    this.markers = newMarkers;
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
    this.markers.forEach((marker) => {
      this.map.removeLayer(marker);
    });
    this.markers.clear();
    document.getElementById("truck-list").innerHTML = "";
  }

  // 開始輪詢
  async startPolling() {
    let retryCount = 0;
    let currentDelay = INITIAL_DELAY;
    this.isPolling = true;

    while (this.isPolling) {
      try {
        this.updateStatus("正在更新資料...");

        // 確保 API 路徑正確
        const apiUrl = `${this.apiBaseUrl}/trucks`;

        const response = await fetch(apiUrl, {
          method: "GET",
          headers: {
            Accept: "application/json",
          },
        });

        if (!response.ok) {
          throw new Error(
            `伺服器錯誤: ${response.status} ${response.statusText}`
          );
        }

        const result = await response.json();

        if (result?.data?.length > 0) {
          this.updateMarkers(result.data);
          this.updateTruckList(result.data);
          this.updateStatus(`已更新 ${result.data.length} 輛垃圾車位置`);
        } else {
          this.clearMarkers();
          this.updateStatus("目前沒有垃圾車資料");
        }

        retryCount = 0;
        currentDelay = INITIAL_DELAY;
      } catch (error) {
        retryCount++;
        if (retryCount > MAX_RETRIES) {
          this.updateStatus(
            `無法連線到伺服器，${Math.floor(MAX_DELAY / 1000)} 秒後重試...`,
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
}

// 確保全域變數可用
window.TruckMap = TruckMap;
