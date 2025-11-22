using PetShop.DTOs;
using PetShop.Models;

namespace PetShop.Services.Interfaces
{
    public interface IProductService
    {
        Product GetProductById(int id);
        Task AddProductAsync(Product product);
        void DeleteProduct(int id);
        Task UpdateProductAsync (ProductDto productDto); 
        List<Product> GetAllProducts(); 
        bool ProductExists(int id); 
        GroupCategoryDto GetProductsByCategory(int categoryId);
    }
}
