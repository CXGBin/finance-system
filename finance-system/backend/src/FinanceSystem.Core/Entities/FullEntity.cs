namespace FinanceSystem.Core.Entities;

/// <summary>
/// 带更新时间的实体基类
/// </summary>
public class FullEntity : BaseEntity
{
    /// <summary>
    /// 更新时间
    /// </summary>
    public DateTime? UpdatedTime { get; set; }
}
