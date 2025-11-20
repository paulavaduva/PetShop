using PetShop.Models;
using Microsoft.EntityFrameworkCore;

namespace PetShop.Context
{
    public class PetShopContext : DbContext
    {
        public PetShopContext(DbContextOptions<PetShopContext> options) : base(options) {}
        public DbSet<Product> Products { get; set; }    
        public DbSet<Category> Categories { get; set; } 
        public DbSet<Order> Orders { get; set; }    
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<HistoryOrders> HistoryOrders { get; set; }
    }
}
