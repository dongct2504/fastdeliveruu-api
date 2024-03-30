using Dapper;

namespace FastDeliveruu.Domain.Interfaces;

public interface ISP_Call : IDisposable
{
    Task<T?> SingleAsync<T>(string procedureName, DynamicParameters? parameters = null);

    Task<T?> OneRecordAsync<T>(string procedureName, DynamicParameters? parameters = null);

    Task ExecuteAsync(string procedureName, DynamicParameters? parameters = null);

    Task<IEnumerable<T>> ListAsync<T>(string procedureName, DynamicParameters? parameters = null);

    Task<Tuple<IEnumerable<T1>, IEnumerable<T2>>> ListAsync<T1, T2>(string procedureName,
        DynamicParameters? parameters = null);
}