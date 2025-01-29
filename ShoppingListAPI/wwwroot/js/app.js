'use strict';

// 應用程式主要物件
const app = {
    currentList: null,
    detailModal: null,
    addListModal: null,
    errorModal: null,
    successModal: null,

    // 初始化應用程式
    init() {
        try {
            // 初始化 Bootstrap Modal
            const detailModalEl = document.getElementById('detailModal');
            const addListModalEl = document.getElementById('addListModal');
            const errorModalEl = document.getElementById('errorModal');
            const successModalEl = document.getElementById('successModal');

            // 如果存在明細 Modal 則綁定關閉事件
            if (detailModalEl) {
                this.detailModal = new bootstrap.Modal(detailModalEl);
                // 監聽 Modal 關閉事件
                detailModalEl.addEventListener('hidden.bs.modal', () => {
                    this.currentList = null;
                });
            }

            // 如果存在新增 Modal 則綁定關閉事件
            if (addListModalEl) {
                this.addListModal = new bootstrap.Modal(addListModalEl);
            }

            // 如果存在錯誤 Modal 則綁定關閉事件
            if (errorModalEl) {
                this.errorModal = new bootstrap.Modal(errorModalEl);
            }

            // 如果存在成功 Modal 則綁定關閉事件
            if (successModalEl) {
                this.successModal = new bootstrap.Modal(successModalEl);
            }

            // 綁定儲存按鈕事件
            const saveButton = document.getElementById('saveButton');
            if (saveButton) {
                saveButton.addEventListener('click', () => this.saveChanges());
            }

            // 載入清單資料
            this.loadLists();

        } catch (error) {
            this.showError('初始化應用程式失敗');
        }
    },

    // 載入清單
    async loadLists() {
        try {
            // 取得搜尋條件-起始日期
            let startDate = document.getElementById('startDate')?.value?.trim() || '';
            // 取得搜尋條件-結束日期
            let endDate = document.getElementById('endDate')?.value?.trim() || '';
            // 取得搜尋條件-標題
            const title = document.getElementById('searchTitle')?.value?.trim() || '';

            // 處理日期格式 - 只保留日期部分，因為後端只比較日期
            if (startDate) {
                startDate = startDate + 'T00:00:00';
            }
            
            if (endDate) {
                endDate = endDate + 'T00:00:00';
            }

            let url = '/api/shoppinglist/search';
            // 建立查詢字串
            const queryParts = [];
            
            if (startDate) {
                queryParts.push(`startDate=${encodeURIComponent(startDate)}`);
            }
            if (endDate) {
                queryParts.push(`endDate=${encodeURIComponent(endDate)}`);
            }
            if (title) {
                queryParts.push(`title=${encodeURIComponent(title)}`);
            }

            // 如果有查詢參數，則將其加入 URL
            if (queryParts.length > 0) {
                url += '?' + queryParts.join('&');
            }

            // 發送請求
            const response = await fetch(url);
            
            if (!response.ok) {
                throw new Error(`載入清單失敗: ${response.status} ${response.statusText}`);
            }

            // 解析回應的 JSON 資料
            const result = await response.json();
            
            if (!result.success) {
                throw new Error(result.message || '載入清單失敗');
            }

            // 更新清單表格
            this.updateListTable(result.data);

        } catch (error) {
            this.showError(error.message || '載入清單失敗');
        }
    },

    // 搜尋清單
    searchLists(event) {
        if (event) {
            event.preventDefault();
        }
        this.loadLists();
    },

    // 更新清單表格
    updateListTable(lists) {
        // 取得表格元素
        const tbody = document.getElementById('listTableBody');
        if (!tbody) return;

        tbody.innerHTML = '';

        // 如果清單不存在或清單為空，則顯示無資料
        if (!Array.isArray(lists) || lists.length === 0) {
            const tr = document.createElement('tr');
            tr.innerHTML = '<td colspan="4" class="text-center">無資料</td>';
            tbody.appendChild(tr);
            return;
        }

        // 逐一建立清單列
        lists.forEach(list => {
            // 建立新的 row
            const tr = document.createElement('tr');
            // 計算總金額
            const total = Array.isArray(list.items)
                ? list.items.reduce((sum, item) => sum + (item.quantity * item.price), 0)
                : 0;
            // 設定 row 內容
            tr.innerHTML = `
        <td>${new Date(list.buyDate).toLocaleDateString()}</td>
        <td>${list.title}</td>
        <td class="text-end">NT$ ${total.toLocaleString()}</td>
        <td class="text-center">
          <button class="btn btn-sm btn-primary me-2" onclick="app.showDetail('${list.id}')">明細</button>
          <button class="btn btn-sm btn-danger" onclick="app.deleteList('${list.id}')">刪除</button>
        </td>
      `;
            // 將新建的 row 加入 tbody
            tbody.appendChild(tr);
        });
    },

    // 更新項目表格
    updateItemsTable() {
        // 取得表格元素
        const tbody = document.getElementById('itemTableBody');
        if (!tbody) return;

        tbody.innerHTML = '';

        // 確保清單和項目存在
        if (!this.currentList || !Array.isArray(this.currentList.items)) {
            return;
        }

        // 逐一建立項目列
        this.currentList.items.forEach((item, index) => {
            const row = document.createElement('tr');
            const total = (item.quantity || 0) * (item.price || 0);

            // 設定 row 內容
            row.innerHTML = `
        <td class="text-center">
          <input type="checkbox" class="form-check-input" 
            ${item.is_completed ? 'checked' : ''} 
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
          <button type="button" class="btn btn-sm btn-danger" onclick="app.removeItem(${index})">
            刪除
          </button>
        </td>
      `;
            // 將新建的 row 加入 tbody
            tbody.appendChild(row);
        });
        // 更新總金額
        this.updateTotalAmount();
    },

    // 更新總金額
    updateTotalAmount() {
        if (!this.currentList || !Array.isArray(this.currentList.items)) {
            return;
        }

        // 計算總金額使用 reduce 方法計算總金額
        const total = this.currentList.items.reduce((sum, item) => {
            return sum + ((item.quantity || 0) * (item.price || 0));
        }, 0);

        // 更新總金額
        const totalElement = document.getElementById('totalAmount');
        // 如果總金額元素存在，則更新其內容
        if (totalElement) {
            totalElement.textContent = `NT$ ${total.toLocaleString()}`;
        }
    },

    // 新增空白項目
    addEmptyItem() {
        if (!this.currentList) {
            return;
        }

        //  如果 currentList.items 不是陣列，則初始化為空陣列
        if (!Array.isArray(this.currentList.items)) {
            this.currentList.items = [];
        }

        // 新增空白項目
        this.currentList.items.push({
            name: '',
            quantity: 1,
            price: 0,
            is_completed: false
        });

        // 更新表格
        this.updateItemsTable();
    },

    // 更新項目完成狀態
    updateItemCompleted(index, completed) {
        // 確保清單和項目存在
        if (!this.currentList || !Array.isArray(this.currentList.items) || !this.currentList.items[index]) {
            return;
        }
        // 更新指定索引的項目完成狀態
        this.currentList.items[index].is_completed = completed;
        this.updateItemsTable();
    },

    // 更新項目名稱
    updateItemName(index, name) {
        // 確保清單和項目存在
        if (!this.currentList || !Array.isArray(this.currentList.items) || !this.currentList.items[index]) {
            return;
        }
        // 更新指定索引的項目名稱
        this.currentList.items[index].name = name;
        this.updateItemsTable();
    },

    // 更新項目數量
    updateItemQuantity(index, quantity) {
        // 確保清單和項目存在
        if (!this.currentList || !Array.isArray(this.currentList.items) || !this.currentList.items[index]) {
            return;
        }
        // 限制數量最小值為 1
        this.currentList.items[index].quantity = Math.max(1, parseInt(quantity) || 1);
        this.updateItemsTable();
    },

    // 更新項目價格
    updateItemPrice(index, price) {
        // 確保清單和項目存在
        if (!this.currentList || !Array.isArray(this.currentList.items) || !this.currentList.items[index]) {
            return;
        }
        // 限制價格最小值為 0
        this.currentList.items[index].price = Math.max(0, parseFloat(price) || 0);
        this.updateItemsTable();
    },

    // 移除項目
    removeItem(index) {
        // 確保清單和項目存在
        if (!this.currentList || !Array.isArray(this.currentList.items) || !this.currentList.items[index]) {
            return;
        }
        // 移除指定索引的項目
        this.currentList.items.splice(index, 1);
        // 更新表格
        this.updateItemsTable();
    },

    // 儲存變更
    async saveChanges() {
        try {
            // 確保當前清單存在
            if (!this.currentList) {
                throw new Error('找不到要儲存的清單');
            }

            // 從 Modal 取得最新的標題和日期
            const titleInput = document.getElementById('modalTitle');
            const dateInput = document.getElementById('modalBuyDate');

            if (!titleInput || !dateInput) {
                throw new Error('找不到必要的表單欄位');
            }

            // 取得使用者輸入的資料
            const title = titleInput.value.trim();
            // 取得使用者輸入的購買日期
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
                        name: name,
                        quantity: Math.max(1, parseInt(item.quantity) || 1),
                        price: Math.max(0, parseFloat(item.price) || 0),
                        is_completed: Boolean(item.is_completed)
                    };
                });

            // 準備要傳送的資料
            const data = {
                title: this.currentList.title,
                buyDate: this.currentList.buyDate,
                items: validItems
            };

            // 發送更新請求
            const response = await fetch(`/api/shoppinglist/${this.currentList.id}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(data)
            });

            // 處理回應
            const result = await response.json();
            if (!response.ok || !result.success) {
                throw new Error(result.message || '儲存失敗');
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
            this.showError(error.message || '儲存失敗');
        }
    },

    // 顯示新增清單 Modal
    showAddListModal() {
        try {
            // 重置表單
            const form = document.getElementById('addListForm');
            if (form) {
                form.reset();
                // 設定預設日期為今天
                const today = new Date().toISOString().split('T')[0];
                document.getElementById('addBuyDate').value = today;
            }

            // 顯示 Modal
            if (this.addListModal) {
                this.addListModal.show();
                // 聚焦到標題輸入框
                setTimeout(() => {
                    document.getElementById('addTitle')?.focus();
                }, 500);
            }
        } catch (error) {
            this.showError('顯示新增 Modal 失敗');
        }
    },

    // 新增清單
    async addList() {
        try {
            // 取得使用者輸入的資料
            const title = document.getElementById('addTitle').value;
            const buyDate = document.getElementById('addBuyDate').value;

            // 驗證資料
            if (!title || !buyDate) {
                this.showError('請填寫標題和購買日期');
                return;
            }

            // 發送新增請求
            const response = await fetch('/api/shoppinglist', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    title,
                    buyDate
                })
            });

            // 處理回應
            const result = await response.json();
            if (!response.ok || !result.success) {
                throw new Error(result.message || '新增清單失敗');
            }

            // 關閉新增對話框
            if (this.addListModal) {
                const modalElement = document.getElementById('addListModal');
                const modal = bootstrap.Modal.getInstance(modalElement);
                if (modal) {
                    modal.hide();
                    // 等待 Modal 完全關閉後再執行後續操作
                    modalElement.addEventListener('hidden.bs.modal', () => {
                        // 重新載入清單
                        this.loadLists();
                        // 顯示成功訊息
                        this.showSuccess('新增清單成功');
                    }, { once: true });
                }
            }

        } catch (error) {
            this.showError(error.message || '新增清單失敗');
        }
    },

    // 刪除清單
    async deleteList(id) {
        try {
            if (!confirm('確定要刪除此清單？')) {
                return;
            }
            // 發送刪除請求
            const response = await fetch(`/api/shoppinglist/${id}`, {
                method: 'DELETE'
            });
            // 處理回應
            const result = await response.json();
            if (!response.ok || !result.success) {
                throw new Error(result.message || '刪除清單失敗');
            }

            // 重新載入清單
            this.loadLists();

            // 顯示成功訊息
            this.showSuccess('刪除清單成功');

        } catch (error) {
            this.showError(error.message || '刪除清單失敗');
        }
    },

    // 顯示清單明細
    async showDetail(id) {
        try {
            // 發送請求
            const response = await fetch(`/api/shoppinglist/${id}`);
            // 處理回應
            const result = await response.json();
            // 錯誤處理
            if (!response.ok || !result.success) {
                throw new Error(result.message || '載入清單明細失敗');
            }
            // 更新當前清單
            const list = result.data;
            // 更新當前清單
            this.currentList = list;

            // 更新 Modal 內容
            const modalTitle = document.getElementById('modalTitle');
            // 購買日期欄位
            const modalBuyDate = document.getElementById('modalBuyDate');
            // 更新 Modal 內容
            if (modalTitle) modalTitle.value = list.title;
            // 更新 Modal 內容
            if (modalBuyDate) modalBuyDate.value = new Date(list.buyDate).toISOString().split('T')[0];

            // 更新項目表格
            this.updateItemsTable();

            // 顯示 Modal
            if (this.detailModal) {
                this.detailModal.show();
            }

        } catch (error) {
            this.showError(error.message || '載入明細失敗');
        }
    },

    // 顯示錯誤訊息
    showError(message) {
        try {
            // 取得錯誤訊息元素
            const errorMessage = document.getElementById('errorMessage');
            // 如果存在則更新內容
            if (errorMessage) {
                errorMessage.textContent = message;
            }
            // 如果存在錯誤 Modal 則顯示
            if (this.errorModal) {
                this.errorModal.show();
            } else {
                alert(message);
            }
        } catch (error) {
            alert(message);
        }
    },

    // 顯示成功訊息
    showSuccess(message) {
        try {
            // 取得成功訊息元素
            const successMessage = document.getElementById('successMessage');
            // 如果存在則更新內容
            if (successMessage) {
                successMessage.textContent = message;
            }
            // 如果存在成功 Modal 則顯示
            if (this.successModal) {
                this.successModal.show();
            } else {
                alert(message);
            }
        } catch (error) {
            alert(message);
        }
    }
};

// 當 DOM 載入完成後初始化應用程式
document.addEventListener('DOMContentLoaded', () => app.init());
