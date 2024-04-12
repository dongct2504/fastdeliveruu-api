using Dapper;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Extensions;
using FastDeliveruu.Domain.Interfaces;

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

    public async Task<Genre?> GetGenreByIdAsync(int id)
    {
        return await _genreRepository.GetAsync(id);
    }

    public async Task<Genre?> GetGenreWithMenuItemsByIdAsync(int id)
    {
        QueryOptions<Genre> options = new QueryOptions<Genre>
        {
            SetIncludes = "MenuItems",
            Where = g => g.GenreId == id
        };

        return await _genreRepository.GetAsync(options);
    }

    public async Task<Genre?> GetGenreByNameAsync(string name)
    {
        QueryOptions<Genre> options = new QueryOptions<Genre>
        {
            SetIncludes = "MenuItems",
            Where = g => g.Name == name
        };

        return await _genreRepository.GetAsync(options);
    }

    public async Task<int> CreateGenreAsync(Genre genre)
    {
        Genre createdGenre = await _genreRepository.AddAsync(genre);

        return createdGenre.GenreId;
    }

    public async Task UpdateGenreAsync(Genre genre)
    {
        await _genreRepository.UpdateAsync(genre);
    }

    public async Task DeleteGenreAsync(Genre genre)
    {
        await _genreRepository.DeleteAsync(genre);
    }
}
