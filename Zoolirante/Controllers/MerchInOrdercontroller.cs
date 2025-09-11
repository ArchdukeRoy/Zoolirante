using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Zoolirante.Data;
using Zoolirante.Models;

namespace Zoolirante.Controllers
{
    public class MerchInOrdercontroller : Controller
    {

        private readonly ZooliranteContext _context;

        public MerchInOrdercontroller(ZooliranteContext context)
        {
            _context = context;
        }


    }
}
