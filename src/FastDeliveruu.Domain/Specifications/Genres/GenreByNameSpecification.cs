using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Domain.Specifications.Genres;

public class GenreByNameSpecification : Specification<Genre>
{
    public GenreByNameSpecification(string name)
        : base(g => g.Name == name)
    {
    }
}
