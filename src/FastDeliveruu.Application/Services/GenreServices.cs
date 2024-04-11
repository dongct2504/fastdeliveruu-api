using System.Data;
using Dapper;
using FastDeliveruu.Application.Dtos.GenreDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Constants;
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

    public async Task<IEnumerable<Genre>> GetAllGenresAsync(int page)
    {
        string procedureName = "spGetAllGenresPaging";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@RowOffSet", PagingConstants.DefaultPageSize * (page - 1));
        parameters.Add("@FetchNextRow", PagingConstants.DefaultPageSize);

        return await _sP_Call.ListAsync<Genre>(procedureName, parameters);
    }

    public async Task<Genre?> GetGenreByIdAsync(int id)
    {
        string procedureName = "spGetGenreById";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@GenreId", id);

        return await _sP_Call.OneRecordAsync<Genre>(procedureName, parameters);
    }

    public async Task<GenreWithMenuItemsDto?> GetGenreWithMenuItemsByIdAsync(int id)
    {
        string procedureName = "spGetGenreWithMenuItemsById";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@GenreId", id);

        return await _sP_Call.OneRecordAsync<GenreWithMenuItemsDto>(procedureName, parameters);
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

    public async Task DeleteGenreAsync(int id)
    {
        string procedureName = "spDeleteGenre";

        DynamicParameters parameters = new DynamicParameters();
        parameters.Add("@GenreId", id);

        await _sP_Call.ExecuteAsync(procedureName, parameters);
    }
}
