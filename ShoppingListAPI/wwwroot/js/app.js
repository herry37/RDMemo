// 工具函數
function showLoading() {
    const loadingIndicator = document.getElementById('loadingIndicator');
    if (loadingIndicator) {
        loadingIndicator.classList.remove('hidden');
    }
}

function hideLoading() {
    const loadingIndicator = document.getElementById('loadingIndicator');
    if (loadingIndicator) {
        loadingIndicator.classList.add('hidden');
    }
}

function showError(message) {
    const existingError = document.querySelector('.error-message');
    if (existingError) {
        existingError.remove();
    }

    const errorDiv = document.createElement('div');
    errorDiv.className = 'error-message';
    errorDiv.textContent = message;
    document.body.appendChild(errorDiv);

    setTimeout(() => {
        errorDiv.remove();
    }, 3000);
}

function showSuccess(message) {
    const existingSuccess = document.querySelector('.success-message');
    if (existingSuccess) {
        existingSuccess.remove();
    }

    const successDiv = document.createElement('div');
    successDiv.className = 'success-message';
    successDiv.textContent = message;
    document.body.appendChild(successDiv);

    setTimeout(() => {
        successDiv.remove();
    }, 3000);
}

// WebSocket 連接函數
function getWebSocket() {
    return new Promise((resolve, reject) => {
        try {
            const wsUrl = `ws://${window.location.host}/ws`;
            const webSocket = new WebSocket(wsUrl);
            let opened = false;

            webSocket.onopen = () => {
                opened = true;
                console.log('WebSocket 連接成功');
                resolve(webSocket);
            };

            webSocket.onclose = (event) => {
                if (!opened) {
                    reject(new Error('WebSocket 連接失敗'));
                    return;
                }
                console.log('WebSocket 連接已關閉:', event.reason);
                // 嘗試重新連接
                setTimeout(() => {
                    console.log('嘗試重新連接 WebSocket...');
                    window.app.initializeWebSocket();
                }, 5000);
            };

            webSocket.onerror = (error) => {
                console.error('WebSocket 錯誤:', error);
                if (!opened) {
                    reject(error);
                }
            };

            webSocket.onmessage = (event) => {
                console.log('收到WebSocket消息:', event.data);
                try {
                    const message = JSON.parse(event.data);
                    switch (message.type) {
                        case 'shoppinglist_update':
                            window.app.handleShoppingListUpdate(message.data);
                            break;
                        case 'shoppinglist_delete':
                            window.app.handleShoppingListDelete(message.data);
                            break;
                        default:
                            console.warn('未知的WebSocket消息類型:', message.type);
                    }
                } catch (error) {
                    console.error('解析WebSocket消息時發生錯誤:', error);
                }
            };
        } catch (error) {
            reject(error);
        }
    });
}

// 定義應用程式物件
window.app = {
    lists: [],
    webSocket: null,
    currentPage: 1,
    pageSize: 10,
    totalItems: 0,

    async initializeWebSocket() {
        try {
            this.webSocket = await getWebSocket();
        } catch (error) {
            console.error('初始化WebSocket失敗:', error);
            showError('WebSocket連接失敗');
        }
    },

    async init() {
        try {
            await this.loadShoppingLists();
            this.setupEventListeners();
            this.initializeBulkDelete();
            await this.initializeWebSocket();
        } catch (error) {
            console.error('初始化應用程式時發生錯誤:', error);
            showError('初始化失敗，請重新整理頁面');
        }
    },

    async loadShoppingLists() {
        showLoading();
        try {
            const response = await fetch('/api/shoppinglist');
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const lists = await response.json();
            this.lists = lists;
            this.renderShoppingLists(lists);
            hideLoading();
        } catch (error) {
            console.error('載入購物清單時發生錯誤:', error);
            showError('載入購物清單失敗');
            hideLoading();
        }
    },

    handleShoppingListUpdate(list) {
        const index = this.lists.findIndex(l => l.id === list.id);
        if (index !== -1) {
            this.lists[index] = list;
        } else {
            this.lists.unshift(list);
        }
        this.renderShoppingLists(this.lists);
        showSuccess('購物清單已更新');
    },

    handleShoppingListDelete(listId) {
        const index = this.lists.findIndex(l => l.id === listId);
        if (index !== -1) {
            this.lists.splice(index, 1);
            this.renderShoppingLists(this.lists);
            showSuccess('購物清單已刪除');
        }
    },

    renderShoppingLists(lists) {
        const container = document.getElementById('shoppingLists');
        if (!container) return;

        container.innerHTML = '';
        if (lists.length === 0) {
            container.innerHTML = '<p class="text-center text-gray-500">目前沒有購物清單</p>';
            return;
        }

        lists.forEach(list => {
            const listElement = document.createElement('div');
            listElement.className = 'bg-white shadow rounded-lg p-4 mb-4';
            listElement.innerHTML = `
                <div class="flex justify-between items-center mb-4">
                    <h3 class="text-lg font-semibold">${list.title}</h3>
                    <div class="space-x-2">
                        <button onclick="window.app.editList('${list.id}')" 
                                class="bg-blue-500 text-white px-3 py-1 rounded hover:bg-blue-600">
                            編輯
                        </button>
                        <button onclick="window.app.deleteList('${list.id}')"
                                class="bg-red-500 text-white px-3 py-1 rounded hover:bg-red-600">
                            刪除
                        </button>
                    </div>
                </div>
                <div class="space-y-2">
                    ${list.items.map(item => `
                        <div class="flex items-center justify-between p-2 ${item.isCompleted ? 'bg-gray-100' : 'bg-white'} rounded">
                            <div class="flex items-center">
                                <input type="checkbox" ${item.isCompleted ? 'checked' : ''} 
                                       onchange="window.app.toggleItem('${list.id}', '${item.id}')"
                                       class="mr-2">
                                <span class="${item.isCompleted ? 'line-through text-gray-500' : ''}">${item.name}</span>
                            </div>
                            <div class="text-sm text-gray-500">
                                ${item.quantity}件 - $${item.price}
                            </div>
                        </div>
                    `).join('')}
                </div>
            `;
            container.appendChild(listElement);
        });
    },

    async editList(id) {
        // 待實現
        console.log('編輯清單:', id);
    },

    async deleteList(id) {
        if (!confirm('確定要刪除這個購物清單嗎？')) {
            return;
        }

        showLoading();
        try {
            const response = await fetch(`/api/shoppinglist/${id}`, {
                method: 'DELETE'
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            hideLoading();
            showSuccess('購物清單已刪除');
        } catch (error) {
            console.error('刪除購物清單時發生錯誤:', error);
            showError('刪除購物清單失敗');
            hideLoading();
        }
    },

    async toggleItem(listId, itemId) {
        showLoading();
        try {
            const response = await fetch(`/api/shoppinglist/${listId}/items/${itemId}/toggle`, {
                method: 'PUT'
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const updatedList = await response.json();
            this.handleShoppingListUpdate(updatedList);
            hideLoading();
        } catch (error) {
            console.error('切換項目狀態時發生錯誤:', error);
            showError('切換項目狀態失敗');
            hideLoading();
        }
    },

    setupEventListeners() {
        // 待實現
    }
};

// DOM 完成載入後初始化應用程式
document.addEventListener('DOMContentLoaded', () => {
    window.app.init().catch(error => {
        console.error("應用程式啟動失敗:", error);
        showError("應用程式啟動失敗");
    });
});
