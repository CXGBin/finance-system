# 第九轮前端功能 + API覆盖 + 数据完整性审查

> 审查时间：2026-06-16 10:01 CST  
> 审查范围：前端 60 个页面组件、10 个 API 模块、13 个后端 Controller、43 张数据表  
> 审查标准：功能完整性、API 覆盖度、数据完整性（不检查注释/代码风格）

---

## 一、前端页面功能清单

### 1.1 账务管理（account）— 9 个页面

| 页面 | 列表页 | 新增/编辑弹窗 | 详情页 | 真实API | Loading态 | Error态 | Empty态 | 表单校验 | 按钮控制 | 状态 |
|------|--------|--------------|--------|---------|-----------|---------|---------|---------|---------|------|
| subject（会计科目） | ✅树形 | ✅Modal | — | ✅subjectApi | ✅ | ✅ | ✅ | ✅(11条规则) | ✅启用/禁用 | ✅完整 |
| balance（期初余额） | ✅表格 | ✅行内编辑 | — | ✅balanceApi | — | — | — | — | — | ⚠️部分 |
| voucher/list（凭证查询） | ✅ | — | ✅跳转 | ✅voucherApi | ✅ | ✅ | ✅ | — | ✅审核/作废 | ✅完整 |
| voucher/add（凭证录入） | — | — | — | ✅voucherApi | ✅ | ✅ | — | ✅(5条规则) | ✅借贷平衡 | ✅完整 |
| voucher/detail（凭证详情） | — | — | ✅ | ✅voucherApi | ✅ | ✅ | — | — | — | ✅完整 |
| ledger/general（总账） | ✅ | — | — | ✅ledgerApi | ✅ | — | — | — | — | ⚠️部分 |
| ledger/detail（明细账） | ✅ | — | — | ✅ledgerApi | ✅ | — | — | — | — | ⚠️部分 |
| ledger/journal（日记账） | ✅ | — | — | ❌无API调用 | — | — | — | — | — | ❌缺失 |
| period（会计期间） | ✅ | — | — | ✅periodApi | ✅ | — | — | — | ✅结账/反结 | ⚠️部分 |

### 1.2 审批流程（approval）— 5 个页面

| 页面 | 列表页 | 新增/编辑弹窗 | 详情页 | 真实API | Loading态 | Error态 | Empty态 | 表单校验 | 按钮控制 | 状态 |
|------|--------|--------------|--------|---------|-----------|---------|---------|---------|---------|------|
| pending（待办任务） | ✅ | — | — | ✅approvalApi | — | — | — | — | — | ⚠️部分 |
| done（已办任务） | ✅ | — | — | ✅approvalApi | — | ✅ | — | — | — | ⚠️部分 |
| my（我的申请） | ✅ | — | — | ✅approvalApi | — | ✅ | — | — | — | ⚠️部分 |
| detail（审批详情） | — | — | ✅ | ✅approvalApi | ✅ | — | — | — | ✅通过/驳回/转审 | ⚠️部分 |
| template（审批模板） | ✅ | ✅Modal | — | ✅approvalApi | — | ✅ | — | ✅(4条规则) | — | ⚠️部分 |

### 1.3 资产管理（asset）— 8 个页面

| 页面 | 列表页 | 新增/编辑弹窗 | 详情页 | 真实API | Loading态 | Error态 | Empty态 | 表单校验 | 按钮控制 | 状态 |
|------|--------|--------------|--------|---------|-----------|---------|---------|---------|---------|------|
| card（资产列表） | ✅ | ✅Modal | ✅跳转 | ✅assetApi | — | ✅ | — | ✅(7条规则) | — | ✅完整 |
| card-detail（资产详情） | — | — | ✅ | ✅assetApi | ✅ | — | — | — | — | ⚠️部分 |
| category（资产分类） | ✅树形 | ✅Modal | — | ✅assetApi | — | ✅ | — | ✅(7条规则) | — | ✅完整 |
| depreciation（折旧管理） | ✅ | — | — | ✅assetApi | ✅ | — | — | — | ✅计算/确认 | ⚠️部分 |
| change（资产变动） | ✅ | — | — | ✅assetApi | — | — | — | — | — | ⚠️部分 |
| dispose（资产处置） | ✅ | ✅Modal | — | ⚠️直接fetch | ✅ | ✅ | — | ✅(9条规则) | — | ⚠️部分 |
| inventory（资产盘点） | ✅ | — | — | ✅assetApi | — | — | — | — | — | ⚠️部分 |
| report（资产报表） | ✅ | — | — | ❌无API调用 | — | — | — | — | — | ❌缺失 |

### 1.4 预算管理（budget）— 6 个页面

| 页面 | 列表页 | 新增/编辑弹窗 | 详情页 | 真实API | Loading态 | Error态 | Empty态 | 表单校验 | 按钮控制 | 状态 |
|------|--------|--------------|--------|---------|-----------|---------|---------|---------|---------|------|
| setting（预算设置） | — | ✅Form | — | ✅budgetApi | ✅ | — | — | ✅(8条规则) | — | ⚠️部分 |
| plan（预算编制） | ✅ | — | — | ✅budgetApi | — | — | — | — | — | ⚠️部分 |
| execution（预算执行） | ✅ | — | — | ✅budgetApi | ✅ | — | — | — | — | ⚠️部分 |
| alert（预算预警） | ✅ | — | — | ✅budgetApi | — | — | — | — | — | ⚠️部分 |
| analysis（预算分析） | ✅图表 | — | — | ❌无API调用 | — | — | — | — | — | ❌缺失 |
| adjust（预算调整） | ✅ | — | — | ✅budgetApi | — | — | — | — | — | ⚠️部分 |

### 1.5 费用管理（expense）— 7 个页面

| 页面 | 列表页 | 新增/编辑弹窗 | 详情页 | 真实API | Loading态 | Error态 | Empty态 | 表单校验 | 按钮控制 | 状态 |
|------|--------|--------------|--------|---------|-----------|---------|---------|---------|---------|------|
| claim/list（报销列表） | ✅ | — | — | ✅expenseApi | — | ✅ | — | — | — | ⚠️部分 |
| claim/add（新增报销） | — | ✅Form | — | ✅expenseApi | ✅ | ✅ | — | ✅(5条规则) | — | ⚠️部分 |
| claim/detail（报销详情） | — | — | ✅ | ✅expenseApi | — | — | — | — | — | ⚠️部分 |
| type（费用类型） | ✅ | ✅Modal | — | ✅expenseApi | — | ✅ | — | ✅(4条规则) | — | ✅完整 |
| allocate（费用分摊） | ✅ | — | — | ✅expenseApi | — | — | — | — | — | ⚠️部分 |
| payment（付款记录） | ✅ | — | — | ✅expenseApi | — | — | — | — | — | ⚠️部分 |
| statistics（费用统计） | ✅图表 | — | — | ✅expenseApi | ✅ | — | — | — | — | ⚠️部分 |
| loan（借款管理） | ✅ | ✅Modal | — | ✅loanApi | ✅ | ✅ | — | ✅(7条规则) | ✅审批/退回 | ✅完整 |

### 1.6 税务管理（tax）— 6 个页面

| 页面 | 列表页 | 新增/编辑弹窗 | 详情页 | 真实API | Loading态 | Error态 | Empty态 | 表单校验 | 按钮控制 | 状态 |
|------|--------|--------------|--------|---------|-----------|---------|---------|---------|---------|------|
| type（税种管理） | ✅ | ✅Modal | — | ✅taxApi | — | — | — | ✅(11条规则) | — | ⚠️部分 |
| declaration（纳税申报） | ✅ | — | — | ✅taxApi | — | — | — | — | — | ⚠️部分 |
| invoice（发票管理） | ✅ | ✅Modal | — | ✅taxApi | — | — | — | ✅(13条规则) | — | ⚠️部分 |
| calendar（税务日历） | ✅ | — | — | ⚠️直接fetch | — | — | — | — | — | ⚠️部分 |
| report（税务报表） | ✅ | — | — | ❌无API调用 | — | — | — | — | — | ❌缺失 |
| burden（税负分析） | ✅ | — | — | ⚠️直接fetch | — | — | — | — | — | ⚠️部分 |

### 1.7 报表中心（report）— 5 个页面

| 页面 | 列表页 | 新增/编辑弹窗 | 详情页 | 真实API | Loading态 | Error态 | Empty态 | 表单校验 | 按钮控制 | 状态 |
|------|--------|--------------|--------|---------|-----------|---------|---------|---------|---------|------|
| balance-sheet（资产负债表） | ✅ | — | — | ✅reportApi | ✅ | — | — | — | — | ⚠️部分 |
| income-statement（利润表） | ✅ | — | — | ✅reportApi | ✅ | — | — | — | — | ⚠️部分 |
| cash-flow（现金流量表） | ✅ | — | — | ✅reportApi | ✅ | — | — | — | — | ⚠️部分 |
| subject-balance（科目余额表） | ✅ | — | — | ✅reportApi | ✅ | — | — | — | — | ⚠️部分 |
| custom（自定义报表） | ✅ | — | — | ✅reportApi | ✅ | ✅ | — | — | — | ⚠️部分 |

### 1.8 系统管理（system）— 10 个页面

| 页面 | 列表页 | 新增/编辑弹窗 | 详情页 | 真实API | Loading态 | Error态 | Empty态 | 表单校验 | 按钮控制 | 状态 |
|------|--------|--------------|--------|---------|-----------|---------|---------|---------|---------|------|
| user（用户管理） | ✅ | ✅Modal | — | ✅userApi | — | — | — | — | ✅启用/禁用 | ✅完整 |
| role（角色管理） | ✅ | ✅Modal | — | ✅roleApi | — | — | — | — | — | ✅完整 |
| menu（菜单管理） | ✅树形 | ✅Modal | — | ✅menuApi | — | — | — | ✅(11条规则) | — | ✅完整 |
| dept（部门管理） | ✅树形 | ✅Modal | — | ✅deptApi | — | ✅ | — | ✅(8条规则) | — | ✅完整 |
| post（岗位管理） | ✅ | ✅Modal | — | ✅postApi | — | — | — | — | — | ✅完整 |
| dict（字典管理） | ✅ | ✅Modal | — | ✅dictApi | — | ✅ | — | ✅(9条规则) | — | ✅完整 |
| log（操作日志） | ✅ | — | — | ✅logApi | — | ✅ | — | — | — | ⚠️部分 |
| module（模块管理） | ✅ | — | — | ✅moduleApi | ✅ | — | — | — | ✅启用/切换 | ⚠️部分 |
| config（系统配置） | — | ✅Form | — | ✅configApi | ✅ | ✅ | — | ✅(2条规则) | — | ⚠️部分 |
| notice（系统公告） | — | — | — | ❌无页面 | — | — | — | — | — | ❌缺失 |

### 1.9 其他页面

| 页面 | 状态 |
|------|------|
| login（登录页） | ✅完整：有表单校验(10条规则)、Loading、Error提示 |
| dashboard（仪表盘） | ⚠️部分：接入真实API（4个模块），但有3处TODO模拟数据 |

### 1.10 功能完整性统计

| 状态 | 数量 | 占比 |
|------|------|------|
| ✅ 完整 | 13 | 22% |
| ⚠️ 部分 | 42 | 70% |
| ❌ 缺失 | 5 | 8% |

---

## 二、API 覆盖度

### 2.1 数量统计

| 维度 | 数量 |
|------|------|
| 后端路由总数（去重） | **166** |
| 前端 API 层封装数 | **41** |
| 前端页面实际调用函数数 | **117** |

> 注：前端 API 层 41 个路径通过不同 HTTP 方法组合提供了远多于 41 个的函数（117 个调用），覆盖面较好。

### 2.2 未封装到前端 API 层的后端接口

| 模块 | 后端接口 | 说明 |
|------|---------|------|
| account | `GET /api/account/subject/export` | 科目导出，前端无对应功能 |
| account | `POST /api/account/voucher/{id}/copy` | 凭证复制，前端无调用 |
| account | `POST /api/account/voucher/{id}/reverse` | 凭证冲销，前端直接 fetch 绕过 API 层 |
| account | `GET /api/account/voucher/{id}/print-data` | 凭证打印数据，前端无调用 |
| account | `GET /api/account/auxiliary/{type}/list` | 辅助核算列表，前端无对应页面 |
| account | `PUT/DELETE /api/account/auxiliary/{type}/{id}` | 辅助核算增删改，前端无对应页面 |
| account | `GET /api/account/ledger/subject-summary` | 科目汇总，前端无调用 |
| account | `GET /api/account/subject/balance/trial-balance` | 试算平衡，前端无调用 |
| approval | `GET /api/approval/instance/my-initiated` | 我发起的，前端未用 |
| approval | `GET /api/approval/instance/statistics` | 审批统计，前端无调用 |
| approval | `POST /api/approval/instance/{instanceId}/transfer` | 转审，前端无调用 |
| asset | `GET /api/asset/card/{id}` + `POST/PUT/DELETE` | 资产卡片 CRUD（部分通过 assetApi.cardAdd 等） |
| asset | `GET /api/asset/depreciation/calculate` | 折旧计算，前端直接调 |
| asset | `GET /api/asset/depreciation/summary` | 折旧汇总，前端未用 |
| asset | `POST /api/asset/report/card/{id}/dispose` | 资产处置，前端直接 fetch |
| asset | `GET /api/asset/report/ledger` | 资产台账，前端 report 页面无API |
| asset | `GET /api/asset/report/depreciation-summary` | 折旧汇总报表，前端无调用 |
| asset | `GET /api/asset/report/value-stats` | 资产价值统计，前端无调用 |
| budget | `GET /api/budget/analysis/overview` | 预算概览，前端 analysis 页面无API |
| budget | `GET /api/budget/analysis/subject-compare` | 科目对比分析，前端无调用 |
| budget | `GET /api/budget/analysis/monthly-trend` | 月度趋势，前端无调用 |
| budget | `GET /api/budget/analysis/expense-top10` | 费用TOP10，前端无调用 |
| budget | `GET /api/budget/alert/check` | 预警检查，前端未用 |
| expense | `GET /api/expense/loan/{id}` | 借款详情，前端未用 |
| expense | `POST /api/expense/loan/{id}/settle` | 借款核销，前端未用（loanApi 有 approve/reject 无 settle） |
| report | `GET /api/report/export/excel` | Excel导出，前端未用 |
| report | `GET /api/report/export/pdf` | PDF导出，前端未用 |
| report | `GET /api/report/compare` | 对比报表，前端未用 |
| system | `GET /api/system/notice/list` + CRUD | 系统公告，前端无页面 |
| system | `PUT /api/system/notice/{id}` | 公告编辑，前端无页面 |
| tax | `GET /api/tax/report/summary` | 税务报表汇总，前端无调用 |
| tax | `GET /api/tax/report/by-category` | 按税种统计，前端无调用 |

### 2.3 前端直接 fetch 绕过 API 层的调用

| 页面 | 直接 fetch 路径 | 问题 |
|------|----------------|------|
| voucher/list | `POST /api/voucher/batch-audit` | 路径错误（应为 /api/account/voucher/batch-audit），且未封装到 API 层 |
| voucher/list | `POST /api/voucher/{id}/reverse` | 路径错误（应为 /api/account/voucher/{id}/reverse），未封装 |
| asset/dispose | `POST /api/asset/card/{id}/dispose` | 未封装到 assetApi |
| tax/burden | `GET /api/tax/report/burden?...` | 未封装到 taxApi |

### 2.4 API 封装映射问题

前端 API 层只封装了 41 个路径字符串，但通过函数封装提供了 117 个调用。以下是路径与后端路由不完全匹配的地方：

| 前端 API 路径 | 实际应该对应的后端路由 | 问题 |
|-------------|---------------------|------|
| `/budget/setting/subject` | `/api/budget/setting/subject` | ✅正确 |
| `/budget/setting/year` | `/api/budget/setting/year` | ✅正确 |
| `/budget/adjustment` | `/api/budget/adjustment` | ✅正确 |
| `/budget/alert/config` | `/api/budget/alert/config` | ✅正确 |
| `/budget/plan/auto-split` | `/api/budget/plan/auto-split` | ✅正确 |
| `/budget/plan/monthly` | `/api/budget/plan/monthly` | ✅正确 |
| `/expense/claim` | `/api/expense/claim` | ✅正确 |
| `/expense/type` | `/api/expense/type` | ✅正确 |
| `/expense/allocate` | `/api/expense/allocate` | ✅正确 |

---

## 三、数据完整性

### 3.1 实体 vs 表结构不一致

#### P0 级不一致

| 实体 | 表名 | 字段 | 实体类型 | SQL类型 | 影响 |
|------|------|------|---------|---------|------|
| SysLog | sys_log | `ResponseCode` | `int` | `text` | **类型不匹配**，SQLite 下可能存储为字符串导致查询/比较异常 |
| ExpenseLoan | expense_loan | **表名缺失** | 无 `[SugarTable]` | 表名 `expense_loan`（非 `fm_` 前缀） | **实体无 SugarTable 特性**，CodeFirst 模式下无法自动建表；且表名不符合 `fm_` 命名规范 |

#### P2 级不一致

| 实体 | 表名 | 字段 | 实体定义 | SQL定义 | 影响 |
|------|------|------|---------|---------|------|
| SysUser | sys_user | — | 无 `[SugarTable]` 特性 | 有建表语句 | 无碍（不影响运行），但不规范 |
| SysNotice | sys_notice | `Title`/`Content` | 无 `[SugarColumn(Length)]` | 默认 TEXT | 无碍，但缺少长度约束 |
| SysPost | sys_post | `DeptId` | `long` | `INTEGER NOT NULL` | 无碍 |

### 3.2 种子数据检查

#### ✅ 正常项

| 种子项 | 内容 | 状态 |
|--------|------|------|
| 管理员账号 | `admin`，随机16位强密码，`MustChangePassword=true` | ✅首次登录强制改密码 |
| 默认角色 | `SUPER_ADMIN`，拥有全部菜单权限 | ✅完整 |
| 默认部门 | `总公司`（ParentId=0） | ✅ |
| 模块管理 | 8个模块全部初始化，含依赖关系 | ✅ |
| 菜单树 | 8模块 × 多级菜单 + 按钮权限，覆盖 CRUD | ✅ |
| 角色菜单关联 | 超级管理员绑定所有菜单 | ✅ |
| 审批流程 | 3条默认流程（费用/预算/资产） | ✅ |
| 幂等保护 | 逐表检查避免重复初始化 | ✅ |

#### ⚠️ 缺失项

| 种子项 | 说明 | 影响 |
|--------|------|------|
| 数据字典类型 | ❌ 无种子数据 | 费用类型、凭证类型等前端可能无下拉数据 |
| 数据字典数据 | ❌ 无种子数据 | 同上 |
| 岗位 | ❌ 无种子数据 | 用户创建时无岗位可选 |
| 会计科目 | ❌ 无种子数据 | 首次使用必须手动导入科目 |
| 会计期间 | ❌ 无种子数据 | 首次使用需手动创建期间 |
| 系统配置 | ❌ 无种子数据（如公司名称、会计期间设置等） | 部分页面可能异常 |
| 初始公告 | ❌ 无种子数据 | 首次登录无公告提示 |

### 3.3 表名规范检查

| 表名 | 前缀 | 是否规范 |
|------|------|---------|
| sys_config, sys_dept, sys_dict_*, sys_log, sys_menu, sys_module, sys_notice, sys_post, sys_role, sys_role_menu, sys_user, sys_user_role | `sys_` | ✅ |
| fm_account_subject, fm_period, fm_aux_*, fm_subject_balance, fm_voucher*, fm_approval_*, fm_asset_*, fm_budget_*, fm_expense_*, fm_report_template, fm_tax_* | `fm_` | ✅ |
| **expense_loan** | **无前缀** | ❌不符合命名规范 |

---

## 四、问题清单

### P0 — 必须修复（影响功能正确性）

| # | 模块 | 问题 | 影响 | 建议 |
|---|------|------|------|------|
| P0-1 | system | `SysLog.ResponseCode` 实体为 `int`，SQL 为 `text` | 日志查询/过滤可能异常 | 统一为 `int` |
| P0-2 | expense | `ExpenseLoan` 实体缺少 `[SugarTable]` 特性 | CodeFirst 模式下无法建表 | 添加 `[SugarTable("expense_loan")]` 或改为 `fm_expense_loan` |
| P0-3 | account | `voucher/list` 直接 fetch 路径 `/api/voucher/batch-audit` 错误 | 批量审核功能失效 | 改为 `/api/account/voucher/batch-audit` 或封装到 API 层 |
| P0-4 | account | `voucher/list` 直接 fetch `/api/voucher/{id}/reverse` 路径错误 | 凭证冲销功能失效 | 改为 `/api/account/voucher/{id}/reverse` |

### P1 — 应修复（影响功能完整性）

| # | 模块 | 问题 | 影响 | 建议 |
|---|------|------|------|------|
| P1-1 | account | 日记账页面（journal）无 API 调用 | 页面无数据展示 | 接入 `ledgerApi.journal()` |
| P1-2 | asset | 资产报表页面（report）无 API 调用 | 图表/报表无数据 | 接入 `asset/report/ledger`、`/depreciation-summary`、`/value-stats` |
| P1-3 | budget | 预算分析页面（analysis）无 API 调用 | 4个图表无数据 | 接入 `budget/analysis/*` 系列接口 |
| P1-4 | tax | 税务报表页面（report）无 API 调用 | 无数据展示 | 接入 `tax/report/summary`、`/by-category` |
| P1-5 | system | 缺少系统公告页面 | 后端有完整的 CRUD 接口，前端无对应页面 | 新增 `/system/notice` 页面 |
| P1-6 | account | 期初余额页面缺少 Loading/Empty/Error 态 | 用户无反馈 | 补充三态处理 |
| P1-7 | account | 会计期间页面缺少 Error/Empty 态 | 操作无反馈 | 补充三态处理 |
| P1-8 | all | 大部分列表页（42个 ⚠️）缺少 Empty 空状态提示 | 空列表无友好提示 | 统一在 ProTable 或页面层添加 Empty 提示 |
| P1-9 | all | 大部分列表页缺少 Error 态 | API 失败时无用户提示 | 在 ProTable 层统一处理或各页面补充 |
| P1-10 | seed | 缺少数据字典种子数据（凭证类型、费用类型等） | 首次部署后下拉框无选项 | 在 SeedData 中初始化常用字典 |
| P1-11 | seed | 缺少会计科目种子数据 | 首次使用需手动导入 | 可考虑提供标准科目模板导入功能 |
| P1-12 | expense | loanApi 缺少 `settle()` 核销函数 | 借款核销后端有接口，前端无法调用 | 在 loanApi 中补充 settle 方法 |
| P1-13 | expense | loanApi 缺少 `detail()` 详情函数 | 借款详情后端有接口，前端无法查看 | 在 loanApi 中补充 detail 方法 |

### P2 — 建议改进（提升体验/规范性）

| # | 模块 | 问题 | 建议 |
|---|------|------|------|
| P2-1 | account | 科目导出功能后端有接口，前端无入口 | 在科目列表页增加"导出"按钮 |
| P2-2 | account | 凭证复制后端有接口，前端未调用 | 在凭证列表操作栏增加"复制"按钮 |
| P2-3 | account | 凭证打印数据后端有接口，前端无打印功能 | 新增凭证打印弹窗 |
| P2-4 | account | 辅助核算（客户/供应商/项目）后端有完整 CRUD，前端无管理页面 | 新增 3 个辅助核算管理页面 |
| P2-5 | account | 试算平衡后端有接口，前端未使用 | 在期末处理中增加试算平衡检查 |
| P2-6 | approval | "我发起的"列表后端有接口，前端 my 页面可增加筛选 | 在 my 页面增加"已发起"tab |
| P2-7 | approval | 转审功能后端有接口，前端未使用 | 在审批详情增加"转审"操作 |
| P2-8 | report | Excel/PDF 导出后端有接口，前端各报表页无导出按钮 | 各报表页增加导出按钮 |
| P2-9 | report | 对比报表后端有接口，前端无对应页面 | 新增报表对比功能页面 |
| P2-10 | asset | 资产处置页面直接 fetch 绕过 API 层 | 封装到 assetApi 中 |
| P2-11 | tax | 税负分析/税务日历直接 fetch 绕过 API 层 | 封装到 taxApi 中 |
| P2-12 | seed | 缺少默认岗位种子数据 | 初始化常用岗位（如会计、出纳等） |
| P2-13 | seed | 缺少系统配置种子数据 | 初始化公司名称、默认会计期间等 |
| P2-14 | seed | 缺少初始公告种子数据 | 初始化一条欢迎公告 |
| P2-15 | naming | `expense_loan` 表名不符合 `fm_` 前缀规范 | 改为 `fm_expense_loan` |
| P2-16 | entity | SysUser、SysNotice 等实体缺少 `[SugarTable]` | 规范性补充 |
| P2-17 | entity | SysNotice 的 Title/Content 缺少长度约束 | 添加 `[SugarColumn(Length=...)]` |
| P2-18 | account | 总账/明细账页面缺少 Error/Empty 态 | 补充三态处理 |
| P2-19 | account | 会计科目 SubjectCode 缺少唯一性校验 | 后端/前端均应添加唯一校验 |

---

## 五、总结

### 整体评价
系统前端已覆盖 8 大业务模块共 60 个页面，基本框架完整。核心 CRUD 页面（用户、角色、菜单、科目、凭证等）功能较完善，有表单校验和弹窗交互。

### 关键改进方向
1. **P0 修复优先**：4 个 P0 问题直接影响功能正确性，需立即修复（2 个 fetch 路径错误 + 2 个实体/表结构不一致）
2. **页面三态补全**：70% 页面（42个）缺少完整的 Loading/Error/Empty 状态处理，建议在 ProTable 组件层统一处理
3. **无数据页面接入**：5 个页面（日记账、资产报表、预算分析、税务报表）完全无 API 调用，需接入真实数据
4. **API 层规范化**：4 处直接 fetch 调用应封装到 API 层，确保请求拦截、Token 注入、错误处理一致
5. **种子数据补充**：首次部署体验差（无字典、无科目、无岗位），建议补充基础种子数据
