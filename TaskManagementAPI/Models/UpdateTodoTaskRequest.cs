/// <summary>
/// 任務管理 API 的請求模型定義
/// 用於更新任務信息的相關操作
/// </summary>

using System.ComponentModel.DataAnnotations;
using TodoTaskManagementAPI.Domain;

namespace TodoTaskManagementAPI.Models
{
    /// <summary>
    /// 更新待辦事項任務的請求模型
    /// 用於接收客戶端更新任務信息的請求
    /// </summary>
    /// <remarks>
    /// 此模型用於：
    /// 1. 接收任務更新請求
    /// 2. 驗證更新數據的有效性
    /// 3. 提供完整的任務更新介面
    /// 
    /// 特點：
    /// - 包含所有可更新的任務屬性
    /// - 使用數據注解進行驗證
    /// - 支持部分更新操作
    /// 
    /// 注意事項：
    /// - 不包含任務 ID，ID 通過路由參數傳遞
    /// - 不包含任務狀態，狀態通過專門的 API 更新
    /// - 不包含審計字段（創建時間、更新時間）
    /// </remarks>
    public class UpdateTodoTaskRequest
    {
        /// <summary>
        /// 任務標題
        /// 用於更新任務的主要描述文本
        /// </summary>
        /// <remarks>
        /// 驗證規則：
        /// 1. 必填字段，不能為 null 或空字符串
        /// 2. 最大長度為 200 個字符
        /// 3. 會自動去除首尾空格
        /// 
        /// 使用建議：
        /// - 保持簡潔明瞭
        /// - 避免使用特殊字符
        /// - 確保描述準確性
        /// </remarks>
        [Required(ErrorMessage = "標題是必填的")]
        [StringLength(200, ErrorMessage = "標題最多200個字符")]
        public string Title { get; set; }

        /// <summary>
        /// 任務描述
        /// 用於更新任務的詳細說明文本
        /// </summary>
        /// <remarks>
        /// 驗證規則：
        /// 1. 可選字段，允許為 null
        /// 2. 最大長度為 1000 個字符
        /// 3. 支持多行文本
        /// 
        /// 使用建議：
        /// - 提供完整的任務上下文
        /// - 包含重要的參考信息
        /// - 使用格式化文本提高可讀性
        /// </remarks>
        [StringLength(1000, ErrorMessage = "描述最多1000個字符")]
        public string Description { get; set; }

        /// <summary>
        /// 任務優先級
        /// 用於更新任務的重要程度和緊急程度
        /// </summary>
        /// <remarks>
        /// 特點：
        /// 1. 使用枚舉類型確保值的有效性
        /// 2. 默認值由系統處理
        /// 3. 影響任務的排序和展示
        /// 
        /// 可選值：
        /// - Low：低優先級任務
        /// - Medium：中優先級任務
        /// - High：高優先級任務
        /// 
        /// 使用場景：
        /// - 調整任務優先順序
        /// - 重新規劃工作重點
        /// - 資源分配優化
        /// </remarks>
        public Priority Priority { get; set; }

        /// <summary>
        /// 任務截止日期
        /// 用於更新任務需要完成的最後期限
        /// </summary>
        /// <remarks>
        /// 特點：
        /// 1. 可選字段，允許為 null
        /// 2. 使用 UTC 時間存儲
        /// 3. 支持日期和時間
        /// 
        /// 使用建議：
        /// - 設置合理的完成期限
        /// - 考慮時區轉換
        /// - 與項目里程碑同步
        /// 
        /// 業務規則：
        /// - 可以清除現有截止日期（設為 null）
        /// - 建議設置在當前時間之後
        /// - 用於任務逾期判斷
        /// </remarks>
        public DateTime? DueDate { get; set; }
    }
}
