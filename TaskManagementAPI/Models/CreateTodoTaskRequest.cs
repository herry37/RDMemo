/// <summary>
/// 任務管理 API 的請求/響應模型定義
/// </summary>

using System.ComponentModel.DataAnnotations;
using TodoTaskManagementAPI.Domain;

namespace TodoTaskManagementAPI.Models
{
    /// <summary>
    /// 創建待辦事項任務的請求模型
    /// 用於接收客戶端創建新任務的數據
    /// </summary>
    /// <remarks>
    /// 此模型用於：
    /// 1. 驗證客戶端提交的任務數據
    /// 2. 定義任務創建所需的必要信息
    /// 3. 提供任務屬性的默認值
    /// 
    /// 數據驗證：
    /// - 使用 DataAnnotations 進行屬性驗證
    /// - 確保數據符合業務規則
    /// - 提供友好的錯誤信息
    /// </remarks>
    public class CreateTodoTaskRequest
    {
        /// <summary>
        /// 任務標題
        /// 簡短描述任務的主要內容
        /// </summary>
        /// <remarks>
        /// 驗證規則：
        /// 1. 必填字段，不能為 null 或空字符串
        /// 2. 最大長度為 200 個字符
        /// 3. 會自動去除首尾空格
        /// 
        /// 錯誤信息：
        /// - 為空時："標題是必填的"
        /// - 超長時："標題最多200個字符"
        /// </remarks>
        [Required(ErrorMessage = "標題是必填的")]
        [StringLength(200, ErrorMessage = "標題最多200個字符")]
        public string Title { get; set; }

        /// <summary>
        /// 任務描述
        /// 詳細說明任務的具體內容、要求和注意事項
        /// </summary>
        /// <remarks>
        /// 驗證規則：
        /// 1. 可選字段，允許為 null
        /// 2. 最大長度為 1000 個字符
        /// 3. 支持多行文本
        /// 
        /// 錯誤信息：
        /// - 超長時："描述最多1000個字符"
        /// </remarks>
        [StringLength(1000, ErrorMessage = "描述最多1000個字符")]
        public string? Description { get; set; }

        /// <summary>
        /// 任務優先級
        /// 指定任務的重要程度和緊急程度
        /// </summary>
        /// <remarks>
        /// 特點：
        /// 1. 默認值為 Priority.Low
        /// 2. 使用枚舉確保值的有效性
        /// 3. 用於任務的排序和分類
        /// 
        /// 可選值：
        /// - Low：低優先級
        /// - Medium：中優先級
        /// - High：高優先級
        /// </remarks>
        public Priority Priority { get; set; } = Priority.Low;

        /// <summary>
        /// 任務截止日期
        /// 指定任務需要完成的最後期限
        /// </summary>
        /// <remarks>
        /// 特點：
        /// 1. 可選字段，允許為 null
        /// 2. 使用 UTC 時間存儲
        /// 3. 用於任務逾期判斷
        /// 
        /// 建議：
        /// - 建議設置在當前時間之後
        /// - 考慮時區轉換
        /// - 用於提醒和報告生成
        /// </remarks>
        public DateTime? DueDate { get; set; }
    }
}
