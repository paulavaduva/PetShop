using PetShop.Models;

namespace PetShop.Repositories.Interfaces
{
    public interface IOrderRepository : IRepositoryBase<Order>
    {
        string? GetUserEmailById(string userId);
        Order? GetFinishedOrderById(int id);
    }
}
