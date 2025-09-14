using System;
using System.Collections.Generic;

namespace Zoolirante.Models;

public partial class Visitor
{
    public int VisitorId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string? Contact { get; set; }

    public string? PaymentMethod { get; set; }

    public virtual ICollection<FavouriteAnimal> FavouriteAnimals { get; set; } = new List<FavouriteAnimal>();

    public virtual PurchaseHistory? PurchaseHistory { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual ICollection<VisitorMerchOrder> VisitorMerchOrders { get; set; } = new List<VisitorMerchOrder>();

    public virtual Person VisitorNavigation { get; set; } = null!;
}
