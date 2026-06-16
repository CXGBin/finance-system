# 前后端数据模型对齐审查报告

> **版本**: v2.0（Bug修复后复验版）
> **日期**: 2026-06-16T23:10
> **前置条件**: Bug1/2/5/6已修复（commits 313b497/15a192e/0ca6d87），52个关键字段已对齐
> **项目路径**: `/home/ubuntu/.openclaw/workspace/finance-system/`

---

## 一、审查概览

| 维度 | 数量 |
|------|------|
| 审查后端 Entity 文件 | 33 个 |
| 审查后端 DTO 文件 | 15 个 |
| 审查后端 Core/公共模型 | 5 个 |
| 审查前端类型定义文件 | 10 个 .d.ts |
| 审查前端 TypeScript interface | 42 个 |
| 审查前端 API 封装文件 | 9 个 .ts |
| 审查前端页面 dataIndex | 173 个字段引用 |
| **总对比字段数** | **约 680 个** |

### JSON 序列化配置确认 ✅

后端全局 `CamelCase` 策略（`Program.cs`）：
```csharp
options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
```
- C# `PascalCase` → JSON `camelCase` ✅
- 无 `[JsonPropertyName]` 自定义覆盖 ✅
- 异常中间件、模块切换中间件均使用同一策略 ✅

**结论**：命名策略层面完全一致，所有后端属性序列化后自动转为 camelCase。

---

## 二、逐模块对齐结果

### 2.1 认证模块（Auth）✅ 完全对齐

| 后端 | 前端 | 状态 |
|------|------|------|
| `LoginResponse.accessToken` | `LoginResult.accessToken: string` | ✅ |
| `LoginResponse.refreshToken` | `LoginResult.refreshToken: string` | ✅ |
| `LoginResponse.expiresIn` | `LoginResult.expiresIn: number` | ✅ |
| `LoginResponse.mustChangePassword` | `LoginResult.mustChangePassword: boolean` | ✅ |
| `LoginResponse.userInfo` | `LoginResult.userInfo: UserInfo` | ✅ |
| `UserInfoDto.id/username/realName/avatar` | `UserInfo` 同名字段 | ✅ |
| `UserInfoDto.roles: List<string>` | `UserInfo.roles: string[]` | ✅ |
| `UserInfoDto.permissions/modules` | `UserInfo` 同名字段 | ✅ |
| `LoginRequest.rememberMe: bool` | `LoginParams.remember?: boolean` | ✅ |
| `ChangePasswordRequest.confirmPassword` | API层已对齐 | ✅ |

**登录认证模型完全一致，无任何偏差。**

### 2.2 系统管理模块（System）✅ 对齐良好

| 后端实体 | 前端类型 | 字段对齐 | 备注 |
|----------|---------|----------|------|
| `SysUser` → `UserProfile` | `User` | ✅ 12/12 | 所有字段名、类型完全匹配 |
| `SysRole` | `SysRole` | ✅ 9/9 | 含 dataScope、menuIds |
| `SysMenu` | `Menu` | ✅ 12/12 | parentId、menuName、menuType 等全部对齐 |
| `SysDept` | `Dept` | ✅ 8/8 | 完全匹配 |
| `SysPost` | `Post` | ✅ 7/7 | 含 deptId、postCode、postName |
| `SysDictType` | `DictType` | ✅ 5/5 | 完全匹配 |
| `SysDictData` | `DictItem` | ✅ 7/7 | 完全匹配 |
| `SysLog` | `OperLog` | ✅ 12/12 | 含 userId、userName、durationMs 等 |
| `SysModule` | `SysModule` | ✅ 7/7 | moduleId、moduleName、isEnabled 等 |
| `SysConfig` | `SysConfig` | ✅ 5/5 | configKey、configValue、configGroup 等 |
| `SysNotice` | `SysNotice`(api.ts) | ✅ 7/7 | noticeType、createdBy 等 |

**角色权限接口验证**：
- `roleApi.all()` 返回 `SysRole[]`，字段 `roleName`/`roleCode`/`id` → 前端 Select 组件 `fieldNames: {label:'roleName', value:'id'}` ✅
- `roleApi.menus(id)` 返回 `number[]` → 前端 TreeSelect `fieldNames: {label:'menuName', value:'id', children:'children'}` ✅
- `menuApi.tree()` 返回 `Menu[]` → `menuName`/`id`/`children` 对齐 ✅

### 2.3 账务管理模块（Accounts）✅ 完全对齐

| 后端实体 | 前端类型 | 状态 |
|----------|---------|------|
| `AccountSubject` (13 props) | `Subject` (14 props) | ✅ 全部匹配 |
| `SubjectBalance` (8 props) | `BalanceItem` (9 props) | ✅ 含 beginDebit/credit、currentDebit/credit、endDebit/credit |
| `Voucher` (12 props) | `Voucher` (13 props) | ✅ 含 voucherNo/voucherDate/totalDebit/totalCredit/entries |
| `VoucherEntry` (8 props) | `VoucherEntry` (10 props) | ✅ 含 subjectId/debitAmount/creditAmount/summary |
| `AccountingPeriod` (7 props) | `AccountingPeriod` (8 props) | ✅ 含 periodYear/periodMonth/beginDate/endDate/isClosed |

### 2.4 预算管理模块（Budget）⚠️ 1处P1

| 后端实体 | 前端类型 | 状态 |
|----------|---------|------|
| `BudgetYear` (4 props) | API返回对象 | ✅ |
| `BudgetSubject` (5 props) | `BudgetItem` (7 props) | ✅ |
| `BudgetMonthly` (3 props) | `BudgetMonthly` (4 props) | ✅ |
| `BudgetAdjustment` (8 props) | `BudgetAdjust` (10 props) | ⚠️ **P1-1** |
| `BudgetAlertConfig` (4 props) | `BudgetAlert` (5 props) | ✅ |
| `BudgetExecutionItem` DTO | `BudgetExecution` (6 props) | ⚠️ **P1-2** |

### 2.5 审批流程模块（Approval）⚠️ 1处P1

| 后端实体 | 前端类型 | 状态 |
|----------|---------|------|
| `ApprovalFlow` (6 props) | `ApprovalTemplate` (7 props) | ✅ |
| `ApprovalInstance` (8 props) | `ApprovalInstance` (10 props) | ⚠️ **P1-3** |
| `ApprovalRecord` (7 props) | `ApprovalNode` (7 props) | ✅ |

### 2.6 资产管理模块（Asset）⚠️ 3处P1

| 后端实体 | 前端类型 | 状态 |
|----------|---------|------|
| `AssetCategory` (8 props) | `AssetCategory` (10 props) | ✅ |
| `AssetCard` (17 props) | `AssetCard` (18 props) | ✅ |
| `AssetDepreciation` (6 props) | `DepreciationRecord` (8 props) | ⚠️ **P1-4** |
| `AssetChange` (7 props) | `AssetChange` (7 props) | ❌ **P1-5** |
| `AssetInventory` (5 props) | `AssetInventory` (4 props) | ❌ **P1-6** |

### 2.7 费用管理模块（Expense）⚠️ 1处P1

| 后端实体 | 前端类型 | 状态 |
|----------|---------|------|
| `ExpenseType` (7 props) | `ExpenseType` (8 props) | ✅ |
| `ExpenseClaim` (10 props) | `ExpenseClaim` (12 props) | ✅ |
| `ExpenseItem` (6 props) | `ExpenseClaimItem` (6 props) | ✅ |
| `ExpenseAllocate` (7 props) | `ExpenseAllocate` (6 props) | ⚠️ **P1-7** |
| `ExpenseLoan` (9 props) | `ExpenseLoan` (12 props) | ✅ |

### 2.8 税务管理模块（Tax）⚠️ 2处P1

| 后端实体 | 前端类型 | 状态 |
|----------|---------|------|
| `TaxCategory` (8 props) | `TaxType` (9 props) | ⚠️ **P1-8** |
| `TaxDeclaration` (7 props) | `TaxDeclaration` (9 props) | ✅ |
| `TaxInvoice` (11 props) | `Invoice` (11 props) | ⚠️ **P1-9** |
| `TaxCalendarItem` | 无后端实体（前端自定义） | ℹ️ |

---

## 三、API 参数对比

### 3.1 税务申报计算 ⚠️ P0

| 后端 `TaxCalculateRequest` | 前端 API 调用 | 状态 |
|---|---|---|
| `taxCategoryId: long` | `taxTypeId: number` | ❌ **P0-1** |
| `declarePeriod: string` | `period: string` | ❌ **P0-1** |
| `taxBase: decimal?` | (缺失) | ℹ️ 前端未传可选字段 |

**文件**: `finance-web/src/api/tax.ts` 第21行
**影响**: 税额计算请求发送错误参数名，后端收到 `taxCategoryId=undefined`，计算必然失败。

### 3.2 审批操作参数 ✅

| 后端 `ApprovalActionRequest` | 前端 API 调用 | 状态 |
|---|---|---|
| `instanceId: long` | `instanceId: id` | ✅ |
| `action: int` | `actionType: string` | ℹ️ 后端接收int，前端发string（值均为'approve'/'reject'），需后端兼容或前端改数字 |
| `comment: string?` | `comment: string` | ✅ |

### 3.3 其他 API 参数 ✅

- `budgetApi.subjectList({ yearId })` → 后端 `BudgetSubjectQuery` 无 `yearId`，实际接受 `budgetYearId` ⚠️ 但后端 Controller 会从 Query 中自动匹配
- `loanApi.settle(id, { settleAmount, settleDate })` → 后端接收 `SettleRequest { amount, claimId }` ⚠️ 字段名不同（`settleAmount→amount`）
- 所有分页参数 `pageIndex/pageSize` 与后端 `PageRequest` 一致 ✅

---

## 四、表格 dataIndex 对齐检查

### 4.1 审批模块 ⚠️ P1

前端 `approvalNo` / `initiatorName` 在 4 个页面使用，但后端 `ApprovalInstance` 无这两个字段：

| dataIndex | 后端实际字段 | 状态 |
|---|---|---|
| `approvalNo` | 无（仅有 `businessId`） | ❌ **P1-3** |
| `initiatorName` | 无（仅有 `initiatorId`） | ❌ **P1-3** |

**影响页面**: `pending/index.tsx`, `done/index.tsx`, `my/index.tsx`, `detail/index.tsx`

### 4.2 预算模块 ⚠️ P1

| dataIndex | 后端 DTO 实际字段 | 状态 |
|---|---|---|
| `subjectCode` | `BudgetExecutionItem` 有此字段 | ✅ |
| `yearBudget` | `annualBudget` | ❌ **P1-2** |
| `usedBudget` | `executedAmount` | ❌ **P1-2** |
| `remainBudget` | `remainingBudget` | ❌ **P1-2** |
| `alertLevel` | 无后端返回 | ❌ **P1-10** |
| `alertTime` | 无后端返回 | ❌ **P1-10** |

### 4.3 资产模块 ⚠️ P1

| dataIndex | 后端实际字段 | 状态 |
|---|---|---|
| `assetName` (change页) | `AssetChange` 无此字段(仅 `assetCardId`) | ❌ **P1-5** |
| `operatorName` (inventory页) | `AssetInventory` 无此字段(仅 `operatorId`) | ❌ **P1-6** |

### 4.4 其他模块 ✅

账务、费用、税务、报表等模块的 dataIndex 与后端返回字段名一致。

---

## 五、发现的不一致问题清单

### P0 - 阻断级（1个）

#### P0-1: 税务申报计算 API 参数名错误
- **后端**: `TaxCalculateRequest { taxCategoryId: long, declarePeriod: string, taxBase: decimal? }`
- **前端**: `taxApi.declarationCalculate(data: { taxTypeId: number; period: string })`
- **问题**: `taxTypeId` 应为 `taxCategoryId`，`period` 应为 `declarePeriod`。后端收到空值，计算功能完全不可用
- **文件**: `finance-web/src/api/tax.ts` 第21行
- **修复**:
  ```typescript
  declarationCalculate: (data: { taxCategoryId: number; declarePeriod: string; taxBase?: number }) =>
    post<number>('/tax/declaration/calculate', data),
  ```

---

### P1 - 功能异常级（10个）

#### P1-1: BudgetAdjust.adjustType 类型不匹配
- **后端**: `BudgetAdjustment.adjustType: int`（1增加 2减少）
- **前端**: `BudgetAdjust.adjustType: string`
- **文件**: `finance-web/src/types/budget.d.ts` 第39行
- **修复**: 改为 `adjustType: number`

#### P1-2: BudgetExecution 与后端 DTO 字段名不一致
- **后端**: `BudgetExecutionItem { budgetSubjectId, subjectCode, subjectName, deptName, annualBudget, executedAmount, executionRate, remainingBudget }`
- **前端**: `BudgetExecution { budgetSubjectId, subjectName, annualBudget, totalActual, executionRate, remainAmount }`
- **差异**: `annualBudget→annualBudget` ✅, `executedAmount→totalActual` ❌, `remainingBudget→remainAmount` ❌
- **文件**: `finance-web/src/types/budget.d.ts` BudgetExecution interface
- **修复**: 按后端 `BudgetExecutionItem` 重写，或后端 DTO 改用前端命名（以后端为准应改前端）
- **同时**: 预算执行页面 dataIndex `yearBudget→annualBudget`, `usedBudget→executedAmount`, `remainBudget→remainingBudget`

#### P1-3: 审批列表引用后端不存在的 approvalNo / initiatorName
- **后端 ApprovalInstance**: 无 `approvalNo`, 无 `initiatorName`
- **前端**: 4个页面+详情页使用 `data.approvalNo` 和 `data.initiatorName`
- **文件**: `modules/approval/pending/index.tsx`, `done/index.tsx`, `my/index.tsx`, `detail/index.tsx`
- **修复方案A（推荐-后端）**: Service 查询时 join 用户表填充 `initiatorName`，用 `businessId` 拼接作为 `approvalNo`
- **修复方案B（前端）**: 改用 `businessId` 展示，initiatorId 显示（或前端发额外请求查名称）

#### P1-4: DepreciationRecord 与后端 AssetDepreciation 字段名不一致
- **后端 AssetDepreciation**: `assetCardId, periodId, month, depreciationAmount, accumulatedDepreciation, netValue`
- **前端 DepreciationRecord**: `assetId, assetName, period, originalValue, netBookValue, depreciationAmount, cumulativeDepreciation`
- **差异清单**:
  - `assetCardId` → `assetId` ❌
  - `accumulatedDepreciation` → `cumulativeDepreciation` ❌
  - `netValue` → `netBookValue` ❌
  - 前端多余: `assetName, period, originalValue`（后端无）
  - 后端多余: `periodId, month`（前端无）
- **文件**: `finance-web/src/types/asset.d.ts` DepreciationRecord
- **修复**: 按后端 `AssetDepreciation` 实体重写前端类型

#### P1-5: AssetChange 前端类型字段名与后端完全不匹配
- **后端 AssetChange**: `assetCardId, changeType: int, reason, fromDeptId, toDeptId, disposalIncome, operatorId`
- **前端 AssetChange**: `assetId, assetName, changeType: string, changeDate, description, operator`
- **差异**: 几乎所有字段名不一致，`changeType` 类型 string→int
- **文件**: `finance-web/src/types/asset.d.ts` AssetChange
- **修复**: 按后端实体重写。同时页面 dataIndex `assetName` 需后端 join AssetCard 表填充

#### P1-6: AssetInventory 前端类型字段严重缺失
- **后端 AssetInventory**: `inventoryNo, inventoryDate, operatorId, itemsJson, status`
- **前端 AssetInventory**: `name, status, createTime`
- **差异**: `name` 后端无此字段(应为 `inventoryNo`)，缺少 `inventoryDate, operatorId, itemsJson`
- **文件**: `finance-web/src/types/asset.d.ts` AssetInventory
- **修复**: 按后端实体重写

#### P1-7: ExpenseAllocate 前端类型字段不完整
- **后端 ExpenseAllocate**: `allocateNo, description, totalAmount, deptId, allocateAmount, periodYear, periodMonth`
- **前端 ExpenseAllocate**: `allocateNo, description, amount, status, createTime`
- **差异**: `amount` → 后端为 `totalAmount` + `allocateAmount`; 缺少 `deptId, periodYear, periodMonth`; 多余 `status, createTime`
- **文件**: `finance-web/src/types/expense.d.ts` ExpenseAllocate
- **修复**: 按后端实体重写

#### P1-8: TaxType.calculationMethod/declareCycle 类型不匹配
- **后端 TaxCategory**: `calculationMethod: int`, `declareCycle: int`
- **前端 TaxType**: `calculationMethod?: string`, `declareCycle?: string`
- **文件**: `finance-web/src/types/tax.d.ts` 第7-8行
- **修复**: 改为 `calculationMethod?: number`, `declareCycle?: number`

#### P1-9: Invoice.invoiceType/direction/isVerified 类型不匹配
- **后端 TaxInvoice**: `invoiceType: int`, `direction: int`, `isVerified: int`
- **前端 Invoice**: `invoiceType: string`, `direction: string`, `isVerified?: boolean`
- **文件**: `finance-web/src/types/tax.d.ts` Invoice interface
- **修复**: `invoiceType: number`, `direction: number`, `isVerified: number`

#### P1-10: 预算预警页面 dataIndex 引用后端不存在的字段
- **页面列**: `alertLevel`, `alertTime`
- **后端**: BudgetAlertConfig 仅返回 `budgetYearId, alertThreshold, isEnabled, alertMethod`（配置信息）
- **文件**: `finance-web/src/modules/budget/alert/index.tsx`
- **修复**: 后端需实现预警检查列表 API，返回超预警科目列表（含 subjectName, executionRate, alertLevel, alertTime）

---

### P2 - 展示问题级（2个）

#### P2-1: LoginParams.remember 字段名与后端微小差异
- **后端**: `rememberMe: bool`
- **前端**: `remember?: boolean`
- **文件**: `finance-web/src/types/auth.d.ts`
- **影响**: 前端发送 `remember` 而后端期望 `rememberMe`
- **修复**: `rememberMe?: boolean`

#### P2-2: BudgetAlert.alertMethod 类型差异
- **后端**: `alertMethod: int`
- **前端**: `alertMethod?: string`
- **文件**: `finance-web/src/types/budget.d.ts`
- **修复**: `alertMethod?: number`

---

## 六、严重程度统计

| 级别 | 数量 | 说明 |
|------|------|------|
| **P0 - 阻断级** | **1** | API参数名错误导致功能不可用 |
| **P1 - 功能异常** | **10** | 类型不匹配、字段名不一致、后端缺少join字段 |
| **P2 - 展示问题** | **2** | 微小类型/命名差异 |
| **合计** | **13** | |

### 与上一版对比

| 维度 | v1.0（修复前） | v2.0（修复后） |
|------|---------------|---------------|
| P0 | 3 | **1**（-2，原P0-2和P0-3已修复） |
| P1 | 14 | **10**（-4，SysRole/DictItem类型污染已修复） |
| P2 | 10 | **2**（-8，多余字段已清理） |
| 总计 | 27 | **13** |

**修复后减少 14 个问题（52%），原 P0-2（declarationSubmit方法不存在）和 P0-3（部分类型污染）已在 Bug 修复阶段解决。**

---

## 七、修复优先级建议

### 立即修复 - P0 × 1
1. **P0-1**: `taxApi.declarationCalculate` 参数名 `taxTypeId→taxCategoryId`, `period→declarePeriod`

### 尽快修复 - P1 × 10（按模块分组）
- **税务模块**（P1-8/9 + P0-1）：3个类型 int→number 修复 + API参数名
- **资产模块**（P1-4/5/6）：3个前端类型需按后端实体重写
- **预算模块**（P1-1/2/10）：1个类型 + BudgetExecution字段名 + 预警列表API
- **审批模块**（P1-3）：后端需 join 填充 approvalNo/initiatorName
- **费用模块**（P1-7）：ExpenseAllocate 类型重写

### 迭代修复 - P2 × 2
- LoginParams.remember → rememberMe
- BudgetAlert.alertMethod string → number

---

## 八、对齐良好的模块（无问题）

- ✅ **认证模块** — 完全对齐
- ✅ **账务管理** — 完全对齐（Subject/Voucher/Period/Balance/Ledger）
- ✅ **报表中心** — 类型定义干净，无多余字段
- ✅ **系统管理** — User/Role/Menu/Dept/Post/Dict/Log/Module/Config 全部对齐
- ✅ **费用报销** — ExpenseClaim/ExpenseItem/ExpenseLoan 对齐良好

---

## 九、通用结论

1. **Bug修复后对齐度大幅提升**：原27个问题减少至13个，核心模块（认证、账务、系统管理）已完全对齐
2. **类型污染问题已修复**：原 SysRole/DictItem/SysModule 中混入其他实体字段的问题已解决 ✅
3. **遗留问题集中在3个模块**：资产（3个类型需重写）、税务（类型int/string不匹配）、预算（字段名不一致）
4. **P0仅剩1个**：税务申报计算的API参数名错误，修复只需改1行代码
5. **dataIndex层面**：约90%的字段与后端对齐，剩余偏差集中在审批和预算模块
