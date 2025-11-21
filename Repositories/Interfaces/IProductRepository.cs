using PetShop.Models;

namespace PetShop.Repositories.Interfaces
{
    public interface IProductRepository : IRepositoryBase<Product>
    {
        Product GetById(int id);
        bool ProductExists(int id);
        List<OrderItem> GetAllOrderItems();
        Category GetProductByCategory(int categoryId);
    }
}
