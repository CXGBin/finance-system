-- Auto-generated SQLite DDL
-- Tables: 43

-- sys_config
CREATE TABLE IF NOT EXISTS sys_config (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, ConfigGroup TEXT NOT NULL, ConfigKey TEXT NOT NULL, ConfigValue TEXT NOT NULL, ConfigName TEXT NOT NULL, Remark TEXT);

-- sys_dept
CREATE TABLE IF NOT EXISTS sys_dept (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, ParentId INTEGER NOT NULL, DeptName TEXT NOT NULL, SortOrder INTEGER NOT NULL, Leader TEXT, Phone TEXT, Email TEXT, Status INTEGER NOT NULL);

-- sys_dict_data
CREATE TABLE IF NOT EXISTS sys_dict_data (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, DictType TEXT NOT NULL, DictLabel TEXT NOT NULL, DictValue TEXT NOT NULL, SortOrder INTEGER NOT NULL, Status INTEGER NOT NULL, Remark TEXT);

-- sys_dict_type
CREATE TABLE IF NOT EXISTS sys_dict_type (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, DictName TEXT NOT NULL, DictType TEXT NOT NULL, Status INTEGER NOT NULL, Remark TEXT);

-- sys_log
CREATE TABLE IF NOT EXISTS sys_log (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UserId INTEGER NOT NULL, UserName TEXT, Module TEXT, Action TEXT, Description TEXT, IpAddress TEXT, RequestUrl TEXT, RequestMethod TEXT, RequestBody text, ResponseCode text NOT NULL, DurationMs INTEGER NOT NULL);

-- sys_menu
CREATE TABLE IF NOT EXISTS sys_menu (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, ParentId INTEGER NOT NULL, MenuName TEXT NOT NULL, MenuType INTEGER NOT NULL, Path TEXT, Component TEXT, Permission TEXT, Icon TEXT, ModuleId TEXT, SortOrder INTEGER NOT NULL, Visible INTEGER NOT NULL, Status INTEGER NOT NULL);

-- sys_module
CREATE TABLE IF NOT EXISTS sys_module (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, ModuleId TEXT NOT NULL, ModuleName TEXT NOT NULL, Description TEXT, IsEnabled INTEGER NOT NULL, IsCore INTEGER NOT NULL, SortOrder INTEGER NOT NULL, Dependencies TEXT);

-- sys_notice
CREATE TABLE IF NOT EXISTS sys_notice (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, Title TEXT NOT NULL, Content TEXT NOT NULL, NoticeType INTEGER NOT NULL, Status INTEGER NOT NULL, CreatedBy INTEGER NOT NULL);

-- sys_post
CREATE TABLE IF NOT EXISTS sys_post (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, DeptId INTEGER NOT NULL, PostCode TEXT NOT NULL, PostName TEXT NOT NULL, SortOrder INTEGER NOT NULL, Status INTEGER NOT NULL, Remark TEXT);

-- sys_role
CREATE TABLE IF NOT EXISTS sys_role (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, RoleName TEXT NOT NULL, RoleCode TEXT NOT NULL, Description TEXT, SortOrder INTEGER NOT NULL, Status INTEGER NOT NULL, DataScope INTEGER NOT NULL);

-- sys_role_menu
CREATE TABLE IF NOT EXISTS sys_role_menu (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, RoleId INTEGER NOT NULL, MenuId INTEGER NOT NULL);

-- sys_user
CREATE TABLE IF NOT EXISTS sys_user (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, Username TEXT NOT NULL, PasswordHash TEXT, RealName TEXT NOT NULL, Email TEXT, Phone TEXT, Avatar TEXT, DeptId INTEGER, PostId INTEGER, Status INTEGER NOT NULL, Remark TEXT, LoginFailCount INTEGER NOT NULL, LockoutEndTime TEXT, MustChangePassword INTEGER NOT NULL DEFAULT 0);

-- sys_user_role
CREATE TABLE IF NOT EXISTS sys_user_role (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UserId INTEGER NOT NULL, RoleId INTEGER NOT NULL);

-- fm_account_subject
CREATE TABLE IF NOT EXISTS fm_account_subject (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, SubjectCode TEXT NOT NULL, SubjectName TEXT NOT NULL, ParentId INTEGER, SubjectLevel INTEGER NOT NULL, SubjectType INTEGER NOT NULL, BalanceDirection INTEGER NOT NULL, IsEnabled INTEGER NOT NULL, IsCash INTEGER NOT NULL, IsBank INTEGER NOT NULL, AuxiliaryType TEXT, SortOrder INTEGER NOT NULL, Remark TEXT);

-- fm_period
CREATE TABLE IF NOT EXISTS fm_period (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, PeriodYear INTEGER NOT NULL, PeriodMonth INTEGER NOT NULL, BeginDate TEXT NOT NULL, EndDate TEXT NOT NULL, IsClosed INTEGER NOT NULL, ClosedTime TEXT, ClosedBy INTEGER);

-- fm_aux_customer
CREATE TABLE IF NOT EXISTS fm_aux_customer (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, CustomerCode TEXT NOT NULL, CustomerName TEXT NOT NULL, Contact TEXT, Phone TEXT, Address TEXT, TaxNo TEXT, Status INTEGER NOT NULL);

-- fm_aux_project
CREATE TABLE IF NOT EXISTS fm_aux_project (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, ProjectCode TEXT NOT NULL, ProjectName TEXT NOT NULL, Manager TEXT, BeginDate TEXT, EndDate TEXT, Status INTEGER NOT NULL);

-- fm_aux_supplier
CREATE TABLE IF NOT EXISTS fm_aux_supplier (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, SupplierCode TEXT NOT NULL, SupplierName TEXT NOT NULL, Contact TEXT, Phone TEXT, Address TEXT, TaxNo TEXT, BankName TEXT, BankAccount TEXT, Status INTEGER NOT NULL);

-- fm_subject_balance
CREATE TABLE IF NOT EXISTS fm_subject_balance (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, SubjectId INTEGER NOT NULL, PeriodId INTEGER NOT NULL, BeginDebit REAL NOT NULL, BeginCredit REAL NOT NULL, CurrentDebit REAL NOT NULL, CurrentCredit REAL NOT NULL, EndDebit REAL NOT NULL, EndCredit REAL NOT NULL);

-- fm_voucher
CREATE TABLE IF NOT EXISTS fm_voucher (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, VoucherNo TEXT NOT NULL, VoucherDate TEXT NOT NULL, PeriodId INTEGER NOT NULL, VoucherType INTEGER NOT NULL, AbstractText TEXT, Status INTEGER NOT NULL, TotalDebit REAL NOT NULL, TotalCredit REAL NOT NULL, PreparedBy INTEGER, ReviewedBy INTEGER, ReviewedTime TEXT);

-- fm_voucher_entry
CREATE TABLE IF NOT EXISTS fm_voucher_entry (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, VoucherId INTEGER NOT NULL, Summary TEXT, SubjectId INTEGER NOT NULL, DebitAmount REAL NOT NULL, CreditAmount REAL NOT NULL, AuxiliaryId INTEGER, AuxiliaryType TEXT, Subject TEXT);

-- fm_approval_flow
CREATE TABLE IF NOT EXISTS fm_approval_flow (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, FlowName TEXT NOT NULL, FlowCode TEXT NOT NULL, ModuleType TEXT NOT NULL, Description TEXT, IsEnabled INTEGER NOT NULL, NodesJson TEXT NOT NULL);

-- fm_approval_instance
CREATE TABLE IF NOT EXISTS fm_approval_instance (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, FlowId INTEGER NOT NULL, BusinessId INTEGER NOT NULL, ModuleType TEXT NOT NULL, Title TEXT NOT NULL, InitiatorId INTEGER NOT NULL, CurrentNodeIndex INTEGER NOT NULL, Status INTEGER NOT NULL, DeptId INTEGER);

-- fm_approval_record
CREATE TABLE IF NOT EXISTS fm_approval_record (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, InstanceId INTEGER NOT NULL, NodeIndex INTEGER NOT NULL, NodeName TEXT NOT NULL, ApproverId INTEGER NOT NULL, Action INTEGER NOT NULL, Comment TEXT, ApproveTime TEXT NOT NULL);

-- fm_asset_category
CREATE TABLE IF NOT EXISTS fm_asset_category (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, ParentId INTEGER, CategoryCode TEXT NOT NULL, CategoryName TEXT NOT NULL, DepreciationMethod INTEGER NOT NULL, UsefulLifeMonths INTEGER NOT NULL, ResidualRate REAL NOT NULL, SortOrder INTEGER NOT NULL, IsEnabled INTEGER NOT NULL);

-- fm_asset_card
CREATE TABLE IF NOT EXISTS fm_asset_card (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, AssetCode TEXT NOT NULL, AssetName TEXT NOT NULL, CategoryId INTEGER NOT NULL, Specification TEXT, OriginalValue REAL NOT NULL, ResidualRate REAL NOT NULL, ResidualValue REAL NOT NULL, DepreciationMethod INTEGER NOT NULL, UsefulLifeMonths INTEGER NOT NULL, AcquisitionDate TEXT NOT NULL, DeptId INTEGER, Keeper TEXT, Location TEXT, Status INTEGER NOT NULL, AccumulatedDepreciation REAL NOT NULL, NetValue REAL NOT NULL, Remark TEXT);

-- fm_asset_depreciation
CREATE TABLE IF NOT EXISTS fm_asset_depreciation (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, AssetCardId INTEGER NOT NULL, PeriodId INTEGER NOT NULL, Month INTEGER NOT NULL, DepreciationAmount REAL NOT NULL, AccumulatedDepreciation REAL NOT NULL, NetValue REAL NOT NULL);

-- fm_asset_change
CREATE TABLE IF NOT EXISTS fm_asset_change (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, AssetCardId INTEGER NOT NULL, ChangeType INTEGER NOT NULL, Reason TEXT NOT NULL, FromDeptId INTEGER, ToDeptId INTEGER, DisposalIncome REAL, OperatorId INTEGER NOT NULL);

-- fm_asset_inventory
CREATE TABLE IF NOT EXISTS fm_asset_inventory (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, InventoryNo TEXT NOT NULL, InventoryDate TEXT NOT NULL, OperatorId INTEGER NOT NULL, ItemsJson TEXT, Status INTEGER NOT NULL);

-- fm_budget_year
CREATE TABLE IF NOT EXISTS fm_budget_year (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, Year INTEGER NOT NULL, Status INTEGER NOT NULL, Description TEXT, CreatedBy INTEGER NOT NULL);

-- fm_budget_subject
CREATE TABLE IF NOT EXISTS fm_budget_subject (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, BudgetYearId INTEGER NOT NULL, SubjectId INTEGER NOT NULL, DeptId INTEGER, AnnualAmount REAL NOT NULL, Remark TEXT);

-- fm_budget_monthly
CREATE TABLE IF NOT EXISTS fm_budget_monthly (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, BudgetSubjectId INTEGER NOT NULL, Month INTEGER NOT NULL, Amount REAL NOT NULL);

-- fm_budget_adjustment
CREATE TABLE IF NOT EXISTS fm_budget_adjustment (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, BudgetSubjectId INTEGER NOT NULL, AdjustType INTEGER NOT NULL, BeforeAmount REAL NOT NULL, AfterAmount REAL NOT NULL, Reason TEXT NOT NULL, ApproveStatus INTEGER NOT NULL, ApplyDeptId INTEGER, ApplyBy INTEGER NOT NULL);

-- fm_budget_alert_config
CREATE TABLE IF NOT EXISTS fm_budget_alert_config (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, BudgetYearId INTEGER NOT NULL, AlertThreshold REAL NOT NULL, IsEnabled INTEGER NOT NULL, AlertMethod INTEGER NOT NULL);

-- fm_expense_type
CREATE TABLE IF NOT EXISTS fm_expense_type (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, TypeCode TEXT NOT NULL, TypeName TEXT NOT NULL, SubjectId INTEGER, SingleLimit REAL, MonthlyLimit REAL, SortOrder INTEGER NOT NULL, IsEnabled INTEGER NOT NULL);

-- fm_expense_claim
CREATE TABLE IF NOT EXISTS fm_expense_claim (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, ClaimNo TEXT NOT NULL, Title TEXT NOT NULL, ClaimantId INTEGER NOT NULL, DeptId INTEGER, TotalAmount REAL NOT NULL, Status INTEGER NOT NULL, ApprovalInstanceId INTEGER, PaymentDate TEXT, VoucherId INTEGER, Remark TEXT);

-- fm_expense_item
CREATE TABLE IF NOT EXISTS fm_expense_item (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, ClaimId INTEGER NOT NULL, ExpenseTypeId INTEGER NOT NULL, Description TEXT NOT NULL, Amount REAL NOT NULL, ExpenseDate TEXT NOT NULL, InvoiceNo TEXT);

-- fm_expense_allocate
CREATE TABLE IF NOT EXISTS fm_expense_allocate (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, AllocateNo TEXT NOT NULL, Description TEXT, TotalAmount REAL NOT NULL, DeptId INTEGER, AllocateAmount REAL NOT NULL, PeriodYear INTEGER NOT NULL, PeriodMonth INTEGER NOT NULL);

-- expense_loan
CREATE TABLE IF NOT EXISTS expense_loan (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, LoanNo TEXT NOT NULL, ApplicantId INTEGER NOT NULL, LoanAmount REAL NOT NULL, SettledAmount REAL NOT NULL, Reason TEXT, ExpectedReturnDate TEXT, VoucherId INTEGER, Status INTEGER NOT NULL, ApprovalInstanceId INTEGER);

-- fm_report_template
CREATE TABLE IF NOT EXISTS fm_report_template (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, TemplateName TEXT NOT NULL, Description TEXT, TemplateData TEXT NOT NULL, CreatedBy INTEGER NOT NULL);

-- fm_tax_category
CREATE TABLE IF NOT EXISTS fm_tax_category (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, TaxCode TEXT NOT NULL, TaxName TEXT NOT NULL, TaxRate REAL NOT NULL, CalculationMethod INTEGER NOT NULL, DeclareCycle INTEGER NOT NULL, SubjectId INTEGER, IsEnabled INTEGER NOT NULL, Remark TEXT);

-- fm_tax_declaration
CREATE TABLE IF NOT EXISTS fm_tax_declaration (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, TaxCategoryId INTEGER NOT NULL, DeclarePeriod TEXT NOT NULL, TaxAmount REAL NOT NULL, ActualPaidAmount REAL NOT NULL, Status INTEGER NOT NULL, DeclaredBy INTEGER NOT NULL, Remark TEXT);

-- fm_tax_invoice
CREATE TABLE IF NOT EXISTS fm_tax_invoice (Id INTEGER PRIMARY KEY AUTOINCREMENT, CreatedTime TEXT NOT NULL, UpdatedTime TEXT, InvoiceType INTEGER NOT NULL, InvoiceNo TEXT NOT NULL, InvoiceDate TEXT NOT NULL, CounterpartyName TEXT NOT NULL, TaxAmount REAL NOT NULL, AmountWithoutTax REAL NOT NULL, TotalAmount REAL NOT NULL, Direction INTEGER NOT NULL, VoucherId INTEGER, IsVerified INTEGER NOT NULL, Remark TEXT);

