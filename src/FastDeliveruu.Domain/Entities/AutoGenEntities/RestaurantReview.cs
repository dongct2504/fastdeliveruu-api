using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FastDeliveruu.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    [Index("AppUserId", Name = "IX_RestaurantsReviews_AppUserId")]
    [Index("RestaurantId", Name = "IX_RestaurantsReviews_RestaurantId")]
    public partial class RestaurantReview
    {
        [Key]
        public Guid Id { get; set; }
        public Guid RestaurantId { get; set; }
        public Guid AppUserId { get; set; }
        public byte Rating { get; set; }
        [StringLength(1500)]
        public string? ReviewText { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("RestaurantId")]
        [InverseProperty("RestaurantReviews")]
        public virtual Restaurant Restaurant { get; set; } = null!;

        [ForeignKey("AppUserId")]
        [InverseProperty("RestaurantReviews")]
        public virtual AppUser AppUser { get; set; } = null!;
    }
}
