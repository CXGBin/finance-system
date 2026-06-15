/**
 * 财务管理系统 - SQL Server 建表脚本
 * 生成于 2026-06-15
 * 8个模块，26张表
 */

-- AccountSubject: fm_account_subject
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_account_subject' AND xtype='U')
BEGIN
CREATE TABLE [fm_account_subject] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [SubjectCode] NVARCHAR(20) NOT NULL,
    [SubjectName] NVARCHAR(20) NOT NULL,
    [ParentId] BIGINT NOT NULL,
    [SubjectLevel] INT NOT NULL,
    [SubjectType] INT NOT NULL,
    [BalanceDirection] INT NOT NULL,
    [IsEnabled] INT NOT NULL,
    [IsCash] INT NOT NULL,
    [IsBank] INT NOT NULL,
    [AuxiliaryType] NVARCHAR(50) NULL,
    [SortOrder] INT NULL,
    [Remark] NVARCHAR(500) NULL
    CONSTRAINT [PK_fm_account_subject] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- AccountingPeriod: fm_period
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_period' AND xtype='U')
BEGIN
CREATE TABLE [fm_period] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [PeriodYear] INT NOT NULL,
    [PeriodMonth] INT NOT NULL,
    [BeginDate] DATETIME NOT NULL,
    [EndDate] DATETIME NOT NULL,
    [IsClosed] INT NOT NULL,
    [ClosedTime] DATETIME NOT NULL,
    [ClosedBy] BIGINT NOT NULL
    CONSTRAINT [PK_fm_period] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- AuxCustomer: fm_aux_customer
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_aux_customer' AND xtype='U')
BEGIN
CREATE TABLE [fm_aux_customer] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [CustomerCode] NVARCHAR(50) NOT NULL,
    [CustomerName] NVARCHAR(50) NOT NULL,
    [Contact] NVARCHAR(100) NULL,
    [Phone] NVARCHAR(50) NULL,
    [Address] NVARCHAR(20) NULL,
    [TaxNo] NVARCHAR(200) NULL,
    [Status] INT NULL
    CONSTRAINT [PK_fm_aux_customer] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- AuxProject: fm_aux_project
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_aux_project' AND xtype='U')
BEGIN
CREATE TABLE [fm_aux_project] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [ProjectCode] NVARCHAR(50) NOT NULL,
    [ProjectName] NVARCHAR(50) NOT NULL,
    [Manager] NVARCHAR(100) NULL,
    [BeginDate] DATETIME NULL,
    [EndDate] DATETIME NOT NULL,
    [Status] INT NOT NULL
    CONSTRAINT [PK_fm_aux_project] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- AuxSupplier: fm_aux_supplier
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_aux_supplier' AND xtype='U')
BEGIN
CREATE TABLE [fm_aux_supplier] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [SupplierCode] NVARCHAR(50) NOT NULL,
    [SupplierName] NVARCHAR(50) NOT NULL,
    [Contact] NVARCHAR(100) NULL,
    [Phone] NVARCHAR(50) NULL,
    [Address] NVARCHAR(20) NULL,
    [TaxNo] NVARCHAR(200) NULL,
    [BankName] NVARCHAR(50) NULL,
    [BankAccount] NVARCHAR(100) NULL,
    [Status] INT NULL
    CONSTRAINT [PK_fm_aux_supplier] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- SubjectBalance: fm_subject_balance
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_subject_balance' AND xtype='U')
BEGIN
CREATE TABLE [fm_subject_balance] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [SubjectId] BIGINT NOT NULL,
    [PeriodId] BIGINT NOT NULL,
    [BeginDebit] DECIMAL(18,2) NOT NULL,
    [BeginCredit] DECIMAL(18,2) NOT NULL,
    [CurrentDebit] DECIMAL(18,2) NOT NULL,
    [CurrentCredit] DECIMAL(18,2) NOT NULL,
    [EndDebit] DECIMAL(18,2) NOT NULL,
    [EndCredit] DECIMAL(18,2) NOT NULL
    CONSTRAINT [PK_fm_subject_balance] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- Voucher: fm_voucher
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_voucher' AND xtype='U')
BEGIN
CREATE TABLE [fm_voucher] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [VoucherNo] NVARCHAR(20) NOT NULL,
    [VoucherDate] DATETIME NOT NULL,
    [PeriodId] BIGINT NOT NULL,
    [VoucherType] INT NOT NULL,
    [AbstractText] NVARCHAR(500) NULL,
    [Status] INT NULL,
    [TotalDebit] DECIMAL(18,2) NOT NULL,
    [TotalCredit] DECIMAL(18,2) NOT NULL,
    [PreparedBy] BIGINT NOT NULL,
    [ReviewedBy] BIGINT NOT NULL,
    [ReviewedTime] DATETIME NOT NULL
    CONSTRAINT [PK_fm_voucher] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- VoucherEntry: fm_voucher_entry
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_voucher_entry' AND xtype='U')
BEGIN
CREATE TABLE [fm_voucher_entry] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [VoucherId] BIGINT NOT NULL,
    [Summary] NVARCHAR(200) NULL,
    [SubjectId] BIGINT NULL,
    [DebitAmount] DECIMAL(18,2) NOT NULL,
    [CreditAmount] DECIMAL(18,2) NOT NULL,
    [AuxiliaryId] BIGINT NOT NULL,
    [AuxiliaryType] NVARCHAR(50) NULL,
    [Subject] NVARCHAR(MAX) NULL
    CONSTRAINT [PK_fm_voucher_entry] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- ApprovalFlow: fm_approval_flow
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_approval_flow' AND xtype='U')
BEGIN
CREATE TABLE [fm_approval_flow] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedTime] DATETIME NULL,
    [FlowName] NVARCHAR(100) NOT NULL,
    [FlowCode] NVARCHAR(100) NOT NULL,
    [ModuleType] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(50) NOT NULL,
    [IsEnabled] INT NULL,
    [NodesJson] NVARCHAR(MAX) NOT NULL,
    [FlowId] BIGINT NOT NULL,
    [BusinessId] BIGINT NOT NULL,
    [ModuleType] NVARCHAR(50) NOT NULL,
    [Title] NVARCHAR(50) NOT NULL,
    [InitiatorId] BIGINT NOT NULL,
    [CurrentNodeIndex] INT NOT NULL,
    [Status] INT NOT NULL,
    [DeptId] BIGINT NOT NULL,
    [InstanceId] BIGINT NOT NULL,
    [NodeIndex] INT NOT NULL,
    [NodeName] NVARCHAR(100) NOT NULL,
    [ApproverId] BIGINT NOT NULL,
    [Action] INT NOT NULL,
    [Comment] NVARCHAR(1000) NOT NULL,
    [ApproveTime] DATETIME NULL
    CONSTRAINT [PK_fm_approval_flow] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- AssetCategory: fm_asset_category
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_asset_category' AND xtype='U')
BEGIN
CREATE TABLE [fm_asset_category] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedTime] DATETIME NULL,
    [ParentId] BIGINT NULL,
    [CategoryCode] NVARCHAR(50) NULL,
    [CategoryName] NVARCHAR(50) NULL,
    [DepreciationMethod] INT NOT NULL,
    [UsefulLifeMonths] INT NOT NULL,
    [ResidualRate] DECIMAL(18,2) NOT NULL,
    [SortOrder] INT NOT NULL,
    [IsEnabled] INT NOT NULL,
    [AssetCode] NVARCHAR(50) NOT NULL,
    [AssetName] NVARCHAR(50) NOT NULL,
    [CategoryId] BIGINT NOT NULL,
    [Specification] NVARCHAR(200) NOT NULL,
    [OriginalValue] DECIMAL(18,2) NOT NULL,
    [ResidualRate] DECIMAL(18,2) NULL,
    [ResidualValue] DECIMAL(18,2) NOT NULL,
    [DepreciationMethod] INT NOT NULL,
    [UsefulLifeMonths] INT NOT NULL,
    [AcquisitionDate] DATETIME NOT NULL,
    [DeptId] BIGINT NOT NULL,
    [Keeper] NVARCHAR(50) NOT NULL,
    [Location] NVARCHAR(50) NULL,
    [Status] INT NULL,
    [AccumulatedDepreciation] DECIMAL(18,2) NULL,
    [NetValue] DECIMAL(18,2) NOT NULL,
    [Remark] NVARCHAR(500) NOT NULL,
    [AssetCardId] BIGINT NOT NULL,
    [PeriodId] BIGINT NOT NULL,
    [Month] INT NOT NULL,
    [DepreciationAmount] DECIMAL(18,2) NOT NULL,
    [AccumulatedDepreciation] DECIMAL(18,2) NOT NULL,
    [NetValue] DECIMAL(18,2) NOT NULL,
    [AssetCardId] BIGINT NOT NULL,
    [ChangeType] INT NOT NULL,
    [Reason] NVARCHAR(500) NOT NULL,
    [FromDeptId] BIGINT NOT NULL,
    [ToDeptId] BIGINT NOT NULL,
    [DisposalIncome] DECIMAL(18,2) NULL,
    [OperatorId] BIGINT NULL
    CONSTRAINT [PK_fm_asset_category] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- BudgetYear: fm_budget_year
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_budget_year' AND xtype='U')
BEGIN
CREATE TABLE [fm_budget_year] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedTime] DATETIME NULL,
    [Year] INT NOT NULL,
    [Status] INT NOT NULL,
    [Description] NVARCHAR(500) NOT NULL,
    [CreatedBy] BIGINT NULL,
    [BudgetYearId] BIGINT NOT NULL,
    [SubjectId] BIGINT NOT NULL,
    [DeptId] BIGINT NOT NULL,
    [AnnualAmount] DECIMAL(18,2) NULL,
    [Remark] NVARCHAR(500) NOT NULL,
    [BudgetSubjectId] BIGINT NOT NULL,
    [Month] INT NOT NULL,
    [Amount] DECIMAL(18,2) NOT NULL,
    [BudgetSubjectId] BIGINT NOT NULL,
    [AdjustType] INT NOT NULL,
    [BeforeAmount] DECIMAL(18,2) NOT NULL,
    [AfterAmount] DECIMAL(18,2) NOT NULL,
    [Reason] NVARCHAR(500) NOT NULL,
    [ApproveStatus] INT NOT NULL,
    [ApplyDeptId] BIGINT NOT NULL,
    [ApplyBy] BIGINT NULL,
    [BudgetYearId] BIGINT NOT NULL,
    [AlertThreshold] DECIMAL(18,2) NOT NULL,
    [IsEnabled] INT NOT NULL,
    [AlertMethod] INT NOT NULL
    CONSTRAINT [PK_fm_budget_year] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- ExpenseType: fm_expense_type
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_expense_type' AND xtype='U')
BEGIN
CREATE TABLE [fm_expense_type] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedTime] DATETIME NULL,
    [TypeCode] NVARCHAR(50) NOT NULL,
    [TypeName] NVARCHAR(50) NOT NULL,
    [SubjectId] BIGINT NOT NULL,
    [SingleLimit] DECIMAL(18,2) NOT NULL,
    [MonthlyLimit] DECIMAL(18,2) NULL,
    [SortOrder] INT NULL,
    [IsEnabled] INT NULL,
    [ClaimNo] NVARCHAR(50) NOT NULL,
    [Title] NVARCHAR(50) NOT NULL,
    [ClaimantId] BIGINT NOT NULL,
    [DeptId] BIGINT NOT NULL,
    [TotalAmount] DECIMAL(18,2) NOT NULL,
    [Status] INT NULL,
    [ApprovalInstanceId] BIGINT NOT NULL,
    [PaymentDate] DATETIME NOT NULL,
    [VoucherId] BIGINT NULL,
    [Remark] NVARCHAR(500) NULL,
    [ClaimId] BIGINT NOT NULL,
    [ExpenseTypeId] BIGINT NOT NULL,
    [Description] NVARCHAR(200) NOT NULL,
    [Amount] DECIMAL(18,2) NOT NULL,
    [ExpenseDate] DATETIME NOT NULL,
    [InvoiceNo] NVARCHAR(50) NOT NULL
    CONSTRAINT [PK_fm_expense_type] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- ReportTemplate: fm_report_template
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_report_template' AND xtype='U')
BEGIN
CREATE TABLE [fm_report_template] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedTime] DATETIME NULL,
    [TemplateName] NVARCHAR(100) NOT NULL,
    [Description] NVARCHAR(100) NOT NULL,
    [TemplateData] NVARCHAR(500) NULL,
    [CreatedBy] BIGINT NOT NULL
    CONSTRAINT [PK_fm_report_template] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- SysConfig: sys_config
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_config' AND xtype='U')
BEGIN
CREATE TABLE [sys_config] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [ConfigGroup] NVARCHAR(50) NOT NULL,
    [ConfigKey] NVARCHAR(50) NOT NULL,
    [ConfigValue] NVARCHAR(100) NOT NULL,
    [ConfigName] NVARCHAR(500) NOT NULL,
    [Remark] NVARCHAR(100) NULL
    CONSTRAINT [PK_sys_config] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- SysDept: sys_dept
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_dept' AND xtype='U')
BEGIN
CREATE TABLE [sys_dept] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [ParentId] BIGINT NOT NULL,
    [DeptName] NVARCHAR(50) NOT NULL,
    [SortOrder] INT NOT NULL,
    [Leader] NVARCHAR(50) NULL,
    [Phone] NVARCHAR(50) NULL,
    [Email] NVARCHAR(20) NULL,
    [Status] INT NULL
    CONSTRAINT [PK_sys_dept] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- SysDictData: sys_dict_data
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_dict_data' AND xtype='U')
BEGIN
CREATE TABLE [sys_dict_data] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [DictType] NVARCHAR(100) NOT NULL,
    [DictLabel] NVARCHAR(100) NOT NULL,
    [DictValue] NVARCHAR(100) NOT NULL,
    [SortOrder] INT NOT NULL,
    [Status] INT NOT NULL,
    [Remark] NVARCHAR(200) NULL
    CONSTRAINT [PK_sys_dict_data] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- SysDictType: sys_dict_type
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_dict_type' AND xtype='U')
BEGIN
CREATE TABLE [sys_dict_type] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [DictName] NVARCHAR(100) NOT NULL,
    [DictType] NVARCHAR(100) NOT NULL,
    [Status] INT NOT NULL,
    [Remark] NVARCHAR(200) NULL
    CONSTRAINT [PK_sys_dict_type] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- SysLog: sys_log
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_log' AND xtype='U')
BEGIN
CREATE TABLE [sys_log] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [UserId] BIGINT NOT NULL,
    [UserName] NVARCHAR(50) NULL,
    [Module] NVARCHAR(50) NULL,
    [Action] NVARCHAR(50) NULL,
    [Description] NVARCHAR(50) NULL,
    [IpAddress] NVARCHAR(500) NULL,
    [RequestUrl] NVARCHAR(50) NULL,
    [RequestMethod] NVARCHAR(500) NULL,
    [RequestBody] NVARCHAR(10) NULL,
    [ResponseCode] INT NULL,
    [DurationMs] INT NOT NULL
    CONSTRAINT [PK_sys_log] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- SysMenu: sys_menu
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_menu' AND xtype='U')
BEGIN
CREATE TABLE [sys_menu] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [ParentId] BIGINT NOT NULL,
    [MenuName] NVARCHAR(50) NOT NULL,
    [MenuType] INT NOT NULL,
    [Path] NVARCHAR(200) NULL,
    [Component] NVARCHAR(200) NULL,
    [Permission] NVARCHAR(200) NULL,
    [Icon] NVARCHAR(100) NULL,
    [ModuleId] NVARCHAR(50) NULL,
    [SortOrder] INT NULL,
    [Visible] INT NOT NULL,
    [Status] INT NOT NULL
    CONSTRAINT [PK_sys_menu] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- SysModule: sys_module
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_module' AND xtype='U')
BEGIN
CREATE TABLE [sys_module] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedTime] DATETIME NULL,
    [ModuleId] NVARCHAR(50) NOT NULL,
    [ModuleName] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(100) NULL,
    [IsEnabled] INT NULL,
    [IsCore] INT NOT NULL,
    [SortOrder] INT NOT NULL,
    [Dependencies] NVARCHAR(500) NULL
    CONSTRAINT [PK_sys_module] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- SysPost: sys_post
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_post' AND xtype='U')
BEGIN
CREATE TABLE [sys_post] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedTime] DATETIME NULL,
    [DeptId] BIGINT NOT NULL,
    [PostCode] NVARCHAR(50) NOT NULL,
    [PostName] NVARCHAR(50) NOT NULL,
    [SortOrder] INT NOT NULL,
    [Status] INT NOT NULL,
    [Remark] NVARCHAR(200) NULL
    CONSTRAINT [PK_sys_post] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- SysRole: sys_role
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_role' AND xtype='U')
BEGIN
CREATE TABLE [sys_role] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [RoleName] NVARCHAR(50) NOT NULL,
    [RoleCode] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(50) NULL,
    [SortOrder] INT NULL,
    [Status] INT NOT NULL
    CONSTRAINT [PK_sys_role] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- SysRoleMenu: sys_role_menu
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_role_menu' AND xtype='U')
BEGIN
CREATE TABLE [sys_role_menu] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [RoleId] BIGINT NOT NULL,
    [MenuId] BIGINT NOT NULL
    CONSTRAINT [PK_sys_role_menu] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- SysUser: sys_user
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_user' AND xtype='U')
BEGIN
CREATE TABLE [sys_user] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedTime] DATETIME NULL,
    [Username] NVARCHAR(50) NOT NULL,
    [PasswordHash] NVARCHAR(50) NOT NULL,
    [RealName] NVARCHAR(256) NOT NULL,
    [Email] NVARCHAR(50) NULL,
    [Phone] NVARCHAR(100) NULL,
    [Avatar] NVARCHAR(20) NULL,
    [DeptId] BIGINT NULL,
    [PostId] BIGINT NOT NULL,
    [Status] INT NOT NULL,
    [Remark] NVARCHAR(500) NULL,
    [LoginFailCount] INT NULL,
    [LockoutEndTime] DATETIME NOT NULL
    CONSTRAINT [PK_sys_user] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- SysUserRole: sys_user_role
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_user_role' AND xtype='U')
BEGIN
CREATE TABLE [sys_user_role] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [UserId] BIGINT NOT NULL,
    [RoleId] BIGINT NOT NULL
    CONSTRAINT [PK_sys_user_role] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

-- TaxCategory: fm_tax_category
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_tax_category' AND xtype='U')
BEGIN
CREATE TABLE [fm_tax_category] (
    [Id] BIGINT IDENTITY(1,1) NOT NULL,
    [CreatedTime] DATETIME NOT NULL DEFAULT GETDATE(),
    [UpdatedTime] DATETIME NULL,
    [TaxCode] NVARCHAR(50) NOT NULL,
    [TaxName] NVARCHAR(50) NOT NULL,
    [TaxRate] DECIMAL(18,4) NOT NULL,
    [CalculationMethod] INT NOT NULL,
    [DeclareCycle] INT NOT NULL,
    [SubjectId] BIGINT NOT NULL,
    [IsEnabled] INT NOT NULL,
    [Remark] NVARCHAR(500) NULL,
    [TaxCategoryId] BIGINT NOT NULL,
    [DeclarePeriod] NVARCHAR(10) NOT NULL,
    [TaxAmount] DECIMAL(18,2) NOT NULL,
    [ActualPaidAmount] DECIMAL(18,2) NOT NULL,
    [Status] INT NOT NULL,
    [DeclaredBy] BIGINT NOT NULL,
    [Remark] NVARCHAR(500) NOT NULL,
    [InvoiceType] INT NOT NULL,
    [InvoiceNo] NVARCHAR(50) NOT NULL,
    [InvoiceDate] DATETIME NOT NULL,
    [CounterpartyName] NVARCHAR(50) NOT NULL,
    [TaxAmount] DECIMAL(18,2) NOT NULL,
    [AmountWithoutTax] DECIMAL(18,2) NOT NULL,
    [TotalAmount] DECIMAL(18,2) NOT NULL,
    [Direction] INT NOT NULL,
    [VoucherId] BIGINT NOT NULL,
    [IsVerified] INT NOT NULL,
    [Remark] NVARCHAR(500) NULL
    CONSTRAINT [PK_fm_tax_category] PRIMARY KEY CLUSTERED ([Id] ASC)
);
END
GO

 ====================
-- 凭证期间索引
CREATE NONCLUSTERED INDEX [IX_fm_voucher_period] ON [fm_voucher] ([PeriodId], [VoucherDate]);
GO
-- 凭证分录凭证ID索引
CREATE NONCLUSTERED INDEX [IX_fm_voucher_entry_voucher] ON [fm_voucher_entry] ([VoucherId]);
GO
-- 用户状态索引
CREATE NONCLUSTERED INDEX [IX_sys_user_status] ON [sys_user] ([Status], [UserName]);
GO
-- 报销单状态索引
CREATE NONCLUSTERED INDEX [IX_fm_expense_claim_status] ON [fm_expense_claim] ([Status], [ClaimantId]);
GO
-- 资产卡片分类索引
CREATE NONCLUSTERED INDEX [IX_fm_asset_card_category] ON [fm_asset_card] ([CategoryId], [Status]);
GO
-- 发票方向+认证索引
CREATE NONCLUSTERED INDEX [IX_fm_tax_invoice_direction] ON [fm_tax_invoice] ([Direction], [IsVerified]);
GO
-- 纳税申报税种+期间索引
CREATE NONCLUSTERED INDEX [IX_fm_tax_declaration_tax_period] ON [fm_tax_declaration] ([TaxCategoryId], [DeclarePeriod]);
GO
-- 折旧明细资产+期间索引
CREATE NONCLUSTERED INDEX [IX_fm_asset_depreciation_asset_period] ON [fm_asset_depreciation] ([AssetCardId], [PeriodId]);
GO

-- ==================== 种子数据 ====================
-- 模块定义（8个模块全开）
IF NOT EXISTS (SELECT 1 FROM sys_module WHERE ModuleId = 'system')
BEGIN
    INSERT INTO sys_module (ModuleId, ModuleName, IsEnabled, IsCore, SortOrder, Description, Dependencies) VALUES
    ('system', '系统管理', 1, 1, 1, '核心模块：用户、角色、菜单、部门、字典等', NULL),
    ('account', '账务管理', 1, 1, 2, '核心模块：科目、凭证、账簿、期末处理', 'system'),
    ('report', '报表中心', 1, 0, 3, '资产负债表、利润表、现金流量表等', 'account'),
    ('budget', '预算管理', 1, 0, 4, '年度预算、科目预算、月度预算、执行跟踪', 'account'),
    ('approval', '审批流程', 1, 0, 5, '流程定义、审批实例、审批记录', NULL),
    ('asset', '资产管理', 1, 0, 6, '资产分类、卡片、折旧、变动、处置', 'account'),
    ('expense', '费用管理', 1, 0, 7, '费用类型、报销单、费用统计', 'system'),
    ('tax', '税务管理', 1, 0, 8, '税种配置、纳税申报、发票登记', 'account');
END
GO

-- 初始字典类型
IF NOT EXISTS (SELECT 1 FROM sys_dict_type WHERE DictType = 'sys_normal_disable')
BEGIN
    INSERT INTO sys_dict_type (DictType, DictName, Status) VALUES
    ('sys_normal_disable', '系统开关', 0),
    ('sys_user_sex', '用户性别', 0),
    ('sys_show_hide', '显示状态', 0),
    ('voucher_status', '凭证状态', 0),
    ('asset_status', '资产状态', 0),
    ('expense_status', '报销状态', 0),
    ('approval_status', '审批状态', 0),
    ('tax_declaration_status', '申报状态', 0);
END
GO
