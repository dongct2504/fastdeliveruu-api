using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities.AutoGenEntities;

[Index("ThreadId", Name = "IX_Chats_SenderId")]
public partial class Chat
{
    [Key]
    public Guid Id { get; set; }
    public Guid ThreadId { get; set; }

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

    [ForeignKey("ThreadId")]
    [InverseProperty("Chats")]
    public virtual MessageThread MessageThread { get; set; } = null!;
}
