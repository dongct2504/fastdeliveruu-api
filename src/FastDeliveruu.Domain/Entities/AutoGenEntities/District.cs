using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FastDeliveruu.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    public partial class District
    {
        public District()
        {
            AddressesCustomers = new HashSet<AddressesCustomer>();
            Restaurants = new HashSet<Restaurant>();
            Orders = new HashSet<Order>();
            Wards = new HashSet<Ward>();
        }

        [Key]
        public int Id { get; set; }
        public int CityId { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("CityId")]
        [InverseProperty("Districts")]
        public virtual City City { get; set; } = null!;
        [InverseProperty("District")]
        public virtual ICollection<AddressesCustomer> AddressesCustomers { get; set; }
        [InverseProperty("District")]
        public virtual ICollection<Restaurant> Restaurants { get; set; }
        [InverseProperty("District")]
        public virtual ICollection<Order> Orders { get; set; }
        [InverseProperty("District")]
        public virtual ICollection<Ward> Wards { get; set; }
    }
}
