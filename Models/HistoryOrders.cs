using System.ComponentModel.DataAnnotations;

namespace PetShop.Models
{
    public class HistoryOrders
    {
        [Key]
        public int Id { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
