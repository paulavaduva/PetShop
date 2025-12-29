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
                return Forbid(); // Sau Redirect to Index
            }

            return View(historyOrder);
        }

        //// GET: HistoryOrders/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //// POST: HistoryOrders/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id")] HistoryOrders historyOrders)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(historyOrders);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(historyOrders);
        //}

        //// GET: HistoryOrders/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var historyOrders = await _context.HistoryOrders.FindAsync(id);
        //    if (historyOrders == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(historyOrders);
        //}

        //// POST: HistoryOrders/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id")] HistoryOrders historyOrders)
        //{
        //    if (id != historyOrders.Id)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(historyOrders);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!HistoryOrdersExists(historyOrders.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(historyOrders);
        //}

        //// GET: HistoryOrders/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var historyOrders = await _context.HistoryOrders
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (historyOrders == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(historyOrders);
        //}

        //// POST: HistoryOrders/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var historyOrders = await _context.HistoryOrders.FindAsync(id);
        //    if (historyOrders != null)
        //    {
        //        _context.HistoryOrders.Remove(historyOrders);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool HistoryOrdersExists(int id)
        //{
        //    return _context.HistoryOrders.Any(e => e.Id == id);
        //}
    }
}
