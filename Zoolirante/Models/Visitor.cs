using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Zoolirante.Models;

public partial class Visitor
{
    [Key]
    public int VisitorId { get; set; }

    [Required(ErrorMessage = "Username is required")]
    [MinLength(4, ErrorMessage = "Username must be at least 4 characters")]
    [MaxLength(20, ErrorMessage = "Username exceeds max length of 20")]
    public string Username { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    public string PasswordHash { get; set; } = null!;

    [Required(ErrorMessage = "Phone number is required")]
    [Phone(ErrorMessage = "Invalid phone number")]
    public string? Contact { get; set; }

    public string? PaymentMethod { get; set; }

    public virtual ICollection<FavouriteAnimal> FavouriteAnimals { get; set; } = new List<FavouriteAnimal>();

    public virtual PurchaseHistory? PurchaseHistory { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual ICollection<VisitorMerchOrder> VisitorMerchOrders { get; set; } = new List<VisitorMerchOrder>();

    [ValidateNever]
    public virtual Person VisitorNavigation { get; set; } = null!;
}
