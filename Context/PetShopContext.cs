using PetShop.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace PetShop.Context
{
    public class PetShopContext : IdentityDbContext<IdentityUser>
    {
        public PetShopContext(DbContextOptions<PetShopContext> options) : base(options) {}
        public DbSet<Product> Products { get; set; }    
        public DbSet<Category> Categories { get; set; } 
        public DbSet<Order> Orders { get; set; }    
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<HistoryOrders> HistoryOrders { get; set; }
    }
}
