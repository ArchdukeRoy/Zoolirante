using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zoolirante.Data;
using Zoolirante.Models;

namespace Zoolirante.Controllers
{
    public class LoginController : Controller
    {
        private readonly ZooliranteContext _context;

        public LoginController(ZooliranteContext context)
        {
            _context = context;
        }

        // GET: Login
        public async Task<IActionResult> Index()
        {
            var zooliranteContext = _context.Visitors.Include(v => v.VisitorNavigation);
            return View(await zooliranteContext.ToListAsync());
        }

        // GET: Login/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitor = await _context.Visitors
                .Include(v => v.VisitorNavigation)
                .FirstOrDefaultAsync(m => m.VisitorId == id);
            if (visitor == null)
            {
                return NotFound();
            }

            return View(visitor);
        }

        // GET: Login/Create
        public IActionResult Create()
        {
            ViewData["VisitorId"] = new SelectList(_context.People, "PersonId", "PersonId");
            return View();
        }

        // POST: Login/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VisitorId,Username,PasswordHash,Contact,PaymentMethod")] Visitor visitor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(visitor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["VisitorId"] = new SelectList(_context.People, "PersonId", "PersonId", visitor.VisitorId);
            return View(visitor);
        }

        // GET: Login/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitor = await _context.Visitors.FindAsync(id);
            if (visitor == null)
            {
                return NotFound();
            }
            ViewData["VisitorId"] = new SelectList(_context.People, "PersonId", "PersonId", visitor.VisitorId);
            return View(visitor);
        }

        // POST: Login/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VisitorId,Username,PasswordHash,Contact,PaymentMethod")] Visitor visitor)
        {
            if (id != visitor.VisitorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(visitor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VisitorExists(visitor.VisitorId))
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
            ViewData["VisitorId"] = new SelectList(_context.People, "PersonId", "PersonId", visitor.VisitorId);
            return View(visitor);
        }

        // GET: Login/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitor = await _context.Visitors
                .Include(v => v.VisitorNavigation)
                .FirstOrDefaultAsync(m => m.VisitorId == id);
            if (visitor == null)
            {
                return NotFound();
            }

            return View(visitor);
        }

        // POST: Login/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var visitor = await _context.Visitors.FindAsync(id);
            if (visitor != null)
            {
                _context.Visitors.Remove(visitor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VisitorExists(int id)
        {
            return _context.Visitors.Any(e => e.VisitorId == id);
        }
    }
}
