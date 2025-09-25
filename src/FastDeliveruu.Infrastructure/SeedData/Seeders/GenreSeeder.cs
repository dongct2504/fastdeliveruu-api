using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Infrastructure.SeedData.Seeders;

public class GenreSeeder : IDataSeeder
{
    public int Order => 4;

    public async Task SeedAsync(FastDeliveruuDbContext context)
    {
        var now = DateTime.UtcNow;

        var genres = new List<string> { "Pizza", "Đồ uống", "Cơm Quê", "Tráng miệng" };

        foreach (var genreName in genres)
        {
            if (!context.Genres.Any(g => g.Name == genreName))
            {
                await context.Genres.AddAsync(new Genre
                {
                    Id = Guid.NewGuid(),
                    Name = genreName,
                    CreatedAt = now
                });
            }
        }

        await context.SaveChangesAsync();
    }
}
