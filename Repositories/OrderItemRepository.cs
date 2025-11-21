using PetShop.Context;
using PetShop.Models;
using PetShop.Repositories.Interfaces;

namespace PetShop.Repositories
{
    public class OrderItemRepository : RepositoryBase<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(PetShopContext context) : base(context) { }
    }
}
