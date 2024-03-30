using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities;

public partial class Restaurant
{
    [Key]
    public int RestaurantId { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    public string PhoneNumber { get; set; } = null!;

    public bool IsVerify { get; set; }

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

    [InverseProperty("Restaurant")]
    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}
