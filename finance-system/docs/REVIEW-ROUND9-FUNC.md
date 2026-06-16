# 第九轮功能完整性审查报告

> 审查日期：2026-06-16  
> 审查人：自动化功能审查引擎  
> 项目路径：`/home/ubuntu/.openclaw/workspace/finance-system/`  
> 审查范围：后端 Service 完整性 + 前端页面完整性 + API 对齐 + 数据库一致性 + 核心业务逻辑深度

---

## 一、审查摘要

| 维度 | 数据 |
|------|------|
| 后端业务模块数 | 8（Accounts、Approval、Asset、Budget、Expense、Reports、Tax、System） |
| 后端 Service 类数 | 24（含接口定义） |
| 后端 Controller 类数 | 16 |
| 后端 API 路由数 | ~120 |
| 前端模块页面数 | 58（`.tsx` 文件） |
| 前端 API 封装文件 | 10（account、approval、asset、auth、budget、expense、report、request、system、tax） |
| 数据库表数 | 43（init-sqlite.sql） |
| TODO / NotImplementedException | 0（后端无空壳实现） |
| 种子数据（INSERT 语句） | 0（init-sqlite.sql 无种子数据） |
| **API 覆盖率** | **~85%**（后端有接口但前端未覆盖约15个） |
| **功能完整性评分** | **B+（82/100）** |

---

## 二、各模块功能审查

### 模块1：Accounts（账户/凭证/科目/期间/账簿）

#### 后端功能

| 功能点 | 方法 | 状态 | 说明 |
|--------|------|------|------|
| 科目树查询 | SubjectService.GetTreeAsync | ✅完整 | 支持启用/全部，递归构建树 |
| 科目 CRUD | SubjectService.Create/Update/Delete/Toggle | ✅完整 | 层级校验、子科目检查、发生额检查 |
| 凭证分页查询 | VoucherService.GetPageAsync | ✅完整 | 支持按字号/日期/类型/状态/摘要/科目筛选 |
| 凭证创建 | VoucherService.CreateAsync | ✅完整 | 借贷平衡校验、期间校验、自动编号 |
| 凭证修改 | VoucherService.UpdateAsync | ✅完整 | 草稿状态校验、借贷平衡校验 |
| 凭证审核 | VoucherService.AuditAsync | ✅完整 | 制审分离校验 |
| 凭证反审核 | VoucherService.UnAuditAsync | ✅完整 | 结账期间校验 |
| 凭证作废 | VoucherService.VoidAsync | ⚠️Bug | 后端 Status=2 作废，与前端状态映射不一致（见问题清单） |
| 凭证批量审核 | VoucherService.BatchAuditAsync | ✅完整 | 含批量校验错误收集 |
| 凭证批量作废 | VoucherService.BatchVoidAsync | ⚠️Bug | 逻辑判断有误（检查 status==2 为已存在作废，但应排除而非报错） |
| 凭证红字冲销 | VoucherService.ReverseAsync | ✅完整 | 自动生成反向分录，仅冲销已作废凭证 |
| 凭证复制 | VoucherService.CopyAsync | ✅完整 | 复制分录、自动编号、草稿状态 |
| 凭证删除 | VoucherService.DeleteAsync | ✅完整 | 草稿状态校验 |
| 会计期间列表 | PeriodService.GetListByYearAsync | ✅完整 | |
| 初始化年度 | PeriodService.InitYearAsync | ✅完整 | 自动创建12个月度期间 |
| 期末结账 | PeriodService.CloseAsync | ✅完整 | 未审核凭证检查、试算平衡检查、余额滚转 |
| 反结账 | PeriodService.UnCloseAsync | ✅完整 | 下一期间已结账校验 |
| 年末结转 | PeriodService.YearEndCloseAsync（私有） | ✅完整 | 本年利润→利润分配，含盈利/亏损两种处理 |
| 损益结转 | PeriodService.ProfitTransferAsync | ✅完整 | 收入/费用/成本类科目余额结转本年利润 |
| 期初余额管理 | SubjectBalanceService.GetByPeriodAsync/SaveAsync | ✅完整 | |
| 试算平衡 | SubjectBalanceService.TrialBalanceAsync | ✅完整 | |
| 总账查询 | LedgerService.GetGeneralLedgerAsync | ✅完整 | |
| 明细账查询 | LedgerService.GetDetailLedgerAsync | ✅完整 | |
| 日记账查询 | LedgerService.GetJournalAsync | ✅完整 | 自动筛选现金/银行科目 |
| 辅助核算 CRUD | AuxiliaryService.GetList/Create/Update/Delete | ✅完整 | 支持项目/客户/供应商三种类型 |

#### 前端功能

| 页面 | 状态 | 说明 |
|------|------|------|
| 科目管理 (subject/index.tsx) | ✅完整 | 树形展示、CRUD 弹窗 |
| 期初余额 (balance/index.tsx) | ✅完整 | 余额编辑、试算平衡 |
| 凭证列表 (voucher/list/index.tsx) | ⚠️部分 | 有 loading 状态，但批量审核/冲销用 raw fetch 而非 API 封装；状态映射与后端不一致 |
| 凭证新增 (voucher/add/index.tsx) | ✅完整 | |
| 凭证详情 (voucher/detail/index.tsx) | ✅完整 | |
| 期间管理 (period/index.tsx) | ✅完整 | |
| 总账 (ledger/general/index.tsx) | ✅完整 | |
| 明细账 (ledger/detail/index.tsx) | ✅完整 | |
| 日记账 (ledger/journal/index.tsx) | ✅完整 | |

#### API 对齐

| 后端接口 | 前端封装 | 状态 |
|----------|----------|------|
| GET /api/account/subject/tree | subjectApi.tree | ✅ |
| POST/PUT/DELETE /api/account/subject | subjectApi.add/update/remove | ✅ |
| PUT /api/account/subject/{id}/status | subjectApi.toggleStatus | ✅ |
| GET/POST /api/account/subject/balance | balanceApi.list/save | ✅ |
| GET /api/account/subject/balance/trial-balance | balanceApi.trialBalance | ✅ |
| GET /api/account/voucher/page | voucherApi.page | ✅ |
| GET /api/account/voucher/{id} | voucherApi.detail | ✅ |
| POST/PUT/DELETE /api/account/voucher | voucherApi.add/update/remove | ✅ |
| POST /api/account/voucher/{id}/audit | voucherApi.audit | ✅ |
| POST /api/account/voucher/{id}/unaudit | voucherApi.unaudit | ✅ |
| POST /api/account/voucher/{id}/void | voucherApi.void | ✅ |
| POST /api/account/voucher/batch-audit | voucherBatchApi.batchAudit | ✅（前端页面未使用封装，用 raw fetch） |
| POST /api/account/voucher/batch-void | voucherBatchApi.batchVoid | ✅（前端页面未调用） |
| POST /api/account/voucher/{id}/copy | voucherBatchApi.copy | ✅（前端无按钮调用） |
| POST /api/account/voucher/{id}/reverse | voucherBatchApi.reverse | ✅（前端页面用 raw fetch） |
| GET /api/account/voucher/{id}/print-data | voucherBatchApi.printData | ❌前端无打印功能页面 |
| GET/POST /api/account/period/* | periodApi.* | ✅ |
| GET /api/account/ledger/* | ledgerApi.* | ✅ |
| GET/POST /api/account/auxiliary/{type}/* | auxiliaryApi.* | ✅ |
| GET/POST /api/account/subject/export,import | subjectImportExportApi.* | ❌前端无导入导出页面 |

---

### 模块2：Approval（审批管理）

#### 后端功能

| 功能点 | 方法 | 状态 | 说明 |
|--------|------|------|------|
| 流程列表 | ApprovalFlowService.GetListAsync | ✅完整 | 支持按模块类型筛选 |
| 流程创建 | ApprovalFlowService.CreateAsync | ✅完整 | 编码唯一性校验、节点JSON验证 |
| 流程修改 | ApprovalFlowService.UpdateAsync | ✅完整 | |
| 流程删除 | ApprovalFlowService.DeleteAsync | ✅完整 | ⚠️无进行中实例检查 |
| 实例列表 | ApprovalInstanceService.GetListAsync | ✅完整 | |
| 实例详情 | ApprovalInstanceService.GetByIdAsync | ✅完整 | |
| 发起审批 | ApprovalInstanceService.StartAsync | ✅完整 | 重复提交检查 |
| 审批操作 | ApprovalInstanceService.ActionAsync | ✅完整 | 通过→下一节点/完成，驳回→结束 |
| 撤回审批 | ApprovalInstanceService.WithdrawAsync | ✅完整 | 发起人校验、进行中状态校验、记录撤回日志 |
| 转办 | ApprovalInstanceService.TransferAsync | ✅完整 | 记录转办操作 |
| 批量审批 | ApprovalInstanceService.BatchActionAsync | ✅完整 | 循环调用 ActionAsync |
| 我的待办 | ApprovalInstanceService.GetMyPendingAsync | ✅完整 | 排除已审批实例 |
| 我的已办 | ApprovalInstanceService.GetMyDoneAsync | ✅完整 | |
| 我的申请 | ApprovalInstanceService.GetMyInitiatedAsync | ✅完整 | 分页 |
| 审批记录 | ApprovalInstanceService.GetRecordsAsync | ✅完整 | |
| 审批统计 | ApprovalInstanceService.GetStatisticsAsync | ✅完整 | 待办/已办/发起数 |

#### 前端功能

| 页面 | 状态 | 说明 |
|------|------|------|
| 待办审批 (pending/index.tsx) | ✅完整 | ProTable 封装、通过/驳回操作 |
| 已办审批 (done/index.tsx) | ✅完整 | |
| 我的申请 (my/index.tsx) | ✅完整 | |
| 审批详情 (detail/index.tsx) | ✅完整 | |
| 流程模板 (template/index.tsx) | ✅完整 | |

#### API 对齐

| 后端接口 | 前端封装 | 状态 |
|----------|----------|------|
| GET /api/approval/flow/list | approvalApi.templateList | ✅ |
| POST /api/approval/flow | approvalApi.templateAdd | ✅ |
| PUT /api/approval/flow/{id} | approvalApi.templateUpdate | ✅ |
| GET /api/approval/instance/list | approvalApi.list/pending/done/my | ✅ |
| GET /api/approval/instance/{id} | approvalApi.detail | ✅ |
| POST /api/approval/instance/start | approvalApi.start | ✅ |
| POST /api/approval/instance/action | approvalApi.approve/reject/action | ✅ |
| POST /api/approval/instance/{id}/withdraw | approvalApi.withdraw | ✅ |
| GET /api/approval/instance/{id}/records | approvalApi.records | ✅ |
| POST /api/approval/instance/{id}/transfer | approvalApi.transfer | ✅ |
| POST /api/approval/instance/batch-action | approvalApi.batchAction | ✅ |
| GET /api/approval/instance/statistics | approvalApi.statistics | ✅ |

---

### 模块3：Asset（固定资产）

#### 后端功能

| 功能点 | 方法 | 状态 | 说明 |
|--------|------|------|------|
| 分类树 | AssetCategoryService.GetTreeAsync | ✅完整 | |
| 分类 CRUD | AssetCategoryService.Create/Update/Delete | ✅完整 | 有资产检查 |
| 资产卡片分页 | AssetCardService.GetPageAsync | ✅完整 | 多条件筛选 |
| 资产卡片详情 | AssetCardService.GetByIdAsync | ✅完整 | |
| 资产卡片创建 | AssetCardService.CreateAsync | ✅完整 | 自动编号、残值计算 |
| 资产卡片修改 | AssetCardService.UpdateAsync | ✅完整 | 已折旧校验 |
| 资产卡片删除 | AssetCardService.DeleteAsync | ✅完整 | 处置/报废校验 |
| 资产状态变更 | AssetCardService.ChangeStatusAsync | ✅完整 | |
| 资产处置 | AssetDisposeHelper.DisposeAssetAsync | ✅完整 | 自动生成凭证（含原值转出/折旧转出/处置收入/处置损益） |
| 折旧计算-直线法 | AssetDepreciationService.CalculateStraightLine | ✅完整 | |
| 折旧计算-双倍余额递减法 | AssetDepreciationService.CalculateDoubleDeclining | ✅完整 | |
| 折旧计算-年数总和法 | AssetDepreciationService.CalculateSumOfYears | ✅完整 | |
| 折旧确认入账 | AssetDepreciationService.ConfirmDepreciationAsync | ✅完整 | 更新卡片余额 + 自动生成折旧凭证 |
| 盘点创建/完成 | AssetInventoryService.CreateAsync/CompleteAsync | ✅完整 | |
| 资产台账报表 | AssetReportService.GetLedgerAsync | ✅完整 | |
| 折旧汇总报表 | AssetReportService.GetDepreciationSummaryAsync | ✅完整 | |
| 资产价值统计 | AssetReportService.GetValueStatsAsync | ✅完整 | |

#### 前端功能

| 页面 | 状态 | 说明 |
|------|------|------|
| 资产卡片 (card/index.tsx) | ✅完整 | ProTable + 新增弹窗 |
| 资产卡片详情 (card-detail/index.tsx) | ✅完整 | |
| 资产分类 (category/index.tsx) | ✅完整 | |
| 资产变更 (change/index.tsx) | ✅完整 | |
| 资产折旧 (depreciation/index.tsx) | ✅完整 | |
| 资产处置 (dispose/index.tsx) | ⚠️部分 | 有页面但使用 raw fetch 而非 API 封装 |
| 资产盘点 (inventory/index.tsx) | ✅完整 | |
| 资产报表 (report/index.tsx) | ✅完整 | |

#### API 对齐

| 后端接口 | 前端封装 | 状态 |
|----------|----------|------|
| GET /api/asset/category/tree | assetApi.categoryTree | ✅ |
| POST/PUT/DELETE /api/asset/category | assetApi.category* | ✅ |
| GET /api/asset/card/page | assetApi.cardList | ✅ |
| GET /api/asset/card/{id} | assetApi.cardDetail | ✅ |
| POST/PUT/DELETE /api/asset/card | assetApi.cardAdd/cardUpdate/cardDelete | ✅ |
| POST /api/asset/card/{id}/change | assetApi.cardChange | ✅ |
| POST /api/asset/card/{id}/dispose | ❌未封装 | 前端用 raw fetch |
| GET /api/asset/depreciation/calculate | assetApi.depreciationCalculate | ✅ |
| POST /api/asset/depreciation/confirm | assetApi.depreciationConfirm | ✅ |
| GET /api/asset/depreciation/summary | assetApi.depreciationSummary | ✅ |
| GET/POST /api/asset/inventory/* | assetApi.inventory* | ✅ |
| GET /api/asset/report/* | assetApi.report* | ✅ |

---

### 模块4：Budget（预算管理）

#### 后端功能

| 功能点 | 方法 | 状态 | 说明 |
|--------|------|------|------|
| 年度预算列表 | BudgetYearService.GetListAsync | ✅完整 | |
| 创建年度预算 | BudgetYearService.CreateAsync | ✅完整 | 重复年份校验 |
| 年度状态变更 | BudgetYearService.UpdateStatusAsync | ✅完整 | |
| 预算科目 CRUD | BudgetSubjectService.* | ✅完整 | |
| 月度预算管理 | BudgetMonthlyService.GetBySubjectAsync/SaveAsync | ✅完整 | 月度合计=年度校验 |
| 自动均摊 | BudgetMonthlyService.AutoSplitAsync | ✅完整 | 12月补差 |
| 预算执行跟踪 | BudgetExecutionService.GetExecutionAsync | ⚠️Bug | BudgetYearId 当 PeriodYear 查账期间（见问题清单） |
| 预算调整 | BudgetAdjustService.CreateAdjustAsync/ApproveAdjustAsync | ✅完整 | 调整审批+自动更新金额 |
| 预算预警 | BudgetAlertService.GetConfigAsync/SaveConfigAsync/CheckAlertsAsync | ✅完整 | |
| 科目对比分析 | BudgetAnalysisService.GetSubjectCompareAsync | ✅完整 | |
| 月度趋势分析 | BudgetAnalysisService.GetMonthlyTrendAsync | ✅完整 | |
| 费用TOP10 | BudgetAnalysisService.GetExpenseTop10Async | ✅完整 | |
| 预算概览 | BudgetAnalysisService.GetOverviewAsync | ✅完整 | |

#### 前端功能

| 页面 | 状态 | 说明 |
|------|------|------|
| 预算设置 (setting/index.tsx) | ✅完整 | |
| 预算计划 (plan/index.tsx) | ✅完整 | |
| 预算执行 (execution/index.tsx) | ✅完整 | |
| 预算调整 (adjust/index.tsx) | ✅完整 | |
| 预算预警 (alert/index.tsx) | ✅完整 | |
| 预算分析 (analysis/index.tsx) | ✅完整 | |

#### API 对齐

| 后端接口 | 前端封装 | 状态 |
|----------|----------|------|
| GET /api/budget/year/list | budgetApi.yearList | ✅ |
| POST /api/budget/year | budgetApi.yearAdd | ✅ |
| PUT /api/budget/year/{id}/status | budgetApi.yearUpdateStatus | ✅ |
| GET/POST/PUT/DELETE /api/budget/subject/* | budgetApi.subject* | ✅ |
| GET/POST /api/budget/monthly/* | budgetApi.monthly* | ✅ |
| POST /api/budget/monthly/auto-split | budgetApi.monthlyAutoSplit | ✅ |
| GET /api/budget/execution | budgetApi.executionList | ✅ |
| POST /api/budget/adjust | budgetApi.adjustCreate | ✅ |
| POST /api/budget/adjust/{id}/approve | budgetApi.adjustApprove | ✅ |
| GET/POST /api/budget/alert/* | budgetApi.alert* | ✅ |
| GET /api/budget/analysis/* | budgetApi.analysis* | ✅ |

---

### 模块5：Expense（费用管理）

#### 后端功能

| 功能点 | 方法 | 状态 | 说明 |
|--------|------|------|------|
| 费用类型 CRUD | ExpenseTypeService.* | ✅完整 | |
| 报销单列表/详情 | ExpenseClaimService.GetListAsync/GetByIdAsync | ✅完整 | |
| 报销单创建 | ExpenseClaimService.CreateAsync | ✅完整 | 明细校验、自动编号 |
| 报销单修改 | ExpenseClaimService.UpdateAsync | ✅完整 | 草稿校验 |
| 报销单提交 | ExpenseClaimService.SubmitAsync | ✅完整 | |
| 报销单审批通过 | ExpenseClaimService.ApproveAsync | ✅完整 | 月度限额校验 |
| 报销单驳回 | ExpenseClaimService.RejectAsync | ✅完整 | |
| 确认付款 | ExpenseClaimService.ConfirmPaymentAsync | ✅完整 | 自动生成费用凭证 |
| 费用统计 | ExpenseStatisticsService.GetStatisticsAsync | ✅完整 | |
| 费用分摊 | ExpenseAllocateService.* | ✅完整 | |
| 借款申请/审批/核销 | ExpenseLoanService.* | ✅完整 | 完整生命周期 |

#### 前端功能

| 页面 | 状态 | 说明 |
|------|------|------|
| 费用类型 (type/index.tsx) | ✅完整 | |
| 报销单列表 (claim/list/index.tsx) | ⚠️部分 | 操作后使用 window.location.reload() |
| 报销单新增 (claim/add/index.tsx) | ✅完整 | |
| 报销单详情 (claim/detail/index.tsx) | ✅完整 | |
| 报销付款 (payment/index.tsx) | ✅完整 | |
| 费用统计 (statistics/index.tsx) | ✅完整 | |
| 费用分摊 (allocate/index.tsx) | ✅完整 | |
| 借款管理 (loan/index.tsx) | ✅完整 | |

#### API 对齐

| 后端接口 | 前端封装 | 状态 |
|----------|----------|------|
| GET /api/expense/type/list | expenseApi.typeList | ✅ |
| POST/PUT/DELETE /api/expense/type | expenseApi.type* | ✅ |
| GET /api/expense/claim/list | expenseApi.claimList | ✅ |
| GET /api/expense/claim/{id} | expenseApi.claimDetail | ✅ |
| POST/PUT /api/expense/claim | expenseApi.claimCreate/claimUpdate | ✅ |
| POST /api/expense/claim/{id}/submit | expenseApi.claimSubmit | ✅ |
| POST /api/expense/claim/{id}/approve | expenseApi.claimApprove | ✅ |
| POST /api/expense/claim/{id}/reject | expenseApi.claimReject | ✅ |
| POST /api/expense/claim/{id}/payment | expenseApi.paymentConfirm | ✅ |
| GET /api/expense/statistics | expenseApi.statisticsList | ✅ |
| GET/POST /api/expense/loan/* | expenseApi.loan* | ✅ |

---

### 模块6：Reports（报表）

#### 后端功能

| 功能点 | 方法 | 状态 | 说明 |
|--------|------|------|------|
| 资产负债表 | BalanceSheetService.GenerateAsync | ✅完整 | 从科目余额自动计算资产/负债/权益 |
| 利润表（多层利润） | IncomeStatementService.GenerateAsync | ✅完整 | 12行：营业收入→净利润，支持当期/累计 |
| 现金流量表 | CashFlowService.GenerateAsync | ✅完整 | 经营/投资/筹资三类活动，通过对方科目推断 |
| 科目余额表 | SubjectBalanceReportService.GetReportAsync | ✅完整 | 期初/本期/期末/年度累计 |
| 自定义报表 | CustomReportService.GenerateCustomReportAsync | ✅完整 | 支持公式计算 |
| 报表导出(CSV) | ReportExportService.ExportAsync | ✅完整 | 资产负债表/利润表导出 |
| 多期对比 | CompareService.CompareAsync | ✅完整 | 支持2-4期对比 |

#### 前端功能

| 页面 | 状态 | 说明 |
|------|------|------|
| 资产负债表 (balance-sheet/index.tsx) | ✅完整 | loading + 手动查询按钮 |
| 利润表 (income-statement/index.tsx) | ✅完整 | |
| 现金流量表 (cash-flow/index.tsx) | ✅完整 | |
| 科目余额表 (subject-balance/index.tsx) | ✅完整 | |
| 自定义报表 (custom/index.tsx) | ✅完整 | |

---

### 模块7：Tax（税务管理）

#### 后端功能

| 功能点 | 方法 | 状态 | 说明 |
|--------|------|------|------|
| 税种 CRUD | TaxCategoryService.* | ✅完整 | |
| 纳税计算 | TaxDeclarationService.CalculateAsync | ✅完整 | 从科目余额取税基，计算税额 |
| 纳税申报 | TaxDeclarationService.DeclareAsync | ✅完整 | |
| 确认缴款 | TaxDeclarationService.ConfirmPayAsync | ✅完整 | |
| 附加税计算 | TaxDeclarationService.CalculateSurchargesAsync | ✅完整 | 城建税7%+教育费附加3%+地方教育附加2% |
| 发票管理 | TaxInvoiceService.* | ✅完整 | 创建/删除/验真 |
| 税务汇总 | TaxReportService.GetSummaryAsync | ✅完整 | |
| 分税种汇总 | TaxReportService.GetByCategoryAsync | ✅完整 | |
| 税负率分析 | TaxReportService.GetTaxBurdenAsync | ✅完整 | 增值税税负率+综合税负率 |
| 税务日历 | TaxCalendarService.GetCalendarAsync | ✅完整 | 申报截止日+已申报状态 |

#### 前端功能

| 页面 | 状态 | 说明 |
|------|------|------|
| 税种管理 (type/index.tsx) | ✅完整 | |
| 纳税申报 (declaration/index.tsx) | ⚠️部分 | 状态映射有误：待申报=0显示空，已申报=1显示待申报 |
| 发票管理 (invoice/index.tsx) | ✅完整 | |
| 税务报表 (report/index.tsx) | ✅完整 | |
| 税负分析 (burden/index.tsx) | ⚠️部分 | 使用 raw fetch 而非 API 封装 |
| 税务日历 (calendar/index.tsx) | ✅完整 | |

---

### 模块8：System（系统管理）

#### 后端功能

| 功能点 | 方法 | 状态 | 说明 |
|--------|------|------|------|
| 登录/注册/密码修改 | AuthService.* | ✅完整 | |
| 用户 CRUD | UserService.* | ✅完整 | |
| 角色 CRUD | RoleService.* | ✅完整 | |
| 菜单/部门/岗位/字典 | BusinessServices.* | ✅完整 | |
| 系统模块管理 | SystemModuleDefinition | ✅完整 | |
| 公告通知 | NoticeController | ✅完整 | |
| 操作日志 | DictLogModuleConfigController | ✅完整 | |

#### 前端功能

| 页面 | 状态 | 说明 |
|------|------|------|
| 用户管理 (user/index.tsx) | ✅完整 | |
| 角色管理 (role/index.tsx) | ✅完整 | |
| 菜单管理 (menu/index.tsx) | ✅完整 | |
| 部门管理 (dept/index.tsx) | ✅完整 | |
| 岗位管理 (post/index.tsx) | ✅完整 | |
| 字典管理 (dict/index.tsx) | ✅完整 | |
| 系统配置 (config/index.tsx) | ✅完整 | |
| 模块管理 (module/index.tsx) | ✅完整 | |
| 操作日志 (log/index.tsx) | ✅完整 | |
| 登录页 (login/index.tsx) | ✅完整 | |
| 首页仪表盘 (dashboard/index.tsx) | ✅完整 | |

---

## 三、核心业务逻辑深度

### 3.1 凭证生命周期 ✅ **完整**
- 创建（草稿）→ 审核 → 反审核 → 作废 → 红字冲销 → 复制 → 批量审核 → 批量作废 → 删除
- **深度评价**：全流程闭环，借贷平衡校验、期间校验、制审分离校验完备
- **问题**：前后端状态值不一致（P0）

### 3.2 期末结账 + 反结账 + 年结 ✅ **完整**
- 结账前校验（未审核凭证、试算平衡）
- 余额滚转至下一期（排除损益类科目）
- 反结账检查下一期间
- 年末自动结转本年利润到利润分配
- **深度评价**：逻辑完整，含12月特殊处理和下年度自动初始化

### 3.3 损益结转（收入/费用→本年利润） ✅ **完整**
- 查询损益类科目（类型4收入/5费用/6成本）余额
- 收入类借记结转、费用类贷记结转
- 生成本年利润汇总分录
- **深度评价**：支持收入和费用两种方向的结转，生成自动审核凭证

### 3.4 折旧计算 + 生成折旧凭证 ✅ **完整**
- **直线法**：`(原值-残值)/使用月数`，取 min(月折旧, 剩余可折旧)
- **双倍余额递减法**：`2/使用月数 × 账面价值`，不超过可折旧额
- **年数总和法**：`剩余月数/总月数 × 可折旧额`
- 确认入账：更新卡片累计折旧 + 自动生成折旧凭证（借管理费用6602，贷累计折旧1602）
- **深度评价**：三种方法实现正确，确认入账含完整的凭证生成

### 3.5 纳税计算（增值税 + 附加税） ✅ **完整**
- 税种管理支持关联科目自动取税基
- 无关联科目时支持手动输入税基
- 附加税自动计算：城建税7% + 教育费附加3% + 地方教育附加2%
- 申报→缴纳完整生命周期
- **深度评价**：附加税链式计算逻辑完整

### 3.6 审批流转 ✅ **完整**
- 发起→审批（通过→下一节点/完成，驳回→结束）→撤回→转办→批量操作
- 我的待办（排除已审批实例）、已办、我的申请
- 审批记录、审批统计
- **深度评价**：支持多节点流程，转办、撤回、批量操作完备

### 3.7 资产处置（报废/出售）+ 生成凭证 ✅ **完整**
- 状态校验（仅使用中/闲置可处置）
- 自动生成处置凭证：转出累计折旧、转出原值、处置收入、处置损益
- 损益计入营业外收入/支出
- **深度评价**：凭证生成逻辑完善，覆盖净值/残值/收入/损益四种情况

### 3.8 现金流量表 ✅ **基本完整**
- 经营活动、投资活动、筹资活动三类
- 通过对方科目代码推断现金流分类
- 计算净现金增加额
- **深度评价**：简化实现，通过对方科目推断而非指定现金流科目，精确度有限但功能可用

### 3.9 资产负债表自动计算 ✅ **完整**
- 资产类：货币资金、应收账款、预付账款、存货、固定资产、无形资产
- 负债类：短期借款、应付账款、应付职工薪酬、应交税费、长期借款
- 权益类：实收资本、资本公积、盈余公积、未分配利润
- 自动从科目余额汇总计算
- **深度评价**：使用 AccountingConstants 科目代码前缀匹配，逻辑清晰

### 3.10 利润表自动计算 ✅ **完整**
- 12行多层利润：营业收入→营业成本→税金及附加→销售费用→管理费用→财务费用→营业利润→营业外收入→营业外支出→利润总额→所得税费用→净利润
- 支持当期/累计两种数据类型
- **深度评价**：计算逻辑完整，层次分明

---

## 四、问题清单

### P0 — 功能不可用/数据错误

| # | 问题 | 文件 | 影响 |
|---|------|------|------|
| P0-1 | **凭证状态值前后端不一致**：后端 Status=0草稿/1已审核/2已作废；前端映射 Status=0草稿/1待审核/2已审核/3已作废。导致前端显示状态全部错位，"已审核"凭证显示为"待审核"，"已作废"凭证显示为"已审核" | 后端: `VoucherService.cs` (Status赋值)；前端: `voucher/list/index.tsx` (状态Tag渲染) | 凭证状态显示全部错误，影响用户判断和操作按钮展示 |
| P0-2 | **种子数据缺失**：`init-sqlite.sql` 无任何 INSERT 语句，无 admin 用户、角色、菜单、科目等初始数据。系统部署后无法登录使用 | `init-sqlite.sql` | 全新部署后系统不可用，无法登录 |

### P1 — 功能不完善/存在 Bug

| # | 问题 | 文件 | 影响 |
|---|------|------|------|
| P1-1 | **BudgetExecutionService BudgetYearId 用作 PeriodYear**：`FirstAsync(p => p.PeriodYear == query.BudgetYearId)` 中 BudgetYearId 是数据库主键ID，不是年份数值，应先查 BudgetYear.Year 再匹配 | `BudgetServices.cs:275` | 预算执行跟踪数据永远查不到（除非ID恰好等于年份） |
| P1-2 | **VoucherService.BatchVoidAsync 逻辑错误**：先查询 `status==2`（已作废）的凭证然后报错，但此检查意图是防止重复作废。实际应该查询 `status!=0&&status!=1` 的凭证（已作废的才跳过） | `VoucherService.cs:250-257` | 批量作废可能无法正常工作 |
| P1-3 | **前端多处使用 raw fetch 而非 API 封装**：`voucher/list/index.tsx`（批量审核、冲销）、`asset/dispose/index.tsx`（处置）、`tax/burden/index.tsx`（税负分析）。这些请求缺少 JWT Token 等认证头，在生产环境中会 401 失败 | 4个前端页面 | 相关功能在需要认证的环境中不可用 |
| P1-4 | **前端多处使用 window.location.reload()**：expense/claim/list、approval/pending、approval/done、tax/declaration 等页面的操作按钮使用 `window.location.reload()` 刷新页面 | 8处前端引用 | 用户体验差，全页面刷新而非局部更新 |
| P1-5 | **纳税申报页面状态映射错误**：后端 Status=0待申报/1已申报/2已缴纳；前端 `['', '待申报', '已申报', '已缴纳'][val]` 将 status=0 映射为空字符串 | `tax/declaration/index.tsx` | 待申报的记录状态显示为空 |
| P1-6 | **ApprovalFlowService.DeleteAsync 无使用中检查**：删除流程前未检查是否有进行中的审批实例 | `ApprovalServices.cs` | 可能导致进行中的审批实例找不到流程定义 |
| P1-7 | **ExpenseClaimService.ApproveAsync 月度限额校验数据源偏差**：月度汇总查询条件 `c.Status >= 2`（已通过+已付款），但当前审批的报销单 status=1（审批中），校验时未包含当前这笔 | `ExpenseServices.cs` ApproveAsync | 限额校验可能遗漏当前这笔报销金额 |
| P1-8 | **VoucherService.ReverseAsync 限制过严**：仅允许冲销已作废凭证（`original.Status != 2` 报错）。通常红字冲销应对已审核凭证使用，而非已作废凭证 | `VoucherService.cs:280` | 红字冲销功能的使用场景受限 |

### P2 — 功能缺失但有替代方案

| # | 问题 | 文件 | 影响 |
|---|------|------|------|
| P2-1 | **前端缺少凭证打印页面**：后端有 print-data 接口，前端无打印功能页面 | 无对应前端页面 | 无法打印凭证 |
| P2-2 | **前端缺少凭证复制/批量作废按钮**：后端有接口，前端 API 层有封装，但列表页无对应操作按钮 | `voucher/list/index.tsx` | 用户无法使用复制和批量作废功能 |
| P2-3 | **前端缺少科目导入导出页面**：后端有 export/import 接口，前端 API 有封装，无页面 | 无对应前端页面 | 无法导入导出科目数据 |
| P2-4 | **前端缺少辅助核算管理页面**：后端有完整的 auxiliary CRUD，前端 API 有封装，无独立管理页面 | 无对应前端页面 | 辅助核算（项目/客户/供应商）无法在前端管理 |
| P2-5 | **报表导出仅 CSV 格式**：`ReportExportService` 仅支持 CSV，无 Excel/PDF 导出 | `ReportServices.cs` ExportAsync | 报表导出格式单一 |
| P2-6 | **现金流量表为简化实现**：通过对方科目代码推断现金流分类，未使用指定现金流标识字段，精确度有限 | `ReportServices.cs` CashFlowService | 现金流量表数据可能不准确 |
| P2-7 | **ExpenseAllocateService.CreateAsync 无重复校验**：创建分摊单时无分摊编号唯一性检查 | `ExpenseServices.cs` | 可能产生重复的分摊记录 |
| P2-8 | **TaxInvoiceService 缺少 Update 方法**：只有 Create/Delete/Verify，无 Update | `TaxServices.cs` | 已录入的发票信息无法修改 |

---

## 五、数据库一致性检查

### 表结构 vs 实体类
- `init-sqlite.sql` 包含 **43张表**，所有表字段与实体类定义基本一致
- `fm_voucher_entry` 表有 `Subject TEXT` 冗余字段（实体类也有），用于存储科目名称快照
- `fm_asset_depreciation` 表无 `VoucherId` 字段，但 `ConfirmDepreciationAsync` 代码注释提到"如果该字段不存在则跳过回填"——实际未回填凭证关联

### 缺失的功能表
- 无 `fm_voucher_print` 表（打印记录）
- 无 `fm_subject_import_log` 表（导入日志）
- `expense_loan` 表未使用 `fm_` 前缀（命名不一致）

### 种子数据
- **零条 INSERT 语句** — 系统无初始科目体系、无管理员用户、无默认角色菜单
- 这是 P0 级问题，全新部署后系统无法使用

---

## 六、结论和建议

### 总体评价
系统在第九轮审查中表现出**较高的功能完整度**。后端 8 个业务模块共 24 个 Service 类均无 TODO 或空实现，所有核心业务方法都包含实际业务逻辑。前端 58 个页面组件覆盖了所有主要业务场景，API 层封装较完善。

### 优先修复建议
1. **立即修复 P0-1（凭证状态值不一致）**：统一前后端凭证状态枚举（建议后端改为 0草稿/1待审核/2已审核/3已作废，或前端改为 0草稿/1已审核/2已作废）
2. **立即补充 P0-2（种子数据）**：在 init-sqlite.sql 中添加 admin 用户、默认角色、菜单、标准会计科目体系等 INSERT 语句
3. **修复 P1-1（预算执行跟踪 Bug）**：将 BudgetYearId 转换为实际年份再查询账期间
4. **修复 P1-3（raw fetch 替换）**：将所有 raw fetch 调用替换为 API 封装，确保 JWT Token 正确传递
5. **修复 P1-2（批量作废逻辑）**：修正状态判断条件
6. **修复 P1-5（税务申报状态映射）**：对齐后端 status 枚举

### 完善建议
- 为凭证列表页增加复制、批量作废按钮
- 增加凭证打印页面
- 增加辅助核算管理独立页面
- 增加科目导入导出页面
- 将 `window.location.reload()` 替换为局部状态刷新
- 为发票管理增加 Update 方法
- 将现金流量表改为指定现金流科目标识方式

### 评分明细

| 维度 | 评分 | 说明 |
|------|------|------|
| 后端 Service 完整性 | 90/100 | 所有方法有实际实现，无 TODO/空壳 |
| 前端页面完整性 | 80/100 | 主要页面齐全，缺少打印/导入导出/辅助核算页面 |
| API 覆盖度 | 85/100 | 大部分后端接口有前端封装，部分页面未使用封装 |
| 数据一致性 | 75/100 | 表结构一致，但缺少种子数据、命名不一致 |
| 核心业务逻辑 | 95/100 | 10 个核心业务全部有深度实现 |
| **综合评分** | **82/100** | **B+** |
