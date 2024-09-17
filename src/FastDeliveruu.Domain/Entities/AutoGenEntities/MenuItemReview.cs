using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FastDeliveruu.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    [Index("AppUserId", Name = "IX_MenuItemReviews_AppUserId")]
    [Index("MenuItemId", Name = "IX_MenuItemReviews_MenuItemId")]
    public partial class MenuItemReview
    {
        [Key]
        public Guid Id { get; set; }
        public Guid MenuItemId { get; set; }
        public Guid AppUserId { get; set; }
        public byte Rating { get; set; }
        [StringLength(1500)]
        public string? ReviewText { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("MenuItemId")]
        [InverseProperty("MenuItemReviews")]
        public virtual MenuItem MenuItem { get; set; } = null!;

        [ForeignKey("AppUserId")]
        [InverseProperty("MenuItemReviews")]
        public virtual AppUser AppUser { get; set; } = null!;
    }
}
