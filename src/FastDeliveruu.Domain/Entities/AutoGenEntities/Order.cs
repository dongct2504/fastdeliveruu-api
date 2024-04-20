using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    [Index("LocalUserId", Name = "ORDERLOCALUSERS_FK")]
    [Index("ShipperId", Name = "ORDERSHIPPERS_FK")]
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        [Key]
        public Guid OrderId { get; set; }
        public Guid ShipperId { get; set; }
        public Guid LocalUserId { get; set; }
        [StringLength(128)]
        public string ReceiverName { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime OrderDate { get; set; }
        [StringLength(256)]
        [Unicode(false)]
        public string OrderDescription { get; set; } = null!;
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
        [StringLength(25)]
        [Unicode(false)]
        public string? PaymentMethod { get; set; }
        [StringLength(256)]
        [Unicode(false)]
        public string? TransactionId { get; set; }
        [StringLength(128)]
        public string Address { get; set; } = null!;
        [StringLength(50)]
        public string Ward { get; set; } = null!;
        [StringLength(30)]
        public string District { get; set; } = null!;
        [StringLength(30)]
        public string City { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("LocalUserId")]
        [InverseProperty("Orders")]
        public virtual LocalUser LocalUser { get; set; } = null!;
        [ForeignKey("ShipperId")]
        [InverseProperty("Orders")]
        public virtual Shipper Shipper { get; set; } = null!;
        [InverseProperty("Order")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
