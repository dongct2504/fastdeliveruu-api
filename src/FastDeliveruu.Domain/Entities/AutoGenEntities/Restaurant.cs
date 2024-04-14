using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    public partial class Restaurant
    {
        public Restaurant()
        {
            MenuItems = new HashSet<MenuItem>();
        }

        [Key]
        public Guid RestaurantId { get; set; }
        [StringLength(126)]
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        [StringLength(15)]
        [Unicode(false)]
        public string PhoneNumber { get; set; } = null!;
        public bool IsVerify { get; set; }
        [StringLength(1024)]
        [Unicode(false)]
        public string? ImageUrl { get; set; }
        [StringLength(128)]
        public string Address { get; set; } = null!;
        [StringLength(50)]
        public string Ward { get; set; } = null!;
        [StringLength(30)]
        public string District { get; set; } = null!;
        [StringLength(30)]
        public string City { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }

        [InverseProperty("Restaurant")]
        public virtual ICollection<MenuItem> MenuItems { get; set; }
    }
}
