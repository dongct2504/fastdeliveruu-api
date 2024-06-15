using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastDeliveruu.Domain.Entities.Identity;

public class AppUser : IdentityUser
{

    [StringLength(50)]
    public string? FirstName { get; set; }

    [StringLength(50)]
    public string? LastName { get; set; }

    [StringLength(1024)]
    [Unicode(false)]
    public string? ImageUrl { get; set; }

    [StringLength(256)]
    [Unicode(false)]
    public string? PublicId { get; set; }

    [StringLength(128)]
    public string? Address { get; set; }

    [StringLength(50)]
    public string? Ward { get; set; }

    [StringLength(30)]
    public string? District { get; set; }

    [StringLength(30)]
    public string? City { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime UpdatedAt { get; set; }
}
