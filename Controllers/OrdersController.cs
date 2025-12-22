using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetShop.Context;
using PetShop.Models;
using PetShop.Services.Interfaces;

namespace PetShop.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly UserManager<User> _userManager;

        public OrdersController(IOrderService orderService, UserManager<User> userManager)
        {
            _orderService = orderService;
            _userManager = userManager;
        }

        public string GetCurrentUserId()
        {
            var userId = _userManager.GetUserId(User);
            return userId;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            var userId = GetCurrentUserId();
            var cart = _orderService.GetCartWithItems(userId);
            return View(cart);
        }

        // POST: /Orders/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult AddToCart(int productId, int quantity)
        {
            var userId = GetCurrentUserId();
            _orderService.AddToCart(userId, productId, quantity);

            TempData["SuccessMessage"] = "Product has been added to your cart!";

            return Redirect(Request.Headers["Referer"].ToString());
        }

        // POST: /Orders/RemoveFromCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult RemoveFromCart(int productId)
        {
            var userId = GetCurrentUserId();
            _orderService.RemoveFromCart(userId, productId);
            return RedirectToAction(nameof(Index));
        }

        // POST: /Orders/ClearCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult ClearCart()
        {
            var userId = GetCurrentUserId();
            _orderService.ClearCart(userId);
            return RedirectToAction(nameof(Index));
        }

        // GET: /Orders/Checkout
        [Authorize]
        public IActionResult Checkout()
        {
            var userId = GetCurrentUserId();
            var cart = _orderService.GetCartWithItems(userId);
            if (cart == null)
            {
                return NotFound();
            }

            //var address = _addressService.GetAddressByUserId(userId);
            //ViewBag.Address = address;

            return View(cart);
        }

        // POST: /Orders/PlaceOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult PlaceOrder(string DeliveryMethod, string PaymentMethod, string TotalPrice)
        {
            var userId = GetCurrentUserId();
            var cart = _orderService.GetCartWithItems(userId);
            if (cart == null)
                return NotFound();

            //var address = _addressService.GetAddressByUserId(userId);
            //if (address == null)
            //    return NotFound("Address not found.");

            var stockErrors = _orderService.ValidateStock(cart);
            if (stockErrors.Any())
            {
                TempData["StockErrors"] = string.Join("<br>", stockErrors);
                return RedirectToAction("Checkout");
            }

            var orderId = _orderService.MarkOrderAsFinished(cart);
            _orderService.DecreaseStockForOrder(cart);


            return RedirectToAction("OrderConfirmation", "Orders", new { id = orderId });
        }

        //[Authorize]
        //public async Task<IActionResult> OrderConfirmation(int id)
        //{
        //    var order = _orderService.GetOrderById(id);
        //    if (order == null)
        //        return NotFound();
        //    var defaultDeliveryFee = 9.99f;
        //    var total = defaultDeliveryFee + order.OrderItems.Sum(item => item.Book.Price * item.Quantity);
        //    var date = order.Date.ToString("dd.MM.yyyy HH:mm");

        //    var address = _addressService.GetAddressByUserId(order.IdUser);
        //    var addressDetails = address != null ? $"{address.Street}, {address.City}, {address.ZipCode}, {address.County}" : "No address available";

        //    var itemDetails = string.Join("\n", order.OrderItems.Select(item => $"{item.Book.Title} (x{item.Quantity})"));

        //    var mailBody = $@"
        //        Thank you for your order!

        //        Order Number: #{order.Id}
        //        Date: {date}
        //        Total: {total:0.00} Lei
        //        Delivery Address: {addressDetails}

        //        Items ordered:
        //        {itemDetails}

        //        We appreciate your trust in Bookstore!
        //        ";

        //    var mail = new MailMessage
        //    {
        //        Subject = "Order Confirmation - Bookstore",
        //        Body = mailBody,
        //        IsBodyHtml = false
        //    };

        //    var userEmail = _orderService.GetUserEmailById(order.IdUser);
        //    if (!string.IsNullOrEmpty(userEmail))
        //    {
        //        mail.To.Add(userEmail);
        //        mail.From = new MailAddress("MS_1jRdlE@test-eqvygm003x8l0p7w.mlsender.net", "Bookstore App");
        //        await _mailService.SendEmailAsync(mail);
        //    }

        //    _orderService.ClearCart(order.IdUser);
        //    return View(order);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult IncreaseQuantity(int productId)
        {
            var userId = GetCurrentUserId();
            _orderService.AddToCart(userId, productId, 1);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult DecreaseQuantity(int productId)
        {
            var userId = GetCurrentUserId();
            var cart = _orderService.GetCartWithItems(userId);
            var item = cart.OrderItems.FirstOrDefault(i => i.ProductId == productId);
            if (item != null && item.Quantity > 1)
            {
                _orderService.AddToCart(userId, productId, -1); // Scade 1
            }
            else
            {
                _orderService.RemoveFromCart(userId, productId);
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
