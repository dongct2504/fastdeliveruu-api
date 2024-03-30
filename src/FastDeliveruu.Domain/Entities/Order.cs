using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities;

[Index("UserId", Name = "ORDERLOCALUSERS_FK")]
[Index("ShipperId", Name = "ORDERSHIPPERS_FK")]
public partial class Order
{
    [Key]
    public int OrderId { get; set; }

    public int? ShipperId { get; set; }

    public int? UserId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime OrderDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ShippingDate { get; set; }

    [StringLength(15)]
    [Unicode(false)]
    public string PhoneNumber { get; set; } = null!;

    [Column(TypeName = "money")]
    public decimal TotalAmount { get; set; }

    [StringLength(128)]
    [Unicode(false)]
    public string? TrackingNumber { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? OrderStatus { get; set; }

    [StringLength(20)]
    [Unicode(false)]
    public string? PaymentStatus { get; set; }

    [StringLength(256)]
    [Unicode(false)]
    public string? TransactionId { get; set; }

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

    [InverseProperty("Order")]
    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    [ForeignKey("ShipperId")]
    [InverseProperty("Orders")]
    public virtual Shipper? Shipper { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Orders")]
    public virtual LocalUser? User { get; set; }
}
