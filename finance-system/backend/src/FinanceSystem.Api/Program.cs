using FinanceSystem.Api.Middleware;
using FinanceSystem.Api.Data;
using FinanceSystem.Core.Interfaces;
using FinanceSystem.Core.Modules;
using FinanceSystem.Infrastructure;
using FinanceSystem.Infrastructure.Extensions;
using FinanceSystem.Infrastructure.Services;
using FinanceSystem.Modules.System;
using FinanceSystem.Modules.Accounts;
using FinanceSystem.Modules.Reports;
using FinanceSystem.Modules.Budget;
using FinanceSystem.Modules.Approval;
using FinanceSystem.Modules.Asset;
using FinanceSystem.Modules.Expense;
using FinanceSystem.Modules.Tax;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using SqlSugar;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ========== Serilog 结构化日志 ==========
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/app-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

// ========== 控制器 ==========
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// ========== Swagger（含JWT认证按钮） ==========
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "财务管理系统 API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ========== JWT认证 ==========
var jwtConfig = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtConfig["Secret"] ?? throw new InvalidOperationException("缺少 JwtConfig:Secret 配置项，请在 appsettings.json 中配置JWT密钥");
var issuer = jwtConfig["Issuer"] ?? "FinanceSystem";
var audience = jwtConfig["Audience"] ?? "FinanceSystemClient";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ClockSkew = TimeSpan.Zero
        };
    });
builder.Services.AddAuthorization();

// ========== 基础设施（SqlSugar + 仓储） ==========
builder.Services.AddSqlSugarSetup(builder.Configuration);
builder.Services.AddRepository();

// ========== 模块化 DI 注册（基于模块开关） ==========

// 注册Token黑名单和RefreshToken存储服务（Redis + 内存降级）
builder.Services.AddSingleton<ITokenBlacklistService>(sp =>
{
    var service = new RedisTokenBlacklistService(
        sp.GetRequiredService<IConfiguration>(),
        sp.GetRequiredService<ILogger<RedisTokenBlacklistService>>());
    return service;
});
builder.Services.AddSingleton<IRefreshTokenStoreService>(sp =>
{
    var service = new RedisTokenBlacklistService(
        sp.GetRequiredService<IConfiguration>(),
        sp.GetRequiredService<ILogger<RedisTokenBlacklistService>>());
    return service;
});

// 1. 创建模块注册器并注册所有模块定义
var registry = new ModuleRegistry();
registry.RegisterModules(new IModuleDefinition[]
{
    new SystemModuleDefinition(),
    new AccountsModuleDefinition(),
    new ReportsModuleDefinition(),
    new BudgetModuleDefinition(),
    new ApprovalModuleDefinition(),
    new AssetModuleDefinition(),
    new ExpenseModuleDefinition(),
    new TaxModuleDefinition(),
});

// 2. 注册器注入 DI（供其他服务查询模块状态）
builder.Services.AddSingleton<IModuleRegistry>(registry);

// 3. 根据模块开关状态注册各模块的 Service
//    当前默认全开（所有模块 IsEnabled=true），可通过数据库 SysModule 表动态关闭
foreach (var module in registry.GetAllModules())
{
    if (registry.IsModuleEnabled(module.ModuleId))
    {
        module.RegisterServices(builder.Services);
        Log.Information("模块已注册: {ModuleId} ({ModuleName})", module.ModuleId, module.ModuleName);
    }
    else
    {
        Log.Warning("模块已跳过（未启用）: {ModuleId} ({ModuleName})", module.ModuleId, module.ModuleName);
    }
}

// ========== CORS跨域 ==========
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// ========== 中间件管道 ==========
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<ModuleSwitchMiddleware>();
app.UseMiddleware<OperationLogMiddleware>();

// ========== 种子数据初始化 ==========
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
    await SeedData.InitializeAsync(db);
}

app.MapControllers();
app.Run();
