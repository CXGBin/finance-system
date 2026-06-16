# 第十一轮代码审查报告（前端架构升级后）

**审查时间**：2026-06-16 21:51  
**审查范围**：前端架构升级后的前后端对齐审查  
**编译状态**：前端 tsc 0 Error，后端 dotnet build 0 Error 0 Warning，vite build 成功  
**单元测试**：181/181 通过  
**Git 最新提交**：0ca6d87

---

## 一、总体评价

前端已升级为 Ant Design Pro 多页签紧凑布局（@ant-design/pro-components@3.x + antd v6），架构层面 ProLayout 侧边栏菜单 + TabsView 多页签集成正确。但**前后端数据字段对齐存在残留问题**，部分页面直接使用短字段名（debit/credit）而非后端实际的 camelCase 字段名（debitAmount/creditAmount），会导致页面数据不展示。

**综合评分：B（80/100）**  
- 后端：A+（95/100）  
- 前端架构：A（90/100）  
- 前后端对齐：C（60/100）← 需要修复

---

## 二、发现的问题

### P0 — 功能不可用（5项）

| # | 模块 | 问题 | 影响 | 修复方案 |
|---|------|------|------|---------|
| P0-1 | 账务-凭证编辑 | 前端发送 `debit/credit`，后端期望 `debitAmount/creditAmount` | 凭证创建失败（字段映射不匹配） | entries.map 中改为 `debitAmount: e.debit, creditAmount: e.credit` |
| P0-2 | 账务-凭证详情 | dataIndex 用 `debit/credit`，后端返回 `debitAmount/creditAmount` | 凭证详情分录金额不显示 | dataIndex 改为 `debitAmount/creditAmount` |
| P0-3 | 账务-余额管理 | dataIndex 用 `debit/credit`，后端 SubjectBalance 返回 `beginDebit/beginCredit` | 期初余额不显示 | dataIndex 改为 `beginDebit/beginCredit`，保存时字段名对齐 |
| P0-4 | 账务-明细账/日记账 | dataIndex 用 `debit/credit`，后端匿名对象返回 `debitAmount/creditAmount` | 账簿借贷金额不显示 | dataIndex 改为 `debitAmount/creditAmount` |
| P0-5 | 登录 | LoginParams 用 `remember`，后端 LoginRequest 用 `rememberMe` | rememberMe 始终为 false（后端忽略该字段） | LoginParams 中 `remember` → `rememberMe` |

### P1 — 功能异常（7项）

| # | 模块 | 问题 | 影响 | 修复方案 |
|---|------|------|------|---------|
| P1-1 | 登录 | useAuth hook 传 `{username, password, remember}`，后端接收 `rememberMe` | 记住我功能不生效 | 传递时映射为 `rememberMe` |
| P1-2 | 系统管理 | 后端默认路由（`/Delete`/`/Post`/`/Get`/`/Put`）来自未指定路径的 HTTP 属性 | 可能产生意外路由冲突 | 给所有 HTTP 属性补充明确的路径参数 |
| P1-3 | 费用/审批/税务 | 8处 `window.location.reload()` 全页面刷新 | 操作后体验差（整个页面白屏重载） | 改用 state 刷新或 router 刷新 |
| P1-4 | 前端 | 61个页面中仅38个有 loading/error/empty 三态处理（62%） | 部分页面加载时无反馈 | 统一在 ProTable/API 层添加 |
| P1-5 | 前端 | 凭证编辑页 `entries` 类型为 `any[]` | 无类型安全 | 使用 `VoucherEntry[]` |
| P1-6 | 前端 | 凭证创建传 `periodId`（数字），但后端 VoucherCreateRequest 期望 `voucherDate` + `periodYear/periodMonth` | 可能导致凭证关联错误期间 | 对齐 API 参数 |
| P1-7 | 前端 | 余额保存发送 `{debit, credit}` 对象，后端期望 `{subjectId, periodId, beginDebit, beginCredit}` | 余额保存失败 | 字段名对齐 |

### P2 — 建议改进（10项）

| # | 模块 | 问题 |
|---|------|------|
| P2-1 | 前端 | 页面23个字段 dataIndex 使用了类型定义中不存在的短字段名（如 `action`、`count`、`budgetAmount`、`createdBy`、`alertTime` 等） |
| P2-2 | 前端 | 类型定义中 SysDictItem 缺少 `dictType/dictLabel/dictValue/sortOrder/remark/status` 字段 |
| P2-3 | 前端 | 类型定义中 SysConfig 缺少 `id/createdTime/updatedTime` 字段 |
| P2-4 | 后端 | 后端实体字段注释大量重复（每个属性前有两个 `<summary>` 标签） |
| P2-5 | 前端 | SysRole 类型有 `roleCode` 但页面曾用过 `roleKey`（已在 commit 313b497 修复为 roleCode） |
| P2-6 | 前端 | 税务申报页面字段 `declarePeriod` vs 前端可能用 `period`（需确认） |
| P2-7 | 后端 | 后端约28个端点存在默认路由 `/Delete`/`/Post`/`/Get`/`/Put`（来自无路径参数的 HTTP 属性） |
| P2-8 | 前端 | 主 chunk 较大（622KB），建议进一步 code-split |
| P2-9 | 前端 | TabsView 刷新使用 `window.location.reload()` |
| P2-10 | 前端 | 多处 `navigate()` 缺少 `{ replace: true }`，可能导致历史记录堆积 |

---

## 三、正面结论

✅ **后端 CamelCase JSON 序列化配置正确** — `JsonNamingPolicy.CamelCase` 已全局配置（Program.cs + 两处中间件）  
✅ **登录认证模型完全对齐** — `accessToken/refreshToken/userInfo/realName/roles/permissions/modules`  
✅ **SysRole/SysMenu/SysDept 前后端字段已对齐** — commit 313b497 修复  
✅ **ProLayout 集成正确** — 8大模块菜单树完整，侧边栏/多页签/紧凑布局正常  
✅ **ANTD Pro 组件正确引入** — ProTable/PageContainer 使用正常  
✅ **raw fetch 全部清理** — 所有 API 调用均通过 axios 请求层（含 JWT Token）  
✅ **API 层 any 类型已清理** — 0 残留  
✅ **i18n 已移除** — 统一中文  
✅ **模块页面三态覆盖率 62%**（38/61），比之前的 22% 大幅提升  

---

## 四、修复优先级

1. **立即修复 P0-1~5**（字段映射），影响核心账务模块数据展示
2. **修复 P1-1, P1-6, P1-7**（参数名对齐）
3. **P1-2**（后端默认路由清理）
4. **P1-3**（window.location.reload 替换）
5. **P1-4**（三态处理补全）
6. **P2 按需改进**

---

## 五、统计数据

| 指标 | 值 |
|------|------|
| 后端 .cs 文件 | 192 |
| 后端 API 端点 | 139（含默认路由约28个需清理） |
| 前端 TS/TSX 文件 | 99 |
| 前端页面数 | 61 |
| 前端 API 封装 | 111 个路径 |
| 前端类型定义 | 9 个 .d.ts 文件 |
| 三态覆盖 | 38/61（62%） |
| tsc 错误 | 0 |
| dotnet build | 0 Error 0 Warning |
| 单元测试 | 181/181 |
| vite build | 成功 |
