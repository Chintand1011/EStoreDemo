using EShop.Web.Utility;
using EShop.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace EShop.Web.Controllers;

[SessionValidation]
public class ProductController : Controller
{
    private readonly IApiUtility _apiHelper;
    public ProductController(IApiUtility apiHelper)
    {
        _apiHelper = apiHelper;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<ResponseModel<List<ProductModel>>> GetProducts()
    {
        var responseMessage = await _apiHelper.ExecuteApiAsync("products", HttpMethod.Get, HttpContext);
        var responseAsync = await SharedMethod.ProcessApiResultAsync<List<ProductModel>>(responseMessage);
        return responseAsync;
    }

    [HttpPost]
    public async Task<ResponseModel<string>> CreateProduct([FromBody] ProductModel model)
    {
        model.UserId = HttpContext.Session.GetInt32("UserId");
        var responseMessage = await _apiHelper.ExecuteApiAsync("products", HttpMethod.Post, HttpContext, model);
        var responseAsync = await SharedMethod.ProcessApiResultAsync<string>(responseMessage);
        return responseAsync;
    }

    [HttpPost]
    public async Task<ResponseModel<string>> EditProduct([FromBody] ProductModel model)
    {
        model.UserId = HttpContext.Session.GetInt32("UserId");
        var responseMessage = await _apiHelper.ExecuteApiAsync("products/"+ model.Id, HttpMethod.Put, HttpContext, model);
        var responseAsync = await SharedMethod.ProcessApiResultAsync<string>(responseMessage);
        return responseAsync;
    }

    [HttpPost]
    public async Task<ResponseModel<string>> DeleteProduct([FromBody] ProductModel model)
    {
        model.UserId = HttpContext.Session.GetInt32("UserId");
        var responseMessage = await _apiHelper.ExecuteApiAsync("products/"+ model.Id + "?userId="+ model.UserId, HttpMethod.Delete, HttpContext);
        var responseAsync = await SharedMethod.ProcessApiResultAsync<string>(responseMessage);
        return responseAsync;
    }
}
