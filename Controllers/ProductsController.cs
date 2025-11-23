using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetShop.Context;
using PetShop.DTOs;
using PetShop.Models;
using PetShop.Services.Interfaces;

namespace PetShop.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductsController(IProductService productService, ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            var products = _productService.GetAllProducts().Select(p => mapProduct(p)).ToList();

            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _productService.GetProductById(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(mapProduct(product));
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            var categories = _categoryService.GetAllCategories();
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name");
            return View(new ProductDto());
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,Stock,ProductImage,CategoryId,ImageFile")] ProductDto productDto)
        {
            var product = mapProduct(productDto);

            await _productService.AddProductAsync(product);
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _productService.GetProductById(id.Value);
            if (product == null)
            {
                return NotFound();
            }
            var categories = _categoryService.GetAllCategories();
            ViewData["CategoryId"] = new SelectList(categories, "Id", "Name", product.CategoryId);
            return View(mapProduct(product));
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,Stock,ProductImage,CategoryId,ImageFile")] ProductDto productDto)
        {
            if (id != productDto.Id)
            {
                return NotFound();
            }

            
            try
            {
                await _productService.UpdateProductAsync(productDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_productService.ProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = _productService.GetProductById(id.Value);
            if (product == null)
            {
                return NotFound();
            }

            return View(mapProduct(product));
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = _productService.GetProductById(id);
            if (product != null)
            {
               _productService.DeleteProduct(id);
            }

            return RedirectToAction(nameof(Index));
        }

        public IActionResult ProductsByCategory(int categoryId)
        {
            var productDto = _productService.GetProductsByCategory(categoryId);
            if (productDto == null)
            {
                return NotFound();
            }
            return View(productDto);
        }

        private ProductDto mapProduct(Product p)
        {
            return new ProductDto()
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Stock = p.Stock,
                ProductImage = p.ProductImage,
                ImageFile = p.ImageFile,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name
            };
            
        }
        private Product mapProduct(ProductDto productDto)
        {
            return new Product()
            {
                Id = productDto.Id,
                Name = productDto.Name,
                Description = productDto.Description,
                Price = productDto.Price,
                Stock = productDto.Stock,
                ProductImage = productDto.ProductImage,
                ImageFile = productDto.ImageFile,
                CategoryId = productDto.CategoryId
            };
        }
    }
}
