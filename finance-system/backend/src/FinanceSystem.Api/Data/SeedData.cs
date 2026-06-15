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
    /// 初始化默认管理员账号 admin/admin123
    /// </summary>
    /// <returns>用户ID</returns>
    private static async Task<long> InitAdminUserAsync(ISqlSugarClient db, long roleId, long deptId)
    {
        var user = new SysUser
        {
            Username = "admin",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
            RealName = "系统管理员",
            Email = "admin@finance.com",
            Phone = "13800000000",
            DeptId = deptId,
            PostId = 0,
            Status = 1,
            LoginFailCount = 0,
            Remark = "系统默认超级管理员",
            UpdatedTime = DateTime.Now
        };
        await db.Insertable(user).ExecuteCommandAsync();

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

}
