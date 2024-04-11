using System.ComponentModel.DataAnnotations.Schema;

namespace FastDeliveruu.Domain.Entities;

public partial class MenuItem
{
    [NotMapped]
    public decimal DiscountAmount => Price * DiscountPercent;

    [NotMapped]
    public decimal DiscountPrice => Price - DiscountAmount;
}