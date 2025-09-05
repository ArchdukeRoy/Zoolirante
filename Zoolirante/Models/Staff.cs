using System;
using System.Collections.Generic;

namespace Zoolirante.Models;

public partial class Staff
{
    public int StaffId { get; set; }

    public string? Mobile { get; set; }

    public string? Photo { get; set; }

    public string Password { get; set; } = null!;

    public int RoleId { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual Role Role { get; set; } = null!;

    public virtual Person StaffNavigation { get; set; } = null!;

    public virtual ZooKeeper? ZooKeeper { get; set; }
}
