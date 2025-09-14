using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zoolirante.Data;
using Zoolirante.Models;
using Zoolirante.ViewModels;
using System.Text.Json;

namespace Zoolirante.Controllers
{
    public class AnimalListController : Controller
    {
        private readonly ZooliranteContext _context;

        public AnimalListController(ZooliranteContext context)
        {
            _context = context;
        }
       //search feature
        public async Task<IActionResult> Index(string? searchAnimal, AnimalListViewModel vm) { 
            var species = from s in _context.Species
                        select s;

            // Search feature
            if (!string.IsNullOrEmpty(searchAnimal))
            {
                species = species.Where(s => s.Name.Contains(searchAnimal));
            }

            if (!await species.AnyAsync())
            {
                ViewBag.Error = "Animal not found";
            }

            ViewData["PresentFilter"] = searchAnimal;

            vm.SpeciesList = await species.ToListAsync();
            var vmJson = HttpContext.Session.GetString("DefaultVM");
            if (!string.IsNullOrEmpty(vmJson)) {
                vm.DefaultVM = JsonSerializer.Deserialize<DefaultViewModel>(vmJson)!;
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Like(int id) {

            var faM = new FavouriteAnimal();
            faM.AnimalId = id;
            faM.VisitorId = HttpContext.Session.GetInt32("id")!.Value;
            
            if (ModelState.IsValid) {
                _context.Add(faM);
                await _context.SaveChangesAsync();
                TempData["Liked"] = "Added to liked list";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(Index));
        }


        // GET: AnimalList
        public async Task<IActionResult> DisplayIndex()
        {

            var speciesDb = _context.Species
                .ToListAsync();

            return View(await speciesDb);
        }

        // GET: AnimalList/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var species = await _context.Species
                .FirstOrDefaultAsync(m => m.SpeciesId == id);
            if (species == null)
            {
                return NotFound();
            }

            return View(species);
        }

        // GET: AnimalList/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: AnimalList/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SpeciesId,Name,SpeciesImage")] Species species)
        {
            if (ModelState.IsValid)
            {
                _context.Add(species);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(species);
        }

        // GET: AnimalList/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var species = await _context.Species.FindAsync(id);
            if (species == null)
            {
                return NotFound();
            }
            return View(species);
        }

        // POST: AnimalList/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SpeciesId,Name,SpeciesImage")] Species species)
        {
            if (id != species.SpeciesId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(species);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpeciesExists(species.SpeciesId))
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
            return View(species);
        }

        // GET: AnimalList/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var species = await _context.Species
                .FirstOrDefaultAsync(m => m.SpeciesId == id);
            if (species == null)
            {
                return NotFound();
            }

            return View(species);
        }

        // POST: AnimalList/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var species = await _context.Species.FindAsync(id);
            if (species != null)
            {
                _context.Species.Remove(species);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpeciesExists(int id)
        {
            return _context.Species.Any(e => e.SpeciesId == id);
        }

      
    }
}
