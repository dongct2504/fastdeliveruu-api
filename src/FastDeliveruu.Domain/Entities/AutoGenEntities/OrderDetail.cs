using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    [Index("MenuItemId", Name = "IX_OrderDetails_MenuItemId")]
    [Index("OrderId", Name = "IX_OrderDetails_OrderId")]
    public partial class OrderDetail
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid MenuItemId { get; set; }
        public Guid? MenuVariantId { get; set; }
        [Column(TypeName = "decimal(19, 4)")]
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("MenuItemId")]
        [InverseProperty("OrderDetails")]
        public virtual MenuItem MenuItem { get; set; } = null!;
        [ForeignKey("MenuVariantId")]
        [InverseProperty("OrderDetails")]
        public virtual MenuVariant? MenuVariant { get; set; }
        [ForeignKey("OrderId")]
        [InverseProperty("OrderDetails")]
        public virtual Order Order { get; set; } = null!;
    }
}
