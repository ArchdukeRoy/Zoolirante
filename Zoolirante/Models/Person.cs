using System;
using System.Collections.Generic;

namespace Zoolirante.Models;

public partial class Person
{
    public int PersonId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public virtual Staff? Staff { get; set; }

    public virtual Visitor? Visitor { get; set; }
}
