using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FastDeliveruu.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    [Index("AppUserId", Name = "IX_Orders_AppUserId")]
    [Index("DeliveryMethodId", Name = "IX_Orders_DeliveryMethodId")]
    public partial class Order
    {
        public Order()
        {
            DeliveryAddresses = new HashSet<DeliveryAddress>();
            OrderDeliveries = new HashSet<OrderDelivery>();
            OrderDetails = new HashSet<OrderDetail>();
            Payments = new HashSet<Payment>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid AppUserId { get; set; }
        public int? DeliveryMethodId { get; set; }
        [StringLength(50)]
        public string FirstName { get; set; } = null!;
        [StringLength(50)]
        public string LastName { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime? OrderDate { get; set; }
        [StringLength(256)]
        public string? OrderDescription { get; set; }
        [StringLength(15)]
        [Unicode(false)]
        public string PhoneNumber { get; set; } = null!;
        [Column(TypeName = "decimal(19, 4)")]
        public decimal TotalAmount { get; set; }
        [StringLength(128)]
        [Unicode(false)]
        public string? TrackingNumber { get; set; }
        public byte? OrderStatus { get; set; }
        public byte? PaymentStatus { get; set; }
        public byte? PaymentMethod { get; set; }
        [StringLength(20)]
        [Unicode(false)]
        public string? TransactionId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("DeliveryMethodId")]
        [InverseProperty("Orders")]
        public virtual DeliveryMethod? DeliveryMethod { get; set; }

        [InverseProperty("Order")]
        public virtual ICollection<DeliveryAddress> DeliveryAddresses { get; set; }
        [InverseProperty("Order")]
        public virtual ICollection<OrderDelivery> OrderDeliveries { get; set; }
        [InverseProperty("Order")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [InverseProperty("Order")]
        public virtual ICollection<Payment> Payments { get; set; }

        [ForeignKey("AppUserId")]
        [InverseProperty("Orders")]
        public virtual AppUser AppUser { get; set; } = null!;
    }
}
