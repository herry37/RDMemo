import { useEffect, useState, useCallback, memo } from "react";
import { TaskItem } from "./components/TaskItem";
import { useTasks } from "./hooks/useTasks";
import PropTypes from "prop-types";

export default function App() {
  const {
    tasks,
    loading,
    fetchTasks,
    addTask,
    updateTask,
    deleteTask,
    searchTasks,
  } = useTasks();
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [status, setStatus] = useState("全部");
  const [filterPriority, setFilterPriority] = useState("全部");
  const [priority, setPriority] = useState("低優先級");
  const [dateInput, setDateInput] = useState("");
  const [timeInput, setTimeInput] = useState("");
  const [dueDate, setDueDate] = useState("");
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [errorMessage, setErrorMessage] = useState("");
  const [showError, setShowError] = useState(false);

  // 查詢任務
  const handleSearch = useCallback(async () => {
    try {
      const params = {};
      if (status !== "全部") {
        params.status = status;
      }
      if (filterPriority !== "全部") {
        params.priority = filterPriority;
      }

      await searchTasks(params);
    } catch (err) {
      console.error("查詢失敗:", err);
      setErrorMessage({
        message: "查詢失敗",
        detail: err.response?.data?.error || err.message || "發生未知錯誤",
      });
      setShowError(true);
    }
  }, [status, filterPriority, searchTasks]);

  // 只在初始載入時獲取所有任務
  useEffect(() => {
    fetchTasks();
  }, [fetchTasks]);

  // 新增錯誤提示組件，支援更詳細的錯誤說明
  const ErrorMessage = memo(({ message, detail, onClose }) => {
    return (
      <div className="fixed top-4 right-4 bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded shadow-lg max-w-md">
        <div className="flex justify-between items-start">
          <div>
            <p className="font-bold">{message}</p>
            {detail && <p className="text-sm mt-1">{detail}</p>}
          </div>
          <button
            onClick={onClose}
            className="ml-4 text-red-700 hover:text-red-900"
          >
            ✕
          </button>
        </div>
      </div>
    );
  });
  ErrorMessage.displayName = "ErrorMessage";

  ErrorMessage.propTypes = {
    message: PropTypes.string.isRequired,
    detail: PropTypes.string,
    onClose: PropTypes.func.isRequired,
  };

  // 修改日期驗證函數
  const validateDate = (dateStr, isComplete = false) => {
    if (!dateStr) return { isValid: true };

    // 如果日期尚未輸入完整且不是強制驗證，則不進行驗證
    if (!isComplete && dateStr.length < 10) {
      return { isValid: true };
    }

    const datePattern = /^(\d{4})-(\d{2})-(\d{2})$/;
    const match = dateStr.match(datePattern);

    if (!match) {
      return { isValid: true }; // 輸入中不顯示錯誤
    }

    const year = parseInt(match[1], 10);
    const month = parseInt(match[2], 10);
    const day = parseInt(match[3], 10);

    // 檢查月份
    if (month < 1 || month > 12) {
      return {
        isValid: false,
        message: "日期錯誤",
        detail: `${month} 月不存在，請輸入 1-12 之間的月份`,
      };
    }

    // 檢查日期
    const isLeapYear = (year % 4 === 0 && year % 100 !== 0) || year % 400 === 0;
    const daysInMonth = [
      31,
      isLeapYear ? 29 : 28,
      31,
      30,
      31,
      30,
      31,
      31,
      30,
      31,
      30,
      31,
    ];
    const monthNames = [
      "一",
      "二",
      "三",
      "四",
      "五",
      "六",
      "七",
      "八",
      "九",
      "十",
      "十一",
      "十二",
    ];

    if (day < 1 || day > daysInMonth[month - 1]) {
      return {
        isValid: false,
        message: "日期不存在",
        detail: `${year}年${monthNames[month - 1]}月只有 ${
          daysInMonth[month - 1]
        } 天${
          month === 2 ? `（${isLeapYear ? "潤" : "平"}年）` : ""
        }，${day} 日不存在`,
      };
    }

    // 只在日期完整時才檢查是否為過去的日期
    if (isComplete) {
      const inputDate = new Date(year, month - 1, day);
      const today = new Date();
      today.setHours(0, 0, 0, 0);

      if (inputDate < today) {
        return {
          isValid: false,
          message: "日期錯誤",
          detail: "不能選擇過去的日期",
        };
      }
    }

    return { isValid: true };
  };

  // 修改錯誤提示函數
  const showErrorMessage = (message, detail) => {
    setErrorMessage({ message, detail });
    setShowError(true);
    setTimeout(() => {
      setShowError(false);
      setErrorMessage({ message: "", detail: "" });
    }, 5000); // 延長顯示時間到 5 秒
  };

  // 修改 handleAddTask
  const handleAddTask = async () => {
    try {
      if (!title.trim()) {
        showErrorMessage("標題錯誤", "請輸入任務標題");
        return;
      }

      if (!dateInput || !timeInput) {
        showErrorMessage("截止時間錯誤", "請同時輸入日期和時間");
        return;
      }

      const validation = validateDate(dateInput);
      if (!validation.isValid) {
        showErrorMessage(validation.message, validation.detail);
        return;
      }

      const result = await addTask({
        title,
        description,
        priority,
        dueDate,
      });

      if (result) {
        setTitle("");
        setDescription("");
        setPriority("低優先級");
        setDateInput("");
        setTimeInput("");
        setDueDate("");
        setIsModalOpen(false);
      }
    } catch (error) {
      console.error("新增任務失敗:", error);
      showErrorMessage("新增失敗", error.message || "發生未知錯誤");
    }
  };

  // 優化選單渲染
  const FilterSection = memo(
    ({
      status,
      setStatus,
      filterPriority,
      setFilterPriority,
      loading,
      handleSearch,
      setIsModalOpen,
    }) => (
      <div className="flex items-center gap-4 mb-6">
        <div className="flex items-center gap-2">
          <span className="text-white">完成狀態</span>
          <select
            className="border border-gray-700 rounded px-4 py-1 bg-black text-white w-24"
            value={status}
            onChange={(e) => setStatus(e.target.value)}
          >
            <option>全部</option>
            <option>完成</option>
            <option>未完成</option>
          </select>
        </div>

        <div className="flex items-center gap-2">
          <span className="text-white">優先級</span>
          <select
            className="border border-gray-700 rounded px-4 py-1 bg-black text-white w-26"
            value={filterPriority}
            onChange={(e) => setFilterPriority(e.target.value)}
          >
            <option>全部</option>
            <option>低優先級</option>
            <option>中優先級</option>
            <option>高優先級</option>
          </select>
        </div>

        <button
          className="px-4 py-1 bg-blue-500 text-white rounded hover:bg-blue-600 disabled:opacity-50"
          onClick={handleSearch}
          disabled={loading} // 避免重複點擊
        >
          {loading ? "查詢中..." : "查詢"}
        </button>

        <button
          className="px-4 py-1 bg-green-500 text-white rounded hover:bg-green-600"
          onClick={() => setIsModalOpen(true)}
        >
          新增任務
        </button>
      </div>
    )
  );
  FilterSection.displayName = "FilterSection";

  FilterSection.propTypes = {
    status: PropTypes.string.isRequired,
    setStatus: PropTypes.func.isRequired,
    filterPriority: PropTypes.string.isRequired,
    setFilterPriority: PropTypes.func.isRequired,
    loading: PropTypes.bool.isRequired,
    handleSearch: PropTypes.func.isRequired,
    setIsModalOpen: PropTypes.func.isRequired,
  };

  return (
    <div className="min-h-screen min-w-full bg-[#1e1e1e] absolute inset-0">
      <div className="flex flex-col h-full">
        <h1 className="text-2xl font-bold text-white p-4 text-center">
          待辦事項管理系統
        </h1>

        <div className="flex-1 flex justify-center">
          <div className="w-[800px]">
            <FilterSection
              status={status}
              setStatus={setStatus}
              filterPriority={filterPriority}
              setFilterPriority={setFilterPriority}
              loading={loading}
              handleSearch={handleSearch}
              setIsModalOpen={setIsModalOpen}
            />

            <div className="space-y-4">
              {tasks.map((task) => (
                <TaskItem
                  key={task.id}
                  task={task}
                  onEdit={updateTask}
                  onDelete={deleteTask}
                />
              ))}
            </div>
          </div>
        </div>
      </div>

      {isModalOpen && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center">
          <div className="bg-white rounded-lg p-6 w-[500px]">
            <div className="flex justify-between items-center mb-4">
              <h2 className="text-xl font-bold">新增任務</h2>
              <button
                onClick={() => setIsModalOpen(false)}
                className="text-gray-500 hover:text-gray-700"
              >
                ✕
              </button>
            </div>

            <div className="space-y-4">
              <div>
                <label className="block text-gray-700 mb-2">任務標題</label>
                <input
                  type="text"
                  className="w-full border rounded px-3 py-2"
                  placeholder="請輸入任務標題"
                  value={title}
                  onChange={(e) => setTitle(e.target.value)}
                />
              </div>

              <div>
                <label className="block text-gray-700 mb-2">任務描述</label>
                <input
                  type="text"
                  className="w-full border rounded px-3 py-2"
                  placeholder="請輸入任務描述"
                  value={description}
                  onChange={(e) => setDescription(e.target.value)}
                />
              </div>

              <div>
                <label className="block text-gray-700 mb-2">優先級</label>
                <select
                  className="w-full border rounded px-3 py-2"
                  value={priority}
                  onChange={(e) => setPriority(e.target.value)}
                >
                  <option value="低優先級">低優先級</option>
                  <option value="中優先級">中優先級</option>
                  <option value="高優先級">高優先級</option>
                </select>
              </div>

              <div>
                <label className="block text-gray-700 mb-2">
                  截止時間
                  <span className="text-red-500 ml-1">*</span>
                </label>
                <div className="flex gap-2">
                  <input
                    type="date"
                    className="flex-1 border rounded px-3 py-2 min-w-[200px]"
                    value={dateInput}
                    onInput={(e) => {
                      // 直接更新輸入值，不進行驗證
                      setDateInput(e.target.value);
                    }}
                    onBlur={(e) => {
                      // 在失去焦點時才進行驗證
                      const newDate = e.target.value;

                      // 如果日期為空，直接返回
                      if (!newDate) return;

                      // 確保日期完整才進行驗證
                      if (newDate.length === 10) {
                        const [year, month, day] = newDate
                          .split("-")
                          .map((num) => parseInt(num, 10));

                        // 檢查日期是否有效
                        const isLeapYear =
                          (year % 4 === 0 && year % 100 !== 0) ||
                          year % 400 === 0;
                        const daysInMonth = [
                          31,
                          isLeapYear ? 29 : 28,
                          31,
                          30,
                          31,
                          30,
                          31,
                          31,
                          30,
                          31,
                          30,
                          31,
                        ];
                        const monthNames = [
                          "一",
                          "二",
                          "三",
                          "四",
                          "五",
                          "六",
                          "七",
                          "八",
                          "九",
                          "十",
                          "十一",
                          "十二",
                        ];

                        if (
                          month >= 1 &&
                          month <= 12 &&
                          (day < 1 || day > daysInMonth[month - 1])
                        ) {
                          showErrorMessage(
                            "日期不存在",
                            `${year}年${monthNames[month - 1]}月只有 ${
                              daysInMonth[month - 1]
                            } 天${
                              month === 2
                                ? `（${isLeapYear ? "潤" : "平"}年）`
                                : ""
                            }，${day} 日不存在`
                          );
                          return;
                        }

                        // 檢查是否為過去的日期
                        const inputDate = new Date(year, month - 1, day);
                        const today = new Date();
                        today.setHours(0, 0, 0, 0);

                        if (inputDate < today) {
                          showErrorMessage("日期錯誤", "不能選擇過去的日期");
                          return;
                        }

                        // 更新完整日期時間
                        if (timeInput) {
                          setDueDate(`${newDate}T${timeInput}:00`);
                        }
                      }
                    }}
                    min={new Date().toISOString().split("T")[0]}
                    max={
                      new Date(
                        new Date().setFullYear(new Date().getFullYear() + 10)
                      )
                        .toISOString()
                        .split("T")[0]
                    }
                  />
                  <input
                    type="time"
                    className="w-[150px] border rounded px-3 py-2"
                    value={timeInput}
                    onChange={(e) => {
                      const newTime = e.target.value;
                      setTimeInput(newTime);
                      if (dateInput) {
                        setDueDate(`${dateInput}T${newTime}:00`);
                      }
                    }}
                    required
                  />
                </div>
              </div>

              <button
                className="w-full bg-green-500 text-white rounded py-2 hover:bg-green-600"
                onClick={handleAddTask}
              >
                新增任務
              </button>
            </div>
          </div>
        </div>
      )}

      {showError && (
        <ErrorMessage
          message={errorMessage.message}
          detail={errorMessage.detail}
          onClose={() => {
            setShowError(false);
            setErrorMessage({ message: "", detail: "" });
          }}
        />
      )}
    </div>
  );
}
