using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Zoolirante.Models;

public partial class FavouriteAnimal
{
    [Key]
    public int FavAnimalsId { get; set; }

    [Required]
    public int VisitorId { get; set; }

    [Required]
    public int AnimalId { get; set; }

    public virtual Animal Animal { get; set; } = null!;

    public virtual Visitor Visitor { get; set; } = null!;
}
