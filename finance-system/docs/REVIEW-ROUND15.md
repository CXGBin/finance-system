# REVIEW-ROUND15: 综合前后端对齐审查报告

> **审查日期**: 2026-06-17  
> **审查范围**: 后端 12 个 DTO 文件、13 个 Controller、2 个 Service、2 个 Interface、2 个 Enum 文件 + 前端 10 个 type 文件、10 个 API 文件、62 个页面组件、2 个工具文件  
> **审查方式**: 逐文件交叉比对 Backend C# DTO/Controller 与 Frontend TypeScript Type/API/Page

---

## 一、P0 问题（运行时崩溃/数据丢失风险）

### P0-01: 凭证列表页列 dataIndex 与后端实体字段不匹配
- **模块**: 会计 - 凭证管理
- **前端文件**: `finance-web/src/modules/account/voucher/list/index.tsx`
- **后端文件**: `backend/.../Entities/Voucher.cs`
- **问题**: 前端 ProTable 列使用 `debitAmount` 和 `creditAmount` 作为 dataIndex，但后端 Voucher 实体字段名为 `TotalDebit` / `TotalCredit`（JSON 序列化后为 `totalDebit` / `totalCredit`）
- **后果**: 凭证列表页"借方合计"和"贷方合计"列始终显示为 0，数据不显示
- **修复**: 将列 dataIndex 改为 `totalDebit` 和 `totalCredit`

### P0-02: VoucherType 枚举值 0 不存在于后端
- **模块**: 会计 - 凭证管理
- **前端文件**: `finance-web/src/modules/account/voucher/list/index.tsx`
- **后端文件**: `backend/.../Enums/CommonEnums.cs` → `VoucherType` 枚举
- **问题**: 前端 valueEnum 包含 `0: { text: '记账凭证' }`，但后端 VoucherType 枚举值从 1 开始（Receipt=1, Payment=2, Transfer=3），不存在值 0
- **后果**: 后端永远不会返回 voucherType=0 的凭证，该选项永远不会被使用；如果前端筛选选择"记账凭证"（值0），后端将返回空结果
- **修复**: 移除 `0: { text: '记账凭证' }`，或将后端枚举增加 General=0 值

### P0-03: 报销单状态枚举值错位导致功能按钮错误
- **模块**: 费用 - 报销管理
- **前端文件**: `finance-web/src/modules/expense/claim/list/index.tsx`
- **后端文件**: `backend/.../Enums/BusinessEnums.cs` → `ExpenseClaimStatus` 枚举
- **问题**:
  - 后端 ExpenseClaimStatus: Draft=0, PendingApproval=1, **Approving=2**, Approved=3, Rejected=4, **Paid=5**, **Voided=6**
  - 前端 valueEnum: `{ 0:'草稿', 1:'审批中', 2:'已通过', 3:'已驳回', 4:'已付款' }` — 偏移了 1
  - 前端 TypeScript 类型: `status: number; // 0草稿 1待审批 2已审批 3已驳回 4已付款` — 也偏移了
  - **关键**: 前端"确认付款"按钮显示条件 `r.status === 2`，但后端已通过是 3、已付款是 5。前端认为 2 是"已通过"，实际 2 是后端的"审批中"
  - 后端有 3 个状态（Approving=2, Paid=5, Voided=6）在前端完全没有映射
- **后果**: 付款确认按钮在错误的状态下显示/隐藏；状态筛选全部错位
- **修复**: 统一为后端 7 个状态值

### P0-04: 资产状态枚举值完全错位
- **模块**: 资产 - 资产卡片
- **前端文件**: `finance-web/src/modules/asset/card/index.tsx`
- **后端文件**: `backend/.../Enums/BusinessEnums.cs` → `AssetStatus` 枚举
- **问题**:
  - 后端: InUse=0, Idle=1, Disposed=2, Scrapped=3
  - 前端: `{ 1:'在用', 2:'闲置', 3:'维修中', 4:'已处置', 5:'已报废' }` — 全部偏移 1 且多了不存在的"维修中"
- **后果**: 所有资产状态显示错误，筛选无效
- **修复**: 改为 `{ 0:'在用', 1:'闲置', 2:'已处置', 3:'已报废' }`

### P0-05: 资产报表页面状态枚举同样错位
- **模块**: 资产 - 资产报表
- **前端文件**: `finance-web/src/modules/asset/report/index.tsx`
- **后端文件**: `backend/.../Enums/BusinessEnums.cs` → `AssetStatus`
- **问题**: 使用 `{ 1:'在用', 2:'停用', 3:'已处置' }` 而后端是 `{ 0:'在用', 1:'闲置', 2:'已处置', 3:'已报废' }`
- **后果**: 状态显示和筛选全部错误
- **修复**: 与后端枚举对齐

### P0-06: SysModule 类型 IsEnabled/IsCore 布尔 vs 整型不匹配
- **模块**: 系统 - 模块管理
- **前端文件**: `finance-web/src/types/system.d.ts` → `SysModule`
- **后端文件**: `backend/.../Entities/SysModule.cs`
- **问题**: 前端类型定义 `isEnabled: boolean` 和 `isCore: boolean`，后端实体是 `public int IsEnabled` 和 `public int IsCore`（0/1 整数）
- **后果**: JSON 序列化后前端收到数字 0/1 但 TypeScript 期望布尔值，`<Switch checked={m.isEnabled}>` 可能表现为始终选中（因为 1 是 truthy），但类型检查会报错
- **修复**: 前端类型改为 `isEnabled: number` 和 `isCore: number`

### P0-07: 审批模板 API 返回类型不匹配
- **模块**: 审批 - 流程模板
- **前端文件**: `finance-web/src/api/approval.ts`
- **后端文件**: `backend/.../Controllers/ApprovalControllers.cs` → `ApprovalFlowController.GetList`
- **问题**: 前端调用 `templateList` 发送 `PageParams`（pageIndex/pageSize），期望 `PagedResult<ApprovalTemplate>`；但后端 `GetList` 返回 `ApiResult<List<ApprovalFlow>>`（无分页）
- **后果**: 前端 `createProTableRequest` 解构 `res.data.list` / `res.data.total`，但后端返回的是数组不是分页对象，导致 ProTable 不显示数据或报错
- **修复**: 后端增加分页支持，或前端改为非分页模式处理

### P0-08: 纳税申报页面缺少 TaxCategoryId 查询参数
- **模块**: 税务 - 纳税申报
- **前端文件**: `finance-web/src/modules/tax/declaration/index.tsx`
- **后端文件**: `backend/.../DTOs/TaxDtos.cs` → `TaxDeclarationQuery`
- **问题**: 前端 ProTable 的列 `dataIndex: 'taxName'` 用于搜索，但后端 `TaxDeclarationQuery` 的筛选字段是 `TaxCategoryId`（long?），前端直接传 `taxName` 字符串作为查询参数，后端不识别
- **后果**: 按税种名称搜索将无效
- **修复**: 前端搜索字段改为 taxCategoryId，使用 Select 下拉选择税种

### P0-09: 纳税申报状态枚举值与后端不一致
- **模块**: 税务 - 纳税申报
- **前端文件**: `finance-web/src/modules/tax/declaration/index.tsx`
- **后端文件**: `backend/.../Enums/BusinessEnums.cs` → `TaxDeclarationStatus`
- **问题**:
  - 后端: Pending=0, Declared=1, Paid=2（3 个状态）
  - 前端: `{ 0:'草稿', 1:'待申报', 2:'已申报', 3:'已缴纳' }`（4 个状态，且标签语义不一致）
  - 前端操作按钮：status=0 点击"申报"，status=1 点击"确认缴纳"，但 status=0 对应后端"待申报"而非"草稿"
- **后果**: 状态文字显示与后端不一致，操作按钮可能在不正确状态下显示
- **修复**: 对齐为 `{ 0:'待申报', 1:'已申报', 2:'已缴纳' }`

### P0-10: 纳税申报操作按钮路由错误
- **模块**: 税务 - 纳税申报
- **前端文件**: `finance-web/src/modules/tax/declaration/index.tsx`
- **后端文件**: `backend/.../Controllers/TaxControllers.cs` → `TaxDeclarationController`
- **问题**: 前端对 status=0（待申报）和 status=1（已申报）的操作都调用 `taxApi.declarationSubmit(r.id)`，但后端有独立接口 `POST /{id}/declare`（申报）和 `POST /{id}/pay`（缴款）
- **后果**: 确认缴纳操作实际调用了错误接口
- **修复**: status=0 调用 `declarationDeclare`，status=1 调用 `declarationPay`

---

## 二、P1 问题（数据显示错误/功能异常）

### P1-01: 资产负债表 API 返回类型完全不同
- **模块**: 报表 - 资产负债表
- **前端文件**: `finance-web/src/types/report.d.ts`, `finance-web/src/api/report.ts`
- **后端文件**: `backend/.../DTOs/ReportDtos.cs` → `BalanceSheetResult`, `BalanceSheetItem`
- **问题**:
  - 后端返回: `{ reportDate, currency, unit, assets: BalanceSheetItem[], liabilities: BalanceSheetItem[], equity: BalanceSheetItem[], totalAssets, totalLiabilitiesAndEquity, isBalanced }` — 一个对象
  - 前端期望: `BalanceSheetRow[]` — 一个扁平数组，且字段不同：`startBalance`/`endBalance`（后端是 `beginningBalance`/`endingBalance`），前端有 `lineType: 'header'|'item'|'total'`（后端没有）
- **后果**: 前端无法正确解析后端返回数据，资产负债表页面无法渲染
- **修复**: 适配后端返回结构，或后端改为返回扁平列表

### P1-02: 利润表 API 返回类型不完全匹配
- **模块**: 报表 - 利润表
- **前端文件**: `finance-web/src/types/report.d.ts`, `finance-web/src/api/report.ts`
- **后端文件**: `backend/.../DTOs/ReportDtos.cs` → `IncomeStatementResult`, `IncomeStatementItem`
- **问题**:
  - 后端返回: `{ period, dataType, items: [{ lineNo, itemName, currentAmount, previousAmount?, growthRate? }] }`
  - 前端期望: `IncomeStatementRow[]`，字段含 `yearToDateAmount` 但后端没有
  - 后端有 `previousAmount` 和 `growthRate` 但前端类型没有
- **后果**: 利润表页面部分列数据缺失
- **修复**: 对齐字段名

### P1-03: 现金流量表 API 返回类型不匹配
- **模块**: 报表 - 现金流量表
- **前端文件**: `finance-web/src/types/report.d.ts`, `finance-web/src/api/report.ts`
- **后端文件**: `backend/.../DTOs/ReportDtos.cs` → `CashFlowResult`, `CashFlowItem`
- **问题**:
  - 后端返回: `{ period, operatingActivities: CashFlowItem[], investingActivities: CashFlowItem[], financingActivities: CashFlowItem[], netCashIncrease }` — 分三段
  - 后端 CashFlowItem 字段: `inflowAmount`, `outflowAmount`, `netAmount`
  - 前端期望: `CashFlowRow[]`，字段为 `currentAmount`, `yearToDateAmount`（无 inflow/outflow 区分）
- **后果**: 现金流量表页面无法正确渲染
- **修复**: 对齐类型

### P1-04: 科目余额表 API 返回类型不匹配
- **模块**: 报表 - 科目余额表
- **前端文件**: `finance-web/src/types/report.d.ts`
- **后端文件**: `backend/.../Controllers/ReportControllers.cs` → `SubjectBalanceReportController.Get` 返回 `ApiResult<object>`
- **问题**: 后端返回 `object`（未强类型化），前端期望 `SubjectBalanceRow[]`。结构差异未知，需确认后端实际返回格式
- **后果**: 前端类型不安全，可能运行时字段不匹配
- **修复**: 后端定义明确的返回 DTO

### P1-05: 多期对比 API 返回类型不匹配
- **模块**: 报表 - 多期对比
- **前端文件**: `finance-web/src/api/report.ts` → `CompareRow`
- **后端文件**: `backend/.../DTOs/ReportDtos.cs` → `CompareResult`, `CompareItem`
- **问题**:
  - 后端 CompareItem: `{ lineNo, itemName, values: Dictionary<string, decimal> }`（字典映射）
  - 前端 CompareRow: `{ itemName, periods: { period: string; amount: number }[] }`（数组映射）
- **后果**: 前端解析后端数据时结构不匹配
- **修复**: 统一数据结构

### P1-06: 预算年度列表 API 参数类型不匹配
- **模块**: 预算 - 年度设置
- **前端文件**: `finance-web/src/api/budget.ts` → `budgetApi.years()`
- **后端文件**: `backend/.../Controllers/BudgetControllers.cs` → `GetYears([FromQuery] BudgetYearQuery query)`
- **问题**:
  - 前端调用: `years: (params?: { status?: number }) => get<...>('/budget/setting/years', params)`
  - 后端 BudgetYearQuery 继承 PageRequest，有 `pageIndex`/`pageSize`/`sortField`/`sortOrder`，还有 `year?`/`status?`
  - 前端只传 `status`，不传分页参数，后端期望 `List<BudgetYear>`（非分页），所以分页参数会被忽略
  - 但 BudgetYearService.GetListAsync 使用 `ApplySort` 且有 WhereIF 条件，如果前端传了多余字段会作为查询参数但不被识别
- **修复**: 前端应传 `{ year?: number; status?: number }` 而非只传 status

### P1-07: 预算预警配置 API 字段名不匹配
- **模块**: 预算 - 预算预警
- **前端文件**: `finance-web/src/api/budget.ts` → `alertConfig`, `alertSaveConfig`
- **后端文件**: `backend/.../DTOs/BudgetDtos.cs` → `BudgetAlertConfig`, `BudgetAlertConfigRequest`
- **问题**:
  - 前端期望: `{ budgetYearId, thresholdRate, alertEnabled: boolean }`
  - 后端实际: `BudgetAlertConfig` 有 `BudgetYearId`, `AlertThreshold`（decimal）, `IsEnabled`（int）, `AlertMethod`（int）
  - 后端 `BudgetAlertConfigRequest` 有 `AlertThreshold`, `IsEnabled`, `AlertMethod`
  - 前端 `thresholdRate` ≠ 后端 `AlertThreshold`，`alertEnabled`（boolean）≠ `IsEnabled`（int）
- **后果**: 预警配置无法正确读取和保存
- **修复**: 统一字段名和类型

### P1-08: 预算调整列表使用废弃 API 别名
- **模块**: 预算 - 预算调整
- **前端文件**: `finance-web/src/modules/budget/adjust/index.tsx`
- **API 文件**: `finance-web/src/api/budget.ts` → `adjustList`（@deprecated 别名）
- **问题**: 调用 `budgetApi.adjustList(params)` 走 `/budget/adjustment`（无 `/list` 后缀），但后端没有 `GET /api/budget/adjustment` 列表接口（只有 `POST /api/budget/adjustment` 创建和 `POST /api/budget/adjustment/{id}/approve`）
- **后果**: 调用 404 接口，调整列表无法加载
- **修复**: 后端增加调整列表查询接口

### P1-09: 预算预警列表使用废弃 API 别名
- **模块**: 预算 - 预算预警
- **前端文件**: `finance-web/src/modules/budget/alert/index.tsx`
- **API 文件**: `finance-web/src/api/budget.ts` → `alertList`（@deprecated 别名）
- **问题**: 调用 `budgetApi.alertList(params)` 走 `/budget/alert/check`，但前端传分页参数（PageParams），后端 `CheckAlerts` 只接受 `budgetYearId` 参数
- **后果**: 预警列表页面不正常工作
- **修复**: 重新设计预警列表页面或后端增加分页接口

### P1-10: 预算编制页面使用废弃 API
- **模块**: 预算 - 预算编制
- **前端文件**: `finance-web/src/modules/budget/plan/index.tsx`
- **API 文件**: `finance-web/src/api/budget.ts` → `planList`（@deprecated）
- **问题**: 调用 `budgetApi.planList(params)` 实际调用 `/budget/setting/subject/list`，传分页参数但 subjectList 需要 `yearId` 必填参数
- **后果**: 列表无法加载（缺少 yearId）
- **修复**: 使用正确的 API 或重新设计页面

### P1-11: 自定义报表模板新增 API 请求体字段不匹配
- **模块**: 报表 - 自定义报表
- **前端文件**: `finance-web/src/api/report.ts` → `templateAdd`
- **后端文件**: `backend/.../DTOs/ReportDtos.cs` → `ReportTemplateRequest`
- **问题**:
  - 前端发送: `{ templateName, reportType: string, config: Record<string, unknown> }`
  - 后端期望: `{ templateName, description?, templateData: string }` — 后端是 `templateData`（JSON 字符串），前端发的是 `config`（对象）和 `reportType`（后端没有此字段）
- **后果**: 创建模板时后端收到空 templateData
- **修复**: 前端发送 `{ templateName, description, templateData: JSON.stringify(config) }`

### P1-12: 自定义报表模板修改 API 请求体不匹配
- **模块**: 报表 - 自定义报表
- **前端文件**: `finance-web/src/api/report.ts` → `templateUpdate`
- **后端文件**: `backend/.../DTOs/ReportDtos.cs` → `ReportTemplateRequest`
- **问题**: 与 P1-11 相同，前端发 `config` 对象，后端期望 `templateData` 字符串
- **后果**: 修改模板时数据丢失
- **修复**: 同 P1-11

### P1-13: Dashboard 页面 noticeApi.list 返回类型与使用不匹配
- **模块**: Dashboard - 工作台
- **前端文件**: `finance-web/src/modules/dashboard/index.tsx`
- **API 文件**: `finance-web/src/api/system.ts` → `noticeApi.list`
- **问题**: `noticeApi.list(2)` 返回 `SysNotice[]`，但 Dashboard 中 `const noticeData = noticeRes.value.data as number[]` 将其断言为 `number[]`，然后 `notices.map((n: SysNotice) => n.title)` 使用 SysNotice 类型
- **后果**: 类型断言错误但运行时不一定崩溃（因为 JavaScript 动态类型），但语义混乱
- **修复**: 去掉错误的 `as number[]` 断言

### P1-14: Dashboard 页面 expenseApi.statistics 返回类型不确定
- **模块**: Dashboard - 工作台
- **前端文件**: `finance-web/src/modules/dashboard/index.tsx`
- **API 文件**: `finance-web/src/api/expense.ts` → `expenseApi.statistics`
- **后端文件**: `backend/.../Controllers/ExpenseControllers.cs` → `ExpenseStatisticsController.GetStatistics`
- **问题**: 后端返回 `ApiResult<List<object>>`（弱类型），前端 `expenseApi.statistics` 声明返回 `ExpenseStats[]`，Dashboard 又断言为 `number[] | { totalAmount?: number }`。多重类型不一致
- **后果**: 月度费用统计数据可能无法正确显示
- **修复**: 后端定义明确的统计结果 DTO

### P1-15: 借款列表 API URL 不匹配
- **模块**: 费用 - 借款管理
- **前端文件**: `finance-web/src/api/expense.ts` → `loanApi.list`
- **后端文件**: `backend/.../Controllers/ExpenseControllers.cs` → `ExpenseLoanController.GetList`
- **问题**: 前端调用 `GET /expense/loan`（带 PageParams），后端接收 `ExpenseLoanQuery`（有 PageIndex/PageSize/Status/Keyword/SortField/SortOrder）。URL 路径一致，但前端 `loanApi.list` 传 `PageParams`（含 sortField/sortOrder），而后端 `ExpenseLoanQuery` 有自己的 `SortField`/`SortOrder` — 这个是一致的
- **问题实际**: 前端 `loanApi.create` 发送 `Omit<ExpenseLoan, 'id'>` 但后端 `ExpenseLoanRequest` 只有 `{ LoanAmount, Reason?, ExpectedReturnDate? }`，前端可能发送多余字段
- **后果**: 后端忽略多余字段但不会出错
- **修复**: 前端发送请求体只包含后端需要的字段

### P1-16: 发票管理 API 缺少 totalAmount 字段
- **模块**: 税务 - 发票管理
- **前端文件**: `finance-web/src/types/tax.d.ts` → `Invoice`
- **后端文件**: `backend/.../DTOs/TaxDtos.cs` → `TaxInvoiceRequest`
- **问题**: 前端 Invoice 类型有 `totalAmount` 字段，前端 `invoiceAdd` 发送 `amountWithoutTax` 和 `taxAmount`，后端不自动计算 `totalAmount`
- **后果**: `totalAmount` 在发票列表中可能为 0
- **修复**: 后端自动计算或前端手动传 totalAmount

### P1-17: 借款模块页面缺失
- **模块**: 费用 - 借款管理
- **前端文件**: `finance-web/src/modules/expense/loan/index.tsx`（存在但需验证）
- **后端文件**: `backend/.../Controllers/ExpenseControllers.cs` → `ExpenseLoanController`
- **问题**: 后端有完整的借款 CRUD + 审批 + 核销接口，前端 `loanApi` 也定义了对应 API 方法，但需确认借款页面是否集成了所有功能（审批、核销等）
- **后果**: 部分借款功能可能未在 UI 上暴露

### P1-18: 资产报表台账使用 reportLedger 字段不匹配
- **模块**: 资产 - 报表
- **前端文件**: `finance-web/src/modules/asset/report/index.tsx`
- **API 文件**: `finance-web/src/api/asset.ts` → `reportLedger`
- **问题**: 前端发送 `params as any` 到 `assetApi.reportLedger`，包含 `categoryId`, `status`，但后端 `AssetReportQuery` 接收 `AssetCode`, `AssetName`, `CategoryId`, `Status` — 前端没有传 AssetCode/AssetName 的搜索能力
- **后果**: 搜索功能不完整但不会崩溃

---

## 三、P2 问题（一致性/最佳实践）

### P2-01: createProTableRequest 使用不一致
- **模块**: 多模块
- **问题描述**: 部分页面使用 `createProTableRequest` 包装请求，部分使用内联 request 函数

| 使用 createProTableRequest ✅ | 内联 request 函数 ⚠️ |
|---|---|
| account/subject, account/voucher/list, account/voucher/detail | account/ledger/general |
| account/auxiliary, account/balance, account/period | account/ledger/detail |
| approval/pending, approval/done, approval/my | account/ledger/journal |
| approval/template | approval/pending, approval/done, approval/my（已改用 createProTableRequest） |
| asset/card, asset/card-detail, asset/category | budget/execution（自定义 request 因非分页） |
| asset/depreciation, asset/dispose, asset/inventory | system/notice（自定义 request 因非分页） |
| asset/change, asset/report | |
| budget/setting, budget/adjust | |
| expense/claim/list, expense/type, expense/allocate | |
| expense/payment, expense/statistics, expense/loan | |
| system/user, system/role, system/post, system/dict | |
| system/log | |
| tax/type, tax/invoice, tax/declaration | |

- **需要统一**: account/ledger/general, account/ledger/detail, account/ledger/journal 三个账簿页面（非标准分页 API，可接受自定义 request）

### P2-02: Search defaultCollapsed 配置不一致
- **问题**: 部分 ProTable 页面的搜索栏配置有 `defaultCollapsed: true`，部分缺少

| 有 defaultCollapsed: true ✅ | 缺少 ⚠️ |
|---|---|
| account/voucher/list, account/subject, account/auxiliary | account/ledger/general |
| account/balance, account/period | account/ledger/detail |
| approval/template | account/ledger/journal |
| asset/card, asset/card-detail, asset/category | |
| asset/depreciation, asset/dispose, asset/inventory | |
| asset/change, asset/report | |
| budget/setting, budget/plan, budget/adjust, budget/alert | |
| expense/claim/list, expense/type, expense/allocate | |
| expense/payment, expense/statistics, expense/loan | |
| system/user, system/role, system/post | |
| system/dict (type table 有, item table 无 search) | |
| system/log | |
| tax/type, tax/invoice, tax/declaration | |

- **说明**: 缺少的三个账簿页面使用内联 request，且 search 配置方式不同。建议统一添加

### P2-03: 三态处理（Loading/Error/Empty）覆盖不完整
- **有完整三态（Loading/Error/Empty）的页面 ✅**:
  - system/menu, system/dept, system/module, system/config
  - tax/report, budget/analysis
  - dashboard（有 Loading/Error 但无 Empty）

- **仅有 Loading 的页面（无 Error/Empty）**:
  - 大多数 ProTable 页面（ProTable 自带 loading 和 empty 状态）
  - 这是可接受的，因为 ProTable 内置处理了

- **完全无 Loading/Error 处理的页面 ⚠️**:
  - tax/burden（数据全为硬编码 0，无 API 调用）
  - tax/calendar（仅渲染日历组件，无 API 调用）

### P2-04: DictType 搜索缺少 defaultCollapsed
- **模块**: 系统 - 数据字典
- **文件**: `finance-web/src/modules/system/dict/index.tsx`
- **问题**: 字典类型 ProTable 的 search 配置没有 `defaultCollapsed`，展开所有搜索字段
- **修复**: 添加 `defaultCollapsed: true`

### P2-05: 审批模板页面缺少 isEnabled 字段映射
- **模块**: 审批 - 流程模板
- **文件**: `finance-web/src/modules/approval/template/index.tsx`
- **问题**: ProTable 列 status 的 valueEnum 是 `{ 0:'禁用', 1:'启用' }`，但后端 ApprovalFlow 实体的字段名是 `IsEnabled`（不是 `status`），前端列 dataIndex 为 `status` 不会匹配到后端数据
- **修复**: 列 dataIndex 改为 `isEnabled`

### P2-06: 审批模板页面缺少编辑/删除操作按钮
- **模块**: 审批 - 流程模板
- **文件**: `finance-web/src/modules/approval/template/index.tsx`
- **问题**: ProTable 没有操作列，API 层定义了 templateUpdate 和 templateRemove 方法但页面未使用
- **修复**: 增加操作列

### P2-07: Dashboard 未导入 SysNotice 类型
- **模块**: Dashboard - 工作台
- **文件**: `finance-web/src/modules/dashboard/index.tsx`
- **问题**: 代码中使用 `SysNotice` 类型（`notices.map((n: SysNotice) => n.title)`），但未从 `@/api/system` 导入
- **后果**: TypeScript 编译错误（如果 TS 严格模式下）
- **修复**: `import { noticeApi, type SysNotice } from '@/api/system'`

### P2-08: 费用统计 API 参数类型不明确
- **模块**: 费用 - 统计
- **前端文件**: `finance-web/src/api/expense.ts` → `expenseApi.statistics`
- **后端文件**: `backend/.../DTOs/ExpenseDtos.cs` → `ExpenseStatisticsQuery`
- **问题**: 前端声明 `statistics: (params: Record<string, unknown>)`（弱类型），后端期望 `{ year?, startDate, endDate, deptId?, expenseTypeId? }`。Dashboard 调用 `expenseApi.statistics({ startDate: '...' })` 只传了 startDate 没传 endDate
- **修复**: 定义明确的 TypeScript 接口

### P2-09: 系统配置 API list group 参数后端不存在
- **模块**: 系统 - 配置管理
- **前端文件**: `finance-web/src/api/system.ts` → `configApi.list`
- **后端文件**: `backend/.../Controllers/DictLogModuleConfigController.cs` → `ConfigController.GetList`
- **问题**: 前端 `list: (group?: string) => get<SysConfig[]>('/system/config/list', { group })` 传 group 参数，但后端 `GetList()` 无参数
- **后果**: group 参数被后端忽略，不影响功能但语义不一致
- **修复**: 后端增加 group 过滤或前端移除参数

### P2-10: 系统配置 batchUpdate 请求体类型不匹配
- **模块**: 系统 - 配置管理
- **前端文件**: `finance-web/src/modules/system/config/index.tsx`
- **后端文件**: `backend/.../Controllers/DictLogModuleConfigController.cs` → `ConfigController.BatchUpdate`
- **问题**: 前端发送 `{ configKey: string; configValue: string }[]`，后端期望 `List<ConfigUpdateRequest>`（需确认 ConfigUpdateRequest 结构）
- **修复**: 需确认 ConfigUpdateRequest 是否有 configKey/configValue 字段

### P2-11: 税务报表 reportList 使用错误返回类型
- **模块**: 税务 - 报表
- **前端文件**: `finance-web/src/api/tax.ts` → `reportList`（兼容旧调用别名）
- **问题**: 声明返回 `TaxReport[]` 但实际请求的是 `/tax/declaration/list`（返回 PagedResult<TaxDeclaration>），类型完全不对
- **修复**: 使用正确的类型或移除废弃别名

### P2-12: 税务日历 calendar 使用错误 API
- **模块**: 税务 - 日历
- **前端文件**: `finance-web/src/api/tax.ts` → `calendar`（兼容旧调用别名）
- **问题**: 请求 `/tax/category/list`（税种列表）而非 `/tax/calendar`（日历接口）。API 文件已有正确的 `calendarList` 方法，但旧别名仍然存在
- **修复**: 移除废弃别名 `calendar`

### P2-13: 报表导出使用 GET 方法传复杂参数
- **模块**: 报表 - 导出
- **前端文件**: `finance-web/src/api/report.ts` → `reportExportApi.excel/pdf`
- **后端文件**: `backend/.../Controllers/ReportControllers.cs` → `ReportExportController`
- **问题**: 前端使用 GET 请求传 `{ reportType, period }` 参数，如果后端返回文件流，GET 方法通常是合适的，但需确认后端是否返回 URL 字符串还是文件流
- **说明**: 后端返回 `ApiResult<string>`（URL），前端用 `get<string>` 是正确的

---

## 四、Budget 模块 ApplySort 集成验证

### ApplySort 扩展实现
- **文件**: `backend/.../Extensions/QueryableSortExtensions.cs`
- **功能**: 支持 camelCase/PascalCase 字段名动态排序
- **签名**: `ApplySort<T>(this ISugarQueryable<T> queryable, string? sortField, string? sortOrder)`

### 集成验证结果

| 服务 | 使用 ApplySort ✅ | 参数来源 | 验证结果 |
|---|---|---|---|
| BudgetYearService.GetListAsync | ✅ `ApplySort(query.SortField, query.SortOrder)` | BudgetYearQuery（继承 PageRequest） | ✅ 正确 |
| BudgetSubjectService.GetListAsync | ✅ `ApplySort(sortField, sortOrder)` | Controller 直接传参 | ✅ 正确 |
| 其他服务 | 未使用 | N/A | N/A（不需要） |

### 前端排序参数传递验证
- `createProTableRequest` 会将 ProTable 的排序参数转换为 `sortField` 和 `sortOrder`
- 前端 `PageParams` 包含 `sortField?: string` 和 `sortOrder?: string`
- 后端 `PageRequest` 包含 `SortField` 和 `SortOrder`（JSON 序列化后 camelCase 一致）
- **结论**: ApplySort 集成正确，前端排序参数可正确传递到后端

---

## 五、统计摘要

| 严重级别 | 数量 | 说明 |
|---|---|---|
| **P0（运行时崩溃/数据丢失）** | **10** | 必须立即修复 |
| **P1（功能异常/数据错误）** | **18** | 需尽快修复 |
| **P2（一致性/最佳实践）** | **13** | 计划修复 |

### 按模块分布

| 模块 | P0 | P1 | P2 |
|---|---|---|---|
| 会计（Account） | 2 | 0 | 2 |
| 费用（Expense） | 1 | 3 | 1 |
| 资产（Asset） | 2 | 1 | 0 |
| 审批（Approval） | 1 | 0 | 2 |
| 预算（Budget） | 0 | 5 | 0 |
| 报表（Report） | 0 | 5 | 1 |
| 税务（Tax） | 3 | 2 | 2 |
| 系统（System） | 1 | 0 | 3 |
| Dashboard | 0 | 2 | 1 |

### 重点修复建议（按优先级）

1. **P0-03/P0-04**: 费用报销和资产状态枚举全面错位 — 影响核心业务流程
2. **P0-01**: 凭证列表借方/贷方金额不显示 — 财务核心数据
3. **P0-06**: SysModule 布尔/整型不匹配 — 可能导致模块管理异常
4. **P0-07**: 审批模板分页/非分页不匹配 — 审批管理不可用
5. **P1-01/P1-02/P1-03**: 三大报表返回类型完全不同 — 报表模块不可用
6. **P1-07**: 预算预警配置字段名不匹配 — 预算预警不可用
7. **P1-08/P1-09/P1-10**: 预算模块使用废弃 API — 部分预算功能不可用
8. **P0-08/P0-09/P0-10**: 税务申报参数和操作路由错误 — 税务流程异常
