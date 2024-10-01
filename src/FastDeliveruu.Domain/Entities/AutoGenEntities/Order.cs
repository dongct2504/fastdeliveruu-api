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
    [Index("CityId", Name = "IX_Orders_CityId")]
    [Index("DistrictId", Name = "IX_Orders_DistrictId")]
    [Index("WardId", Name = "IX_Orders_WardId")]
    public partial class Order
    {
        public Order()
        {
            OrderDeliveries = new HashSet<OrderDelivery>();
            OrderDetails = new HashSet<OrderDetail>();
            Payments = new HashSet<Payment>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid AppUserId { get; set; }
        public Guid ShipperId { get; set; }
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
        public byte? PaymentMethod { get; set; }
        [StringLength(20)]
        [Unicode(false)]
        public string? TransactionId { get; set; }
        [StringLength(60)]
        public string Address { get; set; } = null!;
        public int CityId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }
        [Column(TypeName = "decimal(9, 6)")]
        public decimal? Latitude { get; set; }
        [Column(TypeName = "decimal(9, 6)")]
        public decimal? Longitude { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("CityId")]
        [InverseProperty("Orders")]
        public virtual City City { get; set; } = null!;
        [ForeignKey("DistrictId")]
        [InverseProperty("Orders")]
        public virtual District District { get; set; } = null!;
        [ForeignKey("WardId")]
        [InverseProperty("Orders")]
        public virtual Ward Ward { get; set; } = null!;

        [ForeignKey("DeliveryMethodId")]
        [InverseProperty("Orders")]
        public virtual DeliveryMethod? DeliveryMethod { get; set; }

        [InverseProperty("Order")]
        public virtual ICollection<OrderDelivery> OrderDeliveries { get; set; }
        [InverseProperty("Order")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [InverseProperty("Order")]
        public virtual ICollection<Payment> Payments { get; set; }

        // identity
        [ForeignKey("AppUserId")]
        [InverseProperty("Orders")]
        public virtual AppUser AppUser { get; set; } = null!;

        [ForeignKey("ShipperId")]
        [InverseProperty("Orders")]
        public virtual Shipper Shipper { get; set; } = null!;
    }
}
