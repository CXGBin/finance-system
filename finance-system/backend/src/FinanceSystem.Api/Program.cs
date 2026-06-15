using FinanceSystem.Api.Middleware;
using FinanceSystem.Infrastructure;
using FinanceSystem.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// 注册服务
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "财务管理系统 API", Version = "v1" });
});

// 注册基础设施
builder.Services.AddSqlSugarSetup(builder.Configuration);
builder.Services.AddRepository();

var app = builder.Build();

// 配置中间件管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();

app.MapControllers();

app.Run();
