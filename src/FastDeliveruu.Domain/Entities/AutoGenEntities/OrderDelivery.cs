using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    [Index("OrderId", Name = "IX_OrderDeliveries_OrderId")]
    [Index("ShipperId", Name = "IX_OrderDeliveries_ShipperId")]
    public partial class OrderDelivery
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ShipperId { get; set; }
        public byte DeliveryStatus { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EstimatedDeliveryTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ActualDeliveryTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("OrderId")]
        [InverseProperty("OrderDeliveries")]
        public virtual Order Order { get; set; } = null!;
    }
}
