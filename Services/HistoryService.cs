using Microsoft.EntityFrameworkCore;
using PetShop.Models;
using PetShop.Repositories.Interfaces;
using PetShop.Services.Interfaces;
using PetShop.Repositories;

namespace PetShop.Services
{
    public class HistoryService : IHistoryService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        //private readonly IOrderRepository _orderRepository;
        public HistoryService(IRepositoryWrapper repositoryWrapper/*, IOrderRepository orderRepository*/)
        {
            _repositoryWrapper = repositoryWrapper;
            //_orderRepository = orderRepository;
        }
        public IEnumerable<HistoryOrders> GetHistoryOrders()
        {
            return _repositoryWrapper.HistoryRepository.FindAll()
                .Include(h => h.Orders)
                    .ThenInclude(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .AsEnumerable()
                .Where(h => h.Orders.Any(
                    o => o.OrderItems != null &&
                    o.OrderItems.Any(oi => oi.Product != null)))
                .ToList();
        }

        public Order? GetFinishedOrderById(int orderId)
        {
            return _repositoryWrapper.OrderRepository.GetFinishedOrderById(orderId);
        }

    }
}
