using PetShop.Context;
using PetShop.Models;
using PetShop.Repositories.Interfaces;

namespace PetShop.Repositories
{
    public class HistoryRepository : RepositoryBase<HistoryOrders>, IHistoryRepository
    {
        public HistoryRepository(PetShopContext context) : base(context) { }
    }
}
