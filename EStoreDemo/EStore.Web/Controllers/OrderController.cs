using EShop.Web.Utility;
using EShop.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace EShop.Web.Controllers;

[SessionValidation]
public class OrderController : Controller
{
    private readonly IApiUtility _apiUtility;
    public OrderController(IApiUtility apiUtility)
    {
        _apiUtility = apiUtility;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<ResponseModel<List<OrderModel>>> GetOrders()
    {
        var userId = HttpContext.Session.GetInt32("UserId");
        var responseMessage = await _apiUtility.ExecuteApiAsync("orders?userId=" + userId, HttpMethod.Get, HttpContext);
        var responseAsync = await SharedMethod.ProcessApiResultAsync<List<OrderModel>>(responseMessage);
        return responseAsync;
    }

    [HttpPost]
    public async Task<ResponseModel<List<OrderItemModel>>> GetOrderDetails([FromBody] OrderModel model)
    {
        var responseMessage = await _apiUtility.ExecuteApiAsync("orders/" + model.Id, HttpMethod.Get, HttpContext);
        var responseAsync = await SharedMethod.ProcessApiResultAsync<List<OrderItemModel>>(responseMessage);
        return responseAsync;
    }

    [HttpPost]
    public async Task<ResponseModel<string>> CreateOrder([FromBody] OrderModel model)
    {
        model.Status = "Pending";
        model.UserId = HttpContext.Session.GetInt32("UserId").Value;
        var responseMessage = await _apiUtility.ExecuteApiAsync("orders", HttpMethod.Post, HttpContext, model);
        var responseAsync = await SharedMethod.ProcessApiResultAsync<string>(responseMessage);
        return responseAsync;
    }
}