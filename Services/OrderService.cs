using PetShop.Repositories.Interfaces;
using PetShop.Services.Interfaces;
using PetShop.Models;
using Microsoft.EntityFrameworkCore;
using NuGet.Versioning;

namespace PetShop.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        public OrderService(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
        }

        public Order GetOrCreateCart(string userId)
        {
            var cart = _repositoryWrapper.OrderRepository.FindByCondition(o => o.UserId == userId && o.statusOrder == "Unfinished")
                                                         .FirstOrDefault();

            if (cart == null)
            {
                cart = new Order
                {
                    UserId = userId,
                    Date = DateTime.Now.ToUniversalTime(),
                    statusOrder = "Unfinished",
                    OrderItems = new List<OrderItem>()
                };
                _repositoryWrapper.OrderRepository.Create(cart);
                _repositoryWrapper.Save();
            }
            return cart;
        }

        public void UpdateOrder(Order order)
        {
            _repositoryWrapper.OrderRepository.Update(order);
            _repositoryWrapper.Save();
        }

        public void AddToCart(string userId, int productId, int quantity)
        {
            var cart = GetOrCreateCart(userId);
            var existingItem = _repositoryWrapper.OrderItemRepository.FindByCondition(oi => oi.IdOrder == cart.Id && oi.ProductId == productId)
                                                                     .FirstOrDefault();

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                _repositoryWrapper.OrderItemRepository.Update(existingItem);
            }
            else
            {
                var newItem = new OrderItem()
                {
                    IdOrder = cart.Id,
                    ProductId = productId,
                    Quantity = quantity,
                    Product = null
                };
                _repositoryWrapper.OrderItemRepository.Create(newItem);
            }
            _repositoryWrapper.Save();
        }

        public void RemoveFromCart(string userId, int productId)
        {
            var cart = GetOrCreateCart(userId);
            var item = _repositoryWrapper.OrderItemRepository.FindByCondition(oi => oi.IdOrder == cart.Id && oi.ProductId== productId)
                                                             .FirstOrDefault();
            if (item != null)
            {
                _repositoryWrapper.OrderItemRepository.Delete(item);
                _repositoryWrapper.Save();
            }
        }

        public void ClearCart(string userId)
        {
            var cart = GetOrCreateCart(userId);
            var items = _repositoryWrapper.OrderItemRepository.FindByCondition(oi => oi.IdOrder == cart.Id).ToList();
            foreach (var item in items)
            {
                _repositoryWrapper.OrderItemRepository.Delete(item);
            }
            _repositoryWrapper.Save();
        }

        public Order GetCartWithItems(string userId)
        {
            return _repositoryWrapper.OrderRepository.FindByCondition(o => o.UserId == userId && o.statusOrder == "Unfinished")
                                                     .Include(o => o.OrderItems)
                                                     .ThenInclude(oi => oi.Product)
                                                     .FirstOrDefault();
        }

        public Order? GetOrderById(int orderId)
        {
            return _repositoryWrapper.OrderRepository.FindByCondition(o => o.Id == orderId)
                                                     .Include(o => o.OrderItems)
                                                     .ThenInclude(oi => oi.Product)
                                                     .Include(o => o.Address) // address
                                                     .FirstOrDefault();
        }

        public float CalculateOrderTotal(Order order)
        {
            if (order == null || order.OrderItems == null) return 0;

            float itemsTotal = order.OrderItems.Sum(x => (x.Product?.Price ?? 0) * x.Quantity);

            float deliveryFee = 9.99f;

            return itemsTotal + deliveryFee;
        }

        public int? MarkOrderAsFinished(Order order)
        {
            var validItems = order.OrderItems.Where(oi => oi.Product != null)
                                             .ToList();                   
            if (!validItems.Any())
            {
                return null;
            }

            var history = new HistoryOrders();
            _repositoryWrapper.HistoryRepository.Create(history);
            _repositoryWrapper.Save();

            float finalTotal = CalculateOrderTotal(order);

            //var finalizedOrder = new Order()
            //{
            //    UserId = order.UserId,
            //    Date = DateTime.UtcNow,
            //    statusOrder = "Finished",
            //    IdHistoryOrders = history.Id,
            //    AddressId = order.AddressId,
            //    TotalPrice = finalTotal,
            //    OrderItems = validItems.Select(oi => new OrderItem
            //    {
            //        ProductId = oi.ProductId,
            //        Quantity = oi.Quantity
            //    }).ToList()
            //};

            //_repositoryWrapper.OrderRepository.Create(finalizedOrder);
            //_repositoryWrapper.Save();
            //return finalizedOrder.Id;

            order.statusOrder = "Finished";
            order.Date = DateTime.UtcNow;
            order.IdHistoryOrders = history.Id;
            order.TotalPrice = finalTotal;

            _repositoryWrapper.OrderRepository.Update(order);
            _repositoryWrapper.Save();
            return order.Id;
        }

        public string? GetUserEmailById(string userId)
        {
            return _repositoryWrapper.OrderRepository.GetUserEmailById(userId);
        }

        public void DecreaseStockForOrder(Order order)
        {
            foreach (var item in order.OrderItems)
            {
                var product = item.Product;
                if (product != null && product.Stock >= item.Quantity)
                {
                    product.Stock -= item.Quantity;
                    _repositoryWrapper.ProductRepository.Update(product);   
                }
                else
                {

                }
            }
            _repositoryWrapper.Save();
        }

        public List<string> ValidateStock(Order order)
        {
            var errors = new List<string>();

            foreach (var item in order.OrderItems)
            {
                var product = item.Product;
                if (product == null || product.Stock < item.Quantity)
                {
                    errors.Add($"Insufficient stock for { product?.Name ?? "unknown product"} (available: { product?.Stock ?? 0})");
                }
            }
            return errors;
        }
    }
}
