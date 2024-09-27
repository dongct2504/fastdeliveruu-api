using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FastDeliveruu.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    public partial class City
    {
        public City()
        {
            AddressesCustomers = new HashSet<AddressesCustomer>();
            Districts = new HashSet<District>();
            Restaurants = new HashSet<Restaurant>();
            Orders = new HashSet<Order>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [InverseProperty("City")]
        public virtual ICollection<AddressesCustomer> AddressesCustomers { get; set; }
        [InverseProperty("City")]
        public virtual ICollection<District> Districts { get; set; }
        [InverseProperty("City")]
        public virtual ICollection<Restaurant> Restaurants { get; set; }
        [InverseProperty("City")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
