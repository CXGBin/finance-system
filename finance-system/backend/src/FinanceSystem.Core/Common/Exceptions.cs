namespace FinanceSystem.Core.Common;

/// <summary>
/// 业务异常
/// </summary>
/// <summary>
/// BusinessException
/// </summary>
public class BusinessException : Exception
{
    /// <summary>
    /// 错误码
    /// </summary>
    public int Code { get; }

    /// <summary>
    /// 创建业务异常
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="code">错误码，默认400</param>
    public BusinessException(string message, int code = 400) : base(message)
    {
        Code = code;
    }
}

/// <summary>
/// 未授权异常（401）
/// </summary>
/// <summary>
/// UnauthorizedException
/// </summary>
public class UnauthorizedException : BusinessException
{
    /// <summary>
    /// 创建未授权异常
    /// </summary>
    /// <param name="message">错误消息</param>
    public UnauthorizedException(string message = "未授权，请先登录") : base(message, 401)
    {
    }
}

/// <summary>
/// 禁止访问异常（403）
/// </summary>
/// <summary>
/// ForbiddenException
/// </summary>
public class ForbiddenException : BusinessException
{
    /// <summary>
    /// 创建禁止访问异常
    /// </summary>
    /// <param name="message">错误消息</param>
    public ForbiddenException(string message = "无权限访问") : base(message, 403)
    {
    }
}

/// <summary>
/// 资源未找到异常（404）
/// </summary>
/// <summary>
/// NotFoundException
/// </summary>
public class NotFoundException : BusinessException
{
    /// <summary>
    /// 创建资源未找到异常
    /// </summary>
    /// <param name="message">错误消息</param>
    public NotFoundException(string message = "资源不存在") : base(message, 404)
    {
    }
}
