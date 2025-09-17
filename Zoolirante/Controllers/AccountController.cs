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

            _defaultViewModel.id = person.PersonId;
            _defaultViewModel.username = user.Username;
            _defaultViewModel.firstName = person.FirstName;
            _defaultViewModel.lastName = person.LastName;

            // Staff object instance is not assigned in dbCreation script. The connection is set here. 
            if (await _context.Staff.Where(i => i.StaffId == person.PersonId).FirstOrDefaultAsync() != null) {
                person.Staff = await _context.Staff.Where(i => i.StaffId == person.PersonId).FirstOrDefaultAsync();
            }

            if (person.Staff != null) {
                if (person.Staff.RoleId == 1001) {
                    _defaultViewModel.admin = true;
                }
            } else {
                _defaultViewModel.admin = false;
            }
            
            var faDB = await _context.FavouriteAnimals
                .Where(i => i.VisitorId == user.VisitorId)
                .ToListAsync();
            _defaultViewModel.favouriteAnimals = faDB.Select(f => new FavouriteAnimalDataTransfer {
                FavAnimalsId = f.FavAnimalsId,
                AnimalId = f.AnimalId,
                VisitorId = f.VisitorId,
                AnimalName = _context.Species.FirstOrDefault(i => i.SpeciesId == f.AnimalId)?.Name
            }).ToList();

            HttpContext.Session.SetString("DefaultVM", JsonSerializer.Serialize(_defaultViewModel));
            HttpContext.Session.SetInt32("id", _defaultViewModel.id);
            return RedirectToAction("Index", "Home"); 
        }

        public IActionResult Logout() {
            HttpContext.Session.Remove("DefaultVM");
            HttpContext.Session.Remove("id");
            TempData["LoggedOut"] = "Successfully Logged Out";
            return RedirectToAction(nameof(Index));
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

        public async Task<IActionResult> MyAccount(AccountViewModel vm) {
            var vmJson = HttpContext.Session.GetString("DefaultVM");
            if (!string.IsNullOrEmpty(vmJson)) {
                vm.DefaultVM = JsonSerializer.Deserialize<DefaultViewModel>(vmJson)!;
            } else {
                TempData["SignInFirst"] = "Sign in to access MyAccount features. ";
                return RedirectToAction(nameof(Index));
            }


            vm.Person = _context.People.FirstOrDefault(i => i.PersonId == vm.DefaultVM.id)!;
            vm.Visitor = _context.Visitors.FirstOrDefault(i => i.VisitorId == vm.DefaultVM.id)!;
            return View(vm);
        }

        private bool PersonExists(int id)
        {
            return _context.People.Any(e => e.PersonId == id);
        }
    }
}
