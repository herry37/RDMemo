using System.ComponentModel.DataAnnotations;
using TodoTaskManagementAPI.Domain;

namespace TodoTaskManagementAPI.Models
{
    /// <summary>
    /// 更新任務請求模型
    /// </summary>
    public class UpdateTodoTaskRequest
    {
        /// <summary>
        /// 任務標題
        /// </summary>
        [Required(ErrorMessage = "標題是必填的")]
        [StringLength(200, ErrorMessage = "標題最多200個字符")]
        public string Title { get; set; }

        /// <summary>
        /// 任務描述
        /// </summary>
        [StringLength(1000, ErrorMessage = "描述最多1000個字符")]
        public string Description { get; set; }

        /// <summary>
        /// 任務優先級
        /// </summary>
        public Priority Priority { get; set; }

        /// <summary>
        /// 任務截止日期
        /// </summary>
        public DateTime? DueDate { get; set; }
    }
}
