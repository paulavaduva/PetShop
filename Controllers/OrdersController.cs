using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetShop.Context;
using PetShop.Models;
using PetShop.Services.Interfaces;
using Stripe;
using Stripe.Checkout;

namespace PetShop.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IAddressService _addressService;
        private readonly UserManager<User> _userManager;

        public OrdersController(IOrderService orderService, IAddressService addressService, UserManager<User> userManager)
        {
            _orderService = orderService;
            _addressService = addressService;
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
        public IActionResult PlaceOrder(string Street, string City, string County, string ZipCode, string PhoneNumber, string DeliveryMethod, string PaymentMethod)
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

            var newAddress = new Models.Address
            {
                Street = Street,
                City = City,
                County = County,
                ZipCode = ZipCode,
                PhoneNumber = PhoneNumber
            };

            _addressService.AddAddress(newAddress);
            cart.AddressId = newAddress.Id;
            
            cart.TotalPrice = _orderService.CalculateOrderTotal(cart);

            _orderService.UpdateOrder(cart);

            if (PaymentMethod == "Card")
            {
                var domain = "https://localhost:7213";

                var options = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",
                    SuccessUrl = domain + $"/Orders/PaymentSuccess?cartId={cart.Id}",
                    CancelUrl = domain + "/Orders/Checkout"
                };

                foreach (var item in cart.OrderItems)
                {
                    options.LineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)((item.Product?.Price ?? 0) * 100),
                            Currency = "ron",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product?.Name ?? "Produs",
                            },
                        },
                        Quantity = item.Quantity,
                    });
                }

                options.LineItems.Add(new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = 999,
                        Currency = "ron",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = "Delivery Fee",
                        },
                    },
                    Quantity = 1,
                });

                var service = new SessionService();
                Session session = service.Create(options);

                Response.Headers.Add("Location", session.Url);
                return new StatusCodeResult(303);
            }
            else
            {
                var orderId = _orderService.MarkOrderAsFinished(cart);
                _orderService.DecreaseStockForOrder(cart);

                //_orderService.ClearCart(userId);

                return RedirectToAction("OrderConfirmation", "Orders", new { id = orderId });
            }

        }

        [Authorize]
        public IActionResult PaymentSuccess(int cartId)
        {
            var userId = GetCurrentUserId();

            var cart = _orderService.GetCartWithItems(userId);
            if (cart == null || cart.Id != cartId)
            {
                return BadRequest("Order not fpund or access denied");
            }

            var orderId = _orderService.MarkOrderAsFinished(cart);
            _orderService.DecreaseStockForOrder(cart);
            return RedirectToAction("OrderConfirmation", "Orders", new { id = orderId });
        }

        [Authorize]
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var order = _orderService.GetOrderById(id);
            if (order == null)
                return NotFound();
            //var defaultDeliveryFee = 9.99f;
            var total = _orderService.CalculateOrderTotal(order);
            var date = order.Date.ToString("dd.MM.yyyy HH:mm");

            //var address = _addressService.GetAddressByUserId(order.IdUser);
            //var addressDetails = address != null ? $"{address.Street}, {address.City}, {address.ZipCode}, {address.County}" : "No address available";

            var itemDetails = string.Join("\n", order.OrderItems.Select(item => $"{item.Product.Name} (x{item.Quantity})"));

            //var mailBody = $@"
            //    Thank you for your order!

            //    Order Number: #{order.Id}
            //    Date: {date}
            //    Total: {total:0.00} Lei
            //    Delivery Address: {addressDetails}

            //    Items ordered:
            //    {itemDetails}

            //    We appreciate your trust in Bookstore!
            //    ";

            //var mail = new MailMessage
            //{
            //    Subject = "Order Confirmation - Bookstore",
            //    Body = mailBody,
            //    IsBodyHtml = false
            //};

            //var userEmail = _orderService.GetUserEmailById(order.UserId);
            //if (!string.IsNullOrEmpty(userEmail))
            //{
            //    mail.To.Add(userEmail);
            //    mail.From = new MailAddress("MS_1jRdlE@test-eqvygm003x8l0p7w.mlsender.net", "Bookstore App");
            //    await _mailService.SendEmailAsync(mail);
            //}

            _orderService.ClearCart(order.UserId);
            return View(order);
        }

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

        [HttpGet]
        public IActionResult GetCartItemCount()
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Ok(0);
                }

                var userId = GetCurrentUserId();
                var cart = _orderService.GetCartWithItems(userId);

                if (cart == null || cart.OrderItems == null)
                {
                    return Ok(0);
                }

                var count = cart.OrderItems.Sum(item => item.Quantity);

                return Ok(count);
            }
            catch
            {
                return Ok(0);
            }
        }
    }
}
