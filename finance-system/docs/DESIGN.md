# 财务管理系统 - 开发设计文档

## 一、项目概述

| 项目 | 说明 |
|------|------|
| 系统名称 | FinanceSystem（财务管理系统） |
| 架构模式 | 前后端分离 |
| 前端 | React 18 + Ant Design 5 + Vite + TypeScript |
| 后端 | .NET 8 Web API |
| ORM | SqlSugar |
| 数据库 | SQL Server 2019+（单租户） |
| 认证 | JWT Bearer Token |
| 核心特性 | **模块化架构，支持动态开启/关闭功能模块** |

---

## 二、系统架构

```
┌─────────────────────────────────────────────────┐
│                   前端 (React + AntD)             │
│  ┌─────────┐ ┌─────────┐ ┌─────────┐ ┌────────┐ │
│  │ 账户管理 │ │ 交易管理 │ │ 预算管理 │ │ 报表   │ │
│  └────┬────┘ └────┬────┘ └────┬────┘ └───┬────┘ │
│       └──────┬───┴────────────┴───────────┘     │
│         模块注册中心 (动态菜单/路由)               │
└──────────────────┬──────────────────────────────┘
                   │ HTTP / JWT
┌──────────────────┴──────────────────────────────┐
│              后端 (.NET 8 Web API)                │
│                                                   │
│  ┌─────────────────────────────────────┐         │
│  │         Module Kernel (模块内核)      │         │
│  │  - 模块注册表 / 模块状态管理            │         │
│  │  - 动态 Controller 注册               │         │
│  └──────────┬──────────────────────────┘         │
│             │                                     │
│  ┌──────────┴──────────────────────────┐         │
│  │           业务模块层                   │         │
│  │  Accounts │ Transactions │ Budget    │         │
│  │  Reports  │ System                    │         │
│  └──────────┬──────────────────────────┘         │
│             │                                     │
│  ┌──────────┴──────────────────────────┐         │
│  │     Infrastructure (基础设施层)        │         │
│  │  SqlSugar ORM │ DI │ JWT │ Middleware │         │
│  └─────────────────────────────────────┘         │
└───────────────────────────────────────────────────┘
```

---

## 三、后端分层结构

```
backend/
├── FinanceSystem.sln
└── src/
    ├── FinanceSystem.Core/                 # 核心层：实体、接口、枚举
    │   ├── Entities/                       # 数据库实体
    │   │   ├── BaseEntity.cs               # 基类（Id, CreatedAt, UpdatedAt）
    │   │   ├── Account.cs                  # 账户
    │   │   ├── Transaction.cs              # 交易记录
    │   │   ├── Category.cs                 # 收支分类
    │   │   ├── Budget.cs                   # 预算
    │   │   ├── ModuleStatus.cs             # 模块状态记录
    │   │   └── User.cs                     # 用户
    │   ├── Enums/
    │   │   ├── TransactionType.cs          # 收入/支出/转账
    │   │   ├── AccountType.cs              # 现金/银行卡/支付宝/微信
    │   │   └── ModuleCode.cs               # 模块枚举
    │   ├── Interfaces/
    │   │   ├── IModule.cs                  # 模块接口
    │   │   ├── IModuleManager.cs           # 模块管理器接口
    │   │   ├── IRepository.cs              # 通用仓储接口
    │   │   └── services/                   # 各业务服务接口
    │   └── Common/
    │       ├── ApiResult.cs                # 统一返回格式
    │       └── PagedResult.cs              # 分页返回
    │
    ├── FinanceSystem.Infrastructure/       # 基础设施层
    │   ├── Configuration/
    │   │   └── SqlSugarConfig.cs           # SqlSugar 配置
    │   ├── Services/
    │   │   ├── Repository.cs               # 通用仓储实现
    │   │   └── ModuleManager.cs            # 模块管理器实现
    │   └── Extensions/
    │       ├── SqlSugarSetup.cs            # SqlSugar DI 注册
    │       └── ServiceCollectionExtensions.cs # 业务服务 DI 注册
    │
    ├── FinanceSystem.Modules.Accounts/     # 模块：账户管理
    │   ├── Controllers/ AccountsController.cs
    │   ├── Services/ AccountService.cs
    │   └── Models/ AccountDto.cs
    │
    ├── FinanceSystem.Modules.Transactions/ # 模块：交易管理
    │   ├── Controllers/ TransactionsController.cs
    │   ├── Services/ TransactionService.cs
    │   └── Models/ TransactionDto.cs
    │
    ├── FinanceSystem.Modules.Budget/       # 模块：预算管理
    │   ├── Controllers/ BudgetController.cs
    │   ├── Services/ BudgetService.cs
    │   └── Models/ BudgetDto.cs
    │
    ├── FinanceSystem.Modules.Reports/      # 模块：报表统计
    │   ├── Controllers/ ReportsController.cs
    │   ├── Services/ ReportService.cs
    │   └── Models/ ReportDto.cs
    │
    ├── FinanceSystem.Modules.System/       # 模块：系统设置
    │   ├── Controllers/
    │   │   ├── AuthController.cs           # 登录/注册
    │   │   └── ModulesController.cs        # 模块开关管理
    │   ├── Services/ AuthService.cs
    │   └── Models/ LoginDto.cs
    │
    └── FinanceSystem.Api/                  # API 启动层
        ├── Program.cs                      # 入口 + DI 容器组装
        ├── appsettings.json
        ├── Middleware/
        │   └── ModuleCheckMiddleware.cs    # 模块状态校验中间件
        └── Extensions/
            └── ModuleSetup.cs             # 模块注册入口
```

---

## 四、模块化设计（核心）

### 4.1 模块定义

每个业务模块实现 `IModule` 接口：

```csharp
public interface IModule
{
    string Code { get; }           // 模块唯一编码
    string Name { get; }           // 模块显示名称
    string Description { get; }    // 模块描述
    int Order { get; }             // 排序
    void RegisterServices(IServiceCollection services); // 注册该模块的DI
}
```

### 4.2 内置模块列表

| 模块编码 | 模块名称 | 说明 | 默认状态 |
|---------|---------|------|---------|
| `accounts` | 账户管理 | 会计科目体系、资金账户管理 | ✅ 开启 |
| `transactions` | 交易管理 | 凭证录入、审核、记账流水 | ✅ 开启 |
| `budget` | 预算管理 | 年度/月度预算设定与预警 | ✅ 开启 |
| `reports` | 报表统计 | 收支统计、分类分析、趋势图表 | ✅ 开启 |
| `system` | 系统设置 | 用户管理、模块管理、系统配置 | ✅ 开启（不可关闭） |

### 4.3 模块开关流程

```
管理员在前端切换模块状态
    ↓
POST /api/system/modules/{code}/toggle
    ↓
ModuleManager 更新 ModuleStatus 表 (数据库持久化)
    ↓
前端下次加载菜单时 GET /api/system/modules
    ↓
返回已启用的模块列表 → 前端动态渲染菜单和路由
    ↓
后端 ModuleCheckMiddleware 拦截已禁用模块的 API 请求 → 返回 403
```

### 4.4 前端动态模块加载

前端通过 `ModuleRegistry` 统一管理：

```typescript
// 模块注册表
const moduleRegistry: ModuleConfig[] = [
  {
    code: 'accounts',
    name: '账户管理',
    icon: 'WalletOutlined',
    path: '/accounts',
    component: lazy(() => import('@/pages/accounts')),
  },
  // ... 其他模块
];

// 启动时请求后端获取已启用模块
const enabledModules = await api.getEnabledModules();

// 根据启用列表动态生成菜单和路由
const activeRoutes = moduleRegistry.filter(m => enabledModules.includes(m.code));
```

---

## 五、数据库设计

### 5.1 ER 关系

```
User (用户)
 ├── Account (账户) ──┐
 │                    ├── Transaction (交易记录)
 Category (分类) ─────┘
 │
 ├── Budget (预算)
 │
 ModuleStatus (模块状态)
```

### 5.2 核心表结构

#### Users 用户表
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | Guid | 主键 |
| Username | string | 用户名 |
| PasswordHash | string | 密码哈希 |
| DisplayName | string | 显示名称 |
| Role | string | 角色：Admin / User |
| CreatedAt | DateTime | 创建时间 |

#### Accounts 账户表
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | Guid | 主键 |
| UserId | Guid | 所属用户 |
| Name | string | 账户名称（如"工资卡"） |
| Type | int | 类型：现金/银行卡/支付宝/微信 |
| Balance | decimal | 余额 |
| Currency | string | 币种（默认 CNY） |
| Remark | string? | 备注 |

#### Categories 分类表
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | Guid | 主键 |
| UserId | Guid | 所属用户 |
| Name | string | 分类名称 |
| Type | int | 1=收入 2=支出 |
| Icon | string? | 图标 |
| ParentId | Guid? | 父分类（支持二级） |

#### Transactions 交易表
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | Guid | 主键 |
| UserId | Guid | 所属用户 |
| AccountId | Guid | 关联账户 |
| CategoryId | Guid? | 关联分类 |
| Type | int | 1=收入 2=支出 3=转账 |
| Amount | decimal | 金额 |
| TransactionDate | DateTime | 交易日期 |
| ToAccountId | Guid? | 转账目标账户 |
| Remark | string? | 备注 |
| CreatedAt | DateTime | 记录时间 |

#### Budgets 预算表
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | Guid | 主键 |
| UserId | Guid | 所属用户 |
| CategoryId | Guid? | 分类（null=总预算） |
| Period | string | 周期：2026-06 |
| BudgetAmount | decimal | 预算金额 |
| ActualAmount | decimal | 实际金额（计算字段，不入库） |

#### ModuleStatus 模块状态表
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | Guid | 主键 |
| ModuleCode | string | 模块编码 |
| IsEnabled | bool | 是否启用 |
| UpdatedAt | DateTime | 更新时间 |

---

## 六、API 设计

### 统一返回格式

```json
{
  "code": 200,
  "message": "success",
  "data": {}
}
```

### 核心 API 列表

#### 系统模块
| 方法 | 路径 | 说明 |
|------|------|------|
| POST | `/api/auth/login` | 登录 |
| POST | `/api/auth/register` | 注册 |
| GET | `/api/auth/profile` | 获取当前用户 |
| GET | `/api/system/modules` | 获取所有模块状态 |
| POST | `/api/system/modules/{code}/toggle` | 开关模块 |

#### 账户模块
| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/accounts` | 账户列表 |
| POST | `/api/accounts` | 创建账户 |
| PUT | `/api/accounts/{id}` | 编辑账户 |
| DELETE | `/api/accounts/{id}` | 删除账户 |
| GET | `/api/accounts/{id}/balance` | 查询余额 |

#### 交易模块
| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/transactions` | 交易列表（分页+筛选） |
| POST | `/api/transactions` | 记录交易 |
| PUT | `/api/transactions/{id}` | 编辑交易 |
| DELETE | `/api/transactions/{id}` | 删除交易 |
| GET | `/api/transactions/summary` | 收支汇总 |
| GET | `/api/categories` | 分类列表 |
| POST | `/api/categories` | 创建分类 |

#### 预算模块
| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/budgets` | 预算列表 |
| POST | `/api/budgets` | 设置预算 |
| PUT | `/api/budgets/{id}` | 编辑预算 |
| GET | `/api/budgets/status` | 预算执行状态 |

#### 报表模块
| 方法 | 路径 | 说明 |
|------|------|------|
| GET | `/api/reports/overview` | 总览数据 |
| GET | `/api/reports/category-breakdown` | 分类收支分析 |
| GET | `/api/reports/monthly-trend` | 月度趋势 |
| GET | `/api/reports/account-summary` | 各账户汇总 |

---

## 七、前端架构

### 7.1 目录结构

```
frontend/
├── package.json
├── vite.config.ts
├── tsconfig.json
├── index.html
└── src/
    ├── main.tsx                        # 入口
    ├── App.tsx                         # 根组件
    ├── router/
    │   └── index.tsx                   # 动态路由生成
    ├── layouts/
    │   └── MainLayout.tsx              # 主布局（侧边栏+顶栏）
    ├── modules/                        # 模块注册中心
    │   ├── registry.ts                 # 全模块注册表
    │   └── types.ts                    # 模块配置类型
    ├── stores/
    │   ├── auth.ts                     # 认证状态 (Zustand)
    │   └── modules.ts                  # 模块状态 (Zustand)
    ├── services/                       # API 请求层
    │   ├── request.ts                  # Axios 封装
    │   ├── auth.ts
    │   ├── account.ts
    │   ├── transaction.ts
    │   ├── budget.ts
    │   └── report.ts
    ├── pages/
    │   ├── login/
    │   ├── dashboard/                  # 首页概览
    │   ├── accounts/                   # 账户管理
    │   ├── transactions/               # 交易管理
    │   ├── budget/                     # 预算管理
    │   ├── reports/                    # 报表统计
    │   └── system/                     # 系统设置
    │       └── modules.tsx             # 模块管理页面
    └── components/                     # 公共组件
```

### 7.2 技术选型

| 库 | 用途 |
|----|------|
| antd 5.x | UI 组件库 |
| @ant-design/icons | 图标 |
| react-router-dom 6 | 路由 |
| zustand | 状态管理 |
| axios | HTTP 请求 |
| dayjs | 日期处理 |
| @ant-design/charts | 图表（报表用） |
| nprogress | 路由加载进度条 |

---

## 八、依赖注入设计

### 后端 DI 注册结构

```csharp
// Program.cs
builder.Services.AddSqlSugarSetup(builder.Configuration);
builder.Services.AddModuleServices();       // 注册所有模块服务
builder.Services.AddJwtAuthentication(...);

// ServiceCollectionExtensions.cs
public static void AddModuleServices(this IServiceCollection services)
{
    // 基础服务
    services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
    services.AddSingleton<IModuleManager, ModuleManager>();

    // 注册各模块
    var modules = new IModule[]
    {
        new AccountsModule(),
        new TransactionsModule(),
        new BudgetModule(),
        new ReportsModule(),
        new SystemModule(),
    };

    foreach (var module in modules)
    {
        module.RegisterServices(services);
    }
}
```

---

## 九、开发计划

| 阶段 | 内容 | 产出 |
|------|------|------|
| 1 | 后端 Core + Infrastructure | 实体、接口、SqlSugar 配置、DI 框架 |
| 2 | 后端业务模块 | 5 个模块的 Controller + Service |
| 3 | 后端 API 层 | Program.cs、中间件、JWT 认证 |
| 4 | 前端框架搭建 | 路由、布局、模块注册中心 |
| 5 | 前端业务页面 | 各功能模块 UI |
| 6 | 联调测试 | 前后端联调、数据验证 |

---

## 十、待确认事项

1. ~~数据库选择~~：**已确认 — SQL Server 2019+，不支持 MySQL/SQLite**
2. **部署方式**：是否需要 Docker 支持？
3. ~~用户体系~~：**已确认 — 单租户，企业级多用户协作（非多租户）**
4. **预算预警方式**：前端提示 vs 邮件/消息推送？
5. **报表导出**：是否需要 Excel/PDF 导出功能？
6. **数据初始化**：是否需要种子数据（默认分类、示例账户）？

---

> **请确认以上设计文档，或提出修改意见，确认后开始编码。**
