using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    [Index("OrderId", Name = "IX_DeliveryAddresses_OrderId")]
    public partial class DeliveryAddress
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        [StringLength(60)]
        public string Address { get; set; } = null!;
        public int CityId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }
        [Column(TypeName = "decimal(9, 6)")]
        public decimal? Latitude { get; set; }
        [Column(TypeName = "decimal(9, 6)")]
        public decimal? Longitude { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("CityId")]
        [InverseProperty("DeliveryAddresses")]
        public virtual City City { get; set; } = null!;
        [ForeignKey("DistrictId")]
        [InverseProperty("DeliveryAddresses")]
        public virtual District District { get; set; } = null!;
        [ForeignKey("OrderId")]
        [InverseProperty("DeliveryAddresses")]
        public virtual Order Order { get; set; } = null!;
        [ForeignKey("WardId")]
        [InverseProperty("DeliveryAddresses")]
        public virtual Ward Ward { get; set; } = null!;
    }
}
