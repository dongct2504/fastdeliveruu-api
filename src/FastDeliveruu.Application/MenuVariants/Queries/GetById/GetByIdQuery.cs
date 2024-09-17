using FastDeliveruu.Application.Dtos.MenuVariantDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.MenuVariants.Queries.GetById;

public class GetByIdQuery : IRequest<Result<MenuVariantDto>>
{
    public GetByIdQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}
