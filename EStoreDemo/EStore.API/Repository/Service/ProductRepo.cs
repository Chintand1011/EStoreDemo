using EShop.API.Data.Entity;
using EShop.API.Data.Entity.DbSet;
using EShop.API.Data.Models;
using EShop.API.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace EShop.API.Repository.Service;

public class ProductRepo : IProductRepo
{
    private readonly EShopDbContext _db;

    public ProductRepo(EShopDbContext dbContext) => _db = dbContext;

    public async Task<ServiceResponse<List<ProductModel>>> GetProducts()
    {
        var result = new ServiceResponse<List<ProductModel>>();
        try
        {
            var products = await _db.Products.Where(x => x.IsDeleted == false).ToListAsync();

            result.IsSuccess = true;
            result.StatusCode = StatusCodes.Status200OK;
            result.Message = "Products fetched successfully.";
            result.Data = new List<ProductModel>();
            for (var i = 0; i < products.Count(); i++)
            {
                var product = products[i];
                result.Data.Add(new ProductModel()
                {
                    Id = product.Id,
                    Name = product.Name,
                    Stock = product.Stock,
                    Description = product.Description,
                    Price = product.Price,
                    CreatedAt = product.CreatedAt
                });
            }
        }
        catch (Exception)
        {
            result.IsSuccess = true;
            result.StatusCode = StatusCodes.Status500InternalServerError;
            result.Message = "Something went wrong. Please try again after some time.";
        }
        return result;
    }

    public async Task<ServiceResponse> AddProduct(ProductModel model)
    {
        try
        {
            var user = _db.Users.Where(x => x.Id == model.UserId).FirstOrDefault();
            if (user != null)
            {
                if (user.IsAdmin)
                {
                    var product = new Product()
                    {
                        Name = model.Name,
                        Description = model.Description,
                        Price = model.Price,
                        Stock = model.Stock,
                        CreatedAt = model.CreatedAt
                    };
                    await _db.Products.AddAsync(product);
                    await _db.SaveChangesAsync();

                    return new ServiceResponse(true, "Product added successfully.", StatusCodes.Status201Created);
                }
                else
                {
                    return new ServiceResponse(false, "You don't have rights to create product.", StatusCodes.Status406NotAcceptable);
                }
            }
            else
            {
                return new ServiceResponse(false, "Invalid user details.", StatusCodes.Status404NotFound);
            }
        }
        catch (Exception ex)
        {
            return new ServiceResponse(false, "Something went wrong. Please try again after some time.", StatusCodes.Status500InternalServerError);
        }
    }
    public async Task<ServiceResponse> UpdateProduct(ProductModel model, int id)
    {
        try
        {
            var user = await _db.Users.Where(x => x.Id == model.UserId).FirstOrDefaultAsync();
            if (user != null)
            {
                if (user.IsAdmin)
                {
                    var product = _db.Products.Where(x => x.Id == id).FirstOrDefault();
                    if (product != null)
                    {
                        product.Name = model.Name;
                        product.Description = model.Description;
                        product.Price = model.Price;
                        product.Stock = model.Stock;

                        _db.Update(product);
                        _db.SaveChanges();

                        return new ServiceResponse(true, "Product updated successfully.", StatusCodes.Status200OK);
                    }
                    else
                    {
                        return new ServiceResponse(true, "Product not found.", StatusCodes.Status404NotFound);
                    }
                }
                else
                {
                    return new ServiceResponse(false, "You don't have rights to update product.", StatusCodes.Status406NotAcceptable);
                }
            }
            else
            {
                return new ServiceResponse(false, "Only admin can update products.", StatusCodes.Status404NotFound);
            }


        }
        catch (Exception)
        {
            return new ServiceResponse(false, "Something went wrong. Please try again after some time.", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ServiceResponse> DeleteProduct(int id, int userId)
    {
        try
        {
            var user = await _db.Users.Where(x => x.Id == userId).FirstOrDefaultAsync();
            if (user != null)
            {
                if (user.IsAdmin)
                {
                    var product = await _db.Products.Where(x => x.Id == id).FirstOrDefaultAsync();

                    if (product != null)
                    {
                        product.IsDeleted = true;
                        _db.Products.Update(product);
                        _db.SaveChanges();

                        return new ServiceResponse(true, "Product deleted successfully.", StatusCodes.Status200OK);
                    }
                    else
                    {
                        return new ServiceResponse(true, "Product not found.", StatusCodes.Status404NotFound);
                    }
                }
                else
                {
                    return new ServiceResponse(false, "You don't have rights to update product.", StatusCodes.Status406NotAcceptable);
                }
            }
            else
            {
                return new ServiceResponse(false, "Invalid user details.", StatusCodes.Status404NotFound);
            }
        }
        catch (Exception)
        {
            return new ServiceResponse(false, "Something went wrong. Please try again after some time.", StatusCodes.Status500InternalServerError);
        }
    }

}
