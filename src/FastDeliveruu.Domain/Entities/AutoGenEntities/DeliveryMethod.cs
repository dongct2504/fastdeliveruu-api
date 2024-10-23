using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    public partial class DeliveryMethod
    {
        public DeliveryMethod()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(80)]
        public string ShortName { get; set; } = null!;
        [StringLength(20)]
        public string? EstimatedDeliveryTime { get; set; }
        [StringLength(64)]
        public string Description { get; set; } = null!;
        [Column(TypeName = "decimal(19, 4)")]
        public decimal Price { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [InverseProperty("DeliveryMethod")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
