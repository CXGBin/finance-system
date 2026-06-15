using SqlSugar;
using FinanceSystem.Core.Entities;

namespace FinanceSystem.Modules.System.Entities;

/// <summary>
/// 操作日志实体
/// </summary>
[SugarTable("sys_log", "系统操作日志表")]
/// <summary>
/// SysLog
/// </summary>
public class SysLog : BaseEntity
{
    /// <summary>
    /// 操作人ID
    /// </summary>
    public long UserId { get; set; }

    /// <summary>
    /// 操作人姓名
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? UserName { get; set; }

    /// <summary>
    /// 所属模块标识
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Module { get; set; }

    /// <summary>
    /// 操作类型（ADD/UPDATE/DELETE/LOGIN/LOGOUT）
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? Action { get; set; }

    /// <summary>
    /// 操作描述
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? Description { get; set; }

    /// <summary>
    /// 客户端IP地址
    /// </summary>
    [SugarColumn(Length = 50, IsNullable = true)]
    public string? IpAddress { get; set; }

    /// <summary>
    /// 请求URL
    /// </summary>
    [SugarColumn(Length = 500, IsNullable = true)]
    public string? RequestUrl { get; set; }

    /// <summary>
    /// HTTP请求方法
    /// </summary>
    [SugarColumn(Length = 10, IsNullable = true)]
    public string? RequestMethod { get; set; }

    /// <summary>
    /// 请求参数（JSON格式）
    /// </summary>
    [SugarColumn(ColumnDataType = "text", IsNullable = true)]
    public string? RequestBody { get; set; }

    /// <summary>
    /// HTTP响应状态码
    /// </summary>
    public int ResponseCode { get; set; }

    /// <summary>
    /// 接口耗时（毫秒）
    /// </summary>
    public int DurationMs { get; set; }
}
