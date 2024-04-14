using System.Data;
using Dapper;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Infrastructure.Repositories;

public class SP_Call : ISP_Call
{
    private readonly FastDeliveruuContext _dbContext;
    private static string ConnectionString = "";

    public SP_Call(FastDeliveruuContext context)
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

            var item1 = await result.ReadAsync<T1>();
            List<T1> item1List = item1.ToList();

            var item2 = await result.ReadAsync<T2>();
            List<T2> item2List = item2.ToList();

            if (item1List != null && item2List != null)
            {
                return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(item1List, item2List);
            }
        }

        return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(new List<T1>(), new List<T2>());
    }

    public async Task<T?> OneRecordAsync<T>(string procedureName, DynamicParameters? parameters = null)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            await connection.OpenAsync();
            var value = await connection.QueryAsync<T>(procedureName, parameters,
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
            var value = await connection.ExecuteScalarAsync<T>(procedureName, parameters,
                commandType: CommandType.StoredProcedure);

            if (value != null)
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }

            return default;
        }
    }
}
