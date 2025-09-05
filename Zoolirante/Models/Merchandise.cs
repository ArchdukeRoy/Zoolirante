using System;
using System.Collections.Generic;

namespace Zoolirante.Models;

public partial class Merchandise
{
    public int ItemId { get; set; }

    public string ItemName { get; set; } = null!;

    public string? ItemDescription { get; set; }

    public decimal ItemCost { get; set; }

    public string? ItemImage { get; set; }

    public virtual ICollection<MerchInOrder> MerchInOrders { get; set; } = new List<MerchInOrder>();
}
