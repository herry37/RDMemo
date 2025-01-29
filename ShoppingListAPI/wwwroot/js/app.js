'use strict';

// 顯示錯誤訊息
function showError(message) {
  const errorDialog = document.getElementById('errorDialog');
  const errorMessage = document.getElementById('errorMessage');
  
  if (errorDialog && errorMessage) {
    errorMessage.textContent = message;
    errorDialog.classList.add('show');
    
    // 點擊背景時關閉對話框
    const clickHandler = (e) => {
      if (e.target === errorDialog) {
        hideError();
      }
    };
    errorDialog.addEventListener('click', clickHandler);

    // 按下 ESC 鍵時關閉對話框
    const keyHandler = (e) => {
      if (e.key === 'Escape') {
        hideError();
      }
    };
    document.addEventListener('keydown', keyHandler);
  }
}

function hideError() {
  const errorDialog = document.getElementById('errorDialog');
  if (errorDialog) {
    errorDialog.classList.remove('show');
    // 移除所有事件監聽器
    errorDialog.replaceWith(errorDialog.cloneNode(true));
  }
}

// 顯示成功訊息
function showSuccess(message) {
  const successDialog = document.getElementById('successDialog');
  const successMessage = document.getElementById('successMessage');
  
  if (successDialog && successMessage) {
    successMessage.textContent = message;
    successDialog.classList.add('show');
    
    // 點擊背景時關閉對話框
    const clickHandler = (e) => {
      if (e.target === successDialog) {
        hideSuccess();
      }
    };
    successDialog.addEventListener('click', clickHandler);

    // 按下 ESC 鍵時關閉對話框
    const keyHandler = (e) => {
      if (e.key === 'Escape') {
        hideSuccess();
      }
    };
    document.addEventListener('keydown', keyHandler);
  }
}

function hideSuccess() {
  const successDialog = document.getElementById('successDialog');
  if (successDialog) {
    successDialog.classList.remove('show');
    // 移除所有事件監聽器
    successDialog.replaceWith(successDialog.cloneNode(true));
  }
}

// 應用程式主要物件
const app = {
  currentList: null,
  detailModal: null,
  addListModal: null,
  errorModal: null,
  successModal: null,
  isSaving: false, // 新增儲存鎖

  // 初始化應用程式
  init() {
    try {
      console.log('初始化應用程式...');
      
      // 初始化 Bootstrap Modal
      const detailModalElement = document.getElementById('detailModal');
      const addListModalElement = document.getElementById('addListModal');
      const errorModalElement = document.getElementById('errorModal');
      const successModalElement = document.getElementById('successModal');

      if (detailModalElement) {
        this.detailModal = new bootstrap.Modal(detailModalElement);
        
        // 監聽 Modal 關閉事件
        detailModalElement.addEventListener('hidden.bs.modal', () => {
          this.isSaving = false;
          this.currentList = null;
          this.clearErrors();
        });
      }

      if (addListModalElement) {
        this.addListModal = new bootstrap.Modal(addListModalElement);
        
        // 監聽新增清單 Modal 關閉事件
        addListModalElement.addEventListener('hidden.bs.modal', () => {
          this.isSaving = false;
          this.currentList = null;
          this.clearErrors();
        });
      }

      if (errorModalElement) {
        this.errorModal = new bootstrap.Modal(errorModalElement);
      }

      if (successModalElement) {
        this.successModal = new bootstrap.Modal(successModalElement);
      }

      // 綁定搜尋表單事件
      const searchForm = document.getElementById('searchForm');
      if (searchForm) {
        searchForm.addEventListener('submit', (e) => {
          e.preventDefault();
          this.searchLists();
        });
      }

      // 載入清單
      this.loadLists();

    } catch (error) {
      console.error('初始化失敗:', error);
      this.showError('初始化失敗，請重新整理頁面');
    }
  },

  // 實際的初始化邏輯
  initialize() {
    try {
      console.log('初始化應用程式...');
      
      // 初始化所有 Modal
      this.detailModal = new bootstrap.Modal(document.getElementById('detailModal'));
      this.addListModal = new bootstrap.Modal(document.getElementById('addListModal'));
      this.errorModal = new bootstrap.Modal(document.getElementById('errorModal'));
      this.successModal = new bootstrap.Modal(document.getElementById('successModal'));
      
      // 綁定事件
      this.bindEvents();
      
      // 載入清單
      this.loadLists();
      
      console.log('初始化完成');
    } catch (error) {
      console.error('初始化失敗:', error);
      this.showError(error.message);
    }
  },

  // 綁定事件
  bindEvents() {
    try {
      console.log('綁定事件...');
      
      // 綁定儲存按鈕事件
      const saveButton = document.getElementById('saveButton');
      if (saveButton) {
        saveButton.addEventListener('click', () => this.saveChanges());
      } else {
        console.error('找不到儲存按鈕');
      }

      // 綁定搜尋表單事件
      const searchForm = document.getElementById('searchForm');
      if (searchForm) {
        searchForm.addEventListener('submit', (e) => {
          e.preventDefault();
          this.searchLists();
        });
      } else {
        console.error('找不到搜尋表單');
      }

      // 綁定新增按鈕事件
      const addButton = document.getElementById('addButton');
      if (addButton) {
        addButton.addEventListener('click', () => {
          console.log('新增按鈕被點擊');
          this.showAddListModal();
        });
      } else {
        console.error('找不到新增按鈕');
      }

      // 綁定新增清單表單的儲存按鈕
      const createButton = document.getElementById('createButton');
      if (createButton) {
        createButton.addEventListener('click', () => {
          console.log('儲存新清單按鈕被點擊');
          this.createList();
        });
      } else {
        console.error('找不到新增清單儲存按鈕');
      }

      console.log('事件綁定完成');
    } catch (error) {
      console.error('綁定事件失敗:', error);
      this.showError(error.message);
    }
  },

  // 設定預設日期
  setDefaultDates() {
    console.log('設定預設日期...');
    const today = new Date().toISOString().split('T')[0];
    const startDateInput = document.getElementById('startDate');
    const endDateInput = document.getElementById('endDate');
    
    if (startDateInput) {
      startDateInput.value = today;
      console.log('起始日期設定成功');
    } else {
      console.error('找不到起始日期輸入框');
    }

    if (endDateInput) {
      endDateInput.value = today;
      console.log('結束日期設定成功');
    } else {
      console.error('找不到結束日期輸入框');
    }
  },

  // 載入所有清單
  async loadAllLists() {
    try {
      console.log('載入所有清單...');
      const response = await fetch('/api/shoppinglist');
      
      if (!response.ok) {
        throw new Error(`無法取得購物清單: ${response.status}`);
      }

      const result = await response.json();
      console.log('API 回應:', result);

      // 檢查 API 回應格式
      let lists = [];
      if (result.success && Array.isArray(result.data)) {
        lists = result.data;
      } else if (Array.isArray(result)) {
        lists = result;
      }

      if (lists.length === 0) {
        console.log('沒有購物清單資料');
      } else {
        console.log('取得購物清單數量:', lists.length);
      }

      this.updateListTable(lists);
    } catch (error) {
      console.error('載入清單失敗:', error);
      this.showError(error.message);
    }
  },

  // 載入清單資料
  async loadLists() {
    try {
      console.log('載入清單資料...');
      const startDateInput = document.getElementById('startDate');
      const endDateInput = document.getElementById('endDate');
      const titleInput = document.getElementById('searchTitle');

      // 取得查詢條件
      const startDate = startDateInput?.value || '';
      const endDate = endDateInput?.value || '';
      const title = titleInput?.value.trim() || '';

      // 構建查詢參數
      const params = new URLSearchParams();
      if (startDate) params.append('startDate', startDate);
      if (endDate) params.append('endDate', endDate);
      if (title) params.append('title', title);

      // 組合 URL
      const url = params.toString()
        ? `/api/shoppinglist/search?${params.toString()}`
        : '/api/shoppinglist';

      console.log('請求 URL:', url);
      const response = await fetch(url);
      const result = await response.json();
      console.log('載入清單結果:', result);

      if (!response.ok) {
        throw new Error(result.message || '載入清單失敗');
      }

      // 檢查 API 回傳格式
      let lists = [];
      if (result.success && Array.isArray(result.data)) {
        lists = result.data;
      }

      // 更新清單表格
      this.updateListTable(lists);
    } catch (error) {
      console.error('載入清單失敗:', error);
      this.showError(error.message || '載入清單失敗，請稍後再試');
      this.updateListTable([]);
    }
  },

  // 搜尋清單
  async searchLists() {
    try {
      const startDateInput = document.getElementById('startDate');
      const endDateInput = document.getElementById('endDate');
      const titleInput = document.getElementById('searchTitle');

      if (!startDateInput || !endDateInput) {
        throw new Error('找不到日期輸入欄位');
      }

      const startDate = startDateInput.value;
      const endDate = endDateInput.value;
      const title = titleInput ? titleInput.value.trim() : '';

      // 檢查日期範圍
      if (startDate && endDate) {
        const startDateTime = new Date(startDate);
        const endDateTime = new Date(endDate);
        
        if (startDateTime > endDateTime) {
          throw new Error('開始日期不能大於結束日期');
        }
      }

      console.log('搜尋條件:', { startDate, endDate, title });

      // 構建查詢參數
      const params = new URLSearchParams();
      if (startDate) params.append('startDate', startDate);
      if (endDate) params.append('endDate', endDate);
      if (title) params.append('title', title);

      const url = params.toString()
        ? `/api/shoppinglist/search?${params.toString()}`
        : '/api/shoppinglist';

      const response = await fetch(url);
      if (!response.ok) {
        const result = await response.json();
        throw new Error(result.message || '搜尋失敗');
      }

      const result = await response.json();
      
      // 更新清單表格
      this.updateListTable(result.data || []);

    } catch (error) {
      console.error('搜尋失敗:', error);
      this.showError(error.message || '搜尋失敗，請稍後再試');
    }
  },

  // 更新清單表格
  updateListTable(lists) {
    const tbody = document.getElementById('listTableBody');
    if (!tbody) {
      console.error('找不到清單表格');
      return;
    }

    tbody.innerHTML = '';

    if (!Array.isArray(lists) || lists.length === 0) {
      tbody.innerHTML = `
        <tr>
          <td colspan="4" class="text-center">沒有找到購物清單</td>
        </tr>
      `;
      return;
    }

    lists.forEach(list => {
      try {
        const row = document.createElement('tr');
        
        // 處理日期顯示
        const buyDate = new Date(list.buyDate);
        const formattedDate = buyDate.toLocaleDateString('zh-TW', {
          year: 'numeric',
          month: '2-digit',
          day: '2-digit'
        });

        const total = Array.isArray(list.items) 
          ? list.items.reduce((sum, item) => sum + (item.quantity * item.price), 0)
          : 0;

        row.innerHTML = `
          <td>${formattedDate}</td>
          <td>${list.title}</td>
          <td class="text-end">NT$ ${total.toLocaleString()}</td>
          <td class="text-center">
            <button type="button" class="btn btn-primary btn-sm me-1" onclick="app.editList('${list.id}')">
              <i class="bi bi-pencil"></i> 編輯
            </button>
            <button type="button" class="btn btn-danger btn-sm" onclick="app.deleteList('${list.id}')">
              <i class="bi bi-trash"></i> 刪除
            </button>
          </td>
        `;
        tbody.appendChild(row);
      } catch (error) {
        console.error('處理清單時發生錯誤:', error, list);
      }
    });
  },

  // 顯示新增清單 Modal
  showAddListModal() {
    try {
      if (this.addListModal) {
        // 設定預設日期為今天
        const today = new Date().toISOString().split('T')[0];
        document.getElementById('addBuyDate').value = today;
        
        // 清空標題
        document.getElementById('addTitle').value = '';
        
        // 顯示 Modal
        this.addListModal.show();
      } else {
        throw new Error('Modal 未初始化');
      }
    } catch (error) {
      console.error('顯示新增 Modal 失敗:', error);
      this.showError(error.message);
    }
  },

  // 新增清單
  async createList() {
    try {
      // 檢查是否正在儲存中
      if (this.isSaving) {
        console.log('正在儲存中，請稍候...');
        return;
      }

      // 設定儲存鎖
      this.isSaving = true;

      const titleInput = document.getElementById('addTitle');
      const dateInput = document.getElementById('addBuyDate');

      if (!titleInput || !dateInput) {
        throw new Error('找不到必要的表單欄位');
      }

      const title = titleInput.value.trim();
      const buyDate = dateInput.value;

      if (!title) {
        throw new Error('清單標題不能為空');
      }

      if (!buyDate) {
        throw new Error('購買日期不能為空');
      }

      const data = {
        title,
        buyDate,
        items: []
      };

      console.log('準備新增清單:', data);

      const response = await fetch('/api/shoppinglist', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
      });

      const result = await response.json();

      if (!response.ok) {
        console.error('新增失敗:', result);
        throw new Error(result.message || '新增清單失敗');
      }

      // 關閉 Modal
      if (this.addListModal) {
        this.addListModal.hide();
      }

      // 清空表單
      titleInput.value = '';
      dateInput.value = '';

      // 顯示成功訊息
      this.showSuccess('新增清單成功');

      // 重新載入清單
      await this.loadLists();
    } catch (error) {
      console.error('新增失敗:', error);
      this.showError(error.message || '新增失敗，請稍後再試');
    } finally {
      // 無論成功或失敗，都要釋放儲存鎖
      this.isSaving = false;
    }
  },

  // 編輯清單
  async editList(id) {
    try {
      console.log('編輯清單:', id);
      const response = await fetch(`/api/shoppinglist/${id}`);
      
      if (!response.ok) {
        throw new Error(`無法取得清單明細: ${response.status}`);
      }

      const result = await response.json();
      console.log('取得清單明細:', result);

      // 檢查 API 回應格式
      let list = null;
      if (result.success && result.data) {
        list = result.data;
      } else if (result.id) {
        list = result;
      }

      if (!list) {
        throw new Error('無效的清單資料');
      }

      // 確保每個項目都有正確的屬性
      if (Array.isArray(list.items)) {
        list.items = list.items.map(item => ({
          id: item.id || crypto.randomUUID(),
          isCompleted: Boolean(item.isCompleted),
          quantity: Math.max(1, parseInt(item.quantity) || 1),
          price: Math.max(0, parseFloat(item.price) || 0),
          name: item.name?.trim() || '',
          description: item.name?.trim() || '' // 同步更新description欄位
        }));
      } else {
        list.items = [];
      }

      // 更新 Modal 內容
      this.currentList = list;
      
      // 更新標題和日期
      const titleInput = document.getElementById('modalTitle');
      const dateInput = document.getElementById('modalBuyDate');
      
      if (titleInput) {
        titleInput.value = list.title || '';
      }

      if (dateInput) {
        // 將日期格式化為 YYYY-MM-DD
        const date = new Date(list.buyDate);
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        dateInput.value = `${year}-${month}-${day}`;
      }

      // 更新項目表格
      this.updateItemsTable();

      // 顯示 Modal
      if (this.detailModal) {
        this.detailModal.show();
      } else {
        console.error('Modal 未初始化');
        this.showError('無法顯示明細視窗');
      }
    } catch (error) {
      console.error('編輯清單失敗:', error);
      this.showError(error.message);
    }
  },

  // 刪除清單
  async deleteList(id) {
    if (!confirm('確定要刪除這個清單嗎？')) {
      return;
    }

    try {
      console.log('刪除清單:', id);
      const response = await fetch(`/api/shoppinglist/${id}`, {
        method: 'DELETE'
      });

      const result = await response.json();
      console.log('API 回應:', result);

      if (!response.ok) {
        throw new Error(result.message || '刪除失敗');
      }

      // 更新清單表格
      if (result.success && Array.isArray(result.data)) {
        this.updateListTable(result.data);
      } else {
        // 如果沒有收到新的清單資料，就重新載入
        await this.loadLists();
      }
      
      // 顯示成功訊息
      this.showSuccess('刪除清單成功');
      
      console.log('刪除成功');
    } catch (error) {
      console.error('刪除失敗:', error);
      this.showError(error.message);
    }
  },

  // 顯示明細 Modal
  async showDetail(id) {
    try {
      console.log('載入清單明細:', id);
      const response = await fetch(`/api/shoppinglist/${id}`);
      
      if (!response.ok) {
        throw new Error('無法取得清單明細');
      }

      const list = await response.json();
      console.log('清單明細:', list);

      // 更新 Modal 內容
      this.currentList = list;
      
      // 更新標題和日期
      const modalTitle = document.getElementById('modalTitle');
      const modalBuyDate = document.getElementById('modalBuyDate');
      
      if (modalTitle) {
        modalTitle.textContent = list.title || '';
      }
      if (modalBuyDate) {
        modalBuyDate.textContent = new Date(list.buyDate).toLocaleDateString();
      }

      // 更新項目表格
      this.updateItemsTable();

      // 顯示 Modal
      if (this.detailModal) {
        this.detailModal.show();
      } else {
        console.error('Modal 未初始化');
        this.showError('無法顯示明細視窗');
      }
    } catch (error) {
      console.error('載入明細失敗:', error);
      this.showError(error.message);
    }
  },

  // 新增空白項目
  addEmptyItem() {
    console.log('新增空白項目');
    if (!this.currentList) {
      console.error('目前沒有選擇的清單');
      return;
    }

    if (!Array.isArray(this.currentList.items)) {
      this.currentList.items = [];
    }

    // 新增項目時給予預設ID和空值
    this.currentList.items.push({
      id: crypto.randomUUID(),
      name: '',
      description: '', // 新增description欄位
      quantity: 1,
      price: 0,
      isCompleted: false
    });

    this.updateItemsTable();
  },

  // 更新項目表格
  updateItemsTable() {
    console.log('更新項目表格');
    const tbody = document.getElementById('itemTableBody');
    if (!tbody) {
      console.error('找不到項目表格');
      return;
    }

    tbody.innerHTML = '';

    if (!this.currentList || !Array.isArray(this.currentList.items)) {
      console.error('清單資料無效:', this.currentList);
      return;
    }

    this.currentList.items.forEach((item, index) => {
      try {
        const row = document.createElement('tr');
        const total = (item.quantity || 0) * (item.price || 0);

        row.innerHTML = `
          <td class="text-center">
            <input type="checkbox" class="form-check-input" 
              ${item.isCompleted ? 'checked' : ''} 
              onchange="app.updateItemCompleted(${index}, this.checked)">
          </td>
          <td>
            <input type="text" class="form-control" 
              value="${item.name || ''}" 
              onchange="app.updateItemName(${index}, this.value)">
          </td>
          <td>
            <input type="number" class="form-control text-end" min="1" 
              value="${item.quantity || 1}" 
              onchange="app.updateItemQuantity(${index}, this.value)">
          </td>
          <td>
            <input type="number" class="form-control text-end" min="0" step="1"
              value="${item.price || 0}" 
              onchange="app.updateItemPrice(${index}, this.value)">
          </td>
          <td class="text-end">NT$ ${total.toLocaleString()}</td>
          <td class="text-center">
            <button type="button" class="btn btn-danger btn-sm" onclick="app.removeItem(${index})">
              <i class="bi bi-trash"></i>
            </button>
          </td>
        `;
        tbody.appendChild(row);
      } catch (error) {
        console.error('處理項目時發生錯誤:', error, item);
      }
    });

    // 更新總金額
    this.updateTotalAmount();
  },

  // 更新總金額
  updateTotalAmount() {
    console.log('更新總金額');
    if (!this.currentList || !Array.isArray(this.currentList.items)) {
      return;
    }

    const total = this.currentList.items.reduce((sum, item) => {
      return sum + ((item.quantity || 0) * (item.price || 0));
    }, 0);

    const totalElement = document.getElementById('totalAmount');
    if (totalElement) {
      totalElement.textContent = `NT$ ${total.toLocaleString()}`;
    }
  },

  // 更新項目完成狀態
  updateItemCompleted(index, completed) {
    console.log('更新項目完成狀態:', index, completed);
    if (!this.currentList || !Array.isArray(this.currentList.items) || !this.currentList.items[index]) {
      return;
    }
    this.currentList.items[index].isCompleted = Boolean(completed);
    this.updateItemsTable();
  },

  // 更新項目名稱
  updateItemName(index, name) {
    console.log('更新項目名稱:', index, name);
    if (!this.currentList || !Array.isArray(this.currentList.items) || !this.currentList.items[index]) {
      return;
    }
    this.currentList.items[index].name = name?.trim() || '';
    this.currentList.items[index].description = name?.trim() || ''; // 同步更新description欄位
    this.updateItemsTable();
  },

  // 更新項目數量
  updateItemQuantity(index, quantity) {
    console.log('更新項目數量:', index, quantity);
    if (!this.currentList || !Array.isArray(this.currentList.items) || !this.currentList.items[index]) {
      return;
    }
    this.currentList.items[index].quantity = Math.max(1, parseInt(quantity) || 1);
    this.updateItemsTable();
  },

  // 更新項目價格
  updateItemPrice(index, price) {
    console.log('更新項目價格:', index, price);
    if (!this.currentList || !Array.isArray(this.currentList.items) || !this.currentList.items[index]) {
      return;
    }
    this.currentList.items[index].price = Math.max(0, parseFloat(price) || 0);
    this.updateItemsTable();
  },

  // 移除項目
  removeItem(index) {
    console.log('移除項目:', index);
    if (!this.currentList || !Array.isArray(this.currentList.items) || !this.currentList.items[index]) {
      return;
    }
    this.currentList.items.splice(index, 1);
    this.updateItemsTable();
  },

  // 生成 GUID
  generateGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
      const r = Math.random() * 16 | 0;
      const v = c === 'x' ? r : (r & 0x3 | 0x8);
      return v.toString(16);
    });
  },

  // 儲存變更
  async saveChanges() {
    try {
      // 檢查是否正在儲存中
      if (this.isSaving) {
        console.log('正在儲存中，請稍候...');
        return;
      }
      
      // 設定儲存鎖
      this.isSaving = true;

      if (!this.currentList) {
        throw new Error('找不到要儲存的清單');
      }

      if (!this.currentList.id) {
        throw new Error('清單 ID 不能為空');
      }

      // 從 Modal 取得最新的標題和日期
      const titleInput = document.getElementById('modalTitle');
      const dateInput = document.getElementById('modalBuyDate');

      if (!titleInput || !dateInput) {
        throw new Error('找不到必要的表單欄位');
      }

      const title = titleInput.value.trim();
      const buyDate = dateInput.value;

      if (!title) {
        throw new Error('清單標題不能為空');
      }

      if (!buyDate) {
        throw new Error('購買日期不能為空');
      }

      // 更新當前清單資料
      this.currentList.title = title;
      this.currentList.buyDate = buyDate;

      // 確保項目列表存在
      if (!Array.isArray(this.currentList.items)) {
        this.currentList.items = [];
      }

      // 過濾並驗證項目
      const validItems = this.currentList.items
        .filter(item => item && typeof item === 'object')
        .map(item => {
          const name = item.name?.trim() || '';
          if (!name) {
            throw new Error('商品名稱不能為空');
          }
          return {
            id: item.id || crypto.randomUUID(),
            name: name,
            description: name,
            quantity: Math.max(1, parseInt(item.quantity) || 1),
            price: Math.max(0, parseFloat(item.price) || 0),
            isCompleted: Boolean(item.isCompleted)
          };
        });

      if (validItems.length === 0) {
        throw new Error('清單必須至少包含一個有效的商品項目');
      }

      // 準備要傳送的資料
      const data = {
        id: this.currentList.id,
        title: this.currentList.title,
        buyDate: this.currentList.buyDate,
        items: validItems
      };

      console.log('準備儲存變更:', data);

      // 發送更新請求
      const response = await fetch(`/api/shoppinglist/${this.currentList.id}`, {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json'
        },
        body: JSON.stringify(data)
      });

      const result = await response.json();
      
      if (!response.ok) {
        console.error('儲存失敗:', result);
        
        // 處理驗證錯誤
        if (result.errors) {
          const errorMessages = [];
          for (const field in result.errors) {
            const fieldErrors = result.errors[field];
            if (Array.isArray(fieldErrors)) {
              errorMessages.push(...fieldErrors);
            } else if (typeof fieldErrors === 'string') {
              errorMessages.push(fieldErrors);
            }
          }
          if (errorMessages.length > 0) {
            throw new Error(errorMessages.join('\n'));
          }
        }
        
        throw new Error(result.message || result.title || '儲存失敗');
      }

      console.log('儲存成功:', result);

      // 更新本地數據以保持一致性
      if (result.data) {
        this.currentList = result.data;
        this.updateItemsTable();
      }

      // 關閉 Modal
      if (this.detailModal) {
        this.detailModal.hide();
      }

      // 顯示成功訊息
      this.showSuccess('儲存清單成功');

      // 重新載入清單
      await this.loadLists();
    } catch (error) {
      console.error('儲存失敗:', error);
      this.showError(error.message || '儲存失敗，請稍後再試');
    } finally {
      // 無論成功或失敗，都要釋放儲存鎖
      this.isSaving = false;
    }
  },

  // 顯示錯誤訊息
  showError(message) {
    const errorMessageElement = document.getElementById('errorMessage');
    if (errorMessageElement) {
      errorMessageElement.textContent = message;
    }
    if (this.errorModal) {
      this.errorModal.show();
    }
  },

  // 清除錯誤訊息
  clearErrors() {
    const errorMessageElement = document.getElementById('errorMessage');
    if (errorMessageElement) {
      errorMessageElement.textContent = '';
    }
  },

  // 顯示成功訊息
  showSuccess(message) {
    const successMessageElement = document.getElementById('successMessage');
    if (successMessageElement) {
      successMessageElement.textContent = message;
    }
    if (this.successModal) {
      this.successModal.show();
    }
  }
};

// 當 DOM 載入完成後初始化應用程式
document.addEventListener('DOMContentLoaded', () => {
  app.init();
});
