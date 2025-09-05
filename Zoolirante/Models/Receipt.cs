using System;
using System.Collections.Generic;

namespace Zoolirante.Models;

public partial class Receipt
{
    public int ReceiptId { get; set; }

    public string? ReceiptDetails { get; set; }

    public DateTime DateTime { get; set; }

    public int PurchaseHistoryId { get; set; }

    public virtual PurchaseHistory PurchaseHistory { get; set; } = null!;
}
