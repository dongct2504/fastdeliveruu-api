using FastDeliveruu.Common.Constants;
using FastDeliveruu.Application.Dtos;
using FastDeliveruu.Application.Dtos.WishListDtos;
using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Application.WishLists.Queries.GetWishList;

public class GetWishListQueryHandler : IRequestHandler<GetWishListQuery, PagedList<WishListDto>>
{
    private readonly FastDeliveruuDbContext _dbContext;

    public GetWishListQueryHandler(FastDeliveruuDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedList<WishListDto>> Handle(GetWishListQuery request, CancellationToken cancellationToken)
    {
        IQueryable<WishList> wishListsQuery = _dbContext.WishLists.AsQueryable();

        wishListsQuery = wishListsQuery.Where(w => w.AppUserId == request.WishListParams.UserId);

        if (!string.IsNullOrEmpty(request.WishListParams.Search))
        {
            wishListsQuery = wishListsQuery.Where(c =>
                c.MenuVariant != null
                    ? c.MenuVariant.VarietyName.ToLower().Contains(request.WishListParams.Search.ToLower())
                    : c.MenuItem.Name.ToLower().Contains(request.WishListParams.Search.ToLower()));
        }

        if (!string.IsNullOrEmpty(request.WishListParams.Sort))
        {
            switch (request.WishListParams.Sort)
            {
                case SortConstants.OldestUpdateAsc:
                    wishListsQuery = wishListsQuery.OrderBy(c => c.UpdatedAt);
                    break;
                case SortConstants.LatestUpdateDesc:
                    wishListsQuery = wishListsQuery.OrderByDescending(c => c.UpdatedAt);
                    break;
                case SortConstants.NameAsc:
                    wishListsQuery = wishListsQuery.OrderBy(c => c.MenuVariant != null
                        ? c.MenuVariant.VarietyName
                        : c.MenuItem.Name);
                    break;
                case SortConstants.NameDesc:
                    wishListsQuery = wishListsQuery.OrderByDescending(c => c.MenuVariant != null
                        ? c.MenuVariant.VarietyName
                        : c.MenuItem.Name);
                    break;
            }
        }
        else
        {
            wishListsQuery = wishListsQuery.OrderBy(c => c.MenuVariant != null
                ? c.MenuVariant.VarietyName
                : c.MenuItem.Name);
        }

        PagedList<WishListDto> pagedList = new PagedList<WishListDto>
        {
            PageNumber = request.WishListParams.PageNumber,
            PageSize = request.WishListParams.PageSize,
            TotalRecords = await wishListsQuery.CountAsync(cancellationToken),
            Items = await wishListsQuery
                .AsNoTracking()
                .ProjectToType<WishListDto>()
                .Skip((request.WishListParams.PageNumber - 1) * request.WishListParams.PageSize)
                .Take(request.WishListParams.PageSize)
                .ToListAsync(cancellationToken)
        };

        return pagedList;
    }
}
