using FinanceSystem.Core.Entities;
using SqlSugar;

namespace FinanceSystem.Modules.System.Entities;

/// <summary>
/// 系统公告实体
/// </summary>
[SugarTable("sys_notice")]
/// <summary>
/// SysNotice
/// </summary>
public class SysNotice : BaseEntity
{
    /// <summary>公告标题</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>公告内容</summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>公告类型（1通知 2公告）</summary>
    public int NoticeType { get; set; }

    /// <summary>状态（0禁用 1启用）</summary>
    public int Status { get; set; }

    /// <summary>创建人ID</summary>
    public long CreatedBy { get; set; }
}
