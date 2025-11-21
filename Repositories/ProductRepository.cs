using PetShop.Context;
using PetShop.Models;
using PetShop.Repositories.Interfaces;

namespace PetShop.Repositories
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(PetShopContext context) : base(context) { }
    }
}
