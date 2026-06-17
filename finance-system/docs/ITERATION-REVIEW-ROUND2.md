# 第2轮迭代审查报告

**审查时间**: 2026-06-17 11:32  
**审查基准**: commit `4c64d95` (前端全量页面补充三态处理)  
**审查范围**: 前端35个ProTable页面 + 后端排序/搜索接口 + 三态处理 + 前后端对齐

---

## 总体结论

代码改造工作完成度较高，核心功能（ProTable替换、后端排序、三态处理）已基本到位。发现 **1个P1问题 + 3个P2问题**，均不影响核心功能运行，但需修复以达到交付标准。

---

## 问题清单

### P1 - Legacy ProTable包装器排序参数缺失（影响15个页面）

**严重性**: P1 — 功能缺陷  
**描述**: `src/components/ProTable.tsx` (LegacyProTableInner) 的 `request` 函数没有提取 ProTable 的 `sortField` 和 `sortOrder` 参数传递给后端。虽然前端列配置了 `sorter: true`（可以点击排序），但排序仅在前端内存执行，不会传递到后端数据库层。对于分页数据，前端排序只排当前页，结果不准确。

**影响页面（15个）**: asset/change, asset/inventory, asset/card, expense/loan, expense/payment, expense/allocate, expense/claim/list, approval/template, approval/my, approval/pending, approval/done, tax/declaration, tax/invoice, budget/plan, budget/alert

**修复方案**: 在 LegacyProTableInner 的 request 函数中添加 sortField/sortOrder 提取逻辑（参考 createProTableRequest 的实现），或将15个页面全部迁移到直接使用 @ant-design/pro-components 的 ProTable + createProTableRequest。

---

### P2-1 - Legacy包装器页面搜索条件覆盖不足

**描述**: 15个使用Legacy包装器的页面，大部分只配置了1-2个搜索条件，远未达到"每列都有对应搜索条件"的要求。

**示例**:
- `asset/change`: 仅 assetCode 可搜索，其余5列不可搜索
- `approval/pending`: 仅 businessId 可搜索，title、initiatorId、createdTime 不可搜索
- `budget/plan`: 仅 subjectCode 可搜索，subjectName、annualBudget、executedAmount 不可搜索
- `expense/payment`: 无任何搜索条件
- `expense/loan`: 无任何搜索条件

---

### P2-2 - Legacy包装器缺少展开/隐藏搜索条件

**描述**: Legacy ProTable包装器将 `columns.some(c => c.search)` 作为是否显示搜索的判断条件。搜索条件多时没有展开/隐藏（collapse）支持。而直接使用 @ant-design/pro-components ProTable 的页面通过 `search={{ labelWidth: 'auto' }}` 也未配置 `collapse` 属性。

**修复**: 在直接使用ProTable的页面添加 `search={{ labelWidth: 'auto', collapse: { collapseRender: ... } }}`，或在Legacy包装器中统一添加。

---

### P2-3 - Budget模块后端分页未接入ApplySort

**描述**: `BudgetServices.GetListAsync` 使用自定义参数 `(long yearId, int pageIndex, int pageSize)` 和 `.OrderBy(s => s.SubjectId)` 硬编码排序，未使用 PageRequest 的 SortField/SortOrder。与前端传参不一致。

---

## ✅ 已验证通过项

1. **后端排序基础设施**: PageRequest 包含 SortField/SortOrder，ApplySort 扩展支持 camelCase/PascalCase，逻辑正确
2. **后端ApplySort接入**: Accounts、Approval、Asset、Expense、System、Tax 6个模块已接入，覆盖率完整
3. **直接ProTable页面（20个）**: 使用 createProTableRequest，排序参数传递正确（camelCase→PascalCase）
4. **三态处理**: commit 4c64d95 覆盖除 login/tax-calendar/tax-burden 外的所有页面（25个文件）
5. **紧凑布局**: 已完全移除，compactAlgorithm 引用数 0
6. **编译状态**: 前端tsc零错误，后端dotnet build零错误，181/181单测通过
7. **代码推送**: 所有commit已推送至远程仓库

---

## 修复优先级

| 优先级 | 问题 | 建议方案 |
|--------|------|---------|
| P1 | Legacy包装器排序缺失 | 将15个页面迁移到直接使用 ProTable + createProTableRequest |
| P2 | 搜索条件覆盖不足 | 逐页补全搜索条件 |
| P2 | 展开/隐藏搜索 | 在 ProTable search 配置中添加 collapse |
| P2 | Budget排序未接入 | 改用 PageRequest + ApplySort |
