using System.ComponentModel.DataAnnotations;
using System.Net.Http.Headers;

namespace PetShop.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
