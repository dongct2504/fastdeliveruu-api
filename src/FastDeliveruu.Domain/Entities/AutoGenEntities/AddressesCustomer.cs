using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FastDeliveruu.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    [Index("AppUserId", Name = "IX_AddressCustomers_AppUserId")]
    [Index("CityId", Name = "IX_AddressCustomers_CityId")]
    [Index("DistrictId", Name = "IX_AddressCustomers_DistrictId")]
    [Index("WardId", Name = "IX_AddressCustomers_WardId")]
    [Index("CityId", Name = "IX_DeliveryAddresses_CityId")]
    [Index("DistrictId", Name = "IX_DeliveryAddresses_DistrictId")]
    [Index("WardId", Name = "IX_DeliveryAddresses_WardId")]
    public partial class AddressesCustomer
    {
        [Key]
        public Guid Id { get; set; }
        public Guid AppUserId { get; set; }
        [StringLength(60)]
        public string Address { get; set; } = null!;
        public int CityId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }
        [Column(TypeName = "decimal(9, 6)")]
        public decimal? Latitude { get; set; }
        [Column(TypeName = "decimal(9, 6)")]
        public decimal? Longitude { get; set; }
        public bool IsPrimary { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("CityId")]
        [InverseProperty("AddressesCustomers")]
        public virtual City City { get; set; } = null!;
        [ForeignKey("DistrictId")]
        [InverseProperty("AddressesCustomers")]
        public virtual District District { get; set; } = null!;
        [ForeignKey("WardId")]
        [InverseProperty("AddressesCustomers")]
        public virtual Ward Ward { get; set; } = null!;

        [ForeignKey("AppUserId")]
        [InverseProperty("AddressesCustomers")]
        public virtual AppUser AppUser { get; set; } = null!;
    }
}
