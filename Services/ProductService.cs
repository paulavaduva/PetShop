using Microsoft.AspNetCore.Identity;
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
            return _repositoryWrapper.ProductRepository.GetById(id);
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
        public List<Product> GetAllProducts()
        {
            return _repositoryWrapper.ProductRepository.FindAll().ToList();
        }
        public bool ProductExists(int id)
        {
            return _repositoryWrapper.ProductRepository.ProductExists(id);
        }

    }
}
