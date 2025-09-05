using System;
using System.Collections.Generic;

namespace Zoolirante.Models;

public partial class Event
{
    public int EventId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<EventRollCall> EventRollCalls { get; set; } = new List<EventRollCall>();
}
