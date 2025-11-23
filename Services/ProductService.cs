using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PetShop.DTOs;
using PetShop.Models;
using PetShop.Repositories.Interfaces;
using PetShop.Services.Interfaces;

namespace PetShop.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepositoryWrapper _repositoryWrapper;
        public ProductService(IRepositoryWrapper repositoryWrapper)
        {
            _repositoryWrapper = repositoryWrapper;
        }

        public Product GetProductById(int id)
        {
            return _repositoryWrapper.ProductRepository.FindByCondition(p => p.Id == id)
                .Include(p => p.Category)
                .FirstOrDefault();
        }
        public async Task AddProductAsync(Product product)
        {
            if (product.ImageFile != null && product.ImageFile.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    await product.ImageFile.CopyToAsync(ms);
                    product.ProductImage = ms.ToArray();
                }
            }
            _repositoryWrapper.ProductRepository.Create(product);
            _repositoryWrapper.Save();
        }

        public void DeleteProduct(int id)
        {
            var product = _repositoryWrapper.ProductRepository.GetById(id);
            if (product != null)
            {
                _repositoryWrapper.ProductRepository.Delete(product);
                _repositoryWrapper.Save();
            }
        }
        public async Task UpdateProductAsync(ProductDto productDto)
        {
            var product = _repositoryWrapper.ProductRepository.GetById(productDto.Id);
            mapProduct(productDto, product);

            using var ms = new MemoryStream();

            if (product.ImageFile != null && product.ImageFile.Length > 0)
            {
                await product.ImageFile.CopyToAsync (ms);
                product.ProductImage = ms.ToArray();
            }

            _repositoryWrapper.ProductRepository.Update(product);
            _repositoryWrapper.Save();
        }
        public List<Product> GetAllProducts()
        {
            return _repositoryWrapper.ProductRepository.FindAll()
                .Include(p => p.Category)
                .OrderByDescending(p => p.Stock > 0)
                .ToList();
        }
        public bool ProductExists(int id)
        {
            return _repositoryWrapper.ProductRepository.ProductExists(id);
        }
        public GroupCategoryDto GetProductsByCategory(int categoryId)
        {
            var category = _repositoryWrapper.ProductRepository.GetProductByCategory(categoryId);
            if (category == null)
            {
                return null;
            }
            var groupCategoryDto = new GroupCategoryDto
            {
                Id = categoryId,
                Name = category.Name,

                Products = category.Products
                    .OrderByDescending(p => p.Stock > 0)
                    .Select(product => new GroupProductDto
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Price = product.Price,
                        Stock = product.Stock,
                        ProductImage = product.ProductImage
                    }).ToList()
            };

            return groupCategoryDto;
        }
        private void mapProduct(ProductDto dto, Product product)
        {
            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.Stock = dto.Stock;
            product.ProductImage = dto.ProductImage;
            product.ImageFile = dto.ImageFile;
            product.CategoryId = dto.CategoryId;
        }
    }
}
