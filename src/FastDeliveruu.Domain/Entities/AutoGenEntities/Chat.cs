using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using FastDeliveruu.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities.AutoGenEntities;

[Index("SenderId", Name = "IX_Chats_SenderId")]
[Index("RecipientId", Name = "IX_Chats_RecipientId")]
public partial class Chat
{
    [Key]
    public Guid Id { get; set; }
    public Guid SenderId { get; set; }
    [StringLength(256)]
    [Unicode(false)]
    public string SenderUserUserName { get; set; } = null!;
    public byte SenderType { get; set; }

    public Guid RecipientId { get; set; }
    [StringLength(256)]
    [Unicode(false)]
    public string RecipientUserName { get; set; } = null!;
    public byte RecipientType { get; set; }

    [StringLength(1024)]
    public string Content { get; set; } = null!;
    [Column(TypeName = "datetime")]
    public DateTime? DateSent { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime? DateRead { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("SenderId")]
    [InverseProperty("SentChats")]
    public virtual AppUser? SenderAppUser { get; set; }
    [ForeignKey("SenderId")]
    [InverseProperty("SentChats")]
    public virtual Shipper? SenderShipper { get; set; }

    [ForeignKey("RecipientId")]
    [InverseProperty("ReceivedChats")]
    public virtual AppUser? RecipientAppUser { get; set; }
    [ForeignKey("RecipientId")]
    [InverseProperty("ReceivedChats")]
    public virtual Shipper? RecipientShipper { get; set; }
}
