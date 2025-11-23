using Microsoft.AspNetCore.Identity;

namespace PetShop.Models
{
    public class User : IdentityUser
    {
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public ICollection<Order> Orders { get; set; }
    }
}
