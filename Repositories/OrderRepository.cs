using Microsoft.EntityFrameworkCore;
using PetShop.Context;
using PetShop.Models;
using PetShop.Repositories.Interfaces;

namespace PetShop.Repositories
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(PetShopContext context) : base(context) { }

        public string? GetUserEmailById(string userId)
        {
            return PetShopContext.Users
                .Where(u => u.Id == userId)
                .Select(u => u.Email)
                .FirstOrDefault();
        }

        public Order? GetFinishedOrderById(int id)
        {
            return PetShopContext.Orders
                .Include(o => o.Address)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.statusOrder == "Finished" && o.Id == id)
                .AsEnumerable()
                .Where(o => o.OrderItems.Any(oi => oi.Product != null))
                .FirstOrDefault();
        }
    }
}
