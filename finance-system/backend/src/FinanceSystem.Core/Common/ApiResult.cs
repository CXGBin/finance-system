namespace FinanceSystem.Core.Common;

/// <summary>
/// 统一响应结果
/// </summary>
/// <typeparam name="T">响应数据类型</typeparam>
/// <summary>
/// ApiResult
/// </summary>
public class ApiResult<T>
{
    /// <summary>
    /// 状态码（200成功，其他为错误码）
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// 响应消息
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// 响应数据
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public long Timestamp { get; set; } = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

    /// <summary>
    /// 创建成功响应
    /// </summary>
    /// <param name="data">响应数据</param>
    /// <param name="message">成功消息</param>
    /// <returns>成功响应结果</returns>
    public static ApiResult<T> Success(T data, string message = "success")
    {
        return new ApiResult<T> { Code = 200, Message = message, Data = data };
    }

    /// <summary>
    /// 创建失败响应
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="code">错误码</param>
    /// <returns>失败响应结果</returns>
    public static ApiResult<T> Fail(string message, int code = 400)
    {
        return new ApiResult<T> { Code = code, Message = message, Data = default };
    }
}

/// <summary>
/// 无数据的统一响应结果
/// </summary>
/// <summary>
/// ApiResult
/// </summary>
public class ApiResult : ApiResult<object>
{
    /// <summary>
    /// 创建成功响应（无数据）
    /// </summary>
    /// <param name="message">成功消息</param>
    /// <returns>成功响应结果</returns>
    public static new ApiResult Success(string message = "success")
    {
        return new ApiResult { Code = 200, Message = message, Data = null };
    }

    /// <summary>
    /// 创建失败响应（无数据）
    /// </summary>
    /// <param name="message">错误消息</param>
    /// <param name="code">错误码</param>
    /// <returns>失败响应结果</returns>
    public static new ApiResult Fail(string message, int code = 400)
    {
        return new ApiResult { Code = code, Message = message, Data = null };
    }
}
