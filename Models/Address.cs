using System.ComponentModel.DataAnnotations;

namespace PetShop.Models
{
    public class Address
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Street { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string County { get; set; }
        [Required]
        public string ZipCode { get; set; }
    }
}
