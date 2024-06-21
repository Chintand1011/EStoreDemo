namespace EShop.API.Data.Models;
public class ServiceResponse
{
    public bool IsSuccess { get; }

    public string Message { get; }

    public int StatusCode { get; set; } = StatusCodes.Status200OK;

    public ServiceResponse(bool isSuccess)
    {
        IsSuccess = isSuccess;
    }

    public ServiceResponse(bool isSuccess, string message, int statusCode)
    {
        IsSuccess = isSuccess;
        Message = message;
        StatusCode = statusCode;
    }

    public static ServiceResponse ReturnSuccess()
    {
        return new ServiceResponse(true);
    }

    public static ServiceResponse ReturnFailed(string message)
    {
        return new ServiceResponse(false, message, StatusCodes.Status400BadRequest);
    }

    public static ServiceResponse ReturnNotFound(string message)
    {
        return new ServiceResponse(false, message, StatusCodes.Status404NotFound);
    }
}


public class ServiceResponse<T>
{
    public bool IsSuccess { get; set; }
    public T Data { get; set; }
    public string Message { get; set; }
    public int StatusCode { get; set; } = StatusCodes.Status200OK;
}
