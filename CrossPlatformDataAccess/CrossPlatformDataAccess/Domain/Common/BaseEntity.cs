using System;

namespace CrossPlatformDataAccess.Domain.Common
{
    /// <summary>
    /// 所有實體的基礎類別
    /// </summary>
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; }
        public DateTime Created { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? LastModified { get; set; }
        public string LastModifiedBy { get; set; }
    }
}
