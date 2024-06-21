using EShop.API.Data.Models;

namespace EShop.API.Repository.Interface;
public interface IOrderRepo
{
    public Task<ServiceResponse> GenerateOrder(OrderModel model);
    public Task<ServiceResponse<List<OrderModel>>> GetOrdersList(int userId);
    public Task<ServiceResponse<List<OrderItemModel>>> GetOrderDetail(int id);
}
