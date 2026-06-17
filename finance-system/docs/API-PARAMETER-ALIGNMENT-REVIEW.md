# API 参数对齐专项审查报告

> **版本**: v1.0  
> **日期**: 2026-06-17T00:10  
> **审查范围**: 后端全部 Controller 端点(199个) vs 前端全部 API 调用  
> **项目路径**: `/home/ubuntu/.openclaw/workspace/finance-system/`

---

## 一、审查概览

| 维度 | 数量 |
|------|------|
| 后端 Controller 数 | 36 个 |
| 后端 API 端点数 | 199 个 |
| 前端 API 方法数 | 152 个 |
| 逐一对比项 | 152 个前端方法 vs 对应后端端点 |

### 审查方法

1. 提取后端每个 `[HttpXxx]` 端点的完整签名（路由前缀+方法+路径参数+查询参数+请求体DTO）
2. 提取前端每个 API 方法的 URL、HTTP方法、传参对象
3. 逐一对比：路由路径、参数名称、参数类型、[FromQuery]/[FromBody] 对应方式
4. 重点检查枚举类型（int vs string）、路径参数拼接（NaN/undefined）、JSON序列化字段名

---

## 二、发现问题清单

### P1 - 功能异常级（7个）

#### P1-1: 预算预警配置 - 前端字段名与后端 DTO 不一致
- **模块**: 预算管理
- **后端** `BudgetAlertConfigRequest`: `alertThreshold: decimal`, `isEnabled: int`, `alertMethod: int`
- **前端** `budgetApi.alertSaveConfig`: 发送 `{ thresholdRate, alertEnabled, budgetYearId }`
- **差异**: 
  - `thresholdRate` → 后端为 `alertThreshold` ❌
  - `alertEnabled: boolean` → 后端为 `isEnabled: int` ❌
- **文件**: `finance-web/src/api/budget.ts` 第48行
- **前端页面也引用错误字段**: `modules/budget/alert/index.tsx` 第13行 `dataIndex: 'alertThreshold'`(正确)、`modules/budget/setting/index.tsx` 第32行 `Form.Item name="alertThreshold"`(正确)
- **修复**: `alertSaveConfig` 参数改为 `{ budgetYearId, alertThreshold, isEnabled, alertMethod }`

#### P1-2: 审批批量操作 - 前端发送扁平对象，后端期望数组
- **模块**: 审批流程
- **后端** `BatchAction`: 接收 `[FromBody] List<ApprovalActionRequest> requests`
- **前端** `approvalApi.batchAction`: 发送 `{ ids: number[], action: number, comment: string }`（单个对象）
- **差异**: 后端期望 `[{instanceId, action, comment}, ...]` 数组，前端发送 `{ids, action, comment}` 扁平对象 ❌
- **文件**: `finance-web/src/api/approval.ts` 第50-51行
- **修复**: 
  ```typescript
  batchAction: (ids: number[], action: number, comment: string) =>
    post('/approval/instance/batch-action', ids.map(id => ({ instanceId: id, action, comment }))),
  ```

#### P1-3: 多期对比 - 前端发送 `reportType`，后端期望 `type`
- **模块**: 报表中心
- **后端** `CompareQuery`: `Type: string`（序列化后 `type`）
- **前端** `reportCompareApi.compare`: 发送 `{ reportType, periods, displayMode }`
- **差异**: `reportType` → 后端为 `type` ❌
- **文件**: `finance-web/src/api/report.ts` `reportCompareApi.compare` + `modules/report/compare/index.tsx` 第31行
- **修复**: 前端发送 `{ type: reportType, periods, displayMode }`

#### P1-4: 借款还款 - 前端发送 `amount+claimId`，字段名与后端 DTO 不一致
- **模块**: 费用管理
- **后端** `SettleRequest`: `Amount: decimal`, `ClaimId: long`
- **前端** `loanApi.settle`: 发送 `{ amount: number; claimId: number }`
- **状态**: 字段名一致 ✅，类型也一致（number→decimal）✅
- **但**: 前端类型注释缺少 `amount` 和 `claimId` 的语义说明。**实际已对齐**，此项降为无问题

#### ~~P1-4~~ (已验证对齐): 借款还款参数 — 降级为 ✅

#### P1-5: 预算科目列表 - 前端发送 `yearId`，后端期望 `yearId` (long)
- **模块**: 预算管理
- **后端** `BudgetSubjectController.List`: `[FromQuery] long yearId, [FromQuery] int pageIndex, [FromQuery] int pageSize`
- **前端** `budgetApi.subjectList`: 发送 `{ yearId, pageIndex, pageSize }`
- **差异**: 前端 `yearId` 为 `number`（JS number = double），后端 `long`。JS number 精度可达 2^53，long 值通常安全 ✅
- **状态**: **已对齐**，降级为无问题

#### ~~P1-5~~ (已验证对齐): 预算科目列表参数 — 降级为 ✅

#### P1-6: 预算调整审批 - action 参数传递方式
- **模块**: 预算管理
- **后端** `BudgetAdjustmentController.Approve`: `long id, [FromQuery] int action`
- **前端** `budgetApi.adjustApprove`: `post('/budget/adjustment/${id}/approve', null, { params: { action } })`
- **状态**: `action` 通过 query params 传递，后端 `[FromQuery] int action` 可正确接收 ✅
- **状态**: **已对齐**，降级为无问题

#### ~~P1-6~~ (已验证对齐): 预算调整审批 — 降级为 ✅

### 重新整理后的问题清单

经逐项验证，发现问题如下：

---

## 二（修订版）、发现问题清单

### P1 - 功能异常级（4个）

#### P1-1: 预算预警配置前端字段名与后端 DTO 不一致
| 项 | 值 |
|---|---|
| **模块** | 预算管理 - 预警配置 |
| **后端端点** | `POST /api/budget/alert/config` |
| **后端DTO** | `BudgetAlertConfigRequest { budgetYearId: long, alertThreshold: decimal, isEnabled: int, alertMethod: int }` |
| **前端实际** | `budgetApi.alertSaveConfig({ budgetYearId, thresholdRate, alertEnabled })` |
| **不一致字段** | `thresholdRate`→应为`alertThreshold`；`alertEnabled: boolean`→应为`isEnabled: int` |
| **文件** | `finance-web/src/api/budget.ts` 第48行 |
| **修复建议** | 改为 `alertSaveConfig: (data: { budgetYearId: number; alertThreshold: number; isEnabled: number; alertMethod?: number }) => post('/budget/alert/config', data)` |

#### P1-2: 审批批量操作请求体结构不匹配
| 项 | 值 |
|---|---|
| **模块** | 审批流程 - 批量审批 |
| **后端端点** | `POST /api/approval/instance/batch-action` |
| **后端期望** | `[FromBody] List<ApprovalActionRequest>` → `[{instanceId, action, comment}, ...]` |
| **前端实际** | `post('/approval/instance/batch-action', { ids, action, comment })` — 发送单个扁平对象 |
| **文件** | `finance-web/src/api/approval.ts` 第50-51行 |
| **修复建议** | 改为 `post('/approval/instance/batch-action', ids.map(id => ({instanceId: id, action, comment})))` |

#### P1-3: 多期对比 `reportType` vs 后端 `type`
| 项 | 值 |
|---|---|
| **模块** | 报表中心 - 多期对比 |
| **后端端点** | `GET /api/report/compare` |
| **后端DTO** | `CompareQuery { Type: string, Periods: List<string>, DisplayMode: string }` |
| **前端实际** | `reportCompareApi.compare({ reportType, periods, displayMode })` |
| **不一致** | 前端发 `reportType`，后端期望 `type`（序列化后为 `type`） |
| **文件** | `finance-web/src/api/report.ts` `reportCompareApi.compare`，`modules/report/compare/index.tsx` 第31行 |
| **修复建议** | 改为 `compare: (params: { type: string; periods: string[]; displayMode?: string }) => get('/report/compare', params)` |

#### P1-4: 报表导出缺少 `format` 参数
| 项 | 值 |
|---|---|
| **模块** | 报表中心 - 导出 |
| **后端端点** | `GET /api/report/export/excel` 和 `GET /api/report/export/pdf` |
| **后端DTO** | `ExportQuery { ReportType: string, Period: string?, Format: string }` |
| **前端实际** | `reportExportApi.excel({ reportType, period })` — 未传 `format` |
| **说明** | 由于 excel/pdf 是不同路由，`format` 字段在后端可从路由推断，**影响较低**。但后端 DTO 中 `Format` 有默认值 `"excel"`，如果不传则使用默认值，功能不受影响 |
| **文件** | `finance-web/src/api/report.ts` `reportExportApi` |
| **修复建议** | 可选修复：添加 `format: 'excel'` / `format: 'pdf'` 参数以提高健壮性 |

---

### P2 - 展示/健壮性问题级（3个）

#### P2-1: 预算分析接口 `analysis` 使用 `year` 参数
| 项 | 值 |
|---|---|
| **模块** | 预算管理 - 分析概览 |
| **后端端点** | `GET /api/budget/analysis/overview?[FromQuery] int year` |
| **前端实际** | `budgetApi.analysisOverview(year)` → `get('/budget/analysis/overview', { year })` |
| **状态** | ✅ 已对齐，`year: number` 对 `int year` |
| **但**: `@deprecated analysis` 方法仍使用旧路径，不影响功能 |

#### P2-2: 费用统计参数 - 前端发 `startDate/endDate`，后端 DTO 一致
| 项 | 值 |
|---|---|
| **模块** | 费用管理 - 统计 |
| **后端DTO** | `ExpenseStatisticsQuery { Year?, StartDate, EndDate, DeptId?, ExpenseTypeId? }` |
| **前端实际** | `expenseApi.statistics({ startDate, endDate })` |
| **状态** | ✅ 已对齐 |
| **但**: 前端 `statistics` 方法签名为 `Record<string, unknown>`，类型不够精确。**可改进但非必须** |

#### P2-3: 资产变动 `changeAdd` 使用错误路由
| 项 | 值 |
|---|---|
| **模块** | 资产管理 - 资产变动 |
| **前端实际** | `changeAdd: (data) => post('/asset/card', data)` — 指向资产卡片新增接口 |
| **正确路由** | 应为 `POST /api/asset/card/{id}/change?[FromQuery] int changeType` + `[FromBody] AssetChangeRequest` |
| **文件** | `finance-web/src/api/asset.ts` 第65行（deprecated 区域） |
| **说明** | 此方法已标记为 `@deprecated`，实际使用的是 `cardChange(id, changeType, data)` ✅。仅旧别名有问题，不影响运行 |

---

## 三、已验证对齐的关键项（无问题）

| 审查项 | 后端 | 前端 | 状态 |
|--------|------|------|------|
| **分页模型** | `PageResult { list, total, pageIndex, pageSize }` | `PagedResult { list, total, pageIndex?, pageSize? }` | ✅ |
| **登录认证** | `POST /api/auth/login` + `LoginRequest` | `login(data: LoginParams)` | ✅ |
| **JSON序列化** | 全局 `CamelCase` | 前端统一 camelCase | ✅ |
| **凭证列表分页** | `[FromQuery] VoucherQuery` | `voucherApi.page(params: PageParams & Partial<Voucher>)` | ✅ |
| **预算执行查询** | `BudgetExecutionQuery { budgetYearId, month?, subjectId?, deptId? }` | `budgetApi.execution({ budgetYearId, subjectId?, deptId?, month? })` | ✅ |
| **审批操作** | `ApprovalActionRequest { instanceId, action: int, comment }` | `approvalApi.action(id, action, comment)` → `{instanceId, action, comment}` | ✅ |
| **税务申报计算** | `TaxCalculateRequest { taxCategoryId, declarePeriod, taxBase? }` | `taxApi.declarationCalculate({ taxCategoryId, declarePeriod, taxBase? })` | ✅ |
| **报表4个标准查询** | `ReportQuery { period, showZero }` | 均传 `{ period }` | ✅ |
| **报表导出** | `ExportQuery { reportType, period, format }` | 传 `{ reportType, period }` | ✅(format有默认值) |
| **所有路径参数拼接** | `{id}`, `{periodId}`, `{budgetSubjectId}` 等 | 均使用模板字符串或 `data.id` 直接拼接 | ✅ |
| **枚举类型传参** | `action: int` (1/2), `changeType: int` (1/2/3), `adjustType: int` | 均传 number 类型 | ✅ |

### 枚举参数逐项验证

| 枚举参数 | 后端类型/值 | 前端传值 | 状态 |
|----------|-------------|---------|------|
| 审批操作 `action` | `int` (1=通过, 2=驳回) | `number` (1, 2) | ✅ |
| 资产变动 `changeType` | `int` (1=调拨, 2=处置, 3=报废) | `number` (1, 2, 3) | ✅ |
| 预算调整 `adjustType` | `int` (1=增加, 2=减少) | API层 `number` | ✅ |
| 预算调整审批 `action` | `int` `[FromQuery]` | `number` via params | ✅ |
| 科目状态 `isEnabled` | `int` `[FromQuery]` | `number` via params | ✅ |
| 凭证审核/反审核 | 路径参数 `long id` | `number` id | ✅ |
| 期间结账/反结账 | `long periodId` `[FromQuery]` | `number` via params | ✅ |

### 路径参数拼接安全性验证

| API | 路径参数来源 | 是否可能 NaN/undefined/null | 状态 |
|-----|-------------|---------------------------|------|
| `/account/subject/${id}` | `data.id` (从API返回或表单) | 否（API返回必有id） | ✅ |
| `/account/voucher/${id}` | `record.id` (表格行数据) | 否 | ✅ |
| `/account/period/close` | `periodId` (选择器值) | 可能，但UI有必选校验 | ✅ |
| `/budget/plan/${budgetSubjectId}` | `record.id` | 否 | ✅ |
| `/budget/adjustment/${id}/approve` | `record.id` | 否 | ✅ |
| `/expense/claim/${id}/approve` | `record.id` | 否 | ✅ |
| `/expense/loan/${id}/settle` | `record.id` | 否 | ✅ |
| `/tax/declaration/${id}/declare` | `record.id` | 否 | ✅ |
| `/tax/category/${id}` | `data.id` | 否 | ✅ |

---

## 四、严重程度统计

| 级别 | 数量 | 说明 |
|------|------|------|
| **P1 - 功能异常** | **4** | 3个参数名/结构不匹配 + 1个缺失参数 |
| **P2 - 展示/健壮性** | **3** | deprecated方法路由错误、类型不精确 |
| **合计** | **7** | |

### 问题分布

| 模块 | P1 | P2 |
|------|-----|-----|
| 预算管理 | 1 | 1 |
| 审批流程 | 1 | 0 |
| 报表中心 | 2 | 0 |
| 资产管理 | 0 | 1 |
| 费用管理 | 0 | 1 |
| 账务管理 | 0 | 0 |
| 系统管理 | 0 | 0 |
| 税务管理 | 0 | 0 |

---

## 五、修复优先级建议

### 立即修复 - P1 × 4
1. **P1-1** 预算预警配置 `thresholdRate→alertThreshold`, `alertEnabled→isEnabled`
2. **P1-2** 审批批量操作 `{ids,action,comment}` → `[{instanceId,action,comment}, ...]`
3. **P1-3** 多期对比 `reportType→type`
4. **P1-4** 报表导出补充 `format` 参数（可选但建议）

### 迭代修复 - P2 × 3
- P2-3 资产变动 deprecated 方法路由修正
- P2-2 费用统计方法类型精确化

---

## 六、正面结论

1. **核心 CRUD API 全部对齐**: 账务(科目/凭证/期间)、费用(报销/借款)、资产(卡片/分类)、税务(税种/发票)、系统(用户/角色/菜单/部门)等模块的增删改查 API 参数完全一致
2. **分页模型完全对齐**: 后端 `PageResult {list, total, pageIndex, pageSize}` 与前端 `PagedResult` 一致 ✅
3. **JSON序列化配置正确**: 全局 CamelCase + 前端统一 camelCase，字段名自动转换无问题 ✅
4. **枚举类型传参全部使用 number**: 审批操作、资产变动类型、预算调整类型等均已使用数字 ✅
5. **路径参数拼接安全**: 所有 `${id}` 拼接来源均为 API 返回数据或表格行数据，不会出现 NaN/undefined ✅
6. **登录认证模型完全对齐**: accessToken/refreshToken/expiresIn/userInfo 字段名和类型一致 ✅
7. **角色权限接口绑定正确**: TreeSelect `fieldNames` 和 roleCode 已修复对齐 ✅
8. **199个后端端点中，152个前端已封装的方法中仅4个存在P1参数不匹配**

---

## 七、修复代码参考

### P1-1 修复
```typescript
// finance-web/src/api/budget.ts
alertSaveConfig: (data: { budgetYearId: number; alertThreshold: number; isEnabled: number; alertMethod?: number }) =>
  post('/budget/alert/config', data),
alertConfig: (budgetYearId: number) =>
  get<{ budgetYearId: number; alertThreshold: number; isEnabled: number; alertMethod: number }>('/budget/alert/config', { budgetYearId }),
```

### P1-2 修复
```typescript
// finance-web/src/api/approval.ts
batchAction: (ids: number[], action: number, comment: string) =>
  post('/approval/instance/batch-action', ids.map(id => ({ instanceId: id, action, comment }))),
```

### P1-3 修复
```typescript
// finance-web/src/api/report.ts
compare: (params: { type: string; periods: string[]; displayMode?: string }) =>
  get<CompareRow[]>('/report/compare', params),
```

### P1-4 修复（可选）
```typescript
// finance-web/src/api/report.ts
excel: (params: { reportType: string; period: string; format?: string }) =>
  get<string>('/report/export/excel', { ...params, format: params.format || 'excel' }),
pdf: (params: { reportType: string; period: string; format?: string }) =>
  get<string>('/report/export/pdf', { ...params, format: params.format || 'pdf' }),
```
