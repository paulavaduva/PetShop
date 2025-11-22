namespace PetShop.DTOs
{
    public class GroupCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<ProductDto> Products { get; set; }
        public ICollection<string> ProductName { get; set; }
    }
}
