using PetShop.Context;
using PetShop.Models;
using PetShop.Repositories.Interfaces;

namespace PetShop.Repositories
{
    public class OrderRepository : RepositoryBase<Order>, IOrderRepository
    {
        public OrderRepository(PetShopContext context) : base(context) { }
    }
}
