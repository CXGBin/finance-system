using FinanceSystem.Core.Common;
using FinanceSystem.Modules.Accounts.DTOs;
using FinanceSystem.Modules.Accounts.Entities;
using SqlSugar;

namespace FinanceSystem.Modules.Accounts.Services;

/// <summary>
/// 会计科目服务实现
/// </summary>
/// <summary>
/// 会计科目服务实现
/// </summary>
public class SubjectService : ISubjectService
{
    private readonly ISqlSugarClient _db;

    /// <summary>
    /// <summary>
    /// 科目管理服务
    /// </summary>
    public SubjectService(ISqlSugarClient db) => _db = db;

    /// <inheritdoc/>
    /// <summary>
    /// <summary>
    /// 获取科目树
    /// </summary>
    public async Task<List<AccountSubject>> GetTreeAsync(bool enabledOnly = true)
    {
        var all = await _db.Queryable<AccountSubject>()
            .WhereIF(enabledOnly, s => s.IsEnabled == 1)
            .OrderBy(s => s.SubjectCode)
            .ToListAsync();
        return BuildTree(all, 0);
    }

    /// <inheritdoc/>
    /// <summary>
    /// <summary>
    /// 获取详情
    /// </summary>
    public async Task<AccountSubject?> GetByIdAsync(long id)
    {
        return await _db.Queryable<AccountSubject>().FirstAsync(s => s.Id == id);
    }

    /// <inheritdoc/>
    /// <summary>
    /// <summary>
    /// 创建
    /// </summary>
    public async Task<long> CreateAsync(SubjectCreateRequest request)
    {
        var exists = await _db.Queryable<AccountSubject>().AnyAsync(s => s.SubjectCode == request.SubjectCode);
        if (exists) throw new BusinessException($"科目编码 '{request.SubjectCode}' 已存在");

        var level = request.SubjectCode.Split('.').Length;
        long? parentId = null;
        if (level > 1)
        {
            var parentCode = string.Join('.', request.SubjectCode.Split('.')[..^1]);
            var parent = await _db.Queryable<AccountSubject>().FirstAsync(s => s.SubjectCode == parentCode);
            if (parent == null) throw new BusinessException($"上级科目 '{parentCode}' 不存在，请先创建");
            parentId = parent.Id;
        }

        var subject = new AccountSubject
        {
            SubjectCode = request.SubjectCode,
            SubjectName = request.SubjectName,
            ParentId = parentId,
            SubjectLevel = level,
            SubjectType = request.SubjectType,
            BalanceDirection = request.BalanceDirection,
            IsEnabled = request.IsEnabled,
            IsCash = request.IsCash,
            IsBank = request.IsBank,
            AuxiliaryType = request.AuxiliaryType,
            SortOrder = request.SortOrder,
            Remark = request.Remark
        };

        await _db.Insertable(subject).ExecuteCommandAsync();
        return subject.Id;
    }

    /// <inheritdoc/>
    /// <summary>
    /// <summary>
    /// 修改
    /// </summary>
    public async Task UpdateAsync(long id, SubjectCreateRequest request)
    {
        var subject = await _db.Queryable<AccountSubject>().FirstAsync(s => s.Id == id)
            ?? throw new NotFoundException("科目不存在");

        subject.SubjectName = request.SubjectName;
        subject.SubjectType = request.SubjectType;
        subject.BalanceDirection = request.BalanceDirection;
        subject.IsEnabled = request.IsEnabled;
        subject.IsCash = request.IsCash;
        subject.IsBank = request.IsBank;
        subject.AuxiliaryType = request.AuxiliaryType;
        subject.SortOrder = request.SortOrder;
        subject.Remark = request.Remark;

        await _db.Updateable(subject).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    /// <summary>
    /// <summary>
    /// 删除
    /// </summary>
    public async Task DeleteAsync(long id)
    {
        var subject = await _db.Queryable<AccountSubject>().FirstAsync(s => s.Id == id)
            ?? throw new NotFoundException("科目不存在");

        var hasChild = await _db.Queryable<AccountSubject>().AnyAsync(s => s.ParentId == id);
        if (hasChild) throw new BusinessException("该科目下有子科目，请先删除子科目");

        var hasEntry = await _db.Queryable<VoucherEntry>().AnyAsync(e => e.SubjectId == id);
        if (hasEntry) throw new BusinessException("该科目已有凭证发生额，仅可停用，不可删除");

        await _db.Deleteable<AccountSubject>().Where(s => s.Id == id).ExecuteCommandAsync();
    }

    /// <inheritdoc/>
    /// <summary>
    /// <summary>
    /// 启用/停用
    /// </summary>
    public async Task ToggleStatusAsync(long id, int isEnabled)
    {
        var subject = await _db.Queryable<AccountSubject>().FirstAsync(s => s.Id == id)
            ?? throw new NotFoundException("科目不存在");

        subject.IsEnabled = isEnabled;
        await _db.Updateable(subject).UpdateColumns(s => s.IsEnabled).ExecuteCommandAsync();
    }

    /// <summary>
    /// 构建科目树形结构
    /// </summary>
    private List<AccountSubject> BuildTree(List<AccountSubject> all, long parentId)
    {
        return all.Where(s => s.ParentId == parentId).Select(s =>
        {
            s.Children = BuildTree(all, s.Id);
            return s;
        }).ToList();
    }
}
