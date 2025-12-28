using PetShop.Models;

namespace PetShop.Services.Interfaces
{
    public interface IOrderService
    {
        Order GetOrCreateCart(string userId);
        void UpdateOrder(Order order);
        void AddToCart(string userId, int productId, int quantity);
        void RemoveFromCart(string userId, int productId);
        void ClearCart(string userId);

        Order GetCartWithItems(string userId);
        Order? GetOrderById(int orderId);
        int? MarkOrderAsFinished(Order order);

        string? GetUserEmailById(string userId);
        void DecreaseStockForOrder(Order order);
        List<string> ValidateStock(Order order);
        float CalculateOrderTotal(Order order);
    }
}
