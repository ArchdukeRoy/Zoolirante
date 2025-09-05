using System;
using System.Collections.Generic;

namespace Zoolirante.Models;

public partial class Species
{
    public int SpeciesId { get; set; }

    public string Name { get; set; } = null!;

    public string? SpeciesImage { get; set; }

    public virtual ICollection<Animal> Animals { get; set; } = new List<Animal>();
}
