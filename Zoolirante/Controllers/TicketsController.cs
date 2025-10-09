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
using Stripe;
using Stripe.Checkout;

namespace Zoolirante.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ZooliranteContext _context;
        private readonly IConfiguration _configuration;

        public TicketsController(ZooliranteContext context)
        public TicketsController(ZooliranteContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // GET: Tickets
        public IActionResult Index(DefaultViewModel vm)
        {
            var vmJson = HttpContext.Session.GetString("DefaultVM");
            if (!string.IsNullOrEmpty(vmJson)) {
            if (!string.IsNullOrEmpty(vmJson))
            {
                vm = JsonSerializer.Deserialize<DefaultViewModel>(vmJson)!;
            }
            return View(vm);
        }

        // GET: Tickets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Visitor)
                .FirstOrDefaultAsync(m => m.TicketId == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // GET: Tickets/Create
        public IActionResult Create()
        {
            ViewData["VisitorId"] = new SelectList(_context.Visitors, "VisitorId", "VisitorId");
            return View();
        }

        // POST: Tickets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TicketId,DateOfEntry,DatePaid,VisitorId")] Ticket ticket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(ticket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["VisitorId"] = new SelectList(_context.Visitors, "VisitorId", "VisitorId", ticket.VisitorId);
            return View(ticket);
        }

        // GET: Tickets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
            {
                return NotFound();
            }
            ViewData["VisitorId"] = new SelectList(_context.Visitors, "VisitorId", "VisitorId", ticket.VisitorId);
            return View(ticket);
        }

        // POST: Tickets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TicketId,DateOfEntry,DatePaid,VisitorId")] Ticket ticket)
        {
            if (id != ticket.TicketId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(ticket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TicketExists(ticket.TicketId))
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
            ViewData["VisitorId"] = new SelectList(_context.Visitors, "VisitorId", "VisitorId", ticket.VisitorId);
            return View(ticket);
        }

        // GET: Tickets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var ticket = await _context.Tickets
                .Include(t => t.Visitor)
                .FirstOrDefaultAsync(m => m.TicketId == id);
            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        // POST: Tickets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null)
            {
                _context.Tickets.Remove(ticket);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TicketExists(int id)
        {
            return _context.Tickets.Any(e => e.TicketId == id);
        }

        [HttpPost]
        public IActionResult CreateCheckoutSession([FromBody] CheckoutRequest request)
        {
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];

            var lineItems = new List<SessionLineItemOptions>();

            foreach (var item in request.Items)
            {
                var description = $"{item.Date} at {item.Time} - {item.Adults} adults, {item.Children} children, {item.Concessions} concessions";

                lineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "aud",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Type,
                            Description = description,
                        },
                        UnitAmount = (long)(item.Price * 100),
                    },
                    Quantity = 1,
                });
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = lineItems,
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Tickets/Success?session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Tickets/Index",
                Metadata = new Dictionary<string, string>
        {
            { "items", System.Text.Json.JsonSerializer.Serialize(request.Items) }
        }
            };

            var service = new SessionService();
            Session session = service.Create(options);

            return Json(new { id = session.Id });
        }

        public async Task<IActionResult> Success(string session_id)
        {
            if (string.IsNullOrEmpty(session_id))
            {
                return RedirectToAction("Index");
            }

            // Verify payment with Stripe
            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
            var service = new SessionService();
            var session = service.Get(session_id);

            if (session.PaymentStatus == "paid")
            {
                // Deserialize items from metadata
                var itemsJson = session.Metadata["items"];
                var items = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(itemsJson);

                // Check if items deserialized successfully
                if (items != null && items.Count > 0)
                {
                    // Save each ticket as merchandise
                    foreach (var item in items)
                    {
                        var merchandise = new Merchandise
                        {
                            ItemName = $"{item.Type} - {item.Date} at {item.Time}",
                            ItemDescription = $"{item.Adults} Adult(s), {item.Children} Child(ren), {item.Concessions} Concession(s)",
                            ItemCost = item.Price,
                            ItemImage = null
                        };

                        _context.Add(merchandise);
                    }

                    await _context.SaveChangesAsync();

                    ViewBag.Message = "Payment successful! Your tickets have been confirmed and saved.";
                }
                else
                {
                    ViewBag.Message = "Payment successful, but there was an issue saving ticket details.";
                }

                ViewBag.SessionId = session_id;
            }
            else
            {
                ViewBag.Message = "Payment verification failed.";
            }

            return View();
        }

    }
}
