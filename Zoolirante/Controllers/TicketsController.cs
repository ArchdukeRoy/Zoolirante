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
using Zoolirante.Services;


namespace Zoolirante.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ZooliranteContext _context;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public TicketsController(ZooliranteContext context, IConfiguration configuration, IEmailService emailService)
        {
            _context = context;
            _configuration = configuration;
            _emailService = emailService;
        }

        // GET: Tickets
        public IActionResult Index(DefaultViewModel vm)
        {
            var vmJson = HttpContext.Session.GetString("DefaultVM");
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
                BillingAddressCollection = "required",
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

            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
            var service = new SessionService();
            var session = service.Get(session_id);

            if (session.PaymentStatus == "paid")
            {
                var itemsJson = session.Metadata["items"];
                var items = System.Text.Json.JsonSerializer.Deserialize<List<CartItem>>(itemsJson);

                if (items != null && items.Count > 0)
                {
                    var ticketDetails = new List<TicketDetail>();

                    // Save to database and collect ticket IDs
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
                        await _context.SaveChangesAsync(); // Save to get the ID

                        // Add ticket details with the generated ID
                        ticketDetails.Add(new TicketDetail
                        {
                            TicketId = merchandise.ItemId,
                            Type = item.Type,
                            Date = item.Date,
                            Time = item.Time,
                            Adults = item.Adults,
                            Children = item.Children,
                            Concessions = item.Concessions,
                            Price = item.Price
                        });
                    }

                    // Send email receipt with QR codes
                    try
                    {
                        var customerEmail = session.CustomerDetails?.Email ?? "test@example.com";
                        var customerName = session.CustomerDetails?.Name ?? "Valued Customer";

                        await _emailService.SendTicketReceiptAsync(
                            customerEmail,
                            customerName,
                            ticketDetails,
                            session_id
                        );

                        ViewBag.Message = "Payment successful! Check your email for your tickets with QR codes.";
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Email failed: {ex.Message}");
                        ViewBag.Message = "Payment successful! (Email confirmation pending)";
                    }
                }

                ViewBag.SessionId = session_id;
            }
            else
            {
                ViewBag.Message = "Payment verification failed.";
            }

            return View();
        }

        public class CheckoutRequest
        {
            public List<CartItem> ?Items { get; set; }
            public decimal Total { get; set; }
        }

        public class CartItem
        {
            public string ?Type { get; set; }
            public string ?Date { get; set; }
            public string ?Time { get; set; }
            public int Adults { get; set; }
            public int Children { get; set; }
            public int Concessions { get; set; }
            public decimal Price { get; set; }
        }
    }
}
