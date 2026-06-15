using FinanceSystem.Core.Common;
using FinanceSystem.Modules.Accounts.DTOs;
using FinanceSystem.Modules.Accounts.Entities;

namespace FinanceSystem.Modules.Accounts.Services;

/// <summary>
/// 会计科目服务接口
/// </summary>
/// <summary>
/// 会计科目服务接口
/// </summary>
public interface ISubjectService
{
    /// <summary>
    /// 获取科目树
    /// </summary>
    /// <param name="enabledOnly">是否仅启用的科目</param>
    /// <returns>科目树形列表</returns>
    Task<List<AccountSubject>> GetTreeAsync(bool enabledOnly = true);

    /// <summary>
    /// 获取科目详情
    /// </summary>
    Task<AccountSubject?> GetByIdAsync(long id);

    /// <summary>
    /// 新增科目
    /// </summary>
    /// <returns>新科目ID</returns>
    Task<long> CreateAsync(SubjectCreateRequest request);

    /// <summary>
    /// 修改科目
    /// </summary>
    Task UpdateAsync(long id, SubjectCreateRequest request);

    /// <summary>
    /// 删除科目（校验无发生额）
    /// </summary>
    Task DeleteAsync(long id);

    /// <summary>
    /// 启用/停用科目
    /// </summary>
    Task ToggleStatusAsync(long id, int isEnabled);
}

/// <summary>
/// 凭证管理服务接口
/// </summary>
/// <summary>
/// 凭证服务接口
/// </summary>
public interface IVoucherService
{
    /// <summary>
    /// 分页查询凭证
    /// </summary>
    Task<PageResult<Voucher>> GetPageAsync(VoucherQuery query);

    /// <summary>
    /// 获取凭证详情（含分录）
    /// </summary>
    Task<Voucher?> GetByIdAsync(long id);

    /// <summary>
    /// 新增凭证（草稿）
    /// </summary>
    /// <returns>新凭证ID</returns>
    Task<long> CreateAsync(VoucherCreateRequest request, long currentUserId);

    /// <summary>
    /// 修改凭证（仅草稿）
    /// </summary>
    Task UpdateAsync(long id, VoucherCreateRequest request);

    /// <summary>
    /// 审核凭证
    /// </summary>
    Task AuditAsync(long id, long currentUserId);

    /// <summary>
    /// 反审核凭证
    /// </summary>
    Task UnAuditAsync(long id);

    /// <summary>
    /// 作废凭证
    /// </summary>
    Task VoidAsync(long id);

    /// <summary>
    /// 复制凭证（生成草稿副本）
    /// </summary>
    Task<long> CopyAsync(long id, long currentUserId);
    /// <summary>
    /// 删除凭证（仅草稿状态）
    /// </summary>
    Task DeleteAsync(long id);

    /// <summary>
    /// 批量审核凭证
    /// </summary>
    Task BatchAuditAsync(List<long> ids, long currentUserId);

    /// <summary>
    /// 批量作废凭证
    /// </summary>
    Task BatchVoidAsync(List<long> ids);

    /// <summary>
    /// 红字冲销凭证（生成原凭证的借贷反向红字凭证）
    /// </summary>
    Task<long> ReverseAsync(long originalId, long currentUserId);
}

/// <summary>
/// 会计期间服务接口
/// </summary>
/// <summary>
/// 会计期间服务接口
/// </summary>
public interface IPeriodService
{
    /// <summary>
    /// 获取年度期间列表
    /// </summary>
    /// <param name="year">年度</param>
    Task<List<AccountingPeriod>> GetListByYearAsync(int year);

    /// <summary>
    /// 获取当前期间
    /// </summary>
    Task<AccountingPeriod?> GetCurrentAsync();

    /// <summary>
    /// 初始化年度期间（12个月）
    /// </summary>
    Task InitYearAsync(int year);

    /// <summary>
    /// 期末结账
    /// </summary>
    Task CloseAsync(long periodId, long currentUserId);

    /// <summary>
    /// 反结账
    /// </summary>
    Task UnCloseAsync(long periodId);

    /// <summary>
    /// 期末损益结转
    /// </summary>
    Task ProfitTransferAsync(long periodId, long currentUserId);
}

/// <summary>
/// 科目期初余额服务接口
/// </summary>
/// <summary>
/// 科目余额服务接口
/// </summary>
public interface ISubjectBalanceService
{
    /// <summary>
    /// 查询某期间期初余额
    /// </summary>
    Task<List<SubjectBalance>> GetByPeriodAsync(long periodId);

    /// <summary>
    /// 批量保存期初余额
    /// </summary>
    Task SaveAsync(List<SubjectBalanceRequest> items);

    /// <summary>
    /// 试算平衡校验
    /// </summary>
    /// <returns>是否平衡</returns>
    Task<(bool IsBalanced, decimal DebitTotal, decimal CreditTotal)> TrialBalanceAsync(long periodId);
}

/// <summary>
/// 账簿查询服务接口
/// </summary>
/// <summary>
/// 账簿查询服务接口
/// </summary>
public interface ILedgerService
{
    /// <summary>
    /// 总账查询
    /// </summary>
    Task<List<SubjectBalance>> GetGeneralLedgerAsync(LedgerQuery query);

    /// <summary>
    /// 明细账查询
    /// </summary>
    Task<PageResult<object>> GetDetailLedgerAsync(LedgerQuery query);

    /// <summary>
    /// 现金/银行日记账查询
    /// </summary>
    Task<PageResult<object>> GetJournalAsync(LedgerQuery query);

    /// <summary>
    /// 科目汇总表
    /// </summary>
    Task<List<SubjectBalance>> GetSubjectSummaryAsync(int year, int month);
}

/// <summary>
/// 辅助核算服务接口
/// </summary>
/// <summary>
/// 辅助核算服务接口
/// </summary>
public interface IAuxiliaryService
{
    /// <summary>
    /// 获取辅助核算项列表
    /// </summary>
    /// <param name="type">类型（project/customer/supplier）</param>
    /// <param name="query">查询条件</param>
    Task<object> GetListAsync(string type, AuxiliaryQuery query);

    /// <summary>
    /// 新增辅助核算项
    /// </summary>
    Task<long> CreateAsync(string type, object request);

    /// <summary>
    /// 修改辅助核算项
    /// </summary>
    Task UpdateAsync(string type, long id, object request);

    /// <summary>
    /// 删除辅助核算项
    /// </summary>
    Task DeleteAsync(string type, long id);
}
