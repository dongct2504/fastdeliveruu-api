using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    [Index("CityId", Name = "IX_Restaurants_CityId")]
    [Index("DistrictId", Name = "IX_Restaurants_DistrictId")]
    [Index("Name", Name = "IX_Restaurants_Name")]
    [Index("WardId", Name = "IX_Restaurants_WardId")]
    public partial class Restaurant
    {
        public Restaurant()
        {
            MenuItems = new HashSet<MenuItem>();
            RestaurantHours = new HashSet<RestaurantHour>();
            RestaurantReviews = new HashSet<RestaurantReview>();
        }

        [Key]
        public Guid Id { get; set; }
        [StringLength(126)]
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        [StringLength(15)]
        [Unicode(false)]
        public string PhoneNumber { get; set; } = null!;
        public bool IsVerify { get; set; }
        [StringLength(256)]
        [Unicode(false)]
        public string ImageUrl { get; set; } = null!;
        [StringLength(256)]
        [Unicode(false)]
        public string PublicId { get; set; } = null!;
        [StringLength(50)]
        public string HouseNumber { get; set; } = null!;
        [StringLength(80)]
        public string StreetName { get; set; } = null!;
        public int CityId { get; set; }
        public int DistrictId { get; set; }
        public int WardId { get; set; }
        [Column(TypeName = "decimal(9, 6)")]
        public decimal Latitude { get; set; }
        [Column(TypeName = "decimal(9, 6)")]
        public decimal Longitude { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("CityId")]
        [InverseProperty("Restaurants")]
        public virtual City City { get; set; } = null!;
        [ForeignKey("DistrictId")]
        [InverseProperty("Restaurants")]
        public virtual District District { get; set; } = null!;
        [ForeignKey("WardId")]
        [InverseProperty("Restaurants")]
        public virtual Ward Ward { get; set; } = null!;
        [InverseProperty("Restaurant")]
        public virtual ICollection<MenuItem> MenuItems { get; set; }
        [InverseProperty("Restaurant")]
        public virtual ICollection<RestaurantHour> RestaurantHours { get; set; }
        [InverseProperty("Restaurant")]
        public virtual ICollection<RestaurantReview> RestaurantReviews { get; set; }
    }
}
