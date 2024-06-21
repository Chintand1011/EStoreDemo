using EShop.API.Data.Models;
using EShop.API.DataUtils.Interface;
using EShop.API.Repository.Interface;

namespace EShop.API.DataUtils.Service;
public class OrderData : IOrderData
{
    private readonly IOrderRepo _order;
    public OrderData(IOrderRepo order) => _order = order;

    public Task<ServiceResponse> GenerateOrder(OrderModel model) => _order.GenerateOrder(model);

    public Task<ServiceResponse<List<OrderItemModel>>> GetOrderDetail(int id) => _order.GetOrderDetail(id);

    public Task<ServiceResponse<List<OrderModel>>> GetOrdersList(int userId) => _order.GetOrdersList(userId);
}
