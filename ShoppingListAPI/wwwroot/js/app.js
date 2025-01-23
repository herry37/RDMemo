// 工具函數
function showLoading() {
  const loadingIndicator = document.getElementById("loadingIndicator");
  if (loadingIndicator) {
    loadingIndicator.classList.remove("hidden");
  }
}

function hideLoading() {
  const loadingIndicator = document.getElementById("loadingIndicator");
  if (loadingIndicator) {
    loadingIndicator.classList.add("hidden");
  }
}

function showError(message) {
  const existingError = document.querySelector(".error-message");
  if (existingError) {
    existingError.remove();
  }

  const errorDiv = document.createElement("div");
  errorDiv.className = "error-message";
  errorDiv.textContent = message;
  document.body.appendChild(errorDiv);

  setTimeout(() => {
    errorDiv.remove();
  }, 3000);
}

function showSuccess(message) {
  const existingSuccess = document.querySelector(".success-message");
  if (existingSuccess) {
    existingSuccess.remove();
  }

  const successDiv = document.createElement("div");
  successDiv.className = "success-message";
  successDiv.textContent = message;
  document.body.appendChild(successDiv);

  setTimeout(() => {
    successDiv.remove();
  }, 3000);
}

// 輔助函數：字符串轉 ArrayBuffer
function str2ab(str) {
  const buf = new ArrayBuffer(str.length);
  const bufView = new Uint8Array(buf);
  for (let i = 0, strLen = str.length; i < strLen; i++) {
    bufView[i] = str.charCodeAt(i);
  }
  return buf;
}

// WebSocket 連接函數
async function getWebSocket(url) {
  return new Promise((resolve, reject) => {
    try {
      console.log("開始建立 WebSocket 連接到:", url);
      const webSocket = new WebSocket(url);

      webSocket.onopen = () => {
        console.log("WebSocket 連接成功");
        resolve(webSocket);
      };

      webSocket.onclose = (event) => {
        const reason = event.reason || "未知原因";
        console.log(`WebSocket 連接關閉: ${event.code} - ${reason}`);
        reject(new Error(`WebSocket 連接關閉: ${event.code} - ${reason}`));
      };

      webSocket.onerror = (error) => {
        console.error("WebSocket 連接錯誤:", error);
        reject(new Error("WebSocket 連接失敗"));
      };
    } catch (error) {
      reject(error);
    }
  });
}

// 初始化 WebSocket 連接
async function initializeWebSocket() {
  const wsUrl = "ws://localhost:5002/ws";
  console.log("嘗試建立 WebSocket 連接到:", wsUrl);
  return await getWebSocket(wsUrl);
}

// 連接重試函數
async function connectWithRetry(maxRetries = 3, retryDelay = 1000) {
  let retryCount = 0;

  while (retryCount < maxRetries) {
    try {
      console.log(`嘗試建立 WebSocket 連接 (第 ${retryCount + 1}/${maxRetries} 次)`);
      const webSocket = await initializeWebSocket();
      console.log("WebSocket 連接成功");
      return webSocket;
    } catch (error) {
      retryCount++;
      console.log(`連接失敗，${maxRetries - retryCount} 次重試機會剩餘`);
      if (retryCount === maxRetries) {
        throw error;
      }
      await new Promise(resolve => setTimeout(resolve, retryDelay));
    }
  }
}

// 初始化應用程式物件
const app = {
  lists: [],
  webSocket: null,
  isConnecting: false,
  reconnectTimeout: null,
  currentPage: 1,
  pageSize: 10,
  totalItems: 0,
  currentListId: null,
  currentItems: [],

  async init() {
    try {
      showLoading();
      await this.loadShoppingLists();
      await this.connect();
      this.setupEventListeners();
      hideLoading();
    } catch (error) {
      console.error("初始化應用程式時發生錯誤:", error);
      showError("初始化應用程式時發生錯誤");
      hideLoading();
    }
  },

  setupEventListeners() {
    // 設置事件監聽器
    document.getElementById("searchForm")?.addEventListener("submit", (e) => {
      e.preventDefault();
      this.searchLists();
    });
  },

  async loadShoppingLists() {
    try {
      console.log("開始載入購物清單...");
      const response = await fetch("/api/shoppinglist", {
        headers: {
          Accept: "application/json",
        },
      });

      console.log("API 回應狀態:", response.status);

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data = await response.json();
      console.log("收到的資料:", data);

      this.lists = data;
      this.renderLists(data);
      console.log("完成載入購物清單，數量:", data.length);
    } catch (error) {
      console.error("載入購物清單時發生錯誤:", error);
    }
  },

  // 查詢購物清單
  async searchLists() {
    try {
      const startDate = document.getElementById("searchStartDate").value;
      const endDate = document.getElementById("searchEndDate").value;
      const searchTitle = document.getElementById("searchTitle").value;

      // 檢查日期格式
      const checkDateFormat = (dateStr) => {
        if (!dateStr) return true;

        // 嚴格檢查日期格式必須為 YYYY-MM-DD
        if (!/^\d{4}-\d{2}-\d{2}$/.test(dateStr)) {
          return false;
        }

        const [yearStr, monthStr, dayStr] = dateStr.split('-');
        const year = parseInt(yearStr);
        const month = parseInt(monthStr);
        const day = parseInt(dayStr);

        // 檢查年份必須是四位數且在合理範圍內
        if (yearStr.length !== 4 || year < 1900 || year > 9999) {
          return false;
        }

        // 檢查月份必須是兩位數且在合理範圍內
        if (monthStr.length !== 2 || month < 1 || month > 12) {
          return false;
        }

        // 檢查日期必須是兩位數且在合理範圍內
        const lastDayOfMonth = new Date(year, month, 0).getDate();
        if (dayStr.length !== 2 || day < 1 || day > lastDayOfMonth) {
          return false;
        }

        return true;
      };

      if (!checkDateFormat(startDate)) {
        alert('開始日期格式必須為 YYYY-MM-DD（年份四位數、月份和日期兩位數），且日期必須有效');
        return;
      }

      if (!checkDateFormat(endDate)) {
        alert('結束日期格式必須為 YYYY-MM-DD（年份四位數、月份和日期兩位數），且日期必須有效');
        return;
      }

      // 日期範圍檢查
      if (startDate && endDate) {
        const start = new Date(startDate);
        const end = new Date(endDate);

        if (start > end) {
          alert('開始日期不能大於結束日期');
          return;
        }

        // 檢查日期範圍是否超過一年
        const oneYear = 365 * 24 * 60 * 60 * 1000; // 一年的毫秒數
        if (end - start > oneYear) {
          alert('查詢日期範圍不能超過一年');
          return;
        }
      }

      // 至少要有一個搜尋條件
      if (!startDate && !endDate && !searchTitle) {
        alert('請至少輸入一個搜尋條件');
        return;
      }

      // 構建查詢參數
      const params = new URLSearchParams();
      if (startDate) params.append('startDate', startDate);
      if (endDate) params.append('endDate', endDate);
      if (searchTitle) params.append('title', searchTitle);

      const response = await fetch(`/api/ShoppingList/search?${params.toString()}`);
      if (!response.ok) {
        throw new Error("搜尋失敗");
      }

      const lists = await response.json();
      this.renderLists(lists);
    } catch (error) {
      console.error("搜尋時發生錯誤:", error);
      showError("搜尋失敗，請稍後再試");
    }
  },

  // 渲染購物清單
  renderLists(lists) {
    const tbody = document.getElementById("listTableBody");
    if (!tbody) {
      console.error("listTableBody element not found");
      return;
    }
    tbody.innerHTML = "";

    if (lists.length === 0) {
      const row = document.createElement("tr");
      row.innerHTML =
        '<td colspan="4" style="text-align: center;">沒有找到符合條件的購物清單</td>';
      tbody.appendChild(row);
      return;
    }

    lists.forEach((list) => {
      const row = document.createElement("tr");
      row.innerHTML = `
                <td>${list.buyDate ? list.buyDate.split("T")[0] : ""}</td>
                <td>${list.title}</td>
                <td>${this.formatCurrency(
                  this.calculateTotalAmount(list.items)
                )}</td>
                <td>
                    <button class="btn btn-primary" onclick="app.showDetail('${
                      list.id
                    }')">明細</button>
                    <button class="btn btn-danger" onclick="app.deleteList('${
                      list.id
                    }')">刪除</button>
                </td>
            `;
      tbody.appendChild(row);
    });
  },

  // 計算總金額
  calculateTotalAmount(items) {
    return items
      ? items.reduce((total, item) => total + item.quantity * item.price, 0)
      : 0;
  },

  // 格式化金額
  formatCurrency(amount) {
    return new Intl.NumberFormat("zh-TW", {
      style: "currency",
      currency: "TWD",
      minimumFractionDigits: 0,
      maximumFractionDigits: 0,
    }).format(amount);
  },

  // 格式化日期
  formatDate(dateString) {
    const date = new Date(dateString);
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const day = String(date.getDate()).padStart(2, '0');
    return `${year}-${month}-${day}`;
  },

  // 顯示明細 Modal
  async showDetail(listId) {
    try {
      console.log("顯示明細，ID:", listId);
      const response = await fetch(`/api/shoppinglist/${listId}`);
      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }
      const list = await response.json();
      console.log("取得清單資料:", list);

      const buyDate = list.buyDate
        ? new Date(list.buyDate).toLocaleDateString("zh-TW")
        : "";
      document.getElementById("modalBuyDate").textContent = buyDate;
      document.getElementById("modalTitle").textContent = list.title || "";

      this.currentListId = listId;
      this.currentItems = list.items || [];
      this.renderItems();

      const modal = document.getElementById("detailModal");
      if (modal) {
        modal.style.display = "block";
      } else {
        console.error("找不到 detailModal 元素");
      }
    } catch (error) {
      console.error("Error fetching list details:", error);
      alert("獲取清單詳情時發生錯誤");
    }
  },

  // 渲染商品項目
  renderItems() {
    const tbody = document.getElementById("itemTableBody");
    if (!tbody) {
      console.error("itemTableBody element not found");
      return;
    }
    tbody.innerHTML = "";

    if (!Array.isArray(this.currentItems)) {
      console.error("currentItems 不是陣列:", this.currentItems);
      return;
    }

    this.currentItems.forEach((item, index) => {
      const row = document.createElement("tr");
      row.innerHTML = `
                <td>
                    <input type="checkbox" ${item.isCompleted ? "checked" : ""} 
                           onchange="app.toggleItemComplete(${index})">
                </td>
                <td>
                    <input type="text" value="${item.name || ""}" 
                           onchange="app.updateItemField(${index}, 'name', this.value)">
                </td>
                <td>
                    <input type="number" value="${item.quantity || 1}" min="1" 
                           onchange="app.updateItemField(${index}, 'quantity', this.value)">
                </td>
                <td>
                    <input type="number" value="${item.price || 0}" min="0" 
                           onchange="app.updateItemField(${index}, 'price', this.value)">
                </td>
                <td>${this.formatCurrency(
                  (item.quantity || 1) * (item.price || 0)
                )}</td>
                <td>
                    <div class="btn-group">
                        <button class="btn btn-danger" onclick="app.deleteItem(${index})">刪除</button>
                    </div>
                </td>
            `;
      tbody.appendChild(row);
    });

    this.updateTotalAmount();
  },

  // 更新總金額
  updateTotalAmount() {
    const total = Array.isArray(this.currentItems)
      ? this.currentItems.reduce(
          (sum, item) => sum + (item.quantity || 1) * (item.price || 0),
          0
        )
      : 0;
    const totalElement = document.getElementById("totalAmount");
    if (totalElement) {
      totalElement.textContent = this.formatCurrency(total);
    }
  },

  // 切換項目完成狀態
  toggleItemComplete(index) {
    if (this.currentItems[index]) {
      this.currentItems[index].isCompleted =
        !this.currentItems[index].isCompleted;
      this.renderItems();
    }
  },

  // 更新項目欄位
  updateItemField(index, field, value) {
    if (this.currentItems[index]) {
      if (field === "quantity" || field === "price") {
        value = parseFloat(value) || 0;
      }
      this.currentItems[index][field] = value;
      this.renderItems();
    }
  },

  // 新增項目
  addNewItem() {
    this.currentItems.push({
      name: "",
      quantity: 1,
      price: 0,
      isCompleted: false,
    });
    this.renderItems();
  },

  // 刪除項目
  deleteItem(index) {
    if (confirm("確定要刪除此項目嗎？")) {
      this.currentItems.splice(index, 1);
      this.renderItems();
    }
  },

  // 儲存變更
  async saveChanges() {
    try {
      const response = await fetch(`/api/shoppinglist/${this.currentListId}`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          id: this.currentListId,
          items: this.currentItems,
        }),
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      alert("儲存成功");
      this.hideDetailModal();
      await this.loadShoppingLists();
    } catch (error) {
      console.error("Error saving changes:", error);
      alert("儲存變更時發生錯誤");
    }
  },

  // 刪除清單
  async deleteList(listId) {
    if (!confirm("確定要刪除此購物清單嗎？")) {
      return;
    }

    try {
      const response = await fetch(`/api/shoppinglist/${listId}`, {
        method: "DELETE",
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      await this.loadShoppingLists();
    } catch (error) {
      console.error("Error deleting list:", error);
      alert("刪除清單時發生錯誤");
    }
  },

  // 顯示新增清單 Modal
  showAddListModal() {
    const modal = document.getElementById('addListModal');
    const titleInput = document.getElementById('newListTitle');
    const dateInput = document.getElementById('newListDate');
    
    // 設置預設日期為今天，確保格式為 YYYY-MM-DD
    const today = new Date();
    const year = today.getFullYear();
    const month = String(today.getMonth() + 1).padStart(2, '0');
    const day = String(today.getDate()).padStart(2, '0');
    dateInput.value = `${year}-${month}-${day}`;
    
    titleInput.value = '';
    modal.style.display = 'block';
  },

  // 隱藏新增清單 Modal
  hideAddListModal() {
    document.getElementById("addListModal").style.display = "none";
  },

  // 隱藏明細 Modal
  hideDetailModal() {
    document.getElementById("detailModal").style.display = "none";
    this.currentListId = null;
    this.currentItems = [];
  },

  // 建立新清單
  async createNewList() {
    const title = document.getElementById('newListTitle').value;
    const buyDate = document.getElementById('newListDate').value;

    // 檢查日期格式
    const checkDateFormat = (dateStr) => {
      if (!dateStr) return true;
      
      // 嚴格檢查日期格式必須為 YYYY-MM-DD
      if (!/^\d{4}-\d{2}-\d{2}$/.test(dateStr)) {
        return false;
      }

      const [yearStr, monthStr, dayStr] = dateStr.split('-');
      const year = parseInt(yearStr);
      const month = parseInt(monthStr);
      const day = parseInt(dayStr);

      // 檢查年份必須是四位數且在合理範圍內
      if (yearStr.length !== 4 || year < 1900 || year > 9999) {
        return false;
      }

      // 檢查月份必須是兩位數且在合理範圍內
      if (monthStr.length !== 2 || month < 1 || month > 12) {
        return false;
      }

      // 檢查日期必須是兩位數且在合理範圍內
      const lastDayOfMonth = new Date(year, month, 0).getDate();
      if (dayStr.length !== 2 || day < 1 || day > lastDayOfMonth) {
        return false;
      }

      return true;
    };

    // 輸入檢查
    if (!title.trim()) {
      alert('請輸入標題');
      return;
    }
    if (!buyDate) {
      alert('請選擇購買日期');
      return;
    }
    if (!checkDateFormat(buyDate)) {
      alert('購買日期格式必須為 YYYY-MM-DD（年份四位數、月份和日期兩位數），且日期必須有效');
      return;
    }

    try {
      const response = await fetch("/api/shoppinglist", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
        },
        body: JSON.stringify({
          title,
          buyDate,
          items: [],
        }),
      });

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      this.hideAddListModal();
      await this.loadShoppingLists();
    } catch (error) {
      console.error("Error creating list:", error);
      alert("創建清單時發生錯誤");
    }
  },

  async connect() {
    if (this.isConnecting || this.webSocket?.readyState === WebSocket.OPEN) {
      console.log("WebSocket 已連接或正在連接中");
      return;
    }

    this.isConnecting = true;
    console.log("開始建立 WebSocket 連接...");

    try {
      this.webSocket = await connectWithRetry();

      // 設置消息處理器
      this.webSocket.onmessage = (event) => {
        try {
          const message = JSON.parse(event.data);
          console.log("收到 WebSocket 消息:", message);

          switch (message.type) {
            case "welcome":
              console.log("連接成功:", message.data);
              break;
            case "shoppinglist_update":
              console.log("購物清單更新");
              this.loadShoppingLists();
              break;
            case "shoppinglist_delete":
              console.log("購物清單刪除:", message.data);
              if (this.currentListId === message.data) {
                this.currentListId = null;
                this.currentItems = [];
              }
              this.loadShoppingLists();
              break;
            default:
              console.log("未知消息類型:", message);
          }
        } catch (error) {
          console.error("處理 WebSocket 消息時發生錯誤:", error);
        }
      };

      // 設置關閉處理器
      this.webSocket.onclose = (event) => {
        const reason = event.reason || "未知原因";
        console.log(`WebSocket 連接已關閉: ${event.code} - ${reason}`);
        this.webSocket = null;
        this.isConnecting = false;
        this.scheduleReconnect();
      };

      // 設置錯誤處理器
      this.webSocket.onerror = (error) => {
        console.error("WebSocket 錯誤:", error);
        this.isConnecting = false;
        this.scheduleReconnect();
      };

      this.isConnecting = false;
      console.log("WebSocket 連接成功建立");
    } catch (error) {
      console.error("建立 WebSocket 連接失敗:", error);
      this.isConnecting = false;
      this.scheduleReconnect();
    }
  },

  scheduleReconnect() {
    if (!this.reconnectTimeout) {
      console.log("安排重新連接...");
      this.reconnectTimeout = setTimeout(() => {
        this.reconnectTimeout = null;
        this.connect();
      }, 3000);
    }
  },

  handleWebSocketMessage(data) {
    console.log("處理 WebSocket 訊息:", data);
    if (data.type === "shoppingListUpdated") {
      this.loadShoppingLists();
    }
  },
};

// 將 app 物件暴露給全域
window.app = app;

// 頁面載入完成後初始化應用程式
document.addEventListener("DOMContentLoaded", () => {
  app.init();
});
