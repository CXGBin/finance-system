# 前后端数据模型对齐审查报告

> **版本**: v1.0  
> **日期**: 2026-06-16  
> **审查范围**: 后端所有 Entity/DTO vs 前端 TypeScript 类型定义 + API调用参数 + 表格 dataIndex  
> **项目路径**: `/home/ubuntu/.openclaw/workspace/finance-system/`

---

## 一、审查概览

| 维度 | 数量 |
|------|------|
| 审查后端 Entity 文件 | 33 个 .cs |
| 审查后端 DTO 文件 | 15 个 .cs |
| 审查后端 Controller 端点 | 199 个 |
| 审查前端类型定义文件 | 10 个 .d.ts |
| 审查前端 API 封装文件 | 9 个 .ts |
| 审查前端页面文件 | 61 个 .tsx |

### JSON 序列化配置确认

后端全局配置 `CamelCase` 策略（`Program.cs` 第40行）：
```csharp
options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
```
- C# `PascalCase` → JSON `camelCase` ✅
- 异常中间件 `GlobalExceptionMiddleware` 和模块切换中间件 `ModuleSwitchMiddleware` 也均使用 `CamelCase` ✅
- 后端 Entity/DTO 无 `[JsonPropertyName]` 自定义重命名 ✅

**结论**：所有后端 C# 属性序列化后自动转为 camelCase，前端类型定义已使用 camelCase，命名策略层面**一致无问题**。

---

## 二、发现的不一致问题清单

### P0 - 阻断级（功能完全不可用）

#### P0-1: 税务申报 API 请求参数名不匹配
- **后端**: `TaxCalculateRequest` 参数 `TaxCategoryId` + `DeclarePeriod`
- **前端API**: `taxApi.declarationCalculate(data: { taxTypeId, period })` 
- **问题**: 前端发送 `{ taxTypeId: 1, period: "2026-06" }` → 后端接收 `TaxCategoryId=undefined`, `DeclarePeriod=undefined`
- **文件**: `finance-web/src/api/tax.ts` 第19行
- **修复**: 改为 `{ taxCategoryId: number; declarePeriod: string }`，与后端 DTO 字段名对齐

#### P0-2: 税务申报页面调用不存在的 API 方法
- **前端页面**: `tax/declaration/index.tsx` 第20-21行调用 `taxApi.declarationSubmit(record.id)`
- **实际API定义**: `tax.ts` 中定义的是 `declarationDeclare(id)` 和 `declarationPay(id)`，**无** `declarationSubmit` 方法
- **问题**: 点击"申报"和"确认缴纳"均会报 `taxApi.declarationSubmit is not a function`，功能完全不可用
- **文件**: `finance-web/src/modules/tax/declaration/index.tsx` 第20-21行
- **修复**: status===0 时调用 `taxApi.declarationDeclare(record.id)`，status===1 时调用 `taxApi.declarationPay(record.id)`

#### P0-3: 审批列表页面引用后端不存在的字段
- **前端列定义**: `dataIndex: 'approvalNo'`, `dataIndex: 'initiatorName'`
- **后端 ApprovalInstance 实体**: 无 `ApprovalNo` 和 `InitiatorName` 字段，只有 `BusinessId` 和 `InitiatorId`
- **问题**: 审批单号和发起人列永远显示为空
- **文件**: `finance-web/src/modules/approval/pending/index.tsx`、`approval/done/index.tsx`、`approval/my/index.tsx`
- **修复**: 方案A（后端）：在 ApprovalInstance 实体添加 `ApprovalNo`(SugarColumn IsIgnore)和 `InitiatorName`字段，Service 查询时 join 用户表填充；方案B（前端）：改用 `businessId` 作为单号，`initiatorId` 通过接口查询名称

---

### P1 - 功能异常级（部分功能失效或数据展示错误）

#### P1-1: TaxType 前端类型 `calculationMethod`/`declareCycle` 类型为 string，后端为 int
- **后端 TaxCategory**: `CalculationMethod: int`（1从价 2从量 3复合），`DeclareCycle: int`（1月 2季 3年）
- **前端 TaxType**: `calculationMethod?: string`，`declareCycle?: string`
- **问题**: 前端类型标注为 string，但实际接收 number，可能导致类型检查通过但运行时逻辑错误（如 `[v]` 下标访问）
- **文件**: `finance-web/src/types/tax.d.ts` 第7-8行
- **修复**: 改为 `calculationMethod?: number`，`declareCycle?: number`

#### P1-2: Invoice 前端类型 `invoiceType`/`direction` 类型为 string，后端为 int
- **后端 TaxInvoice**: `InvoiceType: int`（1专票 2普票 3其他），`Direction: int`（1进项 2销项），`IsVerified: int`
- **前端 Invoice**: `invoiceType: string`，`direction: string`，`isVerified?: boolean`
- **问题**: `isVerified` 后端返回 `0/1`(int)，前端类型标注 `boolean`，实际为 number；`invoiceType`/`direction` 同理
- **文件**: `finance-web/src/types/tax.d.ts` 第28-35行
- **修复**: `invoiceType: number`，`direction: number`，`isVerified: number`

#### P1-3: BudgetAdjust 前端类型 `adjustType` 为 string，后端为 int
- **后端 BudgetAdjustment**: `AdjustType: int`，`ApproveStatus: int`
- **前端 BudgetAdjust**: `adjustType: string`
- **文件**: `finance-web/src/types/budget.d.ts` 第39行
- **修复**: `adjustType: number`

#### P1-4: AssetChange 前端类型 `changeType` 为 string，后端为 int
- **后端 AssetChange**: `ChangeType: int`（1调拨 2处置 3报废）
- **前端 AssetChange**: `changeType: string`
- **文件**: `finance-web/src/types/asset.d.ts` 第39行
- **修复**: `changeType: number`

#### P1-5: SysRole 实体无 CreatedTime，前端类型定义了 createdTime
- **后端 SysRole**: 继承 `BaseEntity`，只有 `Id` + `CreatedTime`
- **前端 SysRole**: 有 `createdTime?: string` 
- **问题**: 实际后端 SysRole **有** CreatedTime（继承自 BaseEntity），前端定义正确 ✅
- **但**: 前端 SysRole 额外包含 `component`, `icon`, `parentId`, `path`, `permission`, `menuName`, `moduleId` 等字段，这些是 **Menu** 的字段，不是 Role 的字段。**类型污染**
- **文件**: `finance-web/src/types/system.d.ts` SysRole interface
- **修复**: 从 SysRole 中移除 `component`, `icon`, `parentId`, `path`, `permission`, `menuName`, `moduleId`

#### P1-6: DictItem 前端类型包含不属于自己的字段
- **后端 SysDictData**: `DictLabel`, `DictType`, `DictValue`, `SortOrder`, `Status`, `Remark` + `Id`, `CreatedTime`
- **前端 DictItem**: 额外包含 `action`, `description`, `ipAddress`, `module`, `requestBody`, `requestMethod`, `requestUrl`, `userId`, `userName`
- **问题**: 这些是 OperLog（操作日志）的字段，被混入 DictItem 类型
- **文件**: `finance-web/src/types/system.d.ts` DictItem interface
- **修复**: 清理 DictItem，仅保留后端实际返回的字段

#### P1-7: AssetCard 前端有 `netBookValue` 字段，后端为 `netValue`
- **后端 AssetCard**: `NetValue` (序列化后 `netValue`)
- **前端 AssetCard**: 同时定义了 `netValue: number` 和 `netBookValue: number`（来自 DepreciationRecord 的混淆）
- **问题**: `netBookValue` 在 AssetCard 列表中永远不会有值（后端不返回此字段），多余字段
- **文件**: `finance-web/src/types/asset.d.ts` AssetCard interface
- **修复**: 移除 `netBookValue`，仅保留 `netValue`

#### P1-8: DepreciationRecord 前端类型字段与后端 AssetDepreciation 不匹配
- **后端 AssetDepreciation**: `AssetCardId`, `PeriodId`, `Month`, `DepreciationAmount`, `AccumulatedDepreciation`, `NetValue` + `Id`, `CreatedTime`
- **前端 DepreciationRecord**: `assetId`, `assetName`, `period`, `originalValue`, `netBookValue`, `depreciationAmount`, `cumulativeDepreciation`
- **问题**: 
  - `assetId` → 后端为 `assetCardId`，**名称不匹配**
  - `period` → 后端无此字段（有 `month` 和 `periodId`）
  - `originalValue` → 后端无此字段
  - `cumulativeDepreciation` → 后端为 `accumulatedDepreciation`，**名称不匹配**
  - `netBookValue` → 后端为 `netValue`，**名称不匹配**
- **文件**: `finance-web/src/types/asset.d.ts` DepreciationRecord interface
- **修复**: 完整重写对齐后端 AssetDepreciation 实体字段

#### P1-9: AssetInventory 前端类型字段严重缺失
- **后端 AssetInventory**: `InventoryNo`, `InventoryDate`, `OperatorId`, `ItemsJson`, `Status` + `Id`, `CreatedTime`
- **前端 AssetInventory**: 只有 `id`, `name`, `status`, `createTime`
- **问题**: 缺少 `inventoryNo`, `inventoryDate`, `operatorId`, `itemsJson` 等关键字段；`name` 字段后端不存在
- **文件**: `finance-web/src/types/asset.d.ts` AssetInventory interface
- **修复**: 按后端 AssetInventory 实体重新定义

#### P1-10: AssetChange 前端类型字段严重缺失
- **后端 AssetChange**: `AssetCardId`, `ChangeType`, `Reason`, `FromDeptId`, `ToDeptId`, `DisposalIncome`, `OperatorId` + `Id`, `CreatedTime`
- **前端 AssetChange**: `changeDate: string`, `description: string`, `operator: string`（后端无这些字段名）
- **问题**: `changeDate`/`description`/`operator` 后端不存在；缺少 `assetCardId`, `reason`, `disposalIncome` 等字段
- **文件**: `finance-web/src/types/asset.d.ts` AssetChange interface
- **修复**: 按后端 AssetChange 实体重新定义

#### P1-11: 资产变动页面 dataIndex 引用后端不存在的字段
- **页面列**: `dataIndex: 'assetCode'`, `'assetName'`, `'changeType'`, `'reason'`, `'disposalIncome'`, `'createdTime'`
- **后端 AssetChange 返回**: 无 `assetCode`, `assetName` 字段（只有 `assetCardId`）
- **问题**: 资产编号和名称列为空，需 join 查询
- **文件**: `finance-web/src/modules/asset/change/index.tsx`
- **修复**: 后端 Service 查询 AssetChange 时 join AssetCard 表返回 `assetCode`/`assetName`

#### P1-12: 资产盘点页面 dataIndex 引用后端不存在的字段
- **页面列**: `dataIndex: 'assetCode'`, `'assetName'`, `'location'`, `'result'`, `'operatorName'`, `'inventoryTime'`
- **后端 AssetInventory 返回**: 无 `assetCode`, `assetName`, `location`, `result`, `operatorName`, `inventoryTime`
- **问题**: 所有展示字段均为空
- **文件**: `finance-web/src/modules/asset/inventory/index.tsx`
- **修复**: 后端需 join 查询或重新设计盘点数据结构

#### P1-13: 预算预警页面 dataIndex 引用后端不存在的字段
- **页面列**: `subjectName`, `budgetAmount`, `usedAmount`, `usageRate`, `alertLevel`, `alertTime`
- **后端 BudgetAlertConfig 返回**: `budgetYearId`, `thresholdRate`, `alertEnabled`（配置信息，非列表数据）
- **问题**: 预警列表的表格字段与后端返回数据完全不匹配
- **文件**: `finance-web/src/modules/budget/alert/index.tsx`
- **修复**: 后端需实现预警检查列表接口，返回 `subjectName`, `budgetAmount`, `usedAmount`, `usageRate`, `alertLevel`

#### P1-14: 预算编制页面 dataIndex 引用后端不存在的字段
- **页面列**: `subjectCode`, `subjectName`, `yearBudget`, `usedBudget`, `remainBudget`
- **后端 BudgetSubject 返回**: `subjectId`, `annualAmount`, `deptId`（无 `subjectCode`, `subjectName`, `usedBudget`, `remainBudget`）
- **问题**: 科目编码、名称、已用、剩余均为空
- **文件**: `finance-web/src/modules/budget/plan/index.tsx`
- **修复**: 后端查询 BudgetSubject 时 join AccountSubject 获取科目信息，计算已用和剩余

---

### P2 - 展示/体验问题级

#### P2-1: SysNotice 前端 API 接口定义有非数据字段
- **文件**: `finance-web/src/api/system.ts` SysNotice interface
- **问题**: 包含 `create`, `list`, `update`, `remove` 等方法名，这些是 API 操作不是数据字段
- **修复**: 将 SysNotice 数据接口和方法定义分开，数据接口仅包含 `id`, `title`, `content`, `noticeType`, `status`, `createdBy`, `createdTime`

#### P2-2: LoginParams 前端 `remember` 字段名与后端不匹配
- **后端 LoginRequest**: `RememberMe` (序列化后 `rememberMe`)
- **前端 LoginParams**: `remember?: boolean`
- **文件**: `finance-web/src/types/auth.d.ts` 第3行
- **修复**: `rememberMe?: boolean`

#### P2-3: SysDept 前端类型包含后端不存在的字段
- **前端 Dept**: 额外包含 `deptId`（冗余，已有 `id`）, `postCode`, `postName`, `remark`
- **后端 SysDept**: 无 `postCode`, `postName`, `remark` 字段
- **文件**: `finance-web/src/types/system.d.ts` Dept interface
- **修复**: 移除 `deptId`, `postCode`, `postName`, `remark`

#### P2-4: DictType 前端类型包含后端不存在的字段
- **前端 DictType**: 包含 `dictLabel`, `dictValue`, `sortOrder`（这些是 DictData 的字段）
- **后端 SysDictType**: 只有 `DictName`, `DictType`, `Status`, `Remark` + `Id`, `CreatedTime`
- **文件**: `finance-web/src/types/system.d.ts` DictType interface
- **修复**: 移除 `dictLabel`, `dictValue`

#### P2-5: Post 前端类型包含后端不存在的字段
- **前端 Post**: 包含 `dictName`, `dictType`（字典相关字段混入）
- **后端 SysPost**: 无 `dictName`, `dictType`
- **文件**: `finance-web/src/types/system.d.ts` Post interface
- **修复**: 移除 `dictName`, `dictType`

#### P2-6: SysModule 前端类型包含后端不存在的字段
- **前端 SysModule**: 包含 `configGroup`, `configKey`, `configName`, `configValue`, `remark`
- **后端 SysModule**: 无这些字段（这些是 SysConfig 的字段）
- **文件**: `finance-web/src/types/system.d.ts` SysModule interface
- **修复**: 移除 `configGroup`, `configKey`, `configName`, `configValue`, `remark`

#### P2-7: CashFlowRow 前端类型包含不属于自己的字段
- **前端 CashFlowRow**: 包含 `level`, `periodBeginCredit`, `periodBeginDebit`, `periodCredit`, `periodDebit`, `periodEndCredit`, `periodEndDebit`, `subjectCode`, `subjectName`, `yearBeginDebit`, `yearBeginCredit`
- **问题**: 这些字段是 **SubjectBalanceRow**（科目余额表）的字段，不应出现在现金流量表行类型中
- **文件**: `finance-web/src/types/report.d.ts` CashFlowRow interface
- **修复**: CashFlowRow 仅保留 `id`, `lineNo`, `itemName`, `lineType`, `currentAmount`, `yearToDateAmount`

#### P2-8: TaxDeclaration 前端类型缺少 `taxName` 但页面使用
- **页面列**: `dataIndex: 'taxName'`
- **后端 TaxDeclaration**: 无 `taxName` 字段（只有 `taxCategoryId`）
- **文件**: `finance-web/src/modules/tax/declaration/index.tsx`
- **修复**: 后端查询时 join TaxCategory 获取 `taxName`，或前端展示 taxCategoryId

#### P2-9: TaxCalendarItem 前端类型与后端日历返回结构不匹配
- **后端返回**: `{ categoryId, taxName, taxCode, deadline, status, taxAmount, paidAmount }` (object)
- **前端 TaxCalendarItem**: `{ taxType, period, deadline, status }`
- **问题**: `taxType` → 后端为 `taxName`；`period` → 后端无此字段；`status` 后端为 number(-1/0/1/2)，前端定义为 string union
- **文件**: `finance-web/src/types/tax.d.ts` TaxCalendarItem interface
- **修复**: `{ categoryId, taxName, taxCode, deadline: string, status: number, taxAmount, paidAmount }`

#### P2-10: ExpenseAllocate 前端类型严重不足
- **后端 ExpenseAllocate**: `AllocateNo`, `Description`, `TotalAmount`, `DeptId`, `AllocateAmount`, `PeriodYear`, `PeriodMonth` + `Id`, `CreatedTime`
- **前端 ExpenseAllocate**: 只有 `id`, `allocateNo`, `description`, `amount`, `status`, `createTime`
- **问题**: `amount` → 后端为 `totalAmount` 和 `allocateAmount`；缺少 `deptId`, `periodYear`, `periodMonth`
- **文件**: `finance-web/src/types/expense.d.ts` ExpenseAllocate interface
- **修复**: 完整对齐后端实体字段

---

## 三、逐模块对齐结果汇总

### 3.1 系统管理模块 (System)

| 后端实体 | 前端类型 | 字段对齐 | 问题 |
|----------|---------|----------|------|
| SysUser → UserProfile | User | ✅ 基本对齐 | — |
| SysRole | SysRole | ⚠️ 类型污染 | P1-5: 混入 Menu 字段 |
| SysMenu | Menu | ✅ 完全对齐 | — |
| SysDept | Dept | ⚠️ 部分多余 | P2-3: 多余 deptId/postCode/postName/remark |
| SysPost | Post | ⚠️ 部分多余 | P2-5: 混入 Dict 字段 |
| SysDictType | DictType | ⚠️ 部分多余 | P2-4: 混入 DictData 字段 |
| SysDictData | DictItem | ❌ 类型污染 | P1-6: 混入 OperLog 字段 |
| SysLog | OperLog | ✅ 完全对齐 | — |
| SysNotice | SysNotice | ⚠️ 定义不规范 | P2-1: 接口定义混入方法名 |
| SysModule | SysModule | ⚠️ 类型污染 | P2-6: 混入 SysConfig 字段 |
| SysConfig | SysConfig | ✅ 完全对齐 | — |
| LoginResponse | LoginResult | ✅ 完全对齐 | — |
| UserInfoDto | UserInfo | ✅ 完全对齐 | — |

### 3.2 账务管理模块 (Accounts)

| 后端实体 | 前端类型 | 字段对齐 | 问题 |
|----------|---------|----------|------|
| AccountSubject | Subject | ✅ 完全对齐 | — |
| SubjectBalance | BalanceItem | ✅ 基本对齐 | — |
| Voucher | Voucher | ✅ 完全对齐 | — |
| VoucherEntry | VoucherEntry | ✅ 完全对齐 | — |
| AccountingPeriod | AccountingPeriod | ✅ 完全对齐 | — |
| LedgerQuery DTO | LedgerRecord | ✅ 按需使用 | — |

### 3.3 报表中心模块 (Reports)

| 后端 DTO | 前端类型 | 字段对齐 | 问题 |
|----------|---------|----------|------|
| BalanceSheetItem | BalanceSheetRow | ✅ 基本对齐 | — |
| IncomeStatementItem | IncomeStatementRow | ✅ 完全对齐 | — |
| CashFlowItem | CashFlowRow | ⚠️ 类型污染 | P2-7: 混入 SubjectBalanceRow 字段 |
| SubjectBalanceReport | SubjectBalanceRow | ✅ 基本对齐 | — |

### 3.4 预算管理模块 (Budget)

| 后端实体 | 前端类型 | 字段对齐 | 问题 |
|----------|---------|----------|------|
| BudgetSubject | BudgetItem | ✅ 基本对齐 | — |
| BudgetMonthly | BudgetMonthly | ✅ 完全对齐 | — |
| BudgetAdjustment | BudgetAdjust | ⚠️ 类型不匹配 | P1-3: adjustType 类型 string→number |
| BudgetAlertConfig | BudgetAlert | ✅ 基本对齐 | P1-13: 页面 dataIndex 全部不匹配 |

### 3.5 审批流程模块 (Approval)

| 后端实体 | 前端类型 | 字段对齐 | 问题 |
|----------|---------|----------|------|
| ApprovalFlow | ApprovalTemplate | ✅ 完全对齐 | — |
| ApprovalInstance | ApprovalInstance | ❌ 缺字段 | P0-3: 缺 approvalNo/initiatorName |

### 3.6 资产管理模块 (Asset)

| 后端实体 | 前端类型 | 字段对齐 | 问题 |
|----------|---------|----------|------|
| AssetCategory | AssetCategory | ✅ 完全对齐 | — |
| AssetCard | AssetCard | ⚠️ 多余字段 | P1-7: 多余 netBookValue |
| AssetDepreciation | DepreciationRecord | ❌ 严重不匹配 | P1-8: 多字段名不一致 |
| AssetChange | AssetChange | ❌ 严重不匹配 | P1-10: 字段完全重写 |
| AssetInventory | AssetInventory | ❌ 严重不匹配 | P1-9: 缺少核心字段 |

### 3.7 费用管理模块 (Expense)

| 后端实体 | 前端类型 | 字段对齐 | 问题 |
|----------|---------|----------|------|
| ExpenseType | ExpenseType | ✅ 完全对齐 | — |
| ExpenseClaim | ExpenseClaim | ✅ 完全对齐 | — |
| ExpenseItem | ExpenseClaimItem | ✅ 完全对齐 | — |
| ExpenseAllocate | ExpenseAllocate | ⚠️ 字段不足 | P2-10: 字段定义不完整 |
| ExpenseLoan | ExpenseLoan | ✅ 完全对齐 | — |

### 3.8 税务管理模块 (Tax)

| 后端实体 | 前端类型 | 字段对齐 | 问题 |
|----------|---------|----------|------|
| TaxCategory | TaxType | ⚠️ 类型不匹配 | P1-1: calculationMethod/declareCycle 类型 |
| TaxDeclaration | TaxDeclaration | ⚠️ 缺字段 | P2-8: 缺 taxName |
| TaxInvoice | Invoice | ⚠️ 类型不匹配 | P1-2: invoiceType/direction/isVerified 类型 |
| 日历返回对象 | TaxCalendarItem | ❌ 结构不匹配 | P2-9: 字段名和类型均不一致 |

---

## 四、严重程度统计

| 级别 | 数量 | 说明 |
|------|------|------|
| **P0 - 阻断级** | 3 | 功能完全不可用（API参数名错误、调用不存在方法、展示字段不存在） |
| **P1 - 功能异常** | 14 | 类型不匹配导致展示/逻辑错误，前端类型定义与后端实体不一致 |
| **P2 - 展示问题** | 10 | 类型定义不规范、多余字段、字段名差异 |
| **合计** | **27** | |

---

## 五、修复优先级建议

### 第一优先（立即修复）- P0 × 3
1. **P0-1** 税务申报 API 参数名 `taxTypeId→taxCategoryId`, `period→declarePeriod`
2. **P0-2** 税务申报页面 `declarationSubmit` → `declarationDeclare` / `declarationPay`
3. **P0-3** 审批列表 `approvalNo`/`initiatorName` 字段缺失（后端需补充字段）

### 第二优先（尽快修复）- P1 × 14
按模块分组修复：
- **资产模块**（P1-7/8/9/10/11/12）：5个类型需重写，2个页面需配合后端 join
- **税务模块**（P1-1/2）：3个类型 int/string 不匹配
- **预算模块**（P1-3/13/14）：1个类型 + 2个页面 dataIndex 问题
- **系统模块**（P1-5/6）：2个类型污染问题
- **审批模块**（P0-3 已计入 P0）

### 第三优先（迭代修复）- P2 × 10
- 清理所有类型定义中的污染字段和多余字段
- 统一命名规范

---

## 六、通用结论

1. **JSON 序列化配置正确**：后端全局 `CamelCase` + 前端统一 `camelCase`，命名策略层面无问题
2. **登录认证模型完全对齐**：`accessToken`/`refreshToken`/`expiresIn`/`userInfo` 前后端字段名和类型完全一致 ✅
3. **账务模块对齐度最高**：Subject/Voucher/VoucherEntry/AccountingPeriod 等核心实体与前端类型完全对齐
4. **类型污染是主要问题**：多个前端类型混入了其他实体的字段（SysRole↔Menu、DictItem↔OperLog、SysModule↔SysConfig 等），需清理
5. **资产和税务模块问题最集中**：资产模块 5 个类型需重写，税务模块存在 P0 级 API 参数名错误
6. **页面 dataIndex 与后端返回数据不匹配**是深层次问题：多个页面的列定义引用了后端不返回的字段，需后端配合 join 查询或前端修改列定义
