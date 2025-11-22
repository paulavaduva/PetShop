using PetShop.Models;

namespace PetShop.Services.Interfaces
{
    public interface ICategoryService
    {
        IEnumerable<Category> GetAllCategories();
        Category? GetCategoryById(int id);
        void AddCategory(Category category);  
        void DeleteCategory(int id);
        void UpdateCategory(Category category); 
    }
}
