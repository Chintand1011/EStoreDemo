using EShop.API.Data.Entity;
using EShop.API.Data.Entity.DbSet;
using EShop.API.Data.Models;
using EShop.API.Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace EShop.API.Repository.Service;

public class OrderRepo : IOrderRepo
{
    private readonly EShopDbContext _db;

    public OrderRepo(EShopDbContext dbContext) => _db = dbContext;

    public async Task<ServiceResponse> GenerateOrder(OrderModel model)
    {
        try
        {
            var checkStock = CheckStock(model.OrderItems);
            if (checkStock.IsSuccess)
            {

                var orderItems = new List<OrderItem>();
                var products = new List<Product>();
                var totalPrice = 0.0M;

                var productIds = model.OrderItems.Select(oi => oi.ProductId).ToList();
                var productsList = _db.Products.Where(p => productIds.Contains(p.Id)).ToList();

                foreach (var orderItem in model.OrderItems)
                {
                    var product = productsList.Where(p => p.Id == orderItem.ProductId).First();
                    product.Stock -= orderItem.Quantity;
                    products.Add(product);

                    orderItems.Add(new OrderItem()
                    {
                        Price = product.Price,
                        ProductId = orderItem.ProductId,
                        Quantity = orderItem.Quantity,
                    });

                    totalPrice += orderItem.Quantity * product.Price;
                }
                var order = new Order()
                {
                    UserId = model.UserId,
                    TotalPrice = totalPrice,
                    Status = model.Status,
                    CreatedAt = DateTime.UtcNow
                };
                await _db.Orders.AddAsync(order);
                await _db.SaveChangesAsync();

                orderItems.ForEach(x => x.OrderId = order.Id);

                _db.Products.UpdateRange(products);
                await _db.OrderItems.AddRangeAsync(orderItems);
                await _db.SaveChangesAsync();
                return new ServiceResponse(true, "Order created successfully.", StatusCodes.Status201Created);
            }
            else
            {
                return checkStock;
            }
        }
        catch (Exception)
        {
            return new ServiceResponse(false, "Something went wrong. Please try again after some time.", StatusCodes.Status500InternalServerError);
        }
    }

    public async Task<ServiceResponse<List<OrderModel>>> GetOrdersList(int userId)
    {
        var result = new ServiceResponse<List<OrderModel>>();
        try
        {
            var orders = await _db.Orders.Where(x => x.UserId == userId).ToListAsync();

            result.IsSuccess = true;
            result.StatusCode = StatusCodes.Status200OK;
            result.Data = new List<OrderModel>();
            result.Message = "Orders fetched successfully.";
            for (var i = 0; i < orders.Count(); i++)
            {
                var order = orders[i];
                result.Data.Add(new OrderModel()
                {
                    Id = order.Id,
                    TotalPrice = order.TotalPrice,
                    UserId = order.UserId,
                    Status = order.Status,
                    CreatedAt = order.CreatedAt
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

    public async Task<ServiceResponse<List<OrderItemModel>>> GetOrderDetail(int id)
    {
        var result = new ServiceResponse<List<OrderItemModel>>();
        try
        {
            var orders = await _db.OrderItems.Where(x => x.OrderId == id).ToListAsync();
            var ids = orders.Select(x => x.ProductId).ToList();
            var products = await _db.Products.Where(x => ids.Contains(x.Id)).ToListAsync();
            result.IsSuccess = true;
            result.StatusCode = StatusCodes.Status200OK;
            result.Message = "Order details fetched Successfully.";
            result.Data = new List<OrderItemModel>();
            foreach (var order in orders)
            {
                var product = products.Where(x => x.Id == order.ProductId).FirstOrDefault() ?? new Product();
                var orderItem = new OrderItemModel()
                {
                    Id = order.Id,
                    Price = order.Price,
                    ProductId = order.ProductId,
                    Quantity = order.Quantity,
                    OrderId = order.OrderId,
                    Product = new ProductModel()
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Description = product.Name,
                        Price = product.Price,
                        Stock = product.Stock
                    }
                };
                result.Data.Add(orderItem);
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

    private ServiceResponse CheckStock(List<OrderItemModel> model)
    {
        var isOutofStock = false;
        var message = string.Empty;
        for (var i = 0; i < model.Count(); i++)
        {
            var order = model[i];
            var product = _db.Products.Where(x => x.Id == order.ProductId).FirstOrDefault() ?? new Product();
            var stock = product.Stock - order.Quantity;
            if (stock < 0)
            {
                isOutofStock = true;
                if (product.Stock == 0)
                {
                    message += product.Name + " is out of stock.";
                }
                else
                {
                    message += $"Ordered quantity for {product.Name} is not available at the moment. However, you can order upto {product.Stock} quantity.";
                }

            }
        }
        return new ServiceResponse(!isOutofStock, message, !isOutofStock ? StatusCodes.Status406NotAcceptable : StatusCodes.Status200OK);
    }
}
