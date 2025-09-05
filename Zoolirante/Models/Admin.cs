using System;
using System.Collections.Generic;

namespace Zoolirante.Models;

public partial class Admin
{
    public int AdminId { get; set; }

    public virtual Staff AdminNavigation { get; set; } = null!;
}
