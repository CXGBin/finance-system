using FinanceSystem.Modules.System.Entities;
using FinanceSystem.Modules.Accounts.Entities;
using FinanceSystem.Modules.Approval.Entities;
using FinanceSystem.Modules.Asset.Entities;
using FinanceSystem.Modules.Budget.Entities;
using FinanceSystem.Modules.Expense.Entities;
using FinanceSystem.Modules.Tax.Entities;
using FinanceSystem.Modules.Reports.Entities;
using SqlSugar;
using BCrypt.Net;

namespace FinanceSystem.Api.Data;

/// <summary>
/// 种子数据初始化器
/// 首次启动时自动初始化基础数据：管理员、角色、菜单、部门、模块状态
/// </summary>
public static class SeedData
{
    /// <summary>
    /// 初始化种子数据（仅在数据库为空时执行）
    /// </summary>
    /// <param name="db">SqlSugar客户端</param>
    public static async Task InitializeAsync(ISqlSugarClient db)
    {
        var isSqlite = db.CurrentConnectionConfig.DbType == SqlSugar.DbType.Sqlite;
        if (isSqlite)
        {
            // SQLite模式：用原生SQL创建表（SqlSugar CodeFirst对可空类型处理不兼容SQLite）
            var sqlPath = Path.Combine(AppContext.BaseDirectory, "init-sqlite.sql");
            if (File.Exists(sqlPath))
            {
                var sql = File.ReadAllText(sqlPath);
                db.Ado.ExecuteCommand(sql);
            }
        }
        else
        {
            // SQL Server等：检查表是否已存在（init.sql已创建则跳过CodeFirst）
            var hasModuleTable = db.Ado.GetScalar("SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE='BASE TABLE' AND TABLE_NAME='sys_module'") != null;
            if (!hasModuleTable)
            {
                // 使用CodeFirst自动建表
                db.CodeFirst.InitTables<SysModule>();
                db.CodeFirst.InitTables<SysRole>();
                db.CodeFirst.InitTables<SysDept>();
                db.CodeFirst.InitTables<SysUser>();
                db.CodeFirst.InitTables<SysUserRole>();
                db.CodeFirst.InitTables<SysMenu>();
                db.CodeFirst.InitTables<SysRoleMenu>();
                db.CodeFirst.InitTables<SysPost>();
                db.CodeFirst.InitTables<SysDictType>();
                db.CodeFirst.InitTables<SysDictData>();
                db.CodeFirst.InitTables<SysLog>();
                db.CodeFirst.InitTables<SysConfig>();
                db.CodeFirst.InitTables<SysNotice>();
                db.CodeFirst.InitTables<AccountSubject>();
                db.CodeFirst.InitTables<AccountingPeriod>();
                db.CodeFirst.InitTables<SubjectBalance>();
                db.CodeFirst.InitTables<Voucher>();
                db.CodeFirst.InitTables<VoucherEntry>();
                db.CodeFirst.InitTables<AuxProject>();
                db.CodeFirst.InitTables<AuxCustomer>();
                db.CodeFirst.InitTables<AuxSupplier>();
                db.CodeFirst.InitTables<ApprovalFlow>();
                db.CodeFirst.InitTables<ApprovalInstance>();
                db.CodeFirst.InitTables<ApprovalRecord>();
                db.CodeFirst.InitTables<AssetCategory>();
                db.CodeFirst.InitTables<AssetCard>();
                db.CodeFirst.InitTables<AssetDepreciation>();
                db.CodeFirst.InitTables<AssetChange>();
                db.CodeFirst.InitTables<AssetInventory>();
                db.CodeFirst.InitTables<BudgetYear>();
                db.CodeFirst.InitTables<BudgetSubject>();
                db.CodeFirst.InitTables<BudgetMonthly>();
                db.CodeFirst.InitTables<BudgetAdjustment>();
                db.CodeFirst.InitTables<BudgetAlertConfig>();
                db.CodeFirst.InitTables<ExpenseType>();
                db.CodeFirst.InitTables<ExpenseClaim>();
                db.CodeFirst.InitTables<ExpenseItem>();
                db.CodeFirst.InitTables<ExpenseAllocate>();
                db.CodeFirst.InitTables<TaxCategory>();
                db.CodeFirst.InitTables<TaxDeclaration>();
                db.CodeFirst.InitTables<TaxInvoice>();
                db.CodeFirst.InitTables<ReportTemplate>();
                db.CodeFirst.InitTables<ExpenseLoan>();
            }
            else
            {
                Console.WriteLine("数据库表已存在，跳过CodeFirst建表");
            }
        }

        // 检查是否已有数据，避免重复初始化（逐表检查，支持init.sql部分种子）
        var moduleCount = await db.Queryable<SysModule>().CountAsync();
        var roleCount = await db.Queryable<SysRole>().CountAsync();
        var userCount = await db.Queryable<SysUser>().CountAsync();
        var menuCount = await db.Queryable<SysMenu>().CountAsync();
        var roleMenuCount = await db.Queryable<SysRoleMenu>().CountAsync();
        var userRoleCount = await db.Queryable<SysUserRole>().CountAsync();

        if (moduleCount == 0) await InitModulesAsync(db);
        long roleId = roleCount > 0 ? (await db.Queryable<SysRole>().FirstAsync())!.Id : await InitRoleAsync(db);
        var deptCount = await db.Queryable<SysDept>().CountAsync();
        long deptId = deptCount > 0 ? (await db.Queryable<SysDept>().FirstAsync())!.Id : await InitDeptAsync(db);

        if (userCount == 0)
        {
            var userId = await InitAdminUserAsync(db, roleId, deptId);
        }
        else if (userRoleCount == 0)
        {
            // 用户已存在但未关联角色（如init.sql只建了用户）
            var adminUser = await db.Queryable<SysUser>().FirstAsync(u => u.Username == "admin")!
                ?? await db.Queryable<SysUser>().FirstAsync();
            if (adminUser != null)
            {
                await db.Insertable(new SysUserRole { UserId = adminUser.Id, RoleId = roleId }).ExecuteCommandAsync();
            }
        }

        List<long> menuIds;
        if (menuCount == 0)
        {
            menuIds = await InitMenusAsync(db);
        }
        else
        {
            menuIds = await db.Queryable<SysMenu>().Select(m => m.Id).ToListAsync();
        }

        if (roleMenuCount == 0 && menuIds.Count > 0)
        {
            await InitRoleMenuAsync(db, roleId, menuIds);
        }

        // 初始化审批流程种子数据
        var approvalFlowCount = await db.Queryable<ApprovalFlow>().CountAsync();
        if (approvalFlowCount == 0)
        {
            await InitApprovalFlowsAsync(db);
        }

        // 初始化数据字典种子数据
        var dictTypeCount = await db.Queryable<SysDictType>().CountAsync();
        if (dictTypeCount == 0)
        {
            await InitDictDataAsync(db);
        }

        // 初始化岗位种子数据
        var postCount = await db.Queryable<SysPost>().CountAsync();
        if (postCount == 0)
        {
            await InitPostsAsync(db);
        }

        // 初始化系统公告种子数据
        var noticeCount = await db.Queryable<SysNotice>().CountAsync();
        if (noticeCount == 0)
        {
            await InitNoticeAsync(db);
        }
    }

    /// <summary>
    /// 初始化8个业务模块（全部开启）
    /// </summary>
    private static async Task InitModulesAsync(ISqlSugarClient db)
    {
        var modules = new List<SysModule>
        {
            new() { ModuleId = "system", ModuleName = "系统管理", IsEnabled = 1, IsCore = 1, SortOrder = 1, Description = "核心模块：用户、角色、菜单、部门、字典等基础管理", UpdatedTime = DateTime.Now },
            new() { ModuleId = "account", ModuleName = "账务管理", IsEnabled = 1, IsCore = 1, SortOrder = 2, Description = "核心模块：科目、凭证、账簿、期末处理", Dependencies = "system", UpdatedTime = DateTime.Now },
            new() { ModuleId = "report", ModuleName = "报表中心", IsEnabled = 1, IsCore = 0, SortOrder = 3, Description = "资产负债表、利润表、现金流量表等", Dependencies = "account", UpdatedTime = DateTime.Now },
            new() { ModuleId = "budget", ModuleName = "预算管理", IsEnabled = 1, IsCore = 0, SortOrder = 4, Description = "预算编制、执行跟踪、预警分析", Dependencies = "account,system", UpdatedTime = DateTime.Now },
            new() { ModuleId = "approval", ModuleName = "审批流程", IsEnabled = 1, IsCore = 0, SortOrder = 5, Description = "通用审批能力，可被多模块调用", Dependencies = "system", UpdatedTime = DateTime.Now },
            new() { ModuleId = "asset", ModuleName = "资产管理", IsEnabled = 1, IsCore = 0, SortOrder = 6, Description = "固定资产全生命周期管理", Dependencies = "account", UpdatedTime = DateTime.Now },
            new() { ModuleId = "expense", ModuleName = "费用管理", IsEnabled = 1, IsCore = 0, SortOrder = 7, Description = "报销、费用分摊、费用统计", Dependencies = "account,system,approval", UpdatedTime = DateTime.Now },
            new() { ModuleId = "tax", ModuleName = "税务管理", IsEnabled = 1, IsCore = 0, SortOrder = 8, Description = "税种维护、纳税申报、发票管理", Dependencies = "account", UpdatedTime = DateTime.Now }
        };
        await db.Insertable(modules).ExecuteCommandAsync();
    }

    /// <summary>
    /// 初始化默认管理员角色
    /// </summary>
    /// <returns>角色ID</returns>
    private static async Task<long> InitRoleAsync(ISqlSugarClient db)
    {
        var role = new SysRole
        {
            RoleName = "超级管理员",
            RoleCode = "SUPER_ADMIN",
            Description = "拥有系统全部权限的管理员角色",
            SortOrder = 1,
            Status = 1
        };
        await db.Insertable(role).ExecuteCommandAsync();
        return role.Id;
    }

    /// <summary>
    /// 初始化默认部门
    /// </summary>
    /// <returns>部门ID</returns>
    private static async Task<long> InitDeptAsync(ISqlSugarClient db)
    {
        var dept = new SysDept
        {
            ParentId = 0,
            DeptName = "总公司",
            Leader = "",
            Phone = "",
            Email = "",
            SortOrder = 1,
            Status = 1
        };
        await db.Insertable(dept).ExecuteCommandAsync();
        return dept.Id;
    }

    /// <summary>
    /// 初始化默认管理员账号 admin（随机强密码，首次登录强制修改）
    /// </summary>
    /// <returns>用户ID</returns>
    private static async Task<long> InitAdminUserAsync(ISqlSugarClient db, long roleId, long deptId)
    {
        // 生成16位随机强密码（包含大小写字母、数字和特殊字符）
        var randomPassword = GenerateRandomPassword(16);
        var user = new SysUser
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(randomPassword),
            RealName = "系统管理员",
            Email = "admin@finance.com",
            Phone = "13800000000",
            DeptId = deptId,
            PostId = 0,
            Status = 1,
            LoginFailCount = 0,
            MustChangePassword = true,
            Remark = "系统默认超级管理员",
            UpdatedTime = DateTime.Now
        };
        await db.Insertable(user).ExecuteCommandAsync();

        // 输出初始密码到控制台日志（部署后请及时修改）
        Console.WriteLine($"========================================================");
        Console.WriteLine($"[SEED] 初始管理员账号已创建: admin");
        Console.WriteLine($"[SEED] 初始密码: {randomPassword}");
        Console.WriteLine($"[SEED] 请妥善保管此密码，首次登录后将强制修改");
        Console.WriteLine($"========================================================");

        // 关联用户和角色
        await db.Insertable(new SysUserRole { UserId = user.Id, RoleId = roleId }).ExecuteCommandAsync();
        return user.Id;
    }

    /// <summary>
    /// 初始化完整菜单树（8个模块的目录、菜单、按钮权限）
    /// </summary>
    /// <returns>所有菜单ID列表</returns>
    private static async Task<List<long>> InitMenusAsync(ISqlSugarClient db)
    {
        var menus = new List<SysMenu>();
        long idCounter = 1;

        // 辅助方法：创建一级目录
        SysMenu AddDir(string name, string path, string icon, string moduleId, int sort)
        {
            var m = new SysMenu { Id = idCounter++, ParentId = 0, MenuName = name, MenuType = 1, Path = path, Icon = icon, ModuleId = moduleId, SortOrder = sort, Visible = 1, Status = 1 };
            menus.Add(m);
            return m;
        }

        // 辅助方法：创建二级菜单 + 按钮权限
        void AddSubMenu(SysMenu parent, string name, string path, string icon, string permPrefix, int sort, bool withDelete = true)
        {
            var m = new SysMenu { Id = idCounter++, ParentId = parent.Id, MenuName = name, MenuType = 2, Path = path, Icon = icon, ModuleId = parent.ModuleId, SortOrder = sort, Visible = 1, Status = 1 };
            menus.Add(m);
            // 查看按钮
            menus.Add(new SysMenu { Id = idCounter++, ParentId = m.Id, MenuName = "查看", MenuType = 3, Permission = $"{permPrefix}:list", SortOrder = 1, Visible = 0, Status = 1 });
            // 新增按钮
            menus.Add(new SysMenu { Id = idCounter++, ParentId = m.Id, MenuName = "新增", MenuType = 3, Permission = $"{permPrefix}:add", SortOrder = 2, Visible = 0, Status = 1 });
            // 修改按钮
            menus.Add(new SysMenu { Id = idCounter++, ParentId = m.Id, MenuName = "修改", MenuType = 3, Permission = $"{permPrefix}:edit", SortOrder = 3, Visible = 0, Status = 1 });
            // 删除按钮
            if (withDelete)
                menus.Add(new SysMenu { Id = idCounter++, ParentId = m.Id, MenuName = "删除", MenuType = 3, Permission = $"{permPrefix}:delete", SortOrder = 4, Visible = 0, Status = 1 });
        }

        // 辅助方法：创建自定义按钮（非标准CRUD）
        void AddBtn(SysMenu parent, string name, string permission, int sort)
        {
            menus.Add(new SysMenu { Id = idCounter++, ParentId = parent.Id, MenuName = name, MenuType = 3, Permission = permission, SortOrder = sort, Visible = 0, Status = 1 });
        }

        // ===== 一、系统管理 =====
        var sysDir = AddDir("系统管理", "/system", "setting", "system", 1);
        AddSubMenu(sysDir, "用户管理", "/system/user", "user", "system:user", 1);
        AddSubMenu(sysDir, "角色管理", "/system/role", "team", "system:role", 2);
        AddSubMenu(sysDir, "菜单管理", "/system/menu", "menu", "system:menu", 3);
        AddSubMenu(sysDir, "部门管理", "/system/dept", "apartment", "system:dept", 4);
        AddSubMenu(sysDir, "岗位管理", "/system/post", "idcard", "system:post", 5);
        AddSubMenu(sysDir, "字典管理", "/system/dict", "book", "system:dict", 6);
        AddSubMenu(sysDir, "操作日志", "/system/log", "file-text", "system:log", 7, withDelete: false);
        AddSubMenu(sysDir, "模块管理", "/system/module", "appstore", "system:module", 8, withDelete: false);
        AddSubMenu(sysDir, "系统配置", "/system/config", "tool", "system:config", 9, withDelete: false);

        // ===== 二、账务管理 =====
        var accDir = AddDir("账务管理", "/account", "account-book", "account", 2);
        AddSubMenu(accDir, "会计科目", "/account/subject", "profile", "account:subject", 1);
        var balMenu = new SysMenu { Id = idCounter++, ParentId = accDir.Id, MenuName = "期初余额", MenuType = 2, Path = "/account/balance", Icon = "calculator", ModuleId = "account", SortOrder = 2, Visible = 1, Status = 1 };
        menus.Add(balMenu);
        AddBtn(balMenu, "查看", "account:balance:view", 1);
        AddBtn(balMenu, "编辑", "account:balance:edit", 2);
        AddSubMenu(accDir, "凭证录入", "/account/voucher/add", "form", "account:voucher", 3, withDelete: false);
        var vMenu = new SysMenu { Id = idCounter++, ParentId = accDir.Id, MenuName = "凭证查询", MenuType = 2, Path = "/account/voucher", Icon = "search", ModuleId = "account", SortOrder = 4, Visible = 1, Status = 1 };
        menus.Add(vMenu);
        AddBtn(vMenu, "查看", "account:voucher:list", 1);
        AddBtn(vMenu, "新增", "account:voucher:add", 2);
        AddBtn(vMenu, "修改", "account:voucher:edit", 3);
        AddBtn(vMenu, "审核", "account:voucher:audit", 4);
        AddBtn(vMenu, "作废", "account:voucher:void", 5);
        AddBtn(vMenu, "打印", "account:voucher:print", 6);
        AddSubMenu(accDir, "总账查询", "/account/ledger/general", "table", "account:ledger:general", 5, withDelete: false);
        AddSubMenu(accDir, "明细账查询", "/account/ledger/detail", "unordered-list", "account:ledger:detail", 6, withDelete: false);
        AddSubMenu(accDir, "日记账", "/account/ledger/journal", "switcher", "account:ledger:journal", 7, withDelete: false);
        var periodMenu = new SysMenu { Id = idCounter++, ParentId = accDir.Id, MenuName = "会计期间", MenuType = 2, Path = "/account/period", Icon = "calendar", ModuleId = "account", SortOrder = 8, Visible = 1, Status = 1 };
        menus.Add(periodMenu);
        AddBtn(periodMenu, "查看", "account:period:view", 1);
        AddBtn(periodMenu, "结账", "account:period:close", 2);
        AddBtn(periodMenu, "反结账", "account:period:unclose", 3);
        AddBtn(periodMenu, "损益结转", "account:period:transfer", 4);

        // ===== 三、报表中心 =====
        var rptDir = AddDir("报表中心", "/report", "bar-chart", "report", 3);
        AddSubMenu(rptDir, "资产负债表", "/report/balance-sheet", "pie-chart", "report:balance-sheet", 1, withDelete: false);
        AddSubMenu(rptDir, "利润表", "/report/income-statement", "line-chart", "report:income-statement", 2, withDelete: false);
        AddSubMenu(rptDir, "现金流量表", "/report/cash-flow", "fund", "report:cash-flow", 3, withDelete: false);
        AddSubMenu(rptDir, "科目余额表", "/report/subject-balance", "split-cells", "report:subject-balance", 4, withDelete: false);

        // ===== 四、预算管理 =====
        var bgtDir = AddDir("预算管理", "/budget", "money-collect", "budget", 4);
        AddSubMenu(bgtDir, "预算编制", "/budget/plan", "edit", "budget:plan", 1, withDelete: false);
        AddSubMenu(bgtDir, "预算执行", "/budget/execution", "dashboard", "budget:execution", 2, withDelete: false);
        AddSubMenu(bgtDir, "预算预警", "/budget/alert", "alert", "budget:alert", 3, withDelete: false);
        AddSubMenu(bgtDir, "预算分析", "/budget/analysis", "rise", "budget:analysis", 4, withDelete: false);

        // ===== 五、审批流程 =====
        var apvDir = AddDir("审批流程", "/approval", "audit", "approval", 5);
        AddSubMenu(apvDir, "待办任务", "/approval/pending", "clock-circle", "approval:instance", 1, withDelete: false);
        AddSubMenu(apvDir, "已办任务", "/approval/done", "check-circle", "approval:instance", 2, withDelete: false);
        AddSubMenu(apvDir, "我的申请", "/approval/my", "file-search", "approval:instance", 3, withDelete: false);
        AddSubMenu(apvDir, "审批模板", "/approval/template", "branches", "approval:template", 4);

        // ===== 六、资产管理 =====
        var astDir = AddDir("资产管理", "/asset", "home", "asset", 6);
        AddSubMenu(astDir, "资产列表", "/asset/card", "appstore", "asset:card", 1);
        AddSubMenu(astDir, "资产分类", "/asset/category", "folder", "asset:category", 2);
        AddSubMenu(astDir, "折旧管理", "/asset/depreciation", "field-time", "asset:depreciation", 3, withDelete: false);
        AddSubMenu(astDir, "资产盘点", "/asset/inventory", "inventory", "asset:inventory", 4, withDelete: false);

        // ===== 七、费用管理 =====
        var expDir = AddDir("费用管理", "/expense", "pay-circle", "expense", 7);
        AddSubMenu(expDir, "报销管理", "/expense/claim", "file-add", "expense:claim", 1, withDelete: false);
        AddSubMenu(expDir, "费用分摊", "/expense/allocate", "split", "expense:allocate", 2, withDelete: false);
        AddSubMenu(expDir, "费用统计", "/expense/statistics", "pie-chart", "expense:statistics", 3, withDelete: false);
        AddSubMenu(expDir, "费用类型", "/expense/type", "tags", "expense:type", 4);

        // ===== 八、税务管理 =====
        var taxDir = AddDir("税务管理", "/tax", "bank", "tax", 8);
        AddSubMenu(taxDir, "税种管理", "/tax/type", "percentage", "tax:type", 1);
        AddSubMenu(taxDir, "纳税申报", "/tax/declaration", "file-text", "tax:declare", 2, withDelete: false);
        AddSubMenu(taxDir, "发票管理", "/tax/invoice", "file-done", "tax:invoice", 3);
        AddSubMenu(taxDir, "税务日历", "/tax/calendar", "calendar", "tax:calendar", 4, withDelete: false);

        await db.Insertable(menus).ExecuteCommandAsync();
        return menus.Select(m => m.Id).ToList();
    }

    /// <summary>
    /// 初始化默认审批流程模板
    /// </summary>
    private static async Task InitApprovalFlowsAsync(ISqlSugarClient db)
    {
        var flows = new List<ApprovalFlow>
        {
            new()
            {
                FlowName = "通用费用审批流程",
                FlowCode = "EXPENSE_DEFAULT",
                ModuleType = "expense",
                Description = "默认费用报销审批：申请人→部门经理→财务总监",
                IsEnabled = 1,
                NodesJson = "[{\"NodeName\":\"部门经理审批\",\"IsFinal\":false},{\"NodeName\":\"财务总监审批\",\"IsFinal\":true}]",
                UpdatedTime = DateTime.Now
            },
            new()
            {
                FlowName = "通用预算审批流程",
                FlowCode = "BUDGET_DEFAULT",
                ModuleType = "budget",
                Description = "默认预算审批：申请人→部门经理→财务总监",
                IsEnabled = 1,
                NodesJson = "[{\"NodeName\":\"部门经理审批\",\"IsFinal\":false},{\"NodeName\":\"财务总监审批\",\"IsFinal\":true}]",
                UpdatedTime = DateTime.Now
            },
            new()
            {
                FlowName = "通用资产采购审批",
                FlowCode = "ASSET_DEFAULT",
                ModuleType = "asset",
                Description = "默认资产采购审批：申请人→部门经理→财务总监",
                IsEnabled = 1,
                NodesJson = "[{\"NodeName\":\"部门经理审批\",\"IsFinal\":false},{\"NodeName\":\"财务总监审批\",\"IsFinal\":true}]",
                UpdatedTime = DateTime.Now
            }
        };
        await db.Insertable(flows).ExecuteCommandAsync();
    }

    /// <summary>
    /// 初始化角色菜单关联（将所有菜单权限分配给超级管理员角色）
    /// </summary>
    private static async Task InitRoleMenuAsync(ISqlSugarClient db, long roleId, List<long> menuIds)
    {
        var roleMenus = menuIds.Select(menuId => new SysRoleMenu { RoleId = roleId, MenuId = menuId }).ToList();
        if (roleMenus.Any())
        {
            // 分批插入，避免单次数据量过大
            var batchSize = 100;
            for (var i = 0; i < roleMenus.Count; i += batchSize)
            {
                var batch = roleMenus.Skip(i).Take(batchSize).ToList();
                await db.Insertable(batch).ExecuteCommandAsync();
            }
        }
    }

    /// <summary>
    /// 初始化数据字典种子数据
    /// </summary>
    private static async Task InitDictDataAsync(ISqlSugarClient db)
    {
        // 字典类型
        var dictTypes = new List<SysDictType>
        {
            new() { DictName = "凭证类型", DictType = "voucher_type", Status = 1, Remark = "凭证类型分类" },
            new() { DictName = "费用类型", DictType = "expense_type", Status = 1, Remark = "费用报销分类" },
            new() { DictName = "资产状态", DictType = "asset_status", Status = 1, Remark = "资产使用状态" },
            new() { DictName = "折旧方法", DictType = "depreciation_method", Status = 1, Remark = "固定资产折旧计算方法" },
            new() { DictName = "审批状态", DictType = "approval_status", Status = 1, Remark = "审批流程状态" },
            new() { DictName = "科目类别", DictType = "subject_category", Status = 1, Remark = "会计科目大类" },
        };
        await db.Insertable(dictTypes).ExecuteCommandAsync();

        // 字典数据
        var dictDatas = new List<SysDictData>();

        // 凭证类型
        dictDatas.AddRange(new List<SysDictData>
        {
            new() { DictType = "voucher_type", DictLabel = "记账凭证", DictValue = "1", SortOrder = 1, Status = 1 },
            new() { DictType = "voucher_type", DictLabel = "收款凭证", DictValue = "2", SortOrder = 2, Status = 1 },
            new() { DictType = "voucher_type", DictLabel = "付款凭证", DictValue = "3", SortOrder = 3, Status = 1 },
            new() { DictType = "voucher_type", DictLabel = "转账凭证", DictValue = "4", SortOrder = 4, Status = 1 },
        });

        // 费用类型
        dictDatas.AddRange(new List<SysDictData>
        {
            new() { DictType = "expense_type", DictLabel = "差旅费", DictValue = "travel", SortOrder = 1, Status = 1 },
            new() { DictType = "expense_type", DictLabel = "办公费", DictValue = "office", SortOrder = 2, Status = 1 },
            new() { DictType = "expense_type", DictLabel = "交通费", DictValue = "transport", SortOrder = 3, Status = 1 },
            new() { DictType = "expense_type", DictLabel = "招待费", DictValue = "entertainment", SortOrder = 4, Status = 1 },
            new() { DictType = "expense_type", DictLabel = "通讯费", DictValue = "communication", SortOrder = 5, Status = 1 },
            new() { DictType = "expense_type", DictLabel = "培训费", DictValue = "training", SortOrder = 6, Status = 1 },
        });

        // 资产状态
        dictDatas.AddRange(new List<SysDictData>
        {
            new() { DictType = "asset_status", DictLabel = "使用中", DictValue = "1", SortOrder = 1, Status = 1 },
            new() { DictType = "asset_status", DictLabel = "闲置", DictValue = "2", SortOrder = 2, Status = 1 },
            new() { DictType = "asset_status", DictLabel = "维修中", DictValue = "3", SortOrder = 3, Status = 1 },
            new() { DictType = "asset_status", DictLabel = "已报废", DictValue = "4", SortOrder = 4, Status = 1 },
        });

        // 折旧方法
        dictDatas.AddRange(new List<SysDictData>
        {
            new() { DictType = "depreciation_method", DictLabel = "直线法", DictValue = "1", SortOrder = 1, Status = 1 },
            new() { DictType = "depreciation_method", DictLabel = "双倍余额递减法", DictValue = "2", SortOrder = 2, Status = 1 },
            new() { DictType = "depreciation_method", DictLabel = "年数总和法", DictValue = "3", SortOrder = 3, Status = 1 },
        });

        // 科目类别
        dictDatas.AddRange(new List<SysDictData>
        {
            new() { DictType = "subject_category", DictLabel = "资产类", DictValue = "1", SortOrder = 1, Status = 1 },
            new() { DictType = "subject_category", DictLabel = "负债类", DictValue = "2", SortOrder = 2, Status = 1 },
            new() { DictType = "subject_category", DictLabel = "权益类", DictValue = "3", SortOrder = 3, Status = 1 },
            new() { DictType = "subject_category", DictLabel = "收入类", DictValue = "4", SortOrder = 4, Status = 1 },
            new() { DictType = "subject_category", DictLabel = "费用类", DictValue = "5", SortOrder = 5, Status = 1 },
            new() { DictType = "subject_category", DictLabel = "成本类", DictValue = "6", SortOrder = 6, Status = 1 },
        });

        if (dictDatas.Count > 0)
        {
            await db.Insertable(dictDatas).ExecuteCommandAsync();
        }
    }

    /// <summary>
    /// 初始化岗位种子数据
    /// </summary>
    private static async Task InitPostsAsync(ISqlSugarClient db)
    {
        var posts = new List<SysPost>
        {
            new() { DeptId = 1, PostCode = "CEO", PostName = "总经理", SortOrder = 1, Status = 1, Remark = "公司总经理", UpdatedTime = DateTime.Now },
            new() { DeptId = 1, PostCode = "CFO", PostName = "财务总监", SortOrder = 2, Status = 1, Remark = "财务部门负责人", UpdatedTime = DateTime.Now },
            new() { DeptId = 1, PostCode = "ACCOUNTANT", PostName = "会计", SortOrder = 3, Status = 1, Remark = "负责日常账务处理", UpdatedTime = DateTime.Now },
            new() { DeptId = 1, PostCode = "CASHIER", PostName = "出纳", SortOrder = 4, Status = 1, Remark = "负责现金和银行存款管理", UpdatedTime = DateTime.Now },
            new() { DeptId = 1, PostCode = "AUDITOR", PostName = "审计员", SortOrder = 5, Status = 1, Remark = "负责内部审计", UpdatedTime = DateTime.Now },
            new() { DeptId = 1, PostCode = "MANAGER", PostName = "部门经理", SortOrder = 6, Status = 1, Remark = "各部门负责人", UpdatedTime = DateTime.Now },
        };
        await db.Insertable(posts).ExecuteCommandAsync();
    }

    /// <summary>
    /// 初始化系统公告种子数据
    /// </summary>
    private static async Task InitNoticeAsync(ISqlSugarClient db)
    {
        var notices = new List<SysNotice>
        {
            new() { Title = "欢迎使用财务管理系统", Content = "本系统已正式上线运行，请各部门人员根据权限使用相关功能模块。如遇问题请联系财务部。", NoticeType = 2, Status = 1, CreatedBy = 1, CreatedTime = DateTime.Now },
            new() { Title = "系统操作指南", Content = "1. 首次登录后请修改默认密码\n2. 请先完成会计科目初始化和会计期间设置\n3. 凭证录入后需经审核才能生效\n4. 期末结账前请确保所有凭证已审核", NoticeType = 1, Status = 1, CreatedBy = 1, CreatedTime = DateTime.Now },
        };
        await db.Insertable(notices).ExecuteCommandAsync();
    }

    /// <summary>
    /// 生成指定长度的随机强密码（包含大小写字母、数字和特殊字符）
    /// </summary>
    /// <param name="length">密码长度（最小8位）</param>
    /// <returns>随机强密码</returns>
    private static string GenerateRandomPassword(int length)
    {
        if (length < 8) length = 8;
        const string upperChars = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lowerChars = "abcdefghjkmnpqrstuvwxyz";
        const string digitChars = "23456789";
        const string specialChars = "!@#$%^&*_+-=";
        var allChars = upperChars + lowerChars + digitChars + specialChars;

        // 使用加密安全的随机数生成器
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        var bytes = new byte[length];
        rng.GetBytes(bytes);

        // 确保至少包含每类字符各一个
        var password = new char[length];
        password[0] = upperChars[bytes[0] % upperChars.Length];
        password[1] = lowerChars[bytes[1] % lowerChars.Length];
        password[2] = digitChars[bytes[2] % digitChars.Length];
        password[3] = specialChars[bytes[3] % specialChars.Length];

        // 剩余位置从所有字符中随机选取
        for (var i = 4; i < length; i++)
        {
            password[i] = allChars[bytes[i] % allChars.Length];
        }

        // 打乱顺序（Fisher-Yates洗牌）
        for (var i = length - 1; i > 0; i--)
        {
            var j = bytes[i] % (i + 1);
            (password[i], password[j]) = (password[j], password[i]);
        }

        return new string(password);
    }
}
