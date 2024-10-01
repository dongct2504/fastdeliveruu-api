using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FastDeliveruu.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    [Index("ShipperId", Name = "IX_ShipperNotifications_ShipperId")]
    public partial class ShipperNotification
    {
        [Key]
        public Guid Id { get; set; }
        public Guid ShipperId { get; set; }
        [StringLength(500)]
        public string? Message { get; set; }
        public bool IsRead { get; set; }
        public byte NotificationType { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("ShipperId")]
        [InverseProperty("ShipperNotifications")]
        public virtual Shipper Shipper { get; set; } = null!;
    }
}
