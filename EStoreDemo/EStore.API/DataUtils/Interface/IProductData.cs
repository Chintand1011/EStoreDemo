using EShop.API.Data.Models;

namespace EShop.API.DataUtils.Interface;

public interface IProductData
{
    public Task<ServiceResponse<List<ProductModel>>> GetProducts();
    public Task<ServiceResponse> CreateProduct(ProductModel model);
    public Task<ServiceResponse> UpdateProduct(ProductModel model, int id);
    public Task<ServiceResponse> DeleteProduct(int id, int userId);
}
