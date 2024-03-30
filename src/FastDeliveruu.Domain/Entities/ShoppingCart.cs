using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities;

[Index("UserId", Name = "SHOPPINGCARTLOCALUSERS_FK")]
[Index("MenuItemId", Name = "SHOPPINGCARTMENUITEMS_FK")]
public partial class ShoppingCart
{
    [Key]
    public int ShoppingCartId { get; set; }

    public int? MenuItemId { get; set; }

    public int? UserId { get; set; }

    public int Quantity { get; set; }

    [ForeignKey("MenuItemId")]
    [InverseProperty("ShoppingCarts")]
    public virtual MenuItem? MenuItem { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("ShoppingCarts")]
    public virtual LocalUser? User { get; set; }
}
