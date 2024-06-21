namespace EShop.Web.Utility;
public interface IApiUtility
{
    public Task<HttpResponseMessage> MakeApiCallAsync(string endPoint, HttpMethod httpMethod, dynamic payload = null);
    public Task<HttpResponseMessage> ExecuteApiAsync(string endPoint, HttpMethod httpMethod, HttpContext httpContext, dynamic payload = null);
}
