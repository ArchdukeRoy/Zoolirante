using Zoolirante.Models;

namespace Zoolirante.ViewModels {
    public class AnimalListViewModel {
        public DefaultViewModel DefaultVM { get; set; } = new DefaultViewModel();
        public List<Species> SpeciesList { get; set; } = new List<Species>();
    }
}
