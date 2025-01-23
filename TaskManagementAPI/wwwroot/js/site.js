let currentPage = 1;
let pollingInterval;
let isPolling = false;

// 任務相關函數
async function loadTasks(page = 1, isPollingCall = false) {
    if (!isPollingCall) {
        currentPage = page;
    }

    try {
        // 獲取篩選條件
        const statusFilter = document.getElementById('statusFilter').value;
        const priorityFilter = document.getElementById('priorityFilter').value;

        // 構建查詢參數
        const params = new URLSearchParams({
            page: currentPage,
            pageSize: 10
        });

        if (statusFilter !== '') {
            params.append('isCompleted', statusFilter === 'true');
        }
        if (priorityFilter !== '') {
            params.append('priority', priorityFilter);
        }

        const response = await fetch(`/api/TodoTasks?${params}`);
        
        if (!response.ok) {
            let errorMessage = '加載任務失敗';
            try {
                const errorData = await response.json();
                if (errorData && errorData.message) {
                    errorMessage = errorData.message;
                }
            } catch {
                errorMessage = `加載失敗 (${response.status}: ${response.statusText})`;
            }
            throw new Error(errorMessage);
        }

        let data;
        try {
            data = await response.json();
        } catch (e) {
            throw new Error('無效的數據格式');
        }

        if (!data || !Array.isArray(data.items)) {
            throw new Error('無效的數據格式');
        }

        displayTasks(data.items);
        updatePagination(currentPage, Math.ceil(data.totalItems / 10));
    } catch (error) {
        console.error('加載任務時出錯:', error);
        if (!isPollingCall) {
            showError('加載任務失敗: ' + error.message);
        }
        stopPolling();
    }
}

function showError(message) {
    const errorDiv = document.createElement('div');
    errorDiv.className = 'error-message';
    errorDiv.textContent = message;
    errorDiv.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background-color: #f8d7da;
        color: #721c24;
        padding: 15px;
        border-radius: 4px;
        box-shadow: 0 2px 4px rgba(0,0,0,0.2);
        z-index: 1000;
    `;
    document.body.appendChild(errorDiv);
    setTimeout(() => {
        errorDiv.remove();
    }, 3000);
}

async function addTask() {
    try {
        const title = document.getElementById('taskTitle').value.trim();
        const description = document.getElementById('taskDescription').value.trim();
        const priority = parseInt(document.getElementById('taskPriority').value);
        const localDueDate = document.getElementById('taskDueDate').value;

        if (!title) {
            showError('請輸入任務標題');
            return;
        }

        // 時區轉換
        let dueDate = null;
        if (localDueDate) {
            dueDate = convertToUTC(localDueDate);
            console.log('輸入的本地時間:', localDueDate);
            console.log('轉換後的UTC時間:', dueDate);
            console.log('轉回本地時間:', convertToLocalTime(dueDate));
        }

        const task = {
            title,
            description,
            priority,
            dueDate
        };

        const response = await fetch('/api/TodoTasks', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(task)
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        // 重置表單
        document.getElementById('addTaskForm').reset();
        // 關閉 Modal
        closeAddTaskModal();
        // 重新加載任務列表
        await loadTasks(currentPage);
        
    } catch (error) {
        console.error('添加任務失敗:', error);
        showError('添加任務失敗: ' + error.message);
    }
}

async function toggleTaskComplete(taskId, currentStatus) {
    try {
        const response = await fetch(`/api/TodoTasks/${taskId}`, {
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                isCompleted: !currentStatus
            })
        });

        if (!response.ok) {
            let errorMessage = '更新任務狀態失敗';
            try {
                const errorData = await response.json();
                if (errorData && errorData.message) {
                    errorMessage = errorData.message;
                }
            } catch {
                errorMessage = `更新失敗 (${response.status}: ${response.statusText})`;
            }
            throw new Error(errorMessage);
        }

        // 重新加載任務列表
        await loadTasks(currentPage);
    } catch (error) {
        console.error('更新任務狀態時出錯:', error);
        showError('更新任務狀態失敗: ' + error.message);
    }
}

function displayTasks(tasks) {
    const taskList = document.getElementById('taskList');
    taskList.innerHTML = '';
    
    if (!tasks || tasks.length === 0) {
        taskList.innerHTML = '<div class="no-tasks">暫無任務</div>';
        return;
    }
    
    tasks.forEach(task => {
        const taskElement = document.createElement('div');
        taskElement.className = `task-item ${task.isCompleted ? 'completed' : ''}`;
        const dueDate = task.dueDate ? convertToLocalTime(task.dueDate) : '';
        taskElement.innerHTML = `
            <div class="task-header">
                <h3>${task.title}</h3>
                <div class="task-actions">
                    <button onclick="toggleTaskComplete(${task.id}, ${task.isCompleted})" 
                            class="complete-button btn ${task.isCompleted ? 'completed' : ''}">
                        ${task.isCompleted ? '取消完成' : '完成'}
                    </button>
                    <button onclick="deleteTask(${task.id})" class="delete-button btn">刪除</button>
                </div>
            </div>
            <p>${task.description || '無描述'}</p>
            <div class="task-meta">
                <span class="priority-badge priority-${getPriorityClass(task.priority)}">
                    優先級: ${getPriorityText(task.priority)}
                </span>
                <span>截止日期: ${dueDate}</span>
                <span>狀態: ${task.isCompleted ? '已完成' : '未完成'}</span>
            </div>
        `;
        taskList.appendChild(taskElement);
    });
}

function getPriorityText(priority) {
    switch (priority) {
        case 0: return '低';
        case 1: return '中';
        case 2: return '高';
        default: return '未知';
    }
}

function getPriorityClass(priority) {
    switch (priority) {
        case 0: return 'low';
        case 1: return 'medium';
        case 2: return 'high';
        default: return 'unknown';
    }
}

async function deleteTask(taskId) {
    if (!confirm('確定要刪除這個任務嗎？')) {
        return;
    }

    try {
        const response = await fetch(`/api/TodoTasks/${taskId}`, {
            method: 'DELETE'
        });

        if (!response.ok) {
            throw new Error('刪除任務失敗');
        }

        // 重新加載任務列表
        loadTasks(currentPage);
    } catch (error) {
        console.error('刪除任務時出錯:', error);
        showError('刪除任務失敗: ' + error.message);
    }
}

function updatePagination(currentPage, totalPages) {
    const pagination = document.getElementById('pagination');
    pagination.innerHTML = '';
    
    if (totalPages <= 1) {
        return;
    }
    
    // 添加上一頁按鈕
    if (currentPage > 1) {
        const prevButton = document.createElement('button');
        prevButton.textContent = '上一頁';
        prevButton.onclick = () => loadTasks(currentPage - 1);
        pagination.appendChild(prevButton);
    }
    
    // 添加頁碼按鈕
    for (let i = 1; i <= totalPages; i++) {
        const button = document.createElement('button');
        button.textContent = i;
        button.className = i === currentPage ? 'current' : '';
        button.onclick = () => loadTasks(i);
        pagination.appendChild(button);
    }
    
    // 添加下一頁按鈕
    if (currentPage < totalPages) {
        const nextButton = document.createElement('button');
        nextButton.textContent = '下一頁';
        nextButton.onclick = () => loadTasks(currentPage + 1);
        pagination.appendChild(nextButton);
    }
}

// 設置截止時間的最小值為當前時間
function setMinDueDate() {
    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    const minDateTime = now.toISOString().slice(0, 16);
    document.getElementById('taskDueDate').min = minDateTime;
}

// 開始輪詢
function startPolling() {
    if (!isPolling) {
        isPolling = true;
        // 每 30 秒輪詢一次
        pollingInterval = setInterval(() => {
            loadTasks(currentPage, true);
        }, 30000);
    }
}

// 停止輪詢
function stopPolling() {
    if (pollingInterval) {
        clearInterval(pollingInterval);
        pollingInterval = null;
    }
    isPolling = false;
}

// Modal 相關函數
function openAddTaskModal() {
    const modal = document.getElementById('addTaskModal');
    if (modal) {
        modal.style.display = 'block';
        setMinDueDate();
    }
}

function closeAddTaskModal() {
    const modal = document.getElementById('addTaskModal');
    if (modal) {
        modal.style.display = 'none';
        document.getElementById('addTaskForm').reset();
    }
}

// 點擊 Modal 外部時關閉
window.onclick = function(event) {
    const modal = document.getElementById('addTaskModal');
    if (event.target === modal) {
        closeAddTaskModal();
    }
}

// 頁面加載和卸載事件處理
document.addEventListener('DOMContentLoaded', () => {
    console.log('頁面加載完成，初始化...');
    loadTasks();
    setMinDueDate();

    // 註冊表單提交事件
    const form = document.getElementById('addTaskForm');
    if (form) {
        form.addEventListener('submit', (e) => {
            e.preventDefault();
            addTask();
        });
    }

    // 註冊查詢按鈕點擊事件
    const searchButton = document.querySelector('.search-button');
    if (searchButton) {
        searchButton.addEventListener('click', () => {
            loadTasks(1);
        });
    }
});

// 頁面卸載時停止輪詢
window.addEventListener('beforeunload', () => {
    stopPolling();
});

// 時區轉換函數
function convertToUTC(localDateString) {
    if (!localDateString) return null;
    
    try {
        console.log('輸入的本地時間字符串:', localDateString);
        
        // 解析本地時間字符串的組件
        const [datePart, timePart] = localDateString.split('T');
        const [year, month, day] = datePart.split('-');
        const [hours, minutes] = timePart.split(':');
        
        // 創建一個表示本地時間的 Date 對象
        const localDate = new Date(year, month - 1, day, hours, minutes);
        
        // 獲取時區偏移（分鐘）
        const tzOffset = localDate.getTimezoneOffset();
        console.log('本地時間:', localDate.toLocaleString());
        console.log('時區偏移（分鐘）:', tzOffset);
        
        // 創建 UTC 時間
        const utcMs = localDate.getTime() - (tzOffset * 60000);
        const utcDate = new Date(utcMs);
        const utcString = utcDate.toISOString();
        
        console.log('UTC時間:', utcString);
        return utcString;
    } catch (error) {
        console.error('時間轉換錯誤:', error);
        return null;
    }
}

function convertToLocalTime(utcDateString) {
    if (!utcDateString) return '';
    try {
        // console.log('輸入的UTC時間:', utcDateString);
        
        // 解析 UTC 時間
        const utcDate = new Date(utcDateString);
        
        // 使用 toLocaleString 自動處理時區轉換
        const localString = utcDate.toLocaleString('zh-TW', {
            year: 'numeric',
            month: '2-digit',
            day: '2-digit',
            hour: '2-digit',
            minute: '2-digit',
            hour12: false,
            timeZone: 'Asia/Taipei'
        }).replace(/\//g, '-');
        
        // console.log('轉換後的本地時間:', localString);
        return localString;
    } catch (error) {
        console.error('時間轉換錯誤:', error);
        return '';
    }
}
