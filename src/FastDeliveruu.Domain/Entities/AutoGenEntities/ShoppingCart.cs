using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    [Index("LocalUserId", Name = "SHOPPINGCARTLOCALUSERS_FK")]
    [Index("MenuItemId", Name = "SHOPPINGCARTMENUITEMS_FK")]
    public partial class ShoppingCart
    {
        [Key]
        public int ShoppingCartId { get; set; }
        public int MenuItemId { get; set; }
        public int LocalUserId { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("LocalUserId")]
        [InverseProperty("ShoppingCarts")]
        public virtual LocalUser LocalUser { get; set; } = null!;
        [ForeignKey("MenuItemId")]
        [InverseProperty("ShoppingCarts")]
        public virtual MenuItem MenuItem { get; set; } = null!;
    }
}
