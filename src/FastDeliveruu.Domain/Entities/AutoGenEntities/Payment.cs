using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    public partial class Payment
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        [Column(TypeName = "decimal(19, 4)")]
        public decimal Amount { get; set; }
        public byte? PaymentStatus { get; set; }
        public byte? PaymentMethod { get; set; }
        [StringLength(50)]
        public string? TransactionId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("OrderId")]
        [InverseProperty("Payments")]
        public virtual Order Order { get; set; } = null!;
    }
}
