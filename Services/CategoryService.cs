using PetShop.Models;
using PetShop.Repositories.Interfaces;
using PetShop.Services.Interfaces;

namespace PetShop.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepositoryWrapper _repository;
        public CategoryService(IRepositoryWrapper repository)
        {
            _repository = repository;
        }

        public IEnumerable<Category> GetAllCategories()
        {
            return _repository.CategoryRepository.FindAll().ToList();
        }

        public Category? GetCategoryById(int id)
        {
            return _repository.CategoryRepository.FindByCondition(c => c.Id == id).FirstOrDefault();
        }

        public void AddCategory(Category category)
        {
            _repository.CategoryRepository.Create(category);
            _repository.Save();
        }
        public void DeleteCategory(int id)
        {
            var category = GetCategoryById(id);
            if (category != null)
            {
                _repository.CategoryRepository.Delete(category);
                _repository.Save();
            }
        }
        public void UpdateCategory(Category category)
        {
            _repository.CategoryRepository.Update(category);
            _repository.Save();
        }
    }
}
