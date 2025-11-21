namespace PetShop.Repositories.Interfaces
{
    public interface IRepositoryWrapper
    {
        IProductRepository ProductRepository { get; }
        void Save();
    }
}
