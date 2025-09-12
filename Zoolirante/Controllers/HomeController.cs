using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zoolirante.Data;
using Zoolirante.Models;
using Zoolirante.ViewModels;
using System.Text.Json;

namespace Zoolirante.Controllers
{
    public class HomeController : Controller
    {
        private readonly ZooliranteContext _context;

        public HomeController(ZooliranteContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {

            var vmJson = HttpContext.Session.GetString("DefaultVM");
            if (!string.IsNullOrEmpty(vmJson)) {
                var vm = JsonSerializer.Deserialize<DefaultViewModel>(vmJson);
                return View(vm);
            }

            return View(new DefaultViewModel()) ;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
