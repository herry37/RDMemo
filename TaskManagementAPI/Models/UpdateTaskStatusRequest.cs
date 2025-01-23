/// <summary>
/// 任務管理 API 的請求模型定義
/// 用於更新任務狀態的相關操作
/// </summary>

using System.ComponentModel.DataAnnotations;

namespace TodoTaskManagementAPI.Models
{
    /// <summary>
    /// 更新任務完成狀態的請求模型
    /// 用於接收客戶端更新任務狀態的請求
    /// </summary>
    /// <remarks>
    /// 此模型用於：
    /// 1. 接收任務狀態更新請求
    /// 2. 驗證狀態更新的有效性
    /// 3. 提供簡潔的狀態更新介面
    /// 
    /// 特點：
    /// - 只包含必要的狀態信息
    /// - 使用數據注解進行驗證
    /// - 支持幂等操作
    /// </remarks>
    public class UpdateTaskStatusRequest
    {
        /// <summary>
        /// 任務完成狀態
        /// 表示任務是否已經完成
        /// </summary>
        /// <remarks>
        /// 驗證規則：
        /// 1. 必填字段，不能為 null
        /// 2. 布爾值類型
        /// 
        /// 值含義：
        /// - true：標記任務為已完成
        /// - false：標記任務為未完成
        /// 
        /// 使用場景：
        /// - 任務完成標記
        /// - 任務狀態切換
        /// - 批量狀態更新
        /// </remarks>
        [Required(ErrorMessage = "完成狀態是必填的")]
        public bool IsCompleted { get; set; }
    }
}
