/// <summary>
/// 待辦事項管理系統的前端 JavaScript 代碼
/// 此檔案包含了整個待辦事項系統的前端邏輯，主要功能包括：
/// 1. 任務的新增、刪除、修改和查詢（CRUD）操作
/// 2. 分頁管理和動態更新
/// 3. 任務狀態和優先級的即時更新
/// 4. 錯誤處理和用戶提示
/// 5. 時區轉換處理
/// 6. WebSocket 即時通訊
/// </summary>

// 全域變數定義區域
/// <summary>
/// currentPage: 用於追蹤當前顯示的頁碼，初始值為 1
/// pollingInterval: 用於存儲定時輪詢的計時器 ID
/// isPolling: 標記當前是否正在進行輪詢
/// </summary>
let currentPage = 1;                // 當前頁碼
let pollingInterval;                // 輪詢間隔計時器
let isPolling = false;             // 輪詢狀態標誌

/// <summary>
/// 載入任務列表的主要函數
/// 負責從伺服器獲取任務數據並更新頁面顯示
/// </summary>
/// <param name="page">要載入的頁碼，預設為 1</param>
/// <param name="isPollingCall">是否為輪詢調用，用於區分用戶手動刷新和自動更新，預設為 false</param>
/// <remarks>
/// 此函數會：
/// 1. 處理分頁邏輯
/// 2. 獲取並應用篩選條件（狀態和優先級）
/// 3. 發送 API 請求獲取數據
/// 4. 處理各種錯誤情況
/// 5. 更新 UI 顯示
/// </remarks>
async function loadTasks(page = 1, isPollingCall = false) {
    // 非輪詢呼叫時更新當前頁碼
    if (!isPollingCall) {
        currentPage = page;
    }

    try {
        // 獲取篩選條件
        const statusFilter = document.getElementById('statusFilter').value;    // 完成狀態篩選
        const priorityFilter = document.getElementById('priorityFilter').value;// 優先級篩選

        // 構建查詢參數
        const params = new URLSearchParams({
            page: currentPage,
            pageSize: 10           // 每頁顯示 10 條記錄
        });

        // 添加篩選條件到查詢參數
        if (statusFilter !== '') {
            params.append('isCompleted', statusFilter === 'true');
        }
        if (priorityFilter !== '') {
            params.append('priority', priorityFilter);
        }

        // 發送請求獲取任務列表
        const response = await fetch(`/api/TodoTasks?${params}`);
        
        // 處理錯誤響應
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

        // 解析響應數據
        let data;
        try {
            data = await response.json();
        } catch (e) {
            throw new Error('無效的數據格式');
        }

        // 驗證數據格式
        if (!data || !Array.isArray(data.items)) {
            throw new Error('無效的數據格式');
        }

        // 顯示任務列表和分頁
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

/// <summary>
/// 顯示錯誤訊息的通用函數
/// 在頁面右上角顯示一個臨時的錯誤提示框
/// </summary>
/// <param name="message">要顯示的錯誤訊息文字</param>
/// <remarks>
/// 錯誤訊息會：
/// 1. 使用固定定位在右上角顯示
/// 2. 具有醒目的紅色背景
/// 3. 3秒後自動消失
/// 4. 使用 z-index 確保顯示在其他元素之上
/// </remarks>
function showError(message) {
    // 創建錯誤訊息元素
    const errorDiv = document.createElement('div');
    errorDiv.className = 'error-message';
    errorDiv.textContent = message;
    
    // 設置錯誤訊息樣式
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
    
    // 添加到頁面並設置自動移除
    document.body.appendChild(errorDiv);
    setTimeout(() => {
        errorDiv.remove();
    }, 3000);
}

/// <summary>
/// 顯示任務列表的渲染函數
/// 負責將任務數據轉換為 HTML 並顯示在頁面上
/// </summary>
/// <param name="tasks">任務數據陣列，包含所有要顯示的任務資訊</param>
/// <remarks>
/// 函數功能：
/// 1. 清空現有任務列表
/// 2. 遍歷任務數據並生成 HTML 元素
/// 3. 為每個任務添加狀態切換和刪除按鈕
/// 4. 設置適當的樣式類別
/// 5. 處理日期顯示格式
/// </remarks>
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

/// <summary>
/// 優先級文字轉換函數
/// 將數字優先級轉換為對應的中文描述
/// </summary>
/// <param name="priority">優先級數值（0-2）</param>
/// <returns>對應的中文優先級描述</returns>
/// <remarks>
/// 優先級對應關係：
/// - 0：低優先級
/// - 1：中優先級
/// - 2：高優先級
/// </remarks>
function getPriorityText(priority) {
    switch (priority) {
        case 0: return '低';
        case 1: return '中';
        case 2: return '高';
        default: return '未知';
    }
}

/// <summary>
/// 優先級樣式類別轉換函數
/// 根據優先級返回對應的 CSS 類別名稱
/// </summary>
/// <param name="priority">優先級數值（0-2）</param>
/// <returns>對應的 CSS 類別名稱</returns>
/// <remarks>
/// 樣式類別對應關係：
/// - 0：low-priority（綠色）
/// - 1：medium-priority（黃色）
/// - 2：high-priority（紅色）
/// </remarks>
function getPriorityClass(priority) {
    switch (priority) {
        case 0: return 'low';
        case 1: return 'medium';
        case 2: return 'high';
        default: return 'unknown';
    }
}

/// <summary>
/// 刪除任務的處理函數
/// 負責向伺服器發送刪除請求並更新界面
/// </summary>
/// <param name="taskId">要刪除的任務 ID</param>
/// <remarks>
/// 執行步驟：
/// 1. 向伺服器發送 DELETE 請求
/// 2. 處理可能的錯誤情況
/// 3. 成功時重新載入任務列表
/// 4. 失敗時顯示錯誤提示
/// 
/// 安全考慮：
/// - 刪除前應該進行確認
/// - 確保任務 ID 有效
/// </remarks>
async function deleteTask(taskId) {
    if (!confirm('確定要刪除這個任務嗎？')) {
        return;
    }

    try {
        // 發送請求刪除任務
        const response = await fetch(`/api/TodoTasks/${taskId}`, {
            method: 'DELETE'
        });

        // 檢查請求是否成功
        if (!response.ok) {
            throw new Error('刪除任務失敗');
        }

        // 重新加載任務列表
        loadTasks(currentPage);
    } catch (error) {
        console.error('刪除任務時出錯:', error);
        showError('刪除任務錯誤: ' + error.message);
    }
}

/// <summary>
/// 更新分頁控制器的顯示函數
/// 根據當前頁碼和總頁數生成分頁導航
/// </summary>
/// <param name="currentPage">當前顯示的頁碼</param>
/// <param name="totalPages">總頁數</param>
/// <remarks>
/// 功能特點：
/// 1. 動態生成頁碼按鈕
/// 2. 標記當前頁碼
/// 3. 處理首頁和末頁的特殊情況
/// 4. 添加上一頁和下一頁按鈕
/// 5. 處理頁碼過多時的省略顯示
/// </remarks>
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

/// <summary>
/// 設置任務截止日期最小值的函數
/// 確保用戶只能選擇當前時間之後的日期
/// </summary>
/// <remarks>
/// 處理邏輯：
/// 1. 獲取當前本地時間
/// 2. 格式化為 HTML datetime-local 支持的格式
/// 3. 設置為日期選擇器的最小值
/// </remarks>
function setMinDueDate() {
    const now = new Date();
    now.setMinutes(now.getMinutes() - now.getTimezoneOffset());
    const minDateTime = now.toISOString().slice(0, 16);
    document.getElementById('taskDueDate').min = minDateTime;
}

/// <summary>
/// 開始定時輪詢的函數
/// 定期檢查任務列表更新
/// </summary>
/// <remarks>
/// 實現細節：
/// 1. 避免重複啟動輪詢
/// 2. 設置適當的輪詢間隔（預設 5 秒）
/// 3. 使用 isPolling 標記防止重複請求
/// </remarks>
function startPolling() {
    if (!isPolling) {
        isPolling = true;
        // 每 30 秒輪詢一次
        pollingInterval = setInterval(() => {
            loadTasks(currentPage, true);
        }, 30000);
    }
}

/// <summary>
/// 停止定時輪詢的函數
/// 清理輪詢計時器
/// </summary>
/// <remarks>
/// 使用場景：
/// 1. 用戶離開頁面時
/// 2. 發生錯誤時
/// 3. 手動停止輪詢時
/// </remarks>
function stopPolling() {
    if (pollingInterval) {
        clearInterval(pollingInterval);
        pollingInterval = null;
    }
    isPolling = false;
}

/// <summary>
/// 開啟新增任務對話框的函數
/// 顯示任務輸入表單
/// </summary>
/// <remarks>
/// 處理步驟：
/// 1. 顯示 Modal 對話框
/// 2. 重置表單內容
/// 3. 設置最小日期限制
/// 4. 設置焦點到第一個輸入框
/// </remarks>
function openAddTaskModal() {
    const modal = document.getElementById('addTaskModal');
    if (modal) {
        modal.style.display = 'block';
        setMinDueDate();
    }
}

/// <summary>
/// 關閉新增任務對話框的函數
/// 隱藏任務輸入表單
/// </summary>
/// <remarks>
/// 清理工作：
/// 1. 隱藏 Modal 對話框
/// 2. 清除表單數據
/// 3. 移除可能的錯誤提示
/// </remarks>
function closeAddTaskModal() {
    const modal = document.getElementById('addTaskModal');
    if (modal) {
        modal.style.display = 'none';
        document.getElementById('addTaskForm').reset();
    }
}

/// <summary>
/// Modal 外部點擊處理函數
/// 點擊對話框外部區域時關閉對話框
/// </summary>
/// <param name="event">點擊事件對象</param>
/// <remarks>
/// 檢查邏輯：
/// 1. 確認點擊目標是否為 Modal 背景
/// 2. 防止事件冒泡
/// 3. 調用關閉函數
/// </remarks>
function onclick(event) {
    const modal = document.getElementById('addTaskModal');
    if (event.target === modal) {
        closeAddTaskModal();
    }
}

/// <summary>
/// 新增任務的處理函數
/// 處理用戶提交新任務的請求
/// </summary>
/// <remarks>
/// 函數流程：
/// 1. 獲取並驗證表單輸入
/// 2. 處理日期時區轉換
/// 3. 發送 API 請求
/// 4. 處理響應和錯誤
/// 5. 更新 UI 顯示
/// 
/// 驗證項目：
/// - 標題不能為空
/// - 日期格式必須正確
/// - 優先級必須為有效值
/// </remarks>
async function addTask() {
    try {
        // 獲取表單數據
        const title = document.getElementById('taskTitle').value.trim();
        const description = document.getElementById('taskDescription').value.trim();
        const priority = parseInt(document.getElementById('taskPriority').value);
        const localDueDate = document.getElementById('taskDueDate').value;

        // 驗證表單數據
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

        // 創建任務物件
        const task = {
            title,
            description,
            priority,
            dueDate
        };

        // 發送請求新增任務
        const response = await fetch('/api/TodoTasks', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(task)
        });

        // 處理錯誤響應
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

/// <summary>
/// 切換任務完成狀態的處理函數
/// </summary>
/// <param name="taskId">要切換狀態的任務 ID</param>
/// <param name="currentStatus">當前的完成狀態</param>
/// <remarks>
/// 函數功能：
/// 1. 發送 PATCH 請求更新任務狀態
/// 2. 處理可能的錯誤情況
/// 3. 成功時觸發 UI 更新
/// 4. 失敗時顯示適當的錯誤訊息
/// </remarks>
async function toggleTaskComplete(taskId, currentStatus) {
    try {
        // 發送請求切換任務完成狀態
        const response = await fetch(`/api/TodoTasks/${taskId}`, {
            method: 'PATCH',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                isCompleted: !currentStatus
            })
        });

        // 處理錯誤響應
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

/// <summary>
/// UTC 時間轉換函數
/// 將本地時間轉換為 UTC 時間
/// </summary>
/// <param name="localDateString">本地時間字符串（格式：YYYY-MM-DDTHH:mm）</param>
/// <returns>UTC 時間字符串</returns>
/// <remarks>
/// 轉換步驟：
/// 1. 解析輸入的本地時間字符串
/// 2. 創建 Date 對象並處理時區差異
/// 3. 轉換為 ISO 格式的 UTC 時間
/// 4. 處理無效日期的情況
/// </remarks>
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

/// <summary>
/// 本地時間轉換函數
/// 將 UTC 時間轉換為本地時間
/// </summary>
/// <param name="utcDateString">UTC 時間字符串</param>
/// <returns>格式化的本地時間字符串</returns>
/// <remarks>
/// 功能特點：
/// 1. 處理 null 或無效日期
/// 2. 自動處理時區轉換
/// 3. 格式化輸出（YYYY-MM-DD HH:mm）
/// 4. 保持時間顯示的一致性
/// </remarks>
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

/// <summary>
/// WebSocket 連接初始化和處理
/// </summary>
let ws;
let reconnectAttempts = 0;
const maxReconnectAttempts = 5;
let isConnected = false;

async function initializeWebSocket() {
    if (isConnected) {
        return;
    }

    try {
        const protocol = window.location.protocol === 'https:' ? 'wss:' : 'ws:';
        const host = window.location.host || 'localhost:7002';
        const wsUrl = `${protocol}//${host}/ws`;
        
        ws = new WebSocket(wsUrl);

        ws.onopen = () => {
            isConnected = true;
            reconnectAttempts = 0;
        };

        ws.onmessage = (event) => {
            if (event.data === 'refresh') {
                loadTasks(currentPage, true);
            }
        };

        ws.onclose = (event) => {
            isConnected = false;
            
            if (reconnectAttempts < maxReconnectAttempts) {
                const delay = Math.min(1000 * Math.pow(2, reconnectAttempts), 10000);
                reconnectAttempts++;
                setTimeout(initializeWebSocket, delay);
            }
        };

        ws.onerror = (error) => {
            isConnected = false;
            
            if (reconnectAttempts < maxReconnectAttempts) {
                const delay = Math.min(1000 * Math.pow(2, reconnectAttempts), 10000);
                reconnectAttempts++;
                setTimeout(initializeWebSocket, delay);
            }
        };
    } catch (error) {
        isConnected = false;
        
        if (reconnectAttempts < maxReconnectAttempts) {
            const delay = Math.min(1000 * Math.pow(2, reconnectAttempts), 10000);
            reconnectAttempts++;
            setTimeout(initializeWebSocket, delay);
        }
    }
}

/// <summary>
/// 頁面卸載時清理資源
/// </summary>
window.addEventListener('beforeunload', () => {
    stopPolling();
    if (ws) {
        ws.close();
    }
});

/// <summary>
/// 頁面加載和卸載事件處理
/// </summary>
document.addEventListener('DOMContentLoaded', () => {
    loadTasks();
    setMinDueDate();
    initializeWebSocket();

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
