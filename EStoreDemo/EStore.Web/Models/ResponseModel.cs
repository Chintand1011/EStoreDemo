namespace EShop.Web.Models;
public class ResponseModel
{
    public string Data { get; set; }
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
}

public class ResponseModel<T>
{
    public string Message { get; set; }
    public T Data { get; set; }
    public bool IsSuccess { get; set; }
}

