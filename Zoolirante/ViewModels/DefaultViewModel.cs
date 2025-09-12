using Microsoft.AspNetCore.Mvc.Rendering;
using Zoolirante.Models;

namespace Zoolirante.ViewModels {
    public class DefaultViewModel {
        public int id { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string username { get; set; }
        public Boolean admin { get; set; }
    }
}
