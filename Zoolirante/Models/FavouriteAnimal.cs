using System;
using System.Collections.Generic;

namespace Zoolirante.Models;

public partial class FavouriteAnimal
{
    public int FavAnimalsId { get; set; }

    public int VisitorId { get; set; }

    public int AnimalId { get; set; }

    public virtual Animal Animal { get; set; } = null!;

    public virtual Visitor Visitor { get; set; } = null!;
}
