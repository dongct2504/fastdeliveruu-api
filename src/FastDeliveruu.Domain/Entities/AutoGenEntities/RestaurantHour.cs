using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    [Index("RestaurantId", Name = "IX_RestaurantHours_RestaurantId")]
    public partial class RestaurantHour
    {
        [Key]
        public Guid Id { get; set; }
        public Guid RestaurantId { get; set; }
        [StringLength(30)]
        public string? WeekenDay { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? EndTime { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("RestaurantId")]
        [InverseProperty("RestaurantHours")]
        public virtual Restaurant Restaurant { get; set; } = null!;
    }
}
