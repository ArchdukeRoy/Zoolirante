using System.ComponentModel.DataAnnotations;

namespace Zoolirante.ViewModels {
    public class FavouriteAnimalDataTransfer {
        [Key]
        public int FavAnimalsId { get; set; }
        [Required]
        public int VisitorId { get; set; }
        [Required]
        public int AnimalId { get; set; }
        [Required]
        public string AnimalName { get; set;}
    }
}
