using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities.AutoGenEntities;
using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Infrastructure.SeedData.Seeders;

public class MenuVariantSeeder : IDataSeeder
{
    public int Order => 6;

    public async Task SeedAsync(FastDeliveruuDbContext context)
    {
        var now = DateTime.UtcNow;

        var pizza = context.MenuItems.FirstOrDefault(mi => mi.Name == "Pizza Hải sản");
        var milkTea = context.MenuItems.FirstOrDefault(mi => mi.Name == "Trà sữa trân châu");

        if (pizza == null || milkTea == null) return;

        var variants = new[]
        {
            new { Item = pizza, Name = "Size M", Price = 120000m, Discount = 0.05m, Img = "https://lh6.googleusercontent.com/8ymZxaileGjh577CcSElIkxDeyT771pRUL39b9ko5yOWF2ExXOriGo_plI1FC6Gy8zpyFFbeRr2qWgGpbTWcYLUTPYiYFvipgWyZX2hDfV999Oh792lRVsfyfTN998UnZSE1r6l3kW3Sxue4K2DkgbeyrnWxOx1i0jCJvJsAHTQ-3o2Rkk5w-kPl754jWg", PublicId = "pizza_haisan_m" },
            new { Item = pizza, Name = "Size L", Price = 150000m, Discount = 0.1m, Img = "https://ladem-bistro.com/upload/thumbs/pizza-haisan-ngon-tayninh-ladem-zega5d-568f58.jpg", PublicId = "pizza_haisan_l" },
            new { Item = milkTea, Name = "Ly 500ml", Price = 45000m, Discount = 0m, Img = "https://covi.vn/wp-content/uploads/2021/11/IMG_9917.jpg", PublicId = "trasua_500" },
            new { Item = milkTea, Name = "Ly 700ml", Price = 55000m, Discount = 0m, Img = "https://covi.vn/wp-content/uploads/2021/11/IMG_9917.jpg", PublicId = "trasua_700" }
        };

        foreach (var v in variants)
        {
            var existingVariant = context.MenuVariants.FirstOrDefault(mv => mv.MenuItemId == v.Item.Id && mv.VarietyName == v.Name);

            if (existingVariant == null)
            {
                var newVariant = new MenuVariant
                {
                    Id = Guid.NewGuid(),
                    MenuItemId = v.Item.Id,
                    VarietyName = v.Name,
                    Price = v.Price,
                    DiscountPercent = v.Discount,
                    ImageUrl = v.Img,
                    PublicId = v.PublicId,
                    CreatedAt = now
                };

                await context.MenuVariants.AddAsync(newVariant);
                await context.SaveChangesAsync();

                // Thêm Inventory cho variant
                await context.MenuVariantInventories.AddAsync(new MenuVariantInventory
                {
                    Id = Guid.NewGuid(),
                    MenuVariantId = newVariant.Id,
                    QuantityAvailable = 50,
                    QuantityReserved = 0,
                    CreatedAt = now
                });
                await context.SaveChangesAsync();
            }
            else
            {
                // Nếu có variant nhưng chưa có inventory => thêm
                if (!context.MenuVariantInventories.Any(inv => inv.MenuVariantId == existingVariant.Id))
                {
                    await context.MenuVariantInventories.AddAsync(new MenuVariantInventory
                    {
                        Id = Guid.NewGuid(),
                        MenuVariantId = existingVariant.Id,
                        QuantityAvailable = 50,
                        QuantityReserved = 0,
                        CreatedAt = now
                    });
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
