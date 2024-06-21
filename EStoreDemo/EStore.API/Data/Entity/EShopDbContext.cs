using EShop.API.Data.Entity.DbSet;
using Microsoft.EntityFrameworkCore;

namespace EShop.API.Data.Entity;
public class EShopDbContext: DbContext
{
    public EShopDbContext(DbContextOptions<EShopDbContext> options)
          : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
}

