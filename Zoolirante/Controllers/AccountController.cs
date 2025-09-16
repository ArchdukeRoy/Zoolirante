using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zoolirante.Data;
using Zoolirante.Models;
using Zoolirante.ViewModels;

namespace Zoolirante.Controllers
{
    public class AccountController : Controller
    {
        private readonly ZooliranteContext _context;

        public AccountController(ZooliranteContext context)
        {
            _context = context;
        }

        public IActionResult Index() {
            return View();
        }

        // GET: Account
        [HttpPost]
        public async Task<IActionResult> Index(string username, string password, DefaultViewModel _defaultViewModel) {

            var user = await _context.Visitors
                .Where(i => i.Username == username && i.PasswordHash == password)
                .FirstOrDefaultAsync();

            if (user == null || user.PasswordHash != password) {
                ViewBag.Error = "Invalid username or password.";
                return View();
            } 

            var person = await _context.People
                .Where(i => i.PersonId == user.VisitorId).FirstOrDefaultAsync();

            if (person == null) {
                ViewBag.Error = "Database Error. Please Contact Staff. ";
                return View();
            }

            var isAdmin = await _context.Staff
                .Include(s => s.Admin)
                .FirstOrDefaultAsync(s => s.Admin != null && s.Admin.AdminId == person.PersonId);

            _defaultViewModel.id = person.PersonId;
            _defaultViewModel.username = user.Username;
            _defaultViewModel.firstName = person.FirstName;
            _defaultViewModel.lastName = person.LastName;

            if (isAdmin != null) {
                _defaultViewModel.admin = true;
            } else {
                _defaultViewModel.admin = false;
            }
            
            var faDB = await _context.FavouriteAnimals
                .Where(i => i.VisitorId == user.VisitorId)
                .ToListAsync();
            _defaultViewModel.favouriteAnimals = faDB.Select(f => new FavouriteAnimalDataTransfer {
                FavAnimalsId = f.FavAnimalsId,
                AnimalId = f.AnimalId,
                VisitorId = f.VisitorId
            }).ToList();

            HttpContext.Session.SetString("DefaultVM", JsonSerializer.Serialize(_defaultViewModel));
            HttpContext.Session.SetInt32("id", _defaultViewModel.id);
            return RedirectToAction("Index", "Home"); 
        }

        // GET: Account/Create
        public IActionResult Create() {
            return View();
        }

        // POST: Account/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AccountViewModel vm) {
            vm.Person.Visitor = vm.Visitor;
            vm.Visitor.VisitorNavigation = vm.Person;

            if (ModelState.IsValid) {
                _context.Add(vm.Person);
                _context.Add(vm.Visitor);
                await _context.SaveChangesAsync();
                TempData["Created"] = "Account Created";
                return RedirectToAction(nameof(Index));
            }

            var errors = new List<string>();
            foreach (var state in ModelState) {
                foreach (var error in state.Value.Errors) {
                    errors.Add($"{state.Key}: {error.ErrorMessage}");
                }
            }
            ViewBag.Error = string.Join("<br/>", errors);

            return View(vm);
        }

        private bool PersonExists(int id)
        {
            return _context.People.Any(e => e.PersonId == id);
        }
    }
}
