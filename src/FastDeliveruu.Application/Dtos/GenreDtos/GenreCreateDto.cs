using System.ComponentModel.DataAnnotations;

namespace FastDeliveruu.Application.Dtos.GenreDtos;

public class GenreCreateDto
{
    [Required(ErrorMessage = "Vui lòng nhập tên thể loại.")]
    public string Name { get; set; } = string.Empty;
}