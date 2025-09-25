using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Infrastructure.SeedData.Seeders;

public class DeliveryMethodSeeder : IDataSeeder
{
    public int Order => 7;

    public async Task SeedAsync(FastDeliveruuDbContext context)
    {
        if (await context.DeliveryMethods.AnyAsync())
        {
            return; // đã có dữ liệu thì bỏ qua
        }

        var methods = new List<DeliveryMethod>
        {
            new DeliveryMethod
            {
                ShortName = "Tiêu chuẩn",
                EstimatedDeliveryTime = "3-5 ngày",
                Description = "Giao hàng tiêu chuẩn",
                Price = 30000m,
                CreatedAt = DateTime.UtcNow
            },
            new DeliveryMethod
            {
                ShortName = "Nhanh",
                EstimatedDeliveryTime = "1-2 ngày",
                Description = "Giao hàng nhanh",
                Price = 60000m,
                CreatedAt = DateTime.UtcNow
            },
            new DeliveryMethod
            {
                ShortName = "Hỏa tốc",
                EstimatedDeliveryTime = "Trong ngày",
                Description = "Giao hàng hỏa tốc",
                Price = 100000m,
                CreatedAt = DateTime.UtcNow
            }
        };

        await context.DeliveryMethods.AddRangeAsync(methods);
        await context.SaveChangesAsync();
    }
}
