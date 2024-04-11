using System.ComponentModel.DataAnnotations.Schema;

namespace FastDeliveruu.Domain.Entities;

public partial class ShoppingCart
{
    [NotMapped]
    public decimal Price { get; set; }
}