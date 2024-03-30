using System.Data;
using Dapper;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Infrastructure.Repositories;

public class SP_Call : ISP_Call
{
    private readonly FastdeliveruuContext _dbContext;
    private static string ConnectionString = "";

    public SP_Call(FastdeliveruuContext context)
    {
        _dbContext = context;
        ConnectionString = _dbContext.Database.GetDbConnection().ConnectionString;
    }

    public void Dispose()
    {
        _dbContext.Dispose();
    }

    public async Task ExecuteAsync(string procedureName, DynamicParameters? parameters = null)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            await connection.OpenAsync();
            await connection.ExecuteAsync(procedureName, parameters, commandType: CommandType.StoredProcedure);
        }
    }

    public async Task<IEnumerable<T>> ListAsync<T>(string procedureName, DynamicParameters? parameters = null)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            await connection.OpenAsync();

            return await connection.QueryAsync<T>(procedureName, parameters,
                commandType: CommandType.StoredProcedure);
        }
    }

    public async Task<Tuple<IEnumerable<T1>, IEnumerable<T2>>> ListAsync<T1, T2>(
        string procedureName, DynamicParameters? parameters = null)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            await connection.OpenAsync();

            var result = await SqlMapper.QueryMultipleAsync(connection,
                procedureName, CommandType.StoredProcedure);
            var item1 = result.Read<T1>().ToList();
            var item2 = result.Read<T2>().ToList();

            if (item1 != null && item2 != null)
            {
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(item1, item2);
            }
        }

        return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(new List<T1>(), new List<T2>());
    }

    public async Task<T?> OneRecordAsync<T>(string procedureName, DynamicParameters? parameters = null)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            await connection.OpenAsync();
            var value = connection.Query<T>(procedureName, parameters,
                commandType: CommandType.StoredProcedure);

            var firstValue = value.FirstOrDefault();

            if (firstValue != null)
            {
                return (T)Convert.ChangeType(firstValue, typeof(T));
            }

            return default;
        }
    }

    public async Task<T?> SingleAsync<T>(string procedureName, DynamicParameters? parameters = null)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            await connection.OpenAsync();
            var value = connection.ExecuteScalarAsync<T>(procedureName, parameters,
                commandType: CommandType.StoredProcedure);

            return (T)Convert.ChangeType(value, typeof(T));
        }
    }
}
