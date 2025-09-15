using System;
using System.Collections.Generic;

namespace Zoolirante.Models;

public partial class Animal
{
    public int AnimalId { get; set; }

    public string Name { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public int Age { get; set; }

    public string? Description { get; set; }

    public int SpeciesId { get; set; }

    public string? AnimalImage { get; set; }

    public string? AnimalImage2 { get; set; }

    public virtual ICollection<FavouriteAnimal> FavouriteAnimals { get; set; } = new List<FavouriteAnimal>();

    public virtual Species Species { get; set; } = null!;
}
