using System;
using System.Collections.Generic;

namespace Zoolirante.Models;

public partial class Ticket
{
    public int TicketId { get; set; }

    public DateOnly DateOfEntry { get; set; }

    public DateTime? DatePaid { get; set; }

    public int VisitorId { get; set; }

    public virtual Visitor Visitor { get; set; } = null!;
}
