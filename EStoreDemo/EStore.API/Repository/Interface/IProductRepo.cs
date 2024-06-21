using EShop.API.Data.Models;

namespace EShop.API.Repository.Interface;

public interface IProductRepo
{
    public Task<ServiceResponse<List<ProductModel>>> GetProducts();
    public Task<ServiceResponse> AddProduct(ProductModel model);
    public Task<ServiceResponse> UpdateProduct(ProductModel model, int id);
    public Task<ServiceResponse> DeleteProduct(int id, int userId);
}