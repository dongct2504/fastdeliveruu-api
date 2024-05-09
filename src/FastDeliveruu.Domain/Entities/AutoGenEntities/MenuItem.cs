using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    [Index("GenreId", Name = "MENUITEMGENRES_FK")]
    [Index("RestaurantId", Name = "MENUITEMRESTAURANTS_FK")]
    public partial class MenuItem
    {
        public MenuItem()
        {
            OrderDetails = new HashSet<OrderDetail>();
            ShoppingCarts = new HashSet<ShoppingCart>();
        }

        [Key]
        public Guid MenuItemId { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid GenreId { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Inventory { get; set; }
        [Column(TypeName = "money")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(3, 2)")]
        public decimal DiscountPercent { get; set; }
        [StringLength(1024)]
        [Unicode(false)]
        public string? ImageUrl { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }

        [ForeignKey("GenreId")]
        [InverseProperty("MenuItems")]
        public virtual Genre Genre { get; set; } = null!;
        [ForeignKey("RestaurantId")]
        [InverseProperty("MenuItems")]
        public virtual Restaurant Restaurant { get; set; } = null!;
        [InverseProperty("MenuItem")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [InverseProperty("MenuItem")]
        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; }
    }
}
