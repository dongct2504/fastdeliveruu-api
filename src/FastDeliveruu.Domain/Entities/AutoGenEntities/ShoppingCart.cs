using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    [Index("AppUserId", Name = "SHOPPINGCARTLOCALUSERS_FK")]
    [Index("MenuItemId", Name = "SHOPPINGCARTMENUITEMS_FK")]
    public partial class ShoppingCart
    {
        [Key]
        public Guid MenuItemId { get; set; }
        [Key]
        public Guid AppUserId { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("MenuItemId")]
        [InverseProperty("ShoppingCarts")]
        public virtual MenuItem MenuItem { get; set; } = null!;
    }
}
