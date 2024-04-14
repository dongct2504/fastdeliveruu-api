using FastDeliveruu.Application.Common.Errors;
using FastDeliveruu.Domain.Entities;
using FluentResults;

namespace FastDeliveruu.Application.Interfaces;

public interface IGenreServices
{
    Task<IEnumerable<Genre>> GetAllGenresAsync();
    Task<Result<Genre>> GetGenreByIdAsync(int id);
    Task<Result<Genre>> GetGenreWithMenuItemsByIdAsync(int id);
    Task<Result<Genre>> GetGenreByNameAsync(string name);

    Task<Result<int>> CreateGenreAsync(Genre genre);
    Task<Result> UpdateGenreAsync(int id, Genre genre);
    Task<Result> DeleteGenreAsync(int id);
}
