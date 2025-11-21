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

        // Category
        private ICategoryRepository? _categoryRepository;
        public ICategoryRepository CategoryRepository
        {
            get
            {
                if (_categoryRepository == null)
                {
                    _categoryRepository = new CategoryRepository(_petShopContext);
                }
                return _categoryRepository;
            }
        }

        // Order
        private IOrderRepository? _oderRepository;
        public IOrderRepository OrderRepository
        {
            get
            {
                if (_oderRepository == null)
                {
                    _oderRepository = new OrderRepository(_petShopContext);
                }
                return _oderRepository;
            }
        }

        // OrderItem
        private IOrderItemRepository? _orderItemRepository;
        public IOrderItemRepository OrderItemRepository
        {
            get
            {
                if (_orderItemRepository == null)
                {
                    _orderItemRepository = new OrderItemRepository(_petShopContext);
                }
                return _orderItemRepository;
            }
        }

        // History
        private IHistoryRepository? _historyRepository;
        public IHistoryRepository HistoryRepository
        {
            get
            {
                if (_historyRepository == null)
                {
                    _historyRepository = new HistoryRepository(_petShopContext);
                }
                return _historyRepository;
            }
        }
    }
}
