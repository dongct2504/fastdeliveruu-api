using System.Data;
using Dapper;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Interfaces;

namespace FastDeliveruu.Application.Services;

public class GenreServices : IGenreServices
{
    private readonly ISP_Call _sP_Call;

    public GenreServices(ISP_Call sP_Call)
    {
        _sP_Call = sP_Call;
    }

    public async Task<IEnumerable<Genre>> GetAllGenresAsync()
    {
        string procedureName = "spGetAllGenres";

        return await _sP_Call.ListAsync<Genre>(procedureName);
    }

    public async Task<Genre?> GetGenreByIdAsync(int id)
    {
        string procedureName = "spGetGenre";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@GenreId", id);

        return await _sP_Call.OneRecordAsync<Genre>(procedureName, parameters);
    }

    public async Task<Genre?> GetGenreByNameAsync(string name)
    {
        string procedureName = "spGetGenreByName";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Name", name);

        return await _sP_Call.OneRecordAsync<Genre>(procedureName, parameters);
    }

    public async Task<int> CreateGenreAsync(Genre genre)
    {
        string procedureName = "spCreateGenre";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@Name", genre.Name);
        parameters.Add("@CreatedAt", genre.CreatedAt);
        parameters.Add("@UpdatedAt", genre.UpdatedAt);
        parameters.Add("@GenreId", DbType.Int32, direction: ParameterDirection.Output);

        await _sP_Call.ExecuteAsync(procedureName, parameters);

        return parameters.Get<int>("@GenreId");
    }

    public async Task UpdateGenreAsync(Genre genre)
    {
        string procedureName = "spUpdateGenre";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@GenreId", genre.GenreId);
        parameters.Add("@Name", genre.Name);
        parameters.Add("@UpdatedAt", genre.UpdatedAt);

        await _sP_Call.ExecuteAsync(procedureName, parameters);
    }

    public async Task RemoveGenreAsync(int id)
    {
        string procedureName = "spDeleteGenre";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@GenreId", id);

        await _sP_Call.ExecuteAsync(procedureName, parameters);
    }
}
