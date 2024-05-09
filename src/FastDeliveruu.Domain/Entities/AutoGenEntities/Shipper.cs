using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    public partial class Shipper
    {
        public Shipper()
        {
            Orders = new HashSet<Order>();
        }

        [Key]
        public Guid ShipperId { get; set; }
        [StringLength(50)]
        public string FirstName { get; set; } = null!;
        [StringLength(50)]
        public string LastName { get; set; } = null!;
        [StringLength(128)]
        [Unicode(false)]
        public string UserName { get; set; } = null!;
        [StringLength(12)]
        [Unicode(false)]
        public string Cccd { get; set; } = null!;
        [StringLength(12)]
        [Unicode(false)]
        public string DriverLicense { get; set; } = null!;
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
        public DateTime DateOfBirth { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? LockoutEndDate { get; set; }
        public bool LockoutEnable { get; set; }
        [StringLength(1024)]
        [Unicode(false)]
        public string? ImageUrl { get; set; }
        [StringLength(126)]
        [Unicode(false)]
        public string? VehicleType { get; set; }
        [StringLength(128)]
        public string Address { get; set; } = null!;
        [StringLength(50)]
        public string Ward { get; set; } = null!;
        [StringLength(30)]
        public string District { get; set; } = null!;
        [StringLength(30)]
        public string City { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }

        [InverseProperty("Shipper")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
