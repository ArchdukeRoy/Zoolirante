using System;
using System.Collections.Generic;

namespace Zoolirante.Models;

public partial class EventRollCall
{
    public int EventRollCallId { get; set; }

    public DateOnly? RollDate { get; set; }

    public TimeOnly RollTime { get; set; }

    public int? ZookeeperId { get; set; }

    public int EventId { get; set; }

    public virtual Event Event { get; set; } = null!;

    public virtual ZooKeeper? Zookeeper { get; set; }
}
