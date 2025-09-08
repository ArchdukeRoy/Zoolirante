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
    public class MerchandiseController : Controller
    {
        private readonly ZooliranteContext _context;

        public MerchandiseController(ZooliranteContext context)
        {
            _context = context;
        }

        // GET: Merchandise
        public async Task<IActionResult> Index(string searchMerchandise)
        {
            
            var items = from i in _context.Merchandises select i;
            if (!string.IsNullOrEmpty(searchMerchandise))
            {
                items = items.Where(i =>
                    i.ItemName.Contains(searchMerchandise) ||
                    i.ItemDescription.Contains(searchMerchandise));
            }
            if (!items.Any())
            {
                return NotFound("Merchandise not found or invalid merchandise name");
            }

            // Keep the search term in the input
            ViewData["CurrentFilter"] = searchMerchandise;

            return View(await items.ToListAsync());
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
