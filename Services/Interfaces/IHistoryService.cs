using PetShop.Models;

namespace PetShop.Services.Interfaces
{
    public interface IHistoryService
    {
        IEnumerable<Order> GetHistoryOrders(string userId, bool isAdmin);
        Order? GetFinishedOrderById(int orderId);
    }
}
