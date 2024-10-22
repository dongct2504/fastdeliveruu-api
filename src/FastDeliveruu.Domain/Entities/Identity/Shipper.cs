using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using FastDeliveruu.Domain.Entities.AutoGenEntities;

namespace FastDeliveruu.Domain.Entities.Identity;

[Index("CityId", Name = "IX_Shippers_CityId")]
[Index("DistrictId", Name = "IX_Shippers_DistrictId")]
[Index("WardId", Name = "IX_Shippers_WardId")]
public class Shipper : IdentityUser<Guid>
{
    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [StringLength(50)]
    public string LastName { get; set; } = null!;

    public bool IsAvailable { get; set; } = true;

    [StringLength(12)]
    [Unicode(false)]
    public string CitizenIdentification { get; set; } = null!;

    [StringLength(255)]
    [Unicode(false)]
    public string? ImageUrl { get; set; }

    [StringLength(256)]
    [Unicode(false)]
    public string? PublicId { get; set; }

    [StringLength(120)]
    public string? ModelType { get; set; }

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

    [InverseProperty("Shipper")]
    public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();

    [InverseProperty("Shipper")]
    public virtual ICollection<ShipperNotification> ShipperNotifications { get; set; } = new HashSet<ShipperNotification>();

    [InverseProperty("SenderShipper")]
    public ICollection<MessageThread> SenderMessageThreads { get; set; } = new HashSet<MessageThread>();

    [InverseProperty("RecipientShipper")]
    public ICollection<MessageThread> RecipientMessageThreads { get; set; } = new HashSet<MessageThread>();

    [ForeignKey("CityId")]
    [InverseProperty("Shippers")]
    public virtual City City { get; set; } = null!;
    [ForeignKey("DistrictId")]
    [InverseProperty("Shippers")]
    public virtual District District { get; set; } = null!;
    [ForeignKey("WardId")]
    [InverseProperty("Shippers")]
    public virtual Ward Ward { get; set; } = null!;
}
