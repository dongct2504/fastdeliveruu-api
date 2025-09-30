using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities.AutoGenEntities;
using FastDeliveruu.Domain.Entities;

namespace FastDeliveruu.Infrastructure.SeedData.Seeders;

public class MenuItemSeeder : IDataSeeder
{
    public int Order => 5;

    public async Task SeedAsync(FastDeliveruuDbContext context)
    {
        if (!context.Restaurants.Any())
            return;

        var now = DateTime.UtcNow;
        var restaurant = context.Restaurants.First();

        var pizzaGenre = context.Genres.FirstOrDefault(g => g.Name == "Pizza");
        var drinkGenre = context.Genres.FirstOrDefault(g => g.Name == "Đồ uống");

        var items = new[]
        {
            new
            {
                Name = "Pizza Hải sản",
                Desc = "Pizza hải sản phô mai thơm ngon",
                Price = 120000m,
                Discount = 0.1m,
                Genre = pizzaGenre,
                Img = "https://media.istockphoto.com/id/1048400936/vi/anh/to%C3%A0n-b%E1%BB%99-pizza-%C3%BD-tr%C3%AAn-b%C3%A0n-g%E1%BB%97-v%E1%BB%9Bi-c%C3%A1c-th%C3%A0nh-ph%E1%BA%A7n.jpg?s=612x612&w=0&k=20&c=HExN8B3MoKQJdMDdpadA3qfguSAUJhhChyHCh87zIA0=",
                PublicId = "pizza_haisan"
            },
            new
            {
                Name = "Trà sữa trân châu",
                Desc = "Trà sữa thơm béo, topping trân châu đen",
                Price = 45000m,
                Discount = 0m,
                Genre = drinkGenre,
                Img = "https://www.huongnghiepaau.com/wp-content/uploads/2019/10/tra-sua-chocolate.jpg",
                PublicId = "trasua"
            }
        };

        foreach (var i in items)
        {
            var existingItem = context.MenuItems.FirstOrDefault(mi => mi.Name == i.Name && mi.RestaurantId == restaurant.Id);

            if (existingItem == null)
            {
                var newItem = new MenuItem
                {
                    Id = Guid.NewGuid(),
                    RestaurantId = restaurant.Id,
                    GenreId = i.Genre!.Id,
                    Name = i.Name,
                    Description = i.Desc,
                    Price = i.Price,
                    DiscountPercent = i.Discount,
                    ImageUrl = i.Img,
                    PublicId = i.PublicId,
                    CreatedAt = now
                };

                await context.MenuItems.AddAsync(newItem);
                await context.SaveChangesAsync();

                // Thêm Inventory cho MenuItem
                await context.MenuItemInventories.AddAsync(new MenuItemInventory
                {
                    Id = Guid.NewGuid(),
                    MenuItemId = newItem.Id,
                    QuantityAvailable = 100,
                    QuantityReserved = 0,
                    CreatedAt = now
                });
                await context.SaveChangesAsync();
            }
            else
            {
                // Nếu có item mà chưa có inventory => thêm
                if (!context.MenuItemInventories.Any(inv => inv.MenuItemId == existingItem.Id))
                {
                    await context.MenuItemInventories.AddAsync(new MenuItemInventory
                    {
                        Id = Guid.NewGuid(),
                        MenuItemId = existingItem.Id,
                        QuantityAvailable = 100,
                        QuantityReserved = 0,
                        CreatedAt = now
                    });
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
