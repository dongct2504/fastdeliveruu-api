using FastDeliveruu.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastDeliveruu.Domain.Entities.AutoGenEntities;

[Index("SenderAppUserId", Name = "IX_MessageThreads_SenderAppUserId")]
[Index("SenderShipperId", Name = "IX_MessageThreads_SenderShipperId")]
[Index("RecipientAppUserId", Name = "IX_MessageThreads_RecipientAppUserId")]
[Index("RecipientShipperId", Name = "IX_MessageThreads_RecipientShipperId")]
public partial class MessageThread
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(256)]
    public string Title { get; set; } = null!;

    public Guid? SenderAppUserId { get; set; }
    public Guid? SenderShipperId { get; set; }

    public Guid? RecipientAppUserId { get; set; }
    public Guid? RecipientShipperId { get; set; }

    public byte RecipientType { get; set; }
    public byte SenderType { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("SenderAppUserId")]
    [InverseProperty("SenderMessageThreads")]
    public virtual AppUser? SenderAppUser { get; set; }
    [ForeignKey("SenderShipperId")]
    [InverseProperty("SenderMessageThreads")]
    public virtual Shipper? SenderShipper { get; set; }

    [ForeignKey("RecipientAppUserId")]
    [InverseProperty("RecipientMessageThreads")]
    public virtual AppUser? RecipientAppUser { get; set; }
    [ForeignKey("RecipientShipperId")]
    [InverseProperty("RecipientMessageThreads")]
    public virtual Shipper? RecipientShipper { get; set; }

    [InverseProperty("MessageThread")]
    public virtual ICollection<Chat> Chats { get; set; } = new List<Chat>();
}
