using System;
using System.Collections.Generic;

namespace Zoolirante.Models;

public partial class ZooKeeper
{
    public int ZookeeperId { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<EventRollCall> EventRollCalls { get; set; } = new List<EventRollCall>();

    public virtual Staff Zookeeper { get; set; } = null!;
}
