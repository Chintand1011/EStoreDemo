using EShop.API.Data.Models;
using EShop.API.DataUtils.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EShop.API.Controllers;

[Route("api/")]
[ApiController]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IProductData _product;
    public ProductController(IProductData productData) => _product = productData;

    [HttpGet]
    [Route("products")]
    public async Task<ServiceResponse<List<ProductModel>>> GetProducts() => await _product.GetProducts();


    [HttpPost]
    [Route("products")]
    public async Task<ServiceResponse> CreateProduct(ProductModel model) => await _product.CreateProduct(model);


    [HttpPut]
    [Route("products/{id}")]
    public async Task<ServiceResponse> UpdateProduct(ProductModel model, int id) => await _product.UpdateProduct(model, id);


    [HttpDelete]
    [Route("products/{id}")]
    public async Task<ServiceResponse> DeleteProduct(int id, int userId) => await _product.DeleteProduct(id, userId);
}
