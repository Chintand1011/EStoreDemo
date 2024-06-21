using EShop.API.Data.Models;
using EShop.API.DataUtils.Interface;
using EShop.API.Repository.Interface;

namespace EShop.API.DataUtils.Service;
public class ProductData : IProductData
{
    private readonly IProductRepo _product;
    public ProductData(IProductRepo productService)
    {
        _product = productService;
    }
    public Task<ServiceResponse> CreateProduct(ProductModel model) => _product.AddProduct(model);

    public Task<ServiceResponse> DeleteProduct(int id, int userId) => _product.DeleteProduct(id, userId);

    public Task<ServiceResponse<List<ProductModel>>> GetProducts() => _product.GetProducts();

    public Task<ServiceResponse> UpdateProduct(ProductModel model, int id) => _product.UpdateProduct(model, id);
}