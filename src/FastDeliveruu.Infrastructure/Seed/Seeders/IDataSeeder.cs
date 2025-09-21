using FastDeliveruu.Domain.Data;

namespace FastDeliveruu.Infrastructure.Seed.Seeders;

public interface IDataSeeder
{
    int Order { get; }
    Task SeedAsync(FastDeliveruuDbContext context);
}
