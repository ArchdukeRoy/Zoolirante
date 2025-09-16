using System;
using System.Collections.Generic;

namespace Zoolirante.Models;

public partial class Species
{
    public int SpeciesId { get; set; }

    public string Name { get; set; } = null!;

    public string? SpeciesImage { get; set; }

    public string? SpeciesDescription { get; set; }

    public string? Habitat { get; set; }

    public string? Diet { get; set; }

    public string? SpeciesImage2 { get; set; }

    public int? EventId { get; set; }

    public virtual ICollection<Animal> Animals { get; set; } = new List<Animal>();

    public virtual Event? Event { get; set; }
}
