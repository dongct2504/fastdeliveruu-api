using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FastDeliveruu.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    public partial class Ward
    {
        public Ward()
        {
            AddressesCustomers = new HashSet<AddressesCustomer>();
            Restaurants = new HashSet<Restaurant>();
            Orders = new HashSet<Order>();
        }

        [Key]
        public int Id { get; set; }
        public int DistrictId { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("DistrictId")]
        [InverseProperty("Wards")]
        public virtual District District { get; set; } = null!;
        [InverseProperty("Ward")]
        public virtual ICollection<AddressesCustomer> AddressesCustomers { get; set; }
        [InverseProperty("Ward")]
        public virtual ICollection<Restaurant> Restaurants { get; set; }
        [InverseProperty("Ward")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
