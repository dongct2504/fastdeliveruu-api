using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.LocalUserDtos;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;
using MapsterMapper;
using MediatR;
using Serilog;

namespace FastDeliveruu.Application.Users.Queries.GetUserById;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<LocalUserDto>>
{
    private readonly ILocalUserRepository _localUserRepository;
    private readonly IMapper _mapper;

    public GetUserByIdQueryHandler(ILocalUserRepository localUserRepository, IMapper mapper)
    {
        _localUserRepository = localUserRepository;
        _mapper = mapper;
    }

    public async Task<Result<LocalUserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
        {
            string message = "Id is empty";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<LocalUserDto>(new BadRequestError(message));
        }

        LocalUser? localUser = await _localUserRepository.GetAsync(request.Id);
        if (localUser == null)
        {
            string message = "User not found";
            Log.Warning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail<LocalUserDto>(new NotFoundError(message));
        }

        return _mapper.Map<LocalUserDto>(localUser);
    }
}