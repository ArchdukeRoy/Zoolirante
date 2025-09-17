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
    public class MemberController : Controller
    {
        private readonly ZooliranteContext _context;

        public MemberController(ZooliranteContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
