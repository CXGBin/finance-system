using FinanceSystem.Core.Common;
using FinanceSystem.Modules.Budget.DTOs;
using FinanceSystem.Modules.Budget.Entities;
using SqlSugar;

namespace FinanceSystem.Modules.Budget.Services;

/// <summary>
/// 预算服务接口
/// </summary>
/// <summary>
/// 预算年度服务接口
/// </summary>
public interface IBudgetYearService
{
    /// <summary>获取预算年度列表</summary>
    Task<List<BudgetYear>> GetListAsync(BudgetYearQuery query);
    /// <summary>创建预算年度</summary>
    Task<long> CreateAsync(BudgetYearRequest request, long currentUserId);
    /// <summary>更新年度状态</summary>
    Task UpdateStatusAsync(long id, int status);
}

/// <summary>
/// 预算科目服务接口
/// </summary>
/// <summary>
/// 预算科目服务接口
/// </summary>
public interface IBudgetSubjectService
{
    /// <summary>获取预算科目列表</summary>
    Task<PageResult<BudgetSubject>> GetListAsync(long yearId, int pageIndex = 1, int pageSize = 20, string? sortField = null, string? sortOrder = null);
    /// <summary>新增预算科目</summary>
    Task<long> CreateAsync(BudgetSubjectRequest request);
    /// <summary>修改预算科目</summary>
    Task UpdateAsync(long id, BudgetSubjectRequest request);
    /// <summary>删除预算科目</summary>
    Task DeleteAsync(long id);
}

/// <summary>
/// 月度预算服务接口
/// </summary>
/// <summary>
/// 月度预算服务接口
/// </summary>
public interface IBudgetMonthlyService
{
    /// <summary>获取月度预算</summary>
    Task<List<BudgetMonthly>> GetBySubjectAsync(long budgetSubjectId);
    /// <summary>保存月度预算</summary>
    Task SaveAsync(BudgetMonthlySaveRequest request);
    /// <summary>自动平均拆分年度预算到月度</summary>
    Task AutoSplitAsync(long budgetSubjectId);
}

/// <summary>
/// 预算执行跟踪服务接口
/// </summary>
/// <summary>
/// 预算执行跟踪服务接口
/// </summary>
public interface IBudgetExecutionService
{
    /// <summary>查询预算执行情况</summary>
    Task<List<BudgetExecutionItem>> GetExecutionAsync(BudgetExecutionQuery query);
}

/// <summary>
/// 预算调整服务接口
/// </summary>
/// <summary>
/// 预算调整服务接口
/// </summary>
public interface IBudgetAdjustService
{
    /// <summary>发起预算调整</summary>
    Task<long> CreateAdjustAsync(BudgetAdjustRequest request, long currentUserId);
    /// <summary>审批预算调整</summary>
    Task ApproveAdjustAsync(long id, int action, long currentUserId);
}

/// <summary>
/// 预算预警服务接口
/// </summary>
/// <summary>
/// 预算预警服务接口
/// </summary>
public interface IBudgetAlertService
{
    /// <summary>获取预警配置</summary>
    Task<BudgetAlertConfig?> GetConfigAsync(long budgetYearId);
    /// <summary>保存预警配置</summary>
    Task SaveConfigAsync(BudgetAlertConfigRequest request);
    /// <summary>检查并返回超预警列表</summary>
    Task<List<BudgetExecutionItem>> CheckAlertsAsync(long budgetYearId);
}

/// <summary>
/// 预算分析服务接口
/// </summary>
/// <summary>
/// 预算分析服务接口
/// </summary>
public interface IBudgetAnalysisService
{
    /// <summary>科目对比分析（预算vs实际）</summary>
    Task<List<object>> GetSubjectCompareAsync(int year);
    /// <summary>月度执行趋势</summary>
    Task<List<object>> GetMonthlyTrendAsync(int year);
    /// <summary>费用TOP10排名</summary>
    Task<List<object>> GetExpenseTop10Async(int year);
    /// <summary>综合分析概览</summary>
    Task<object> GetOverviewAsync(int year);
}
