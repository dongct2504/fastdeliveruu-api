using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using FastDeliveruu.Domain.Entities.AutoGenEntities;

namespace FastDeliveruu.Domain.Entities.Identity;

public class Shipper : IdentityUser<Guid>
{
    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [StringLength(50)]
    public string LastName { get; set; } = null!;

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

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Shipper")]
    public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();

    [InverseProperty("Shipper")]
    public virtual ICollection<ShipperNotification> ShipperNotifications { get; set; } = new HashSet<ShipperNotification>();

    [InverseProperty("SenderShipper")]
    public ICollection<Chat> SentChats { get; set; } = new HashSet<Chat>();

    [InverseProperty("RecipientShipper")]
    public ICollection<Chat> ReceivedChats { get; set; } = new HashSet<Chat>();
}
