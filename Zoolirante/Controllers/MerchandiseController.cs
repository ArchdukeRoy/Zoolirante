using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zoolirante.Data;
using Zoolirante.Models;
using Zoolirante.ViewModels;
using System.Text.Json;

namespace Zoolirante.Controllers
{
    public class MerchandiseController : Controller
    {
        private readonly ZooliranteContext _context;

        public MerchandiseController(ZooliranteContext context)
        {
            _context = context;
        }

        /* search and filter feature*/
        public async Task<IActionResult> Index(string searchMerchandise, string priceFilter, MerchViewModel vm)
        {
            var merch = from i in _context.Merchandises
                        select i;

            // Search feature
            if (!string.IsNullOrEmpty(searchMerchandise))
            {
                merch = merch.Where(i =>
                    i.ItemName.Contains(searchMerchandise) ||
                    i.ItemDescription.Contains(searchMerchandise));
            }

            // Filter feature
            if (!string.IsNullOrEmpty(priceFilter))
            {
                switch (priceFilter)
                {
                    case "low price":
                        merch = merch.Where(i => i.ItemCost < 20);
                        break;
                    case "medium price":
                        merch = merch.Where(i => i.ItemCost >= 20 && i.ItemCost <= 30);
                        break;
                    case "high price":
                        merch = merch.Where(i => i.ItemCost > 30);
                        break;
                }
            }

            //  "no results" error
            if (!await merch.AnyAsync())
            {
                return NotFound("No merchandise in that price range!");
            }

            ViewData["PresentFilter"] = searchMerchandise;
            ViewData["PresentPriceFilter"] = priceFilter;

            vm.MerchList = await merch.ToListAsync();
            var vmJson = HttpContext.Session.GetString("DefaultVM");
            if (!string.IsNullOrEmpty(vmJson)) {
                vm.DefaultVM = JsonSerializer.Deserialize<DefaultViewModel>(vmJson)!;
            }
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Index(int id, int quantity, MerchViewModel vm) {

            vm.MerchList = await _context.Merchandises.ToListAsync();

            var vmJson = HttpContext.Session.GetString("DefaultVM");
            if (!string.IsNullOrEmpty(vmJson)) {
                vm.DefaultVM = JsonSerializer.Deserialize<DefaultViewModel>(vmJson)!;
            }

            if (vm.DefaultVM.username == null) {
                ViewBag.NotLoggedIn = "Please log in to add to cart.";
                return View(vm);
            }

            var MerchItem = vm.MerchList.Where(i => i.ItemId == id).ToList();
            var CartItem = vm.DefaultVM.temporaryCart.FirstOrDefault(i => i.ItemId == id);

            // If item in cart already. 
            if (CartItem != null) {
                CartItem.Quantity += quantity;
            } else {
                var newMerchItem = new MerchInOrder {
                    ItemId = id,
                    Quantity = quantity,
                    UnitPrice = MerchItem.Select(i => i.ItemCost).FirstOrDefault(),
                    Item = vm.MerchList.FirstOrDefault(i => i.ItemId == id)!
                };
                vm.DefaultVM.temporaryCart.Add(newMerchItem);
            }
            
            HttpContext.Session.SetString("DefaultVM", JsonSerializer.Serialize(vm.DefaultVM));

            ViewBag.ItemAdded = quantity + " " + MerchItem.Select(i => i.ItemName).FirstOrDefault() + " added to cart.";
            return View(vm);
        }

        public async Task<IActionResult> Cart(DefaultViewModel vm) {

            var vmJson = HttpContext.Session.GetString("DefaultVM");
            if (!string.IsNullOrEmpty(vmJson)) {
                vm = JsonSerializer.Deserialize<DefaultViewModel>(vmJson)!;
            }

            decimal totalCost = 0;
            foreach (var item in vm.temporaryCart) {
                totalCost += item.UnitPrice * item.Quantity;
            }

            ViewBag.TotalCost = totalCost;

            return View(vm);
        }

            // GET: Merchandise/Details/5
            public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var merchandise = await _context.Merchandises
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (merchandise == null)
            {
                return NotFound();
            }

            return View(merchandise);
        }

        // GET: Merchandise/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Merchandise/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemId,ItemName,ItemDescription,ItemCost,ItemImage")] Merchandise merchandise)
        {
            if (ModelState.IsValid)
            {
                _context.Add(merchandise);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(merchandise);
        }

        // GET: Merchandise/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var merchandise = await _context.Merchandises.FindAsync(id);
            if (merchandise == null)
            {
                return NotFound();
            }
            return View(merchandise);
        }

        // POST: Merchandise/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ItemId,ItemName,ItemDescription,ItemCost,ItemImage")] Merchandise merchandise)
        {
            if (id != merchandise.ItemId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(merchandise);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MerchandiseExists(merchandise.ItemId))
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
            return View(merchandise);
        }

        // GET: Merchandise/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var merchandise = await _context.Merchandises
                .FirstOrDefaultAsync(m => m.ItemId == id);
            if (merchandise == null)
            {
                return NotFound();
            }

            return View(merchandise);
        }

        // POST: Merchandise/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var merchandise = await _context.Merchandises.FindAsync(id);
            if (merchandise != null)
            {
                _context.Merchandises.Remove(merchandise);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MerchandiseExists(int id)
        {
            return _context.Merchandises.Any(e => e.ItemId == id);
        }
    }
}
