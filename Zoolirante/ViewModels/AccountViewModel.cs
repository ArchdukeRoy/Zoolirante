using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Zoolirante.Models;

namespace Zoolirante.ViewModels {
    public class AccountViewModel {
        public Person Person { get; set; } = new Person();
        public Visitor Visitor { get; set; } = new Visitor();

        [ValidateNever]
        public DefaultViewModel DefaultVM { get; set; } = new DefaultViewModel();
    }
}
