using PetShop.Models;

namespace PetShop.Services.Interfaces
{
    public interface IHistoryService
    {
        IEnumerable<HistoryOrders> GetHistoryOrders();
        Order? GetFinishedOrderById(int orderId);
    }
}
