using PetShop.Context;
using PetShop.Repositories.Interfaces;

namespace PetShop.Repositories
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private PetShopContext _petShopContext;
        public RepositoryWrapper(PetShopContext petShopContext)
        {
            _petShopContext = petShopContext;
        }

        public void Save()
        {
            _petShopContext.SaveChanges();
        }

        // Product
        private IProductRepository? _productRepository;
        public IProductRepository ProductRepository
        {
            get
            {
                if (_productRepository == null)
                {
                    _productRepository = new ProductRepository(_petShopContext);
                }
                return _productRepository;
            }
        }
    }
}
