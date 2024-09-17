using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FastDeliveruu.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    [Index("AppUserId", Name = "IX_Notifications_AppUserId")]
    public partial class Notification
    {
        [Key]
        public Guid Id { get; set; }
        public Guid AppUserId { get; set; }
        [StringLength(500)]
        public string? Message { get; set; }
        public bool IsRead { get; set; }
        public byte NotificationType { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("AppUserId")]
        [InverseProperty("Notifications")]
        public virtual AppUser AppUser { get; set; } = null!;
    }
}
