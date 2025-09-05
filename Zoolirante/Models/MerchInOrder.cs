using System;
using System.Collections.Generic;

namespace Zoolirante.Models;

public partial class MerchInOrder
{
    public int OrderNumber { get; set; }

    public int ItemId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public virtual Merchandise Item { get; set; } = null!;

    public virtual VisitorMerchOrder OrderNumberNavigation { get; set; } = null!;
}
