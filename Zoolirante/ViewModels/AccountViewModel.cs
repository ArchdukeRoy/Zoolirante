using Zoolirante.Models;

namespace Zoolirante.ViewModels {
    public class AccountViewModel {
        public Person Person { get; set; } = new Person();
        public Visitor Visitor { get; set; } = new Visitor();
    }
}
