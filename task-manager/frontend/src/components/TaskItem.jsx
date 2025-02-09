import PropTypes from "prop-types";

export const TaskItem = ({ task, onEdit, onDelete }) => {
  const {
    id,
    title,
    description,
    completed,
    priority: taskPriority,
    dueDate: taskDueDate,
  } = task;

  const status = completed ? "已完成" : "未完成";
  const priority = taskPriority || "低優先級";
  const dueDate = taskDueDate || "未設定";

  return (
    <div className="border border-gray-700 rounded p-4 bg-[#1e1e1e] text-white">
      <div className="flex flex-col gap-2">
        <div className="flex justify-between items-start">
          <div>
            <h3
              className={`text-lg font-medium ${
                completed ? "line-through text-gray-500" : ""
              }`}
            >
              {title} #{id}
            </h3>
            <p className="text-sm text-gray-400 mt-1">
              {description || "無描述"}
            </p>
          </div>
          <div className="flex gap-2">
            <button
              onClick={() => onEdit({ ...task, completed: !completed })}
              className={`px-4 py-1 rounded ${
                completed
                  ? "bg-gray-500 hover:bg-gray-600"
                  : "bg-[#20c997] hover:bg-[#1ba97e]"
              } text-white`}
            >
              {completed ? "取消完成" : "完成"}
            </button>
            <button
              onClick={() => onDelete(id)}
              className="px-4 py-1 bg-[#dc3545] text-white rounded hover:bg-[#c82333]"
            >
              刪除
            </button>
          </div>
        </div>

        <div className="text-sm text-gray-400">
          優先級: {priority} | 截止日期: {dueDate} | 狀態: {status}
        </div>
      </div>
    </div>
  );
};

TaskItem.propTypes = {
  task: PropTypes.shape({
    id: PropTypes.number.isRequired,
    title: PropTypes.string.isRequired,
    description: PropTypes.string,
    completed: PropTypes.bool,
    priority: PropTypes.string,
    dueDate: PropTypes.string,
  }).isRequired,
  onEdit: PropTypes.func.isRequired,
  onDelete: PropTypes.func.isRequired,
};
