using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities;

public partial class LocalUser
{
    [Key]
    public int UserId { get; set; }

    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [StringLength(128)]
    [Unicode(false)]
    public string UserName { get; set; } = null!;

    [StringLength(128)]
    [Unicode(false)]
    public string Email { get; set; } = null!;

    [Unicode(false)]
    public string PasswordHash { get; set; } = null!;

    [StringLength(15)]
    [Unicode(false)]
    public string PhoneNumber { get; set; } = null!;

    [StringLength(20)]
    [Unicode(false)]
    public string Role { get; set; } = null!;

    [StringLength(128)]
    public string? Address { get; set; }

    [StringLength(50)]
    public string? Ward { get; set; }

    [StringLength(30)]
    public string? District { get; set; }

    [StringLength(30)]
    public string? City { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    [InverseProperty("User")]
    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
}
