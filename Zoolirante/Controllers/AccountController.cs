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
        public async Task<IActionResult> Index(string username, string password, DefaultViewModel _defaultViewModel)
        {

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

            HttpContext.Session.SetString("DefaultVM", JsonSerializer.Serialize(_defaultViewModel));
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.People
                .FirstOrDefaultAsync(m => m.PersonId == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // GET: Account/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Account/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AccountViewModel vm) 
        {
            vm.Person.Visitor = vm.Visitor;
            vm.Visitor.VisitorNavigation = vm.Person;

            if (ModelState.IsValid) {
                _context.Add(vm.Person);
                _context.Add(vm.Visitor);
                await _context.SaveChangesAsync();
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
        // GET: Account/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.People.FindAsync(id);
            if (person == null)
            {
                return NotFound();
            }
            return View(person);
        }

        // POST: Account/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PersonId,FirstName,LastName,Email")] Person person)
        {
            if (id != person.PersonId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(person);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PersonExists(person.PersonId))
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
            return View(person);
        }

        // GET: Account/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var person = await _context.People
                .FirstOrDefaultAsync(m => m.PersonId == id);
            if (person == null)
            {
                return NotFound();
            }

            return View(person);
        }

        // POST: Account/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var person = await _context.People.FindAsync(id);
            if (person != null)
            {
                _context.People.Remove(person);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PersonExists(int id)
        {
            return _context.People.Any(e => e.PersonId == id);
        }
    }
}
