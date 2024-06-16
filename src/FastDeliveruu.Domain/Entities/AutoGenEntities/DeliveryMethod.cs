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
        public Guid DeliveryMethodId { get; set; }
        [StringLength(20)]
        [Unicode(false)]
        public string ShortName { get; set; } = null!;
        [StringLength(10)]
        [Unicode(false)]
        public string? DeliveryTime { get; set; }
        [StringLength(64)]
        public string Description { get; set; } = null!;
        [Column(TypeName = "money")]
        public decimal Price { get; set; }

        [InverseProperty("DeliveryMethod")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
