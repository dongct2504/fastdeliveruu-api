using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities;

public partial class Genre
{
    [Key]
    public int GenreId { get; set; }

    [StringLength(126)]
    [Unicode(false)]
    public string Name { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; }

    [InverseProperty("Genre")]
    public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
}
