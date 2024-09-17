using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FastDeliveruu.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    [Index("CouponCode", Name = "UQ__Coupons__D349080072B75AA0", IsUnique = true)]
    public partial class Coupon
    {
        [Key]
        public Guid Id { get; set; }
        public Guid AppUserId { get; set; }
        [StringLength(20)]
        public string CouponCode { get; set; } = null!;
        public byte? DiscountType { get; set; }
        [Column(TypeName = "decimal(5, 2)")]
        public decimal DiscountPercent { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? StartDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ExpiryDate { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("AppUserId")]
        [InverseProperty("Coupons")]
        public virtual AppUser AppUser { get; set; } = null!;
    }
}
