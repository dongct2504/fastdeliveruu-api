using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Domain.Entities
{
    [Index("Name", Name = "IX_Genres_Name")]
    public partial class Genre
    {
        public Genre()
        {
            MenuItems = new HashSet<MenuItem>();
        }

        [Key]
        public Guid Id { get; set; }
        [StringLength(126)]
        public string Name { get; set; } = null!;
        [Column(TypeName = "datetime")]
        public DateTime? CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? UpdatedAt { get; set; }

        [InverseProperty("Genre")]
        public virtual ICollection<MenuItem> MenuItems { get; set; }
    }
}
