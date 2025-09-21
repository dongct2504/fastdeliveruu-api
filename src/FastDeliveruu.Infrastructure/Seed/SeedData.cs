using FastDeliveruu.Domain.Data;
using FastDeliveruu.Infrastructure.Seed.Seeders;

namespace FastDeliveruu.Infrastructure.Seed;

public static class SeedData
{
    public static async Task InitializeAsync(FastDeliveruuDbContext context, IEnumerable<IDataSeeder> seeders)
    {
        foreach (var seeder in seeders.OrderBy(x => x.Order))
        {
            await seeder.SeedAsync(context);
        }
    }
}
