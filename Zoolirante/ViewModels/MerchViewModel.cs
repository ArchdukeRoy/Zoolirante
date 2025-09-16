using Zoolirante.Models;

namespace Zoolirante.ViewModels {
    public class MerchViewModel {
        public DefaultViewModel DefaultVM { get; set; } = new DefaultViewModel();
        public List<Merchandise> MerchList { get; set; } = new List<Merchandise>();

    }
}
