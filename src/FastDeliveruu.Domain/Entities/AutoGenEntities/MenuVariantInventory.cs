using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities.AutoGenEntities;

[Index("MenuVariantId", Name = "IX_MenuVariantInventory_MenuVariantId")]
public class MenuVariantInventory
{
    [Key]
    public Guid Id { get; set; }
    public Guid MenuVariantId { get; set; }
    public int QuantityAvailable { get; set; }
    public int QuantityReserved { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("MenuVariantId")]
    [InverseProperty("MenuVariantInventories")]
    public virtual MenuVariant MenuVariant { get; set; } = null!;
}
