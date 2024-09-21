using FastDeliveruu.Application.Dtos.MenuVariantDtos;
using FluentResults;
using MediatR;

namespace FastDeliveruu.Application.MenuVariants.Queries.GetMenuVariantById;

public class GetMenuVariantByIdQuery : IRequest<Result<MenuVariantDto>>
{
    public GetMenuVariantByIdQuery(Guid id)
    {
        Id = id;
    }

    public Guid Id { get; }
}
