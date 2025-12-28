namespace PetShop.Repositories.Interfaces
{
    public interface IRepositoryWrapper
    {
        IProductRepository ProductRepository { get; }
        ICategoryRepository CategoryRepository { get; }
        IOrderRepository OrderRepository { get; }
        IOrderItemRepository OrderItemRepository { get; }
        IHistoryRepository HistoryRepository { get; }
        IAddressRepository AddressRepository { get; }
        void Save();
    }
}
