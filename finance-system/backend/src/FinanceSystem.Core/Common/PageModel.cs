namespace FinanceSystem.Core.Common;

/// <summary>
/// 分页请求参数
/// </summary>
/// <summary>
/// PageRequest
/// </summary>
public class PageRequest
{
    /// <summary>
    /// 页码（从1开始）
    /// </summary>
    public int PageIndex { get; set; } = 1;

    /// <summary>
    /// 每页条数
    /// </summary>
    public int PageSize { get; set; } = 20;

    /// <summary>
    /// 排序字段
    /// </summary>
    public string? SortField { get; set; }

    /// <summary>
    /// 排序方向：asc/desc
    /// </summary>
    public string? SortOrder { get; set; }
}

/// <summary>
/// 分页响应数据
/// </summary>
/// <typeparam name="T">数据项类型</typeparam>
/// <summary>
/// PageResult
/// </summary>
public class PageResult<T>
{
    /// <summary>
    /// 数据列表
    /// </summary>
    public List<T> List { get; set; } = new();

    /// <summary>
    /// 总记录数
    /// </summary>
    public int Total { get; set; }

    /// <summary>
    /// 当前页码
    /// </summary>
    public int PageIndex { get; set; }

    /// <summary>
    /// 每页条数
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// 总页数
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)Total / PageSize) : 0;

    /// <summary>
    /// 创建分页结果
    /// </summary>
    /// <param name="total">总记录数</param>
    /// <param name="list">数据列表</param>
    public PageResult(int total, List<T> list)
    {
        Total = total;
        List = list;
    }
}
