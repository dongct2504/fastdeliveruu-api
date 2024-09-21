using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FastDeliveruu.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    [Index("AppUserId", Name = "IX_ShoppingCart_AppUserId")]
    [Index("MenuItemId", Name = "IX_ShoppingCart_MenuItemId")]
    [Index("MenuVariantId", Name = "IX_ShoppingCart_MenuVariantId")]
    public partial class ShoppingCart
    {
        [Key]
        public Guid Id { get; set; }
        public Guid MenuItemId { get; set; }
        public Guid AppUserId { get; set; }
        public Guid? MenuVariantId { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("AppUserId")]
        [InverseProperty("ShoppingCarts")]
        public virtual AppUser AppUser { get; set; } = null!;

        [ForeignKey("MenuItemId")]
        [InverseProperty("ShoppingCarts")]
        public virtual MenuItem MenuItem { get; set; } = null!;

        [ForeignKey("MenuVariantId")]
        [InverseProperty("ShoppingCarts")]
        public virtual MenuVariant? MenuVariant { get; set; }
    }
}
