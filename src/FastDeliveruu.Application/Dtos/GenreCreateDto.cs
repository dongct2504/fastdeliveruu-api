using System.ComponentModel.DataAnnotations;

namespace FastDeliveruu.Application.Dtos;

public class GenreCreateDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
}