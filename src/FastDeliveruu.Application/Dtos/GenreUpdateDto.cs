using System.ComponentModel.DataAnnotations;

namespace FastDeliveruu.Application.Dtos;

public class GenreUpdateDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
}
