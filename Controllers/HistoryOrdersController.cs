using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetShop.Context;
using PetShop.Models;
using PetShop.Services.Interfaces;
using Stripe.Climate;

namespace PetShop.Controllers
{
    public class HistoryOrdersController : Controller
    {
        private readonly IHistoryService _historyService;
        private readonly UserManager<User> _userManager;

        public HistoryOrdersController(IHistoryService historyService, UserManager<User> userManager)
        {
            _historyService = historyService;
            _userManager = userManager;
        }

        public string GetCurrentUserId()
        {
            var userId = _userManager.GetUserId(User);
            return userId;
        }
        public async Task<string> GetCurrentUserRole()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return null;
            }

            var roles = await _userManager.GetRolesAsync(user);

            return roles.FirstOrDefault();
        }

        [Authorize]
        // GET: HistoryOrders
        public IActionResult Index()
        {
            var userId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            var historyOrders = _historyService.GetHistoryOrders(userId, isAdmin);

            return View(historyOrders);
        }

        [Authorize]
        // GET: HistoryOrders/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var historyOrder = _historyService.GetFinishedOrderById(id.Value);
            if (historyOrder == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);
            if (!User.IsInRole("Admin") && historyOrder.UserId != userId)
            {
                return Forbid();
            }

            return View(historyOrder);
        }
    }
}
