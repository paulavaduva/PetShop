using Microsoft.EntityFrameworkCore;
using PetShop.Context;
using PetShop.Models;
using PetShop.Repositories.Interfaces;

namespace PetShop.Repositories
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(PetShopContext context) : base(context) { }

        public Product GetById(int id)
        {
            return PetShopContext.Products
                .Include(c => c.Category)
                .Include(o => o.OrderItems)
                .FirstOrDefault(m => m.Id == id);
        }

        public bool ProductExists(int id)
        {
            return PetShopContext.Products.Any(m => m.Id == id);
        }

        public List<OrderItem> GetAllOrderItems()
        {
            return PetShopContext.OrderItems.ToList();
        }

        public Category GetProductByCategory(int categoryId)
        {
            return PetShopContext.Categories
                .Include(a => a.Products)
                .FirstOrDefault(c => c.Id == categoryId);
        }
    }
}
