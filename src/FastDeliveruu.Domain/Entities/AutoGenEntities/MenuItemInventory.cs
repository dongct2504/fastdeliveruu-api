using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities.AutoGenEntities;

[Index("MenuItemId", Name = "IX_MenuItemInventory_MenuItemId")]
public partial class MenuItemInventory
{
    [Key]
    public Guid Id { get; set; }
    public Guid MenuItemId { get; set; }
    public int QuantityAvailable { get; set; }
    public int QuantityReserved { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("MenuItemId")]
    [InverseProperty("MenuItemInventories")]
    public virtual MenuItem MenuItem { get; set; } = null!;
}
