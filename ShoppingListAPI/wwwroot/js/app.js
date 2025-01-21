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
                    const data = JSON.parse(event.data);
                    if (data.type === 'update') {
                        window.app.loadShoppingLists();
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

    renderShoppingLists(lists) {
        const container = document.getElementById('shoppingLists');
        if (!container) {
            console.error('找不到購物清單容器');
            return;
        }

        container.innerHTML = '';
        
        if (!lists || lists.length === 0) {
            container.innerHTML = '<div class="no-lists">目前沒有購物清單</div>';
            return;
        }

        lists.forEach(list => {
            const listElement = document.createElement('div');
            listElement.className = 'shopping-list';
            listElement.innerHTML = `
                <div class="list-header">
                    <h3>${list.title || '未命名清單'}</h3>
                    <div class="list-info">
                        <span class="buy-date">購買日期: ${list.buyData || '未設定'}</span>
                        <span class="created-at">建立時間: ${new Date(list.createdAt).toLocaleDateString('zh-TW')}</span>
                    </div>
                </div>
                <div class="list-items">
                    ${list.items.map(item => `
                        <div class="list-item ${item.isCompleted ? 'completed' : ''}">
                            <span class="item-name">${item.name}</span>
                            <span class="item-quantity">數量: ${item.quantity}</span>
                            <span class="item-price">價格: $${item.price}</span>
                            <span class="item-status">${item.isCompleted ? '已完成' : '未完成'}</span>
                        </div>
                    `).join('')}
                </div>
                <div class="list-actions">
                    <button onclick="window.app.editList('${list.id}')">編輯</button>
                    <button onclick="window.app.deleteList('${list.id}')">刪除</button>
                </div>
            `;
            container.appendChild(listElement);
        });
    },

    editList(id) {
        // TODO: 實作編輯功能
        console.log('編輯清單:', id);
    },

    async deleteList(id) {
        if (!confirm('確定要刪除這個購物清單嗎？')) {
            return;
        }

        try {
            const response = await fetch(`/api/shoppinglist/${id}`, {
                method: 'DELETE'
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            await this.loadShoppingLists();
            showSuccess('購物清單已刪除');
        } catch (error) {
            console.error('刪除購物清單時發生錯誤:', error);
            showError('刪除失敗');
        }
    },

    initializeBulkDelete() {
        const bulkDeleteBtn = document.getElementById('bulkDeleteBtn');
        const deleteModal = document.getElementById('deleteModal');
        const closeBtn = deleteModal.querySelector('.close');
        const cancelBtn = document.getElementById('cancelDelete');
        const confirmBtn = document.getElementById('confirmDelete');
        const startDateInput = document.getElementById('startDate');
        const endDateInput = document.getElementById('endDate');

        // 打開模態框
        bulkDeleteBtn.addEventListener('click', () => {
            deleteModal.style.display = 'block';
        });

        // 關閉模態框
        const closeModal = () => {
            deleteModal.style.display = 'none';
            startDateInput.value = '';
            endDateInput.value = '';
        };

        closeBtn.addEventListener('click', closeModal);
        cancelBtn.addEventListener('click', closeModal);
        window.addEventListener('click', (event) => {
            if (event.target === deleteModal) {
                closeModal();
            }
        });

        // 確認刪除
        confirmBtn.addEventListener('click', async () => {
            const startDate = startDateInput.value;
            const endDate = endDateInput.value;

            if (!startDate || !endDate) {
                showError('請選擇日期範圍');
                return;
            }

            if (startDate > endDate) {
                showError('開始日期不能晚於結束日期');
                return;
            }

            try {
                showLoading();
                const response = await fetch('/api/shoppinglist/bulk-delete', {
                    method: 'DELETE',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        startDate: startDate + '-01',
                        endDate: endDate + '-31'
                    })
                });

                if (!response.ok) {
                    throw new Error('批量刪除失敗');
                }

                await this.loadShoppingLists();
                closeModal();
                showSuccess('已成功刪除選定範圍內的購物清單');
            } catch (error) {
                console.error('批量刪除時發生錯誤:', error);
                showError('批量刪除失敗，請稍後再試');
            } finally {
                hideLoading();
            }
        });
    },

    setupEventListeners() {
        // TODO: 添加事件監聽器
    }
};

// 批量刪除相關函數
function initializeBulkDelete() {
    const modal = document.getElementById('deleteModal');
    const bulkDeleteBtn = document.getElementById('bulkDeleteBtn');
    const closeBtn = modal.querySelector('.close');
    const cancelBtn = document.getElementById('cancelDelete');
    const confirmBtn = document.getElementById('confirmDelete');
    const startDateInput = document.getElementById('startDate');
    const endDateInput = document.getElementById('endDate');

    // 設置預設日期範圍（當前月份）
    const now = new Date();
    const currentMonth = now.toISOString().slice(0, 7);
    startDateInput.value = currentMonth;
    endDateInput.value = currentMonth;

    // 開啟 Modal
    bulkDeleteBtn.onclick = () => {
        modal.style.display = 'block';
    };

    // 關閉 Modal
    closeBtn.onclick = () => {
        modal.style.display = 'none';
    };

    cancelBtn.onclick = () => {
        modal.style.display = 'none';
    };

    // 點擊 Modal 外部關閉
    window.onclick = (event) => {
        if (event.target === modal) {
            modal.style.display = 'none';
        }
    };

    // 確認刪除
    confirmBtn.onclick = async () => {
        const startDate = startDateInput.value;
        const endDate = endDateInput.value;

        if (!startDate || !endDate) {
            alert('請選擇日期範圍');
            return;
        }

        if (startDate > endDate) {
            alert('開始日期不能大於結束日期');
            return;
        }

        try {
            const response = await fetch('/api/shoppinglist/bulk-delete', {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    startDate: startDate + '-01',
                    endDate: endDate + '-31'
                })
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const result = await response.json();
            alert(`成功刪除 ${result.deletedCount} 個購物清單`);
            modal.style.display = 'none';
            window.app.loadShoppingLists(); // 重新載入清單
        } catch (error) {
            console.error('批量刪除失敗:', error);
            alert('刪除失敗，請稍後再試');
        }
    };
}

// DOM 完成載入後初始化應用程式
document.addEventListener('DOMContentLoaded', () => {
    window.app.init().catch(error => {
        console.error("應用程式啟動失敗:", error);
        showError("應用程式啟動失敗，請重新整理頁面");
    });
});
