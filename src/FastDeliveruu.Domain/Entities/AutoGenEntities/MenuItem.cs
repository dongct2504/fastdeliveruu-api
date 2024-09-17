using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    [Index("GenreId", Name = "IX_MenuItems_GenreId")]
    [Index("RestaurantId", Name = "IX_MenuItems_RestaurantId")]
    public partial class MenuItem
    {
        public MenuItem()
        {
            MenuItemReviews = new HashSet<MenuItemReview>();
            MenuVariants = new HashSet<MenuVariant>();
            OrderDetails = new HashSet<OrderDetail>();
            WishLists = new HashSet<WishList>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid GenreId { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = null!;
        [StringLength(1200)]
        public string Description { get; set; } = null!;
        [Column(TypeName = "decimal(19, 4)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(3, 2)")]
        public decimal DiscountPercent { get; set; }
        [StringLength(256)]
        [Unicode(false)]
        public string ImageUrl { get; set; } = null!;
        [StringLength(256)]
        [Unicode(false)]
        public string PublicId { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("GenreId")]
        [InverseProperty("MenuItems")]
        public virtual Genre Genre { get; set; } = null!;
        [ForeignKey("RestaurantId")]
        [InverseProperty("MenuItems")]
        public virtual Restaurant Restaurant { get; set; } = null!;
        [InverseProperty("MenuItem")]
        public virtual ICollection<MenuItemReview> MenuItemReviews { get; set; }
        [InverseProperty("MenuItem")]
        public virtual ICollection<MenuVariant> MenuVariants { get; set; }
        [InverseProperty("MenuItem")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        [InverseProperty("MenuItem")]
        public virtual ICollection<WishList> WishLists { get; set; }
    }
}
