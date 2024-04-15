using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;
using FluentResults;

namespace FastDeliveruu.Application.Services;

public class GenreServices : IGenreServices
{
    private readonly IGenreRepository _genreRepository;

    public GenreServices(IGenreRepository genreRepository)
    {
        _genreRepository = genreRepository;
    }

    public async Task<IEnumerable<Genre>> GetAllGenresAsync()
    {
        return await _genreRepository.ListAllAsync();
    }

    public async Task<Result<Genre>> GetGenreByIdAsync(int id)
    {
        Genre? genre = await _genreRepository.GetAsync(id);
        if (genre == null)
        {
            return Result.Fail<Genre>(new NotFoundError($"The requested genre '{id}' is not found."));
        }

        return genre;
    }

    public async Task<Result<Genre>> GetGenreWithMenuItemsByIdAsync(int id)
    {
        QueryOptions<Genre> options = new QueryOptions<Genre>
        {
            SetIncludes = "MenuItems",
            Where = g => g.GenreId == id
        };

        Genre? genre = await _genreRepository.GetAsync(id);
        if (genre == null)
        {
            return Result.Fail<Genre>(new NotFoundError($"The requested genre '{id}' is not found."));
        }

        return genre;
    }

    public async Task<Result<Genre>> GetGenreByNameAsync(string name)
    {
        QueryOptions<Genre> options = new QueryOptions<Genre>
        {
            SetIncludes = "MenuItems",
            Where = g => g.Name == name
        };

        Genre? genre = await _genreRepository.GetAsync(options);
        if (genre == null)
        {
            return Result.Fail<Genre>(new NotFoundError($"The requested genre '{name}' is not found."));
        }

        return genre;
    }

    public async Task<Result<int>> CreateGenreAsync(Genre genre)
    {
        Genre? isGenreExist = await _genreRepository.GetAsync(new QueryOptions<Genre>
        {
            Where = g => g.Name == genre.Name
        });
        if (isGenreExist != null)
        {
            return Result.Fail<int>(
                new DuplicateError($"The requested genre '{genre.Name}' is already exists."));
        }

        Genre createdGenre = await _genreRepository.AddAsync(genre);

        return createdGenre.GenreId;
    }

    public async Task<Result> UpdateGenreAsync(int id, Genre genre)
    {
        Genre? isGenreExist = await _genreRepository.GetAsync(id);
        if (isGenreExist == null)
        {
            Result.Fail(new NotFoundError($"The requested genre '{id}' is not found."));
        }

        await _genreRepository.UpdateAsync(genre);

        return Result.Ok();
    }

    public async Task<Result> DeleteGenreAsync(int id)
    {
        Genre? genre = await _genreRepository.GetAsync(id);
        if (genre == null)
        {
            return Result.Fail(new NotFoundError($"The requested genre '{id}' is not found."));
        }

        await _genreRepository.DeleteAsync(genre);

        return Result.Ok();
    }
}