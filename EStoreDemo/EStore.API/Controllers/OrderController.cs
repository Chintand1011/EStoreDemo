using EShop.API.Data.Models;
using EShop.API.DataUtils.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShop.API.Controllers;

[Route("api/")]
[ApiController]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderData _order;

    public OrderController(IOrderData orderData) => _order = orderData;

    [HttpPost]
    [Route("orders")]
    public async Task<ServiceResponse> GenerateOrder(OrderModel model) => await _order.GenerateOrder(model);

    [HttpGet]
    [Route("orders")]
    public async Task<ServiceResponse<List<OrderModel>>> GetOrdersList(int userId) => await _order.GetOrdersList(userId);

    [HttpGet]
    [Route("orders/{id}")]
    public async Task<ServiceResponse<List<OrderItemModel>>> GetOrderDetail(int id) => await _order.GetOrderDetail(id);
}