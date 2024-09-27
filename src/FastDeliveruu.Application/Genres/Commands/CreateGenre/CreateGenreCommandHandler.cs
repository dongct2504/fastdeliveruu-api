using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Dtos.GenreDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.Genres;
using FluentResults;
using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FastDeliveruu.Application.Genres.Commands.CreateGenre;

public class CreateGenreCommandHandler : IRequestHandler<CreateGenreCommand, Result<GenreDto>>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ILogger<CreateGenreCommandHandler> _logger;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IMapper _mapper;

    public CreateGenreCommandHandler(
        IMapper mapper,
        IFastDeliveruuUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        ILogger<CreateGenreCommandHandler> logger)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    public async Task<Result<GenreDto>> Handle(CreateGenreCommand request, CancellationToken cancellationToken)
    {
        var spec = new GenreByNameSpecification(request.Name);
        Genre? genre = await _unitOfWork.Genres.GetWithSpecAsync(spec, asNoTracking: true);
        if (genre != null)
        {
            string message = "genre is already exist.";
            _logger.LogWarning($"{request.GetType().Name} - {message} - {request}");
            return Result.Fail(new DuplicateError(message));
        }

        genre = _mapper.Map<Genre>(request);
        genre.Id = Guid.NewGuid();
        genre.CreatedAt = _dateTimeProvider.VietnamDateTimeNow;

        _unitOfWork.Genres.Add(genre);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<GenreDto>(genre);
    }
}