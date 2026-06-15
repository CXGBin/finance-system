using FinanceSystem.Core.Common;
using Xunit;

namespace FinanceSystem.Tests;

/// <summary>
/// PageModel 和分页相关测试
/// </summary>
public class PageModelTests
{
    [Fact]
    public void PageModel_DefaultValues()
    {
        var page = new PageRequest();
        Assert.Equal(1, page.PageIndex);
        Assert.Equal(20, page.PageSize);
    }

    [Fact]
    public void PageResult_ShouldHoldData()
    {
        var result = new PageResult<string>(100, new List<string> { "a", "b" });
        Assert.Equal(100, result.Total);
        Assert.Equal(2, result.List.Count);
    }

    [Fact]
    public void PageResult_EmptyList()
    {
        var result = new PageResult<int>(0, new List<int>());
        Assert.Equal(0, result.Total);
        Assert.Empty(result.List);
    }
}

/// <summary>
/// 异常类型测试
/// </summary>
public class ExceptionTests
{
    [Fact]
    public void NotFoundException_ShouldSetMessage()
    {
        var ex = new NotFoundException("资源不存在");
        Assert.Equal("资源不存在", ex.Message);
    }

    [Fact]
    public void BusinessException_ShouldSetMessage()
    {
        var ex = new BusinessException("操作不允许");
        Assert.Equal("操作不允许", ex.Message);
    }
}
