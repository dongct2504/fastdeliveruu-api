using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FastDeliveruu.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    [Index("AppUserId", Name = "IX_WishLists_AppUserId")]
    [Index("MenuItemId", Name = "IX_WishLists_MenuItemId")]
    public partial class WishList
    {
        [Key]
        public Guid Id { get; set; }
        public Guid AppUserId { get; set; }
        public Guid MenuItemId { get; set; }
        public Guid? MenuVariantId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("MenuItemId")]
        [InverseProperty("WishLists")]
        public virtual MenuItem MenuItem { get; set; } = null!;

        [ForeignKey("AppUserId")]
        [InverseProperty("WishLists")]
        public virtual AppUser AppUser { get; set; } = null!;

        [ForeignKey("MenuVariantId")]
        [InverseProperty("WishLists")]
        public virtual MenuVariant? MenuVariant { get; set; }
    }
}
