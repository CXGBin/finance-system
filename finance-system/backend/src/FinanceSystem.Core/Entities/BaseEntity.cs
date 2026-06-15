namespace FinanceSystem.Core.Entities;

/// <summary>
/// 实体基类（包含公共字段）
/// </summary>
/// <summary>
/// BaseEntity
/// </summary>
public class BaseEntity
{
    /// <summary>
    /// 主键
    /// </summary>
    [SqlSugar.SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    /// <summary>
    /// 主键ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 创建时间
    /// </summary>
    public DateTime CreatedTime { get; set; } = DateTime.Now;
}
