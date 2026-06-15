using FinanceSystem.Core.Common;

using FinanceSystem.Modules.Reports.DTOs;
using SqlSugar;

namespace FinanceSystem.Modules.Reports.Services;

/// <summary>
/// 资产负债表服务接口
/// </summary>
public interface IBalanceSheetService
{
    /// <summary>
    /// 生成资产负债表
    /// </summary>
    Task<BalanceSheetResult> GenerateAsync(ReportQuery query);
}

/// <summary>
/// 利润表服务接口
/// </summary>
public interface IIncomeStatementService
{
    /// <summary>
    /// 生成利润表
    /// </summary>
    Task<IncomeStatementResult> GenerateAsync(IncomeStatementQuery query);
}

/// <summary>
/// 现金流量表服务接口
/// </summary>
public interface ICashFlowService
{
    /// <summary>
    /// 生成现金流量表
    /// </summary>
    Task<CashFlowResult> GenerateAsync(ReportQuery query);
}

/// <summary>
/// 科目余额表服务接口
/// </summary>
public interface ISubjectBalanceReportService
{
    /// <summary>
    /// 查询科目余额表
    /// </summary>
    Task<object> GetReportAsync(SubjectBalanceReportQuery query);
}

/// <summary>
/// 自定义报表服务接口
/// </summary>
public interface ICustomReportService
{
    /// <summary>
    /// 获取模板列表
    /// </summary>
    Task<PageResult<object>> GetTemplatesAsync(int pageIndex = 1, int pageSize = 20);

    /// <summary>
    /// 创建模板
    /// </summary>
    Task<long> CreateTemplateAsync(ReportTemplateRequest request, long currentUserId);

    /// <summary>
    /// 修改模板
    /// </summary>
    Task UpdateTemplateAsync(long id, ReportTemplateRequest request);

    /// <summary>
    /// 删除模板
    /// </summary>
    Task DeleteTemplateAsync(long id);

    /// <summary>
    /// 按模板生成报表数据
    /// </summary>
    Task<object> GenerateCustomReportAsync(long templateId, string period);
}

/// <summary>
/// 报表导出服务接口
/// </summary>
public interface IReportExportService
{
    /// <summary>
    /// 导出报表（Excel/PDF）
    /// </summary>
    Task<string> ExportAsync(ExportQuery query);
}

/// <summary>
/// 多期对比服务接口
/// </summary>
public interface ICompareService
{
    /// <summary>
    /// 多期对比分析
    /// </summary>
    Task<CompareResult> CompareAsync(CompareQuery query);
}
