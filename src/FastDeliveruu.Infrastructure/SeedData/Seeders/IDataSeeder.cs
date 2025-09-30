using FastDeliveruu.Domain.Data;

namespace FastDeliveruu.Infrastructure.SeedData.Seeders;

public interface IDataSeeder
{
    int Order { get; }
    Task SeedAsync(FastDeliveruuDbContext context);
}
