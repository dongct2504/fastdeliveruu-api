using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    public partial class LocalUser
    {
        public LocalUser()
        {
            Orders = new HashSet<Order>();
            ShoppingCarts = new HashSet<ShoppingCart>();
        }

        [Key]
        public Guid LocalUserId { get; set; }
        [StringLength(50)]
        public string? FirstName { get; set; }
        [StringLength(50)]
        public string? LastName { get; set; }
        [StringLength(128)]
        [Unicode(false)]
        public string UserName { get; set; } = null!;
        [StringLength(128)]
        [Unicode(false)]
        public string Email { get; set; } = null!;
        public bool IsConfirmEmail { get; set; }
        [Unicode(false)]
        public string PasswordHash { get; set; } = null!;
        [StringLength(15)]
        [Unicode(false)]
        public string PhoneNumber { get; set; } = null!;
        public bool IsConfirmPhoneNumber { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? DateOfBirth { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LockoutEndDate { get; set; }
        public bool LockoutEnable { get; set; }
        [StringLength(20)]
        [Unicode(false)]
        public string Role { get; set; } = null!;
        [StringLength(1024)]
        [Unicode(false)]
        public string? ImageUrl { get; set; }
        [StringLength(128)]
        public string? Address { get; set; }
        [StringLength(50)]
        public string? Ward { get; set; }
        [StringLength(30)]
        public string? District { get; set; }
        [StringLength(30)]
        public string? City { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }

        [InverseProperty("LocalUser")]
        public virtual ICollection<Order> Orders { get; set; }
        [InverseProperty("LocalUser")]
        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; }
    }
}
