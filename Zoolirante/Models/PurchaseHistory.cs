using System;
using System.Collections.Generic;

namespace Zoolirante.Models;

public partial class PurchaseHistory
{
    public int PurchaseHistoryId { get; set; }

    public virtual Visitor PurchaseHistoryNavigation { get; set; } = null!;

    public virtual ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
}
