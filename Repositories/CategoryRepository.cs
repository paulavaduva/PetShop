using PetShop.Context;
using PetShop.Models;
using PetShop.Repositories.Interfaces;

namespace PetShop.Repositories
{
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(PetShopContext context) : base(context) { }
    }
}
