using System.ComponentModel.DataAnnotations;

namespace TodoTaskManagementAPI.Models
{
    /// <summary>
    /// 更新任務狀態請求模型
    /// </summary>
    public class UpdateTaskStatusRequest
    {
        /// <summary>
        /// 是否已完成
        /// </summary>
        [Required(ErrorMessage = "完成狀態是必填的")]
        public bool IsCompleted { get; set; }
    }
}
