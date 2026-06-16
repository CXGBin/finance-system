-- ============================================================
-- 财务管理系统 - 数据库初始化脚本
-- 数据库：SQL Server 2012
-- 字符集：UTF-8
-- 生成日期：2026-06-15
-- 说明：包含全部46张表的CREATE TABLE语句、索引、种子数据
-- ============================================================

-- 使用说明：先创建数据库，再执行此脚本
-- CREATE DATABASE FinanceSystem;
-- GO
-- USE FinanceSystem;
-- GO

-- ============================================================
-- 一、系统管理模块（12张表）
-- ============================================================

-- 1.1 用户表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_user' AND xtype='U')
BEGIN
    CREATE TABLE sys_user (
        Id          BIGINT         IDENTITY(1,1)  NOT NULL,   -- 主键
        Username    NVARCHAR(50)   NOT NULL,                  -- 用户名（唯一）
        PasswordHash NVARCHAR(256) NOT NULL,                  -- 密码哈希（BCrypt）
        RealName    NVARCHAR(50)   NOT NULL,                  -- 真实姓名
        Email       NVARCHAR(100)  NULL,                      -- 邮箱
        Phone       NVARCHAR(20)   NULL,                      -- 手机号
        Avatar      NVARCHAR(500)  NULL,                      -- 头像URL
        DeptId      BIGINT         NULL,                      -- 所属部门ID
        PostId      BIGINT         NULL,                      -- 所属岗位ID
        Status      INT            NOT NULL DEFAULT 1,         -- 状态（0禁用 1启用）
        Remark      NVARCHAR(500)  NULL,                      -- 备注
        LoginFailCount INT         NOT NULL DEFAULT 0,         -- 登录失败次数
        LockoutEndTime DATETIME   NULL,                      -- 账户锁定截止时间
        MustChangePassword BIT NOT NULL DEFAULT 0,        -- 是否需要强制修改密码
        CreatedTime DATETIME      NOT NULL DEFAULT GETDATE(), -- 创建时间
        UpdatedTime DATETIME      NULL,                      -- 更新时间
        CONSTRAINT PK_sys_user PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX IX_sys_user_Username ON sys_user(Username);
END
GO

-- 1.2 角色表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_role' AND xtype='U')
BEGIN
    CREATE TABLE sys_role (
        Id          BIGINT         IDENTITY(1,1)  NOT NULL,
        RoleName    NVARCHAR(50)   NOT NULL,                  -- 角色名称
        RoleCode    NVARCHAR(50)   NOT NULL,                  -- 角色编码（唯一）
        Description NVARCHAR(200)  NULL,                      -- 角色描述
        SortOrder   INT            NOT NULL DEFAULT 0,
        Status      INT            NOT NULL DEFAULT 1,
        CreatedTime DATETIME      NOT NULL DEFAULT GETDATE(),
        CONSTRAINT PK_sys_role PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX IX_sys_role_RoleCode ON sys_role(RoleCode);
END
GO

-- 1.3 用户角色关联表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_user_role' AND xtype='U')
BEGIN
    CREATE TABLE sys_user_role (
        Id      BIGINT IDENTITY(1,1) NOT NULL,
        UserId  BIGINT NOT NULL,                             -- 用户ID
        RoleId  BIGINT NOT NULL,                             -- 角色ID
        CreatedTime DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT PK_sys_user_role PRIMARY KEY (Id)
    );
    CREATE INDEX IX_sys_user_role_UserId ON sys_user_role(UserId);
    CREATE INDEX IX_sys_user_role_RoleId ON sys_user_role(RoleId);
END
GO

-- 1.4 菜单表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_menu' AND xtype='U')
BEGIN
    CREATE TABLE sys_menu (
        Id          BIGINT         IDENTITY(1,1)  NOT NULL,
        ParentId    BIGINT         NOT NULL DEFAULT 0,        -- 父级菜单ID（0为顶级）
        MenuName    NVARCHAR(50)   NOT NULL,                  -- 菜单名称
        MenuType    INT            NOT NULL DEFAULT 1,         -- 菜单类型（1目录 2菜单 3按钮）
        Path        NVARCHAR(200)  NULL,                      -- 路由路径
        Component   NVARCHAR(200)  NULL,                      -- 前端组件路径
        Permission  NVARCHAR(100)  NULL,                      -- 权限标识
        Icon        NVARCHAR(50)   NULL,                      -- 图标
        ModuleId    NVARCHAR(50)   NULL,                      -- 所属模块标识
        SortOrder   INT            NOT NULL DEFAULT 0,
        Visible     INT            NOT NULL DEFAULT 1,         -- 是否可见（0隐藏 1显示）
        Status      INT            NOT NULL DEFAULT 1,
        CreatedTime DATETIME      NOT NULL DEFAULT GETDATE(),
        CONSTRAINT PK_sys_menu PRIMARY KEY (Id)
    );
    CREATE INDEX IX_sys_menu_ParentId ON sys_menu(ParentId);
    CREATE INDEX IX_sys_menu_ModuleId ON sys_menu(ModuleId);
END
GO

-- 1.5 角色菜单关联表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_role_menu' AND xtype='U')
BEGIN
    CREATE TABLE sys_role_menu (
        Id      BIGINT IDENTITY(1,1) NOT NULL,
        RoleId  BIGINT NOT NULL,
        MenuId  BIGINT NOT NULL,
        CreatedTime DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT PK_sys_role_menu PRIMARY KEY (Id)
    );
    CREATE INDEX IX_sys_role_menu_RoleId ON sys_role_menu(RoleId);
END
GO

-- 1.6 部门表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_dept' AND xtype='U')
BEGIN
    CREATE TABLE sys_dept (
        Id          BIGINT         IDENTITY(1,1)  NOT NULL,
        ParentId    BIGINT         NOT NULL DEFAULT 0,
        DeptName    NVARCHAR(50)   NOT NULL,
        SortOrder   INT            NOT NULL DEFAULT 0,
        Leader      NVARCHAR(50)   NULL,
        Phone       NVARCHAR(20)   NULL,
        Email       NVARCHAR(100)  NULL,
        Status      INT            NOT NULL DEFAULT 1,
        CreatedTime DATETIME      NOT NULL DEFAULT GETDATE(),
        CONSTRAINT PK_sys_dept PRIMARY KEY (Id)
    );
    CREATE INDEX IX_sys_dept_ParentId ON sys_dept(ParentId);
END
GO

-- 1.7 岗位表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_post' AND xtype='U')
BEGIN
    CREATE TABLE sys_post (
        Id          BIGINT         IDENTITY(1,1)  NOT NULL,
        DeptId      BIGINT         NOT NULL,                    -- 所属部门ID
        PostCode    NVARCHAR(50)   NOT NULL,
        PostName    NVARCHAR(50)   NOT NULL,
        SortOrder   INT            NOT NULL DEFAULT 0,
        Status      INT            NOT NULL DEFAULT 1,
        Remark      NVARCHAR(500)  NULL,
        CreatedTime DATETIME      NOT NULL DEFAULT GETDATE(),
        UpdatedTime DATETIME      NULL,
        CONSTRAINT PK_sys_post PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX IX_sys_post_PostCode ON sys_post(PostCode);
END
GO

-- 1.8 字典类型表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_dict_type' AND xtype='U')
BEGIN
    CREATE TABLE sys_dict_type (
        Id          BIGINT         IDENTITY(1,1)  NOT NULL,
        DictName    NVARCHAR(100)  NOT NULL,
        DictType    NVARCHAR(100)  NOT NULL,
        Status      INT            NOT NULL DEFAULT 1,
        Remark      NVARCHAR(200)  NULL,
        CreatedTime DATETIME      NOT NULL DEFAULT GETDATE(),
        CONSTRAINT PK_sys_dict_type PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX IX_sys_dict_type_DictType ON sys_dict_type(DictType);
END
GO

-- 1.9 字典项表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_dict_data' AND xtype='U')
BEGIN
    CREATE TABLE sys_dict_data (
        Id          BIGINT         IDENTITY(1,1)  NOT NULL,
        DictType    NVARCHAR(100)  NOT NULL,                   -- 字典类型标识
        DictLabel   NVARCHAR(100)  NOT NULL,                   -- 字典标签
        DictValue   NVARCHAR(100)  NOT NULL,                   -- 字典值
        SortOrder   INT            NOT NULL DEFAULT 0,
        Status      INT            NOT NULL DEFAULT 1,
        Remark      NVARCHAR(200)  NULL,
        CreatedTime DATETIME      NOT NULL DEFAULT GETDATE(),
        CONSTRAINT PK_sys_dict_data PRIMARY KEY (Id)
    );
    CREATE INDEX IX_sys_dict_data_DictType ON sys_dict_data(DictType);
END
GO

-- 1.10 系统配置表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_config' AND xtype='U')
BEGIN
    CREATE TABLE sys_config (
        Id          BIGINT         IDENTITY(1,1)  NOT NULL,
        ConfigGroup NVARCHAR(50)   NOT NULL,
        ConfigKey   NVARCHAR(100)  NOT NULL,
        ConfigValue NVARCHAR(500)  NOT NULL,
        ConfigName  NVARCHAR(100)  NOT NULL,
        Remark      NVARCHAR(200)  NULL,
        CreatedTime DATETIME      NOT NULL DEFAULT GETDATE(),
        CONSTRAINT PK_sys_config PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX IX_sys_config_ConfigKey ON sys_config(ConfigKey);
END
GO

-- 1.11 操作日志表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_log' AND xtype='U')
BEGIN
    CREATE TABLE sys_log (
        Id              BIGINT         IDENTITY(1,1)  NOT NULL,
        UserId          BIGINT         NOT NULL,
        UserName        NVARCHAR(50)   NULL,
        Module          NVARCHAR(50)   NULL,                       -- 操作模块
        Action          NVARCHAR(50)   NULL,                       -- 操作类型
        Description     NVARCHAR(200)  NULL,                       -- 操作描述
        IpAddress       NVARCHAR(50)   NULL,
        RequestUrl      NVARCHAR(500)  NULL,
        RequestMethod   NVARCHAR(10)   NULL,
        RequestBody     NVARCHAR(MAX)  NULL,
        ResponseCode    INT            NOT NULL DEFAULT 0,
        DurationMs      INT            NOT NULL DEFAULT 0,        -- 耗时毫秒
        CreatedTime     DATETIME      NOT NULL DEFAULT GETDATE(),
        CONSTRAINT PK_sys_log PRIMARY KEY (Id)
    );
    CREATE INDEX IX_sys_log_UserId ON sys_log(UserId);
    CREATE INDEX IX_sys_log_CreatedTime ON sys_log(CreatedTime);
END
GO

-- 1.12 模块管理表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_module' AND xtype='U')
BEGIN
    CREATE TABLE sys_module (
        Id              BIGINT         IDENTITY(1,1)  NOT NULL,
        ModuleId        NVARCHAR(50)   NOT NULL,                   -- 模块标识
        ModuleName      NVARCHAR(100)  NOT NULL,
        Description     NVARCHAR(500)  NULL,
        IsEnabled       INT            NOT NULL DEFAULT 1,         -- 是否启用
        IsCore          INT            NOT NULL DEFAULT 0,         -- 是否核心模块
        SortOrder       INT            NOT NULL DEFAULT 0,
        Dependencies    NVARCHAR(500)  NULL,                       -- 依赖模块ID（逗号分隔）
        CreatedTime     DATETIME      NOT NULL DEFAULT GETDATE(),
        UpdatedTime     DATETIME      NULL,
        CONSTRAINT PK_sys_module PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX IX_sys_module_ModuleId ON sys_module(ModuleId);
END
GO

-- ============================================================
-- 二、账务管理模块（8张表）
-- ============================================================

-- 2.1 会计科目表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_account_subject' AND xtype='U')
BEGIN
    CREATE TABLE fm_account_subject (
        Id              BIGINT         IDENTITY(1,1)  NOT NULL,
        SubjectCode     NVARCHAR(50)   NOT NULL,                   -- 科目编码
        SubjectName     NVARCHAR(100)  NOT NULL,                   -- 科目名称
        ParentId        BIGINT         NULL,                       -- 父科目ID
        SubjectLevel    INT            NOT NULL DEFAULT 1,         -- 层级
        SubjectType     INT            NOT NULL,                   -- 科目类型（1资产2负债3权益4收入5费用6成本）
        BalanceDirection INT           NOT NULL DEFAULT 1,         -- 余额方向（1借方 2贷方）
        IsEnabled       INT            NOT NULL DEFAULT 1,         -- 是否启用
        IsCash          INT            NOT NULL DEFAULT 0,         -- 是否现金科目
        IsBank          INT            NOT NULL DEFAULT 0,         -- 是否银行科目
        AuxiliaryType   NVARCHAR(50)   NULL,                       -- 辅助核算类型
        SortOrder       INT            NOT NULL DEFAULT 0,
        Remark          NVARCHAR(500)  NULL,
        CreatedTime     DATETIME      NOT NULL DEFAULT GETDATE(),
        CONSTRAINT PK_fm_account_subject PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX IX_fm_account_subject_Code ON fm_account_subject(SubjectCode);
    CREATE INDEX IX_fm_account_subject_ParentId ON fm_account_subject(ParentId);
END
GO

-- 2.2 凭证主表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_voucher' AND xtype='U')
BEGIN
    CREATE TABLE fm_voucher (
        Id              BIGINT         IDENTITY(1,1)  NOT NULL,
        VoucherNo       NVARCHAR(50)   NOT NULL,                   -- 凭证号
        VoucherDate     DATETIME       NOT NULL,                   -- 凭证日期
        PeriodId        BIGINT         NOT NULL,                   -- 会计期间ID
        VoucherType     INT            NOT NULL DEFAULT 1,         -- 凭证类型（1记账 2转账 3调整）
        AbstractText    NVARCHAR(500)  NULL,                       -- 摘要
        Status          INT            NOT NULL DEFAULT 0,         -- 状态（0草稿 1已审核 2已作废）
        TotalDebit      DECIMAL(18,2)  NOT NULL DEFAULT 0,         -- 借方合计
        TotalCredit     DECIMAL(18,2)  NOT NULL DEFAULT 0,         -- 贷方合计
        PreparedBy      BIGINT         NULL,                       -- 制单人
        ReviewedBy      BIGINT         NULL,                       -- 审核人
        ReviewedTime    DATETIME       NULL,                       -- 审核时间
        CreatedTime     DATETIME       NOT NULL DEFAULT GETDATE(),
        CONSTRAINT PK_fm_voucher PRIMARY KEY (Id)
    );
    CREATE INDEX IX_fm_voucher_PeriodId ON fm_voucher(PeriodId);
    CREATE INDEX IX_fm_voucher_VoucherDate ON fm_voucher(VoucherDate);
    CREATE INDEX IX_fm_voucher_Status ON fm_voucher(Status);
END
GO

-- 2.3 凭证分录表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_voucher_entry' AND xtype='U')
BEGIN
    CREATE TABLE fm_voucher_entry (
        Id              BIGINT         IDENTITY(1,1)  NOT NULL,
        VoucherId       BIGINT         NOT NULL,                   -- 关联凭证ID
        Summary         NVARCHAR(500)  NULL,                       -- 摘要
        SubjectId       BIGINT         NOT NULL,                   -- 科目ID
        DebitAmount     DECIMAL(18,2)  NOT NULL DEFAULT 0,
        CreditAmount    DECIMAL(18,2)  NOT NULL DEFAULT 0,
        AuxiliaryId     BIGINT         NULL,                       -- 辅助核算项ID
        AuxiliaryType   NVARCHAR(50)   NULL,                       -- 辅助核算类型
        CreatedTime     DATETIME       NOT NULL DEFAULT GETDATE(),
        CONSTRAINT PK_fm_voucher_entry PRIMARY KEY (Id)
    );
    CREATE INDEX IX_fm_voucher_entry_VoucherId ON fm_voucher_entry(VoucherId);
    CREATE INDEX IX_fm_voucher_entry_SubjectId ON fm_voucher_entry(SubjectId);
END
GO

-- 2.4 会计期间表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_period' AND xtype='U')
BEGIN
    CREATE TABLE fm_period (
        Id              BIGINT         IDENTITY(1,1)  NOT NULL,
        PeriodYear      INT            NOT NULL,                   -- 年度
        PeriodMonth     INT            NOT NULL,                   -- 月份
        BeginDate       DATETIME       NOT NULL,                   -- 期间开始日期
        EndDate         DATETIME       NOT NULL,                   -- 期间结束日期
        IsClosed        INT            NOT NULL DEFAULT 0,         -- 是否已结账
        ClosedTime      DATETIME       NULL,
        ClosedBy        BIGINT         NULL,
        CreatedTime     DATETIME       NOT NULL DEFAULT GETDATE(),
        CONSTRAINT PK_fm_period PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX IX_fm_period_YearMonth ON fm_period(PeriodYear, PeriodMonth);
END
GO

-- 2.5 科目余额表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_subject_balance' AND xtype='U')
BEGIN
    CREATE TABLE fm_subject_balance (
        Id              BIGINT         IDENTITY(1,1)  NOT NULL,
        SubjectId       BIGINT         NOT NULL,
        PeriodId        BIGINT         NOT NULL,
        BeginDebit      DECIMAL(18,2)  NOT NULL DEFAULT 0,       -- 期初借方
        BeginCredit     DECIMAL(18,2)  NOT NULL DEFAULT 0,       -- 期初贷方
        CurrentDebit    DECIMAL(18,2)  NOT NULL DEFAULT 0,       -- 本期借方发生
        CurrentCredit   DECIMAL(18,2)  NOT NULL DEFAULT 0,       -- 本期贷方发生
        EndDebit        DECIMAL(18,2)  NOT NULL DEFAULT 0,       -- 期末借方
        EndCredit       DECIMAL(18,2)  NOT NULL DEFAULT 0,       -- 期末贷方
        CreatedTime     DATETIME       NOT NULL DEFAULT GETDATE(),
        CONSTRAINT PK_fm_subject_balance PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX IX_fm_subject_balance_Subject_Period ON fm_subject_balance(SubjectId, PeriodId);
END
GO

-- 2.6 辅助核算-项目表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_aux_project' AND xtype='U')
BEGIN
    CREATE TABLE fm_aux_project (
        Id          BIGINT         IDENTITY(1,1)  NOT NULL,
        ProjectCode NVARCHAR(50)   NOT NULL,
        ProjectName NVARCHAR(100)  NOT NULL,
        Manager     NVARCHAR(50)   NULL,
        BeginDate   DATETIME       NULL,
        EndDate     DATETIME       NULL,
        Status      INT            NOT NULL DEFAULT 1,
        CreatedTime DATETIME      NOT NULL DEFAULT GETDATE(),
        CONSTRAINT PK_fm_aux_project PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX IX_fm_aux_project_Code ON fm_aux_project(ProjectCode);
END
GO

-- 2.7 辅助核算-客户表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_aux_customer' AND xtype='U')
BEGIN
    CREATE TABLE fm_aux_customer (
        Id          BIGINT         IDENTITY(1,1)  NOT NULL,
        CustomerCode NVARCHAR(50)  NOT NULL,
        CustomerName NVARCHAR(100) NOT NULL,
        Contact     NVARCHAR(50)   NULL,
        Phone       NVARCHAR(20)   NULL,
        Address     NVARCHAR(200)  NULL,
        TaxNo       NVARCHAR(50)   NULL,                       -- 税号
        Status      INT            NOT NULL DEFAULT 1,
        CreatedTime DATETIME      NOT NULL DEFAULT GETDATE(),
        CONSTRAINT PK_fm_aux_customer PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX IX_fm_aux_customer_Code ON fm_aux_customer(CustomerCode);
END
GO

-- 2.8 辅助核算-供应商表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_aux_supplier' AND xtype='U')
BEGIN
    CREATE TABLE fm_aux_supplier (
        Id          BIGINT         IDENTITY(1,1)  NOT NULL,
        SupplierCode NVARCHAR(50)  NOT NULL,
        SupplierName NVARCHAR(100) NOT NULL,
        Contact     NVARCHAR(50)   NULL,
        Phone       NVARCHAR(20)   NULL,
        Address     NVARCHAR(200)  NULL,
        TaxNo       NVARCHAR(50)   NULL,
        BankName    NVARCHAR(100)  NULL,                       -- 开户银行
        BankAccount NVARCHAR(50)   NULL,                       -- 银行账号
        Status      INT            NOT NULL DEFAULT 1,
        CreatedTime DATETIME      NOT NULL DEFAULT GETDATE(),
        CONSTRAINT PK_fm_aux_supplier PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX IX_fm_aux_supplier_Code ON fm_aux_supplier(SupplierCode);
END
GO

-- ============================================================
-- 三、报表中心模块（1张表）
-- ============================================================

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_report_template' AND xtype='U')
BEGIN
    CREATE TABLE fm_report_template (
        Id          BIGINT         IDENTITY(1,1)  NOT NULL,
        TemplateName NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500)  NULL,
        TemplateData NVARCHAR(MAX) NOT NULL DEFAULT '[]',       -- 模板布局JSON
        CreatedBy   BIGINT         NOT NULL,
        CreatedTime DATETIME      NOT NULL DEFAULT GETDATE(),
        UpdatedTime DATETIME      NULL,
        CONSTRAINT PK_fm_report_template PRIMARY KEY (Id)
    );
END
GO

-- ============================================================
-- 四、预算管理模块（5张表）
-- ============================================================

-- 4.1 预算年度表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_budget_year' AND xtype='U')
BEGIN
    CREATE TABLE fm_budget_year (
        Id          BIGINT         IDENTITY(1,1)  NOT NULL,
        Year        INT            NOT NULL,
        Status      INT            NOT NULL DEFAULT 0,             -- 0草稿 1已审批 2执行中 3已关闭
        Description NVARCHAR(500)  NULL,
        CreatedBy   BIGINT         NOT NULL,
        CreatedTime DATETIME      NOT NULL DEFAULT GETDATE(),
        UpdatedTime DATETIME      NULL,
        CONSTRAINT PK_fm_budget_year PRIMARY KEY (Id)
    );
END
GO

-- 4.2 预算科目表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_budget_subject' AND xtype='U')
BEGIN
    CREATE TABLE fm_budget_subject (
        Id              BIGINT         IDENTITY(1,1)  NOT NULL,
        BudgetYearId    BIGINT         NOT NULL,
        SubjectId       BIGINT         NOT NULL,                   -- 关联会计科目
        DeptId          BIGINT         NULL,                       -- 关联部门（可选）
        AnnualAmount    DECIMAL(18,2)  NOT NULL DEFAULT 0,
        Remark          NVARCHAR(500)  NULL,
        CreatedTime     DATETIME      NOT NULL DEFAULT GETDATE(),
        UpdatedTime     DATETIME      NULL,
        CONSTRAINT PK_fm_budget_subject PRIMARY KEY (Id)
    );
    CREATE INDEX IX_fm_budget_subject_YearId ON fm_budget_subject(BudgetYearId);
END
GO

-- 4.3 月度预算明细表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_budget_monthly' AND xtype='U')
BEGIN
    CREATE TABLE fm_budget_monthly (
        Id               BIGINT         IDENTITY(1,1) NOT NULL,
        BudgetSubjectId  BIGINT         NOT NULL,
        Month            INT            NOT NULL,
        Amount           DECIMAL(18,2)  NOT NULL DEFAULT 0,
        CreatedTime      DATETIME       NOT NULL DEFAULT GETDATE(),
        UpdatedTime      DATETIME       NULL,
        CONSTRAINT PK_fm_budget_monthly PRIMARY KEY (Id)
    );
    CREATE INDEX IX_fm_budget_monthly_SubjectId ON fm_budget_monthly(BudgetSubjectId);
END
GO

-- 4.4 预算调整记录表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_budget_adjustment' AND xtype='U')
BEGIN
    CREATE TABLE fm_budget_adjustment (
        Id               BIGINT         IDENTITY(1,1) NOT NULL,
        BudgetSubjectId  BIGINT         NOT NULL,
        AdjustType       INT            NOT NULL,                  -- 1追加 2调减
        BeforeAmount     DECIMAL(18,2)  NOT NULL DEFAULT 0,
        AfterAmount      DECIMAL(18,2)  NOT NULL DEFAULT 0,
        Reason           NVARCHAR(500)  NOT NULL,
        ApproveStatus    INT            NOT NULL DEFAULT 0,         -- 0待审批 1已通过 2已驳回
        ApplyDeptId      BIGINT         NULL,
        ApplyBy          BIGINT         NOT NULL,
        CreatedTime      DATETIME       NOT NULL DEFAULT GETDATE(),
        UpdatedTime      DATETIME       NULL,
        CONSTRAINT PK_fm_budget_adjustment PRIMARY KEY (Id)
    );
END
GO

-- 4.5 预算预警配置表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_budget_alert_config' AND xtype='U')
BEGIN
    CREATE TABLE fm_budget_alert_config (
        Id          BIGINT         IDENTITY(1,1)  NOT NULL,
        BudgetYearId BIGINT         NOT NULL,
        Threshold   DECIMAL(5,2)   NOT NULL DEFAULT 80.00,         -- 预警阈值百分比
        NotifyType  NVARCHAR(50)   NOT NULL DEFAULT 'system',     -- 通知方式
        CreatedTime DATETIME      NOT NULL DEFAULT GETDATE(),
        UpdatedTime DATETIME      NULL,
        CONSTRAINT PK_fm_budget_alert_config PRIMARY KEY (Id)
    );
END
GO

-- ============================================================
-- 五、审批流程模块（3张表）
-- ============================================================

-- 5.1 审批流程定义表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_approval_flow' AND xtype='U')
BEGIN
    CREATE TABLE fm_approval_flow (
        Id          BIGINT         IDENTITY(1,1)  NOT NULL,
        FlowName    NVARCHAR(100)  NOT NULL,
        FlowCode    NVARCHAR(50)   NOT NULL,
        ModuleType  NVARCHAR(50)   NOT NULL,                   -- 业务类型（expense/budget/asset）
        Description NVARCHAR(500)  NULL,
        IsEnabled   INT            NOT NULL DEFAULT 1,
        NodesJson   NVARCHAR(MAX)  NOT NULL DEFAULT '[]',      -- 审批节点配置JSON
        CreatedTime DATETIME      NOT NULL DEFAULT GETDATE(),
        UpdatedTime DATETIME      NULL,
        CONSTRAINT PK_fm_approval_flow PRIMARY KEY (Id)
    );
END
GO

-- 5.2 审批实例表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_approval_instance' AND xtype='U')
BEGIN
    CREATE TABLE fm_approval_instance (
        Id               BIGINT         IDENTITY(1,1) NOT NULL,
        FlowId           BIGINT         NOT NULL,
        BusinessId       BIGINT         NOT NULL,                   -- 业务单据ID
        ModuleType       NVARCHAR(50)   NOT NULL,
        Title            NVARCHAR(200)  NOT NULL,
        InitiatorId      BIGINT         NOT NULL,                   -- 申请人
        CurrentNodeIndex INT            NOT NULL DEFAULT 0,
        Status           INT            NOT NULL DEFAULT 0,         -- 0审批中 1已通过 2已驳回 3已撤回
        DeptId           BIGINT         NULL,
        CreatedTime      DATETIME       NOT NULL DEFAULT GETDATE(),
        UpdatedTime      DATETIME       NULL,
        CONSTRAINT PK_fm_approval_instance PRIMARY KEY (Id)
    );
    CREATE INDEX IX_fm_approval_instance_Initiator ON fm_approval_instance(InitiatorId);
END
GO

-- 5.3 审批记录表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_approval_record' AND xtype='U')
BEGIN
    CREATE TABLE fm_approval_record (
        Id              BIGINT         IDENTITY(1,1)  NOT NULL,
        InstanceId      BIGINT         NOT NULL,
        NodeIndex       INT            NOT NULL,
        NodeName        NVARCHAR(50)   NOT NULL,
        ApproverId      BIGINT         NOT NULL,
        Action          INT            NOT NULL,                   -- 1通过 2驳回 3转办
        Comment         NVARCHAR(500)  NULL,
        ApproveTime     DATETIME       NOT NULL DEFAULT GETDATE(),
        CreatedTime     DATETIME       NOT NULL DEFAULT GETDATE(),
        CONSTRAINT PK_fm_approval_record PRIMARY KEY (Id)
    );
    CREATE INDEX IX_fm_approval_record_InstanceId ON fm_approval_record(InstanceId);
END
GO

-- ============================================================
-- 六、资产管理模块（4张表）
-- ============================================================

-- 6.1 资产分类表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_asset_category' AND xtype='U')
BEGIN
    CREATE TABLE fm_asset_category (
        Id                BIGINT         IDENTITY(1,1)  NOT NULL,
        ParentId          BIGINT         NULL,
        CategoryCode      NVARCHAR(50)   NOT NULL,
        CategoryName      NVARCHAR(100)  NOT NULL,
        DepreciationMethod INT            NOT NULL DEFAULT 1,     -- 1直线法 2双倍余额递减法 3年数总和法
        UsefulLifeMonths  INT            NOT NULL,                  -- 使用月数
        ResidualRate     DECIMAL(5,2)   NOT NULL DEFAULT 5.00,  -- 残值率%
        SortOrder         INT            NOT NULL DEFAULT 0,
        IsEnabled         INT            NOT NULL DEFAULT 1,
        CreatedTime       DATETIME       NOT NULL DEFAULT GETDATE(),
        UpdatedTime       DATETIME       NULL,
        CONSTRAINT PK_fm_asset_category PRIMARY KEY (Id)
    );
END
GO

-- 6.2 资产卡片表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_asset_card' AND xtype='U')
BEGIN
    CREATE TABLE fm_asset_card (
        Id                    BIGINT         IDENTITY(1,1)  NOT NULL,
        AssetCode             NVARCHAR(50)   NOT NULL,
        AssetName             NVARCHAR(100)  NOT NULL,
        CategoryId            BIGINT         NOT NULL,
        Specification         NVARCHAR(200)  NULL,                       -- 规格型号
        OriginalValue         DECIMAL(18,2)  NOT NULL DEFAULT 0,          -- 原值
        ResidualRate          DECIMAL(5,2)   NOT NULL DEFAULT 5.00,
        ResidualValue         DECIMAL(18,2)  NOT NULL DEFAULT 0,          -- 残值
        DepreciationMethod    INT            NOT NULL DEFAULT 1,
        UsefulLifeMonths     INT            NOT NULL,
        AcquisitionDate       DATETIME       NOT NULL,                      -- 购入日期
        DeptId                BIGINT         NULL,
        Keeper                NVARCHAR(50)   NULL,                       -- 保管人
        Location              NVARCHAR(200)  NULL,                       -- 存放地点
        Status                INT            NOT NULL DEFAULT 1,           -- 0在用 1闲置 2已处置 3已报废
        AccumulatedDepreciation DECIMAL(18,2) NOT NULL DEFAULT 0,       -- 累计折旧
        NetValue              DECIMAL(18,2)  NOT NULL DEFAULT 0,          -- 净值
        Remark                NVARCHAR(500)  NULL,
        CreatedTime           DATETIME       NOT NULL DEFAULT GETDATE(),
        UpdatedTime           DATETIME       NULL,
        CONSTRAINT PK_fm_asset_card PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX IX_fm_asset_card_Code ON fm_asset_card(AssetCode);
END
GO

-- 6.3 资产折旧明细表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_asset_depreciation' AND xtype='U')
BEGIN
    CREATE TABLE fm_asset_depreciation (
        Id               BIGINT         IDENTITY(1,1) NOT NULL,
        AssetId          BIGINT         NOT NULL,
        PeriodId         BIGINT         NOT NULL,                   -- 会计期间
        DeprAmount       DECIMAL(18,2)  NOT NULL DEFAULT 0,          -- 本期折旧额
        AccumulatedAmount DECIMAL(18,2) NOT NULL DEFAULT 0,          -- 累计折旧额
        NetValue         DECIMAL(18,2)  NOT NULL DEFAULT 0,          -- 净值
        VoucherId        BIGINT         NULL,                       -- 关联凭证ID
        CreatedTime      DATETIME       NOT NULL DEFAULT GETDATE(),
        CONSTRAINT PK_fm_asset_depreciation PRIMARY KEY (Id)
    );
    CREATE INDEX IX_fm_asset_depreciation_AssetId ON fm_asset_depreciation(AssetId);
END
GO

-- ============================================================
-- 七、费用管理模块（3张表）
-- ============================================================

-- 7.1 费用类型表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_expense_type' AND xtype='U')
BEGIN
    CREATE TABLE fm_expense_type (
        Id          BIGINT         IDENTITY(1,1)  NOT NULL,
        TypeCode    NVARCHAR(50)   NOT NULL,
        TypeName    NVARCHAR(100)  NOT NULL,
        SubjectId   BIGINT         NULL,                       -- 默认关联科目
        SingleLimit DECIMAL(18,2)  NULL,                       -- 单次限额
        MonthlyLimit DECIMAL(18,2) NULL,                       -- 月度限额
        SortOrder   INT            NOT NULL DEFAULT 0,
        IsEnabled   INT            NOT NULL DEFAULT 1,
        CreatedTime DATETIME      NOT NULL DEFAULT GETDATE(),
        UpdatedTime DATETIME      NULL,
        CONSTRAINT PK_fm_expense_type PRIMARY KEY (Id)
    );
    CREATE UNIQUE INDEX IX_fm_expense_type_Code ON fm_expense_type(TypeCode);
END
GO

-- 7.2 费用报销单表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_expense_claim' AND xtype='U')
BEGIN
    CREATE TABLE fm_expense_claim (
        Id                 BIGINT         IDENTITY(1,1) NOT NULL,
        ClaimNo            NVARCHAR(50)   NOT NULL,
        Title              NVARCHAR(200)  NOT NULL,
        ClaimantId         BIGINT         NOT NULL,                   -- 申请人
        DeptId             BIGINT         NULL,
        TotalAmount        DECIMAL(18,2)  NOT NULL DEFAULT 0,
        Status             INT            NOT NULL DEFAULT 0,         -- 0草稿 1待审批 2已通过 3已驳回 4已付款 5已撤回 6已作废
        ApprovalInstanceId BIGINT         NULL,                       -- 关联审批实例
        PaymentDate        DATETIME       NULL,                       -- 付款日期
        VoucherId          BIGINT         NULL,                       -- 关联凭证
        Remark             NVARCHAR(500)  NULL,
        CreatedTime        DATETIME       NOT NULL DEFAULT GETDATE(),
        UpdatedTime        DATETIME       NULL,
        CONSTRAINT PK_fm_expense_claim PRIMARY KEY (Id)
    );
    CREATE INDEX IX_fm_expense_claim_Claimant ON fm_expense_claim(ClaimantId);
END
GO

-- 7.3 费用报销明细表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_expense_item' AND xtype='U')
BEGIN
    CREATE TABLE fm_expense_item (
        Id              BIGINT         IDENTITY(1,1)  NOT NULL,
        ClaimId         BIGINT         NOT NULL,
        ExpenseTypeId   BIGINT         NOT NULL,
        Description     NVARCHAR(500)  NOT NULL,
        Amount          DECIMAL(18,2)  NOT NULL DEFAULT 0,
        ExpenseDate     DATETIME       NOT NULL,
        InvoiceNo       NVARCHAR(50)   NULL,
        CreatedTime     DATETIME       NOT NULL DEFAULT GETDATE(),
        UpdatedTime     DATETIME       NULL,
        CONSTRAINT PK_fm_expense_item PRIMARY KEY (Id)
    );
    CREATE INDEX IX_fm_expense_item_ClaimId ON fm_expense_item(ClaimId);
END
GO

-- ============================================================
-- 八、税务管理模块（3张表）
-- ============================================================

-- 8.1 税种配置表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_tax_category' AND xtype='U')
BEGIN
    CREATE TABLE fm_tax_category (
        Id                BIGINT         IDENTITY(1,1) NOT NULL,
        TaxCode           NVARCHAR(50)   NOT NULL,
        TaxName           NVARCHAR(100)  NOT NULL,
        TaxRate           DECIMAL(5,2)   NOT NULL DEFAULT 0,
        CalculationMethod INT            NOT NULL DEFAULT 1,    -- 1从价 2从量
        DeclareCycle      INT            NOT NULL DEFAULT 1,    -- 1月度 2季度 3年度
        SubjectId         BIGINT         NULL,                   -- 关联科目
        IsEnabled         INT            NOT NULL DEFAULT 1,
        Remark            NVARCHAR(500)  NULL,
        CreatedTime       DATETIME       NOT NULL DEFAULT GETDATE(),
        UpdatedTime       DATETIME       NULL,
        CONSTRAINT PK_fm_tax_category PRIMARY KEY (Id)
    );
END
GO

-- 8.2 纳税申报记录表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_tax_declaration' AND xtype='U')
BEGIN
    CREATE TABLE fm_tax_declaration (
        Id               BIGINT         IDENTITY(1,1) NOT NULL,
        TaxCategoryId    BIGINT         NOT NULL,
        DeclarePeriod    NVARCHAR(20)   NOT NULL,                -- 申报期间（如2026-06）
        TaxAmount        DECIMAL(18,2)  NOT NULL DEFAULT 0,     -- 应纳税额
        ActualPaidAmount DECIMAL(18,2)  NOT NULL DEFAULT 0,     -- 实缴金额
        Status           INT            NOT NULL DEFAULT 0,      -- 0草稿 1已申报 2已缴款
        DeclaredBy       BIGINT         NOT NULL,
        Remark           NVARCHAR(500)  NULL,
        CreatedTime      DATETIME       NOT NULL DEFAULT GETDATE(),
        UpdatedTime      DATETIME       NULL,
        CONSTRAINT PK_fm_tax_declaration PRIMARY KEY (Id)
    );
END
GO

-- 8.3 发票登记表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='fm_tax_invoice' AND xtype='U')
BEGIN
    CREATE TABLE fm_tax_invoice (
        Id                BIGINT         IDENTITY(1,1) NOT NULL,
        InvoiceType       INT            NOT NULL,                 -- 1进项 2销项
        InvoiceNo         NVARCHAR(50)   NOT NULL,
        InvoiceDate       DATETIME       NOT NULL,
        CounterpartyName  NVARCHAR(200)  NOT NULL,                 -- 对方名称
        TaxAmount         DECIMAL(18,2)  NOT NULL DEFAULT 0,      -- 税额
        AmountWithoutTax  DECIMAL(18,2)  NOT NULL DEFAULT 0,      -- 不含税金额
        TotalAmount       DECIMAL(18,2)  NOT NULL DEFAULT 0,      -- 价税合计
        Direction         INT            NOT NULL DEFAULT 1,      -- 1收入 2支出
        VoucherId         BIGINT         NULL,
        CreatedTime       DATETIME       NOT NULL DEFAULT GETDATE(),
        UpdatedTime       DATETIME       NULL,
        CONSTRAINT PK_fm_tax_invoice PRIMARY KEY (Id)
    );
END
GO

-- ============================================================
-- 种子数据
-- ============================================================

-- 8个模块状态（全部开启）
IF NOT EXISTS (SELECT TOP 1 1 FROM sys_module)
BEGIN
    INSERT INTO sys_module (ModuleId, ModuleName, Description, IsEnabled, IsCore, SortOrder, Dependencies)
    VALUES
    ('system',  '系统管理',  '核心模块',                           1, 1, 1, NULL),
    ('account', '账务管理',  '核心模块：科目/凭证/账簿/期末处理',       1, 1, 2, 'system'),
    ('report',  '报表中心',  '资产负债表/利润表/现金流量表',          1, 0, 3, 'account'),
    ('budget',  '预算管理',  '预算编制/执行跟踪/预警分析',            1, 0, 4, 'account,system'),
    ('approval','审批流程',  '通用审批能力',                        1, 0, 5, 'system'),
    ('asset',   '资产管理',  '固定资产全生命周期',                  1, 0, 6, 'account'),
    ('expense', '费用管理',  '报销/费用分摊/统计',                  1, 0, 7, 'account,system,approval'),
    ('tax',     '税务管理',  '税种维护/纳税申报/发票管理',          1, 0, 8, 'account');
END
GO

-- 默认角色
IF NOT EXISTS (SELECT TOP 1 1 FROM sys_role)
BEGIN
    INSERT INTO sys_role (RoleName, RoleCode, Description, SortOrder, Status)
    VALUES ('超级管理员', 'SUPER_ADMIN', '拥有系统全部权限', 1, 1);
END
GO

-- 默认部门
IF NOT EXISTS (SELECT TOP 1 1 FROM sys_dept)
BEGIN
    INSERT INTO sys_dept (ParentId, DeptName, SortOrder, Status)
    VALUES (0, '总公司', 1, 1);
END
GO

-- 默认管理员账号（密码：admin123，BCrypt哈希）
IF NOT EXISTS (SELECT TOP 1 1 FROM sys_user)
BEGIN
    INSERT INTO sys_user (Username, PasswordHash, RealName, Email, Phone, Status, Remark)
    VALUES ('admin', '$2a$11$N.zmdr9k7uOCQb376NoUnuTJ8iAt6Z5EHsM8lE9lBOsl7iKTVKIUi', '系统管理员', 'admin@finance.com', '13800000000', 1, '系统默认超级管理员');
END
GO

-- CAS标准一级会计科目（中国大陆企业会计准则）
IF NOT EXISTS (SELECT TOP 1 1 FROM fm_account_subject)
BEGIN
    -- 资产类
    INSERT INTO fm_account_subject (SubjectCode, SubjectName, ParentId, SubjectLevel, SubjectType, BalanceDirection, SortOrder)
    VALUES
    ('1001','库存现金',    NULL,1,1,1,0),
    ('1002','银行存款',    NULL,1,1,1,1),
    ('1012','其他货币资金',NULL,1,1,1,2),
    ('1101','交易性金融资产',NULL,1,1,1,3),
    ('1121','应收票据',    NULL,1,1,1,4),
    ('1122','应收账款',    NULL,1,1,1,5),
    ('1123','预付账款',    NULL,1,1,1,6),
    ('1131','应收股利',    NULL,1,1,1,7),
    ('1132','应收利息',    NULL,1,1,1,8),
    ('1221','其他应收款',  NULL,1,1,1,9),
    ('1231','坏账准备',    NULL,1,1,2,10),
    ('1401','材料采购',    NULL,1,1,1,11),
    ('1403','原材料',      NULL,1,1,1,12),
    ('1405','库存商品',    NULL,1,1,1,13),
    ('1471','存货跌价准备',NULL,1,1,2,14),
    ('1511','长期股权投资',NULL,1,1,1,15),
    ('1601','固定资产',    NULL,1,1,1,16),
    ('1602','累计折旧',    NULL,1,1,2,17),
    ('1603','固定资产减值准备',NULL,1,1,2,18),
    ('1604','在建工程',    NULL,1,1,1,19),
    ('1701','无形资产',    NULL,1,1,1,20),
    ('1702','累计摊销',    NULL,1,1,2,21),
    ('1801','长期待摊费用',NULL,1,1,1,22),
    ('1901','待处理财产损溢',NULL,1,1,1,23),
    -- 负债类
    ('2001','短期借款',    NULL,1,2,2,24),
    ('2201','应付票据',    NULL,1,2,2,25),
    ('2202','应付账款',    NULL,1,2,2,26),
    ('2203','预收账款',    NULL,1,2,2,27),
    ('2211','应付职工薪酬',NULL,1,2,2,28),
    ('2221','应交税费',    NULL,1,2,2,29),
    ('2231','应付利息',    NULL,1,2,2,30),
    ('2232','应付股利',    NULL,1,2,2,31),
    ('2241','其他应付款',  NULL,1,2,2,32),
    ('2501','长期借款',    NULL,1,2,2,33),
    ('2701','长期应付款',  NULL,1,2,2,34),
    -- 权益类
    ('4001','实收资本',    NULL,1,3,2,35),
    ('4002','资本公积',    NULL,1,3,2,36),
    ('4101','盈余公积',    NULL,1,3,2,37),
    ('4103','本年利润',    NULL,1,3,2,38),
    ('4104','利润分配',    NULL,1,3,2,39),
    -- 成本类
    ('5001','生产成本',    NULL,1,6,1,40),
    ('5101','制造费用',    NULL,1,6,1,41),
    ('5201','劳务成本',    NULL,1,6,1,42),
    -- 损益类
    ('6001','主营业务收入',NULL,1,4,2,43),
    ('6051','其他业务收入',NULL,1,4,2,44),
    ('6101','公允价值变动损益',NULL,1,4,1,45),
    ('6111','投资收益',    NULL,1,4,1,46),
    ('6301','营业外收入',  NULL,1,4,1,47),
    ('6401','主营业务成本',NULL,1,5,1,48),
    ('6402','其他业务成本',NULL,1,5,1,49),
    ('6403','税金及附加',  NULL,1,5,1,50),
    ('6601','销售费用',    NULL,1,5,1,51),
    ('6602','管理费用',    NULL,1,5,1,52),
    ('6603','财务费用',    NULL,1,5,1,53),
    ('6701','资产减值损失',NULL,1,5,1,54),
    ('6711','营业外支出',  NULL,1,5,1,55),
    ('6801','所得税费用',  NULL,1,5,1,56),
    ('6901','以前年度损益调整',NULL,1,5,1,57);
END
GO

-- ============================================================
-- 初始化完成
-- ============================================================
PRINT '财务管理系统数据库初始化完成';
PRINT '共创建 39 张表（系统管理12 + 账务管理8 + 报表1 + 预算5 + 审批3 + 资产3 + 费用3 + 税务3 + 模块1 = 39）';
PRINT '已插入种子数据：8个模块状态、默认角色、默认部门、管理员账号、CAS一级会计科目57个';

-- 系统公告表
IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='sys_notice' AND xtype='U')
BEGIN
    CREATE TABLE sys_notice (
        Id          BIGINT         IDENTITY(1,1)  NOT NULL,
        Title       NVARCHAR(200)  NOT NULL,
        Content     NVARCHAR(MAX)  NOT NULL,
        NoticeType  INT            NOT NULL DEFAULT 1,       -- 1通知 2公告
        Status      INT            NOT NULL DEFAULT 1,
        CreatedBy   BIGINT         NOT NULL,
        CreatedTime DATETIME       NOT NULL DEFAULT GETDATE(),
        CONSTRAINT PK_sys_notice PRIMARY KEY (Id)
    );
END
GO

-- 示例公告
IF NOT EXISTS (SELECT TOP 1 1 FROM sys_notice)
BEGIN
    INSERT INTO sys_notice (Title, Content, NoticeType, Status, CreatedBy)
    VALUES ('欢迎使用财务管理系统', '系统已初始化完成，包含8大功能模块：系统管理、账务管理、报表中心、预算管理、审批流程、资产管理、费用管理、税务管理。请先完成会计期间初始化和科目设置。', 2, 1, 1);
END
GO
