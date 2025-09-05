using System;
using System.Collections.Generic;

namespace Zoolirante.Models;

public partial class VisitorMerchOrder
{
    public int OrderNumber { get; set; }

    public DateTime OrderDate { get; set; }

    public DateTime? DatePaid { get; set; }

    public int VisitorId { get; set; }

    public virtual ICollection<MerchInOrder> MerchInOrders { get; set; } = new List<MerchInOrder>();

    public virtual Visitor Visitor { get; set; } = null!;
}
