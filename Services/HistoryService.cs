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
        public HistoryService(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
        }
        public IEnumerable<Order> GetHistoryOrders(string userId, bool isAdmin)
        {
            var orders = _repositoryWrapper.OrderRepository.FindByCondition(o => o.statusOrder == "Finished");
            if (orders == null)
            {
                return null;
            }

            if (!isAdmin)
            {
                orders = orders.Where(o => o.UserId == userId);
            }

            return orders.Include(o => o.OrderItems).ThenInclude(oi => oi.Product)
                         .Include(o => o.Address)
                         .Include(o => o.User)
                         //.Include(o => o.TotalPrice)
                         .OrderByDescending(o => o.Date)
                         .ToList();
        }

        public Order? GetFinishedOrderById(int orderId)
        {
            return _repositoryWrapper.OrderRepository.GetFinishedOrderById(orderId);
        }

    }
}
