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
            DeliveryAddresses = new HashSet<DeliveryAddress>();
            Districts = new HashSet<District>();
            Restaurants = new HashSet<Restaurant>();
        }

        [Key]
        public int Id { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = null!;

        [InverseProperty("City")]
        public virtual ICollection<AddressesCustomer> AddressesCustomers { get; set; }
        [InverseProperty("City")]
        public virtual ICollection<DeliveryAddress> DeliveryAddresses { get; set; }
        [InverseProperty("City")]
        public virtual ICollection<District> Districts { get; set; }
        [InverseProperty("City")]
        public virtual ICollection<Restaurant> Restaurants { get; set; }
    }
}
