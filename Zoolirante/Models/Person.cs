using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Zoolirante.Models;

public partial class Person
{
    public int PersonId { get; set; }

    [Required(ErrorMessage = "First Name is required")]
    [MinLength(1, ErrorMessage = "First Name must be at least 1 character")]
    [MaxLength(30, ErrorMessage = "First Name exceeds max length of 30")]
    public string FirstName { get; set; } = null!;

    [Required(ErrorMessage = "Last Name is required")]
    [MinLength(1, ErrorMessage = "Last Name must be at least 1 character")]
    [MaxLength(30, ErrorMessage = "Last Name exceeds max length of 30")]
    public string LastName { get; set; } = null!;

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = null!;

    public virtual Staff? Staff { get; set; }

    public virtual Visitor? Visitor { get; set; }
}
