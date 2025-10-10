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
        private readonly IConfiguration _configuration;

        public MerchandiseController(ZooliranteContext context)
        public MerchandiseController(ZooliranteContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        /* search and filter feature*/
        public async Task<IActionResult> Index(string searchMerchandise, string priceFilter, MerchViewModel vm)
        {
            var merch = from i in _context.Merchandises
                        where !i.ItemName.Contains("Ticket") && !i.ItemName.Contains("Pass")
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

            UpdateCartCountFromDefaultVM(vm.DefaultVM);

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
            UpdateCartCountFromDefaultVM(vm);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Cart(int itemToDel, DefaultViewModel vm) {

            var vmJson = HttpContext.Session.GetString("DefaultVM");
            if (!string.IsNullOrEmpty(vmJson)) {
                vm = JsonSerializer.Deserialize<DefaultViewModel>(vmJson)!;
            }

            var item = vm.temporaryCart.RemoveAll(i => i.ItemId == itemToDel);

            HttpContext.Session.SetString("DefaultVM", JsonSerializer.Serialize(vm));

            return RedirectToAction(nameof(Cart));
        }

        // GET: Merchandise/Details/5
        public async Task<IActionResult> Details(int? id) {
            var merchandise = await _context.Merchandises
                .FirstOrDefaultAsync(m => m.ItemId == id);

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




                    TempData["SuccessMessage"] = $"{merchandise.ItemName} was updated successfully.";
                    return RedirectToAction(nameof(Edit), new { id = merchandise.ItemId });
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

        private void UpdateCartCountFromDefaultVM(Zoolirante.ViewModels.DefaultViewModel vm)
        {
            var totalQty = vm?.temporaryCart?.Sum(i => i.Quantity) ?? 0;
            HttpContext.Session.SetInt32("CartCount", totalQty);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateQuantity(int itemId, int delta)
        {
            var vmJson = HttpContext.Session.GetString("DefaultVM");
            var vm = string.IsNullOrEmpty(vmJson)
                ? new DefaultViewModel()
                : JsonSerializer.Deserialize<DefaultViewModel>(vmJson)!;

            var line = vm.temporaryCart.FirstOrDefault(i => i.ItemId == itemId);
            if (line != null)
            {
                line.Quantity = Math.Max(1, line.Quantity + delta); // clamp at 1
                HttpContext.Session.SetString("DefaultVM", JsonSerializer.Serialize(vm));
                UpdateCartCountFromDefaultVM(vm); // ✅ update badge
            }

            return RedirectToAction(nameof(Cart)); // PRG back to Cart view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout()
        {
            var vmJson = HttpContext.Session.GetString("DefaultVM");
            if (string.IsNullOrEmpty(vmJson))
            {
                return RedirectToAction("Cart");
            }

            var vm = JsonSerializer.Deserialize<DefaultViewModel>(vmJson)!;

            if (vm.temporaryCart == null || !vm.temporaryCart.Any())
            {
                return RedirectToAction("Cart");
            }

            // Set up Stripe
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

            var lineItems = new List<SessionLineItemOptions>();

            foreach (var item in vm.temporaryCart)
            {
                var merchItem = _context.Merchandises.Find(item.ItemId);
                if (merchItem != null)
                {
                    lineItems.Add(new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            Currency = "aud",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = merchItem.ItemName,
                                Description = merchItem.ItemDescription,
                            },
                            UnitAmount = (long)(item.UnitPrice * 100),
                        },
                        Quantity = item.Quantity,
                    });
                }
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                BillingAddressCollection = "required",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Merchandise/PaymentSuccess?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Merchandise/Cart",
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Redirect(session.Url);
        }

        public IActionResult PaymentSuccess(string session_id)
        {
            if (string.IsNullOrEmpty(session_id))
            {
                return RedirectToAction("Index");
            }

            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
            var service = new SessionService();
            var session = service.Get(session_id);

            if (session.PaymentStatus == "paid")
            {
                // Clear the cart
                var vmJson = HttpContext.Session.GetString("DefaultVM");
                if (!string.IsNullOrEmpty(vmJson))
                {
                    var vm = JsonSerializer.Deserialize<DefaultViewModel>(vmJson)!;
                    vm.temporaryCart.Clear();
                    HttpContext.Session.SetString("DefaultVM", JsonSerializer.Serialize(vm));
                    UpdateCartCountFromDefaultVM(vm);
                }

                ViewBag.Message = "Payment successful! Thank you for your purchase.";
                ViewBag.SessionId = session_id;
                ViewBag.CustomerEmail = session.CustomerDetails?.Email;
                ViewBag.AmountPaid = ((decimal)(session.AmountTotal ?? 0) / 100m).ToString("C", System.Globalization.CultureInfo.GetCultureInfo("en-AU"));
            }
            else
            {
                ViewBag.Message = "Payment verification failed.";
            }

            return View();
        }
    }
}
