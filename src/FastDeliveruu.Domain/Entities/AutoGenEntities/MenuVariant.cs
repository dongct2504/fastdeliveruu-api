﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    [Index("MenuItemId", Name = "IX_MenuVariants_MenuItemId")]
    public partial class MenuVariant
    {
        public MenuVariant()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        [Key]
        public Guid Id { get; set; }
        public Guid MenuItemId { get; set; }
        [StringLength(20)]
        public string VarietyName { get; set; } = null!;
        [Column(TypeName = "decimal(19, 4)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(3, 2)")]
        public decimal DiscountPercent { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("MenuItemId")]
        [InverseProperty("MenuVariants")]
        public virtual MenuItem MenuItem { get; set; } = null!;
        [InverseProperty("MenuVariant")]
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
