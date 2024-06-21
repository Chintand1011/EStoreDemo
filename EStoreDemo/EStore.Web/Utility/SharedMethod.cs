using EShop.Web.Models;
using Newtonsoft.Json;
using System.Net;

namespace EShop.Web.Utility;
public class SharedMethod
{
    /// <summary>
    /// Description: To handle API response 
    /// </summary>
    /// <param name="responseMessage"></param>    
    public static async Task<ResponseModel<T>> ProcessApiResultAsync<T>(
        HttpResponseMessage responseMessage)
    {
        var result = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
        var response = JsonConvert.DeserializeObject<ResponseModel<T>>(result);
        return response;
    }
}
