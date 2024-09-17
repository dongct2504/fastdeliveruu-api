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
            DeliveryAddresses = new HashSet<DeliveryAddress>();
            Restaurants = new HashSet<Restaurant>();
        }

        [Key]
        public int Id { get; set; }
        public int DistrictId { get; set; }
        [StringLength(50)]
        public string Name { get; set; } = null!;

        [ForeignKey("DistrictId")]
        [InverseProperty("Wards")]
        public virtual District District { get; set; } = null!;
        [InverseProperty("Ward")]
        public virtual ICollection<AddressesCustomer> AddressesCustomers { get; set; }
        [InverseProperty("Ward")]
        public virtual ICollection<DeliveryAddress> DeliveryAddresses { get; set; }
        [InverseProperty("Ward")]
        public virtual ICollection<Restaurant> Restaurants { get; set; }
    }
}
