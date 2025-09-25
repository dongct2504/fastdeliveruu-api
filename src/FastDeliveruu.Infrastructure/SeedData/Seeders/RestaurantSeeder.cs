using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Infrastructure.SeedData.Seeders;

public class RestaurantSeeder : IDataSeeder
{
    public int Order => 3;

    public async Task SeedAsync(FastDeliveruuDbContext context)
    {
        var now = DateTime.UtcNow;

        var hcm = await context.Cities
            .Include(c => c.Districts).ThenInclude(d => d.Wards)
            .FirstOrDefaultAsync(c => c.Name == "Hồ Chí Minh");

        var hanoi = await context.Cities
            .Include(c => c.Districts).ThenInclude(d => d.Wards)
            .FirstOrDefaultAsync(c => c.Name == "Hà Nội");

        if (hcm == null || hanoi == null) return; // chưa seed AddressSeeder

        var restaurantsData = new List<(string Name, string Description, string Phone, string Img, string House, string Street, City City, District District, Ward Ward)>
        {
            ("Phở 24", "Nhà hàng chuyên phở truyền thống", "0901234567", "https://cdn.pastaxi-manager.onepas.vn/content/uploads/articles/lanmkt/phohanoi/pho-ha-noi-pho-nguyet.jpg", "12", "Lê Lợi",
                hcm, hcm.Districts.First(d => d.Name == "Quận 1"), hcm.Districts.First(d => d.Name == "Quận 1").Wards.First()),

            ("Bún Chả Hà Nội", "Bún chả chuẩn vị Hà Nội", "0912345678", "https://cdn.pastaxi-manager.onepas.vn/content/uploads/articles/giangmkt/BLOG/%C4%90%E1%BA%B7c%20s%E1%BA%A3n%20v%C3%B9ng%20mi%E1%BB%81n/quanbunchangonhanoi/quan-bun-cha-ngon-ha-noi-huong-lien.jpg", "34", "Hàng Bài",
                hanoi, hanoi.Districts.First(d => d.Name == "Hoàn Kiếm"), hanoi.Districts.First(d => d.Name == "Hoàn Kiếm").Wards.First()),

            ("Cơm Tấm Nhà Làm", "Cơm tấm Sài Gòn", "0934567890", "https://media-cdn.tripadvisor.com/media/photo-s/0a/be/ed/9c/nha-hang-qua-ngon.jpg", "45", "Nguyễn Thị Minh Khai",
                hcm, hcm.Districts.First(d => d.Name == "Quận 3"), hcm.Districts.First(d => d.Name == "Quận 3").Wards.First())
        };

        foreach (var (name, desc, phone, img, house, street, city, district, ward) in restaurantsData)
        {
            if (!await context.Restaurants.AnyAsync(r => r.Name == name))
            {
                var restaurant = new Restaurant
                {
                    Id = Guid.NewGuid(),
                    Name = name,
                    Description = desc,
                    PhoneNumber = phone,
                    HouseNumber = house,
                    StreetName = street,
                    CityId = city.Id,
                    DistrictId = district.Id,
                    WardId = ward.Id,
                    IsVerify = true,
                    ImageUrl = img, // mock
                    PublicId = Guid.NewGuid().ToString(),
                    Latitude = 10.762622m,  // tạm thời fix (có thể random)
                    Longitude = 106.660172m,
                    CreatedAt = now,
                    RestaurantHours = GenerateDefaultHours(now)
                };

                await context.Restaurants.AddAsync(restaurant);
            }
        }

        await context.SaveChangesAsync();
    }

    private List<RestaurantHour> GenerateDefaultHours(DateTime now)
    {
        var list = new List<RestaurantHour>();
        var days = new[] { "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ Nhật" };

        foreach (var day in days)
        {
            list.Add(new RestaurantHour
            {
                Id = Guid.NewGuid(),
                WeekenDay = day,
                StartTime = DateTime.Today.AddHours(8),
                EndTime = DateTime.Today.AddHours(22),
                CreatedAt = now
            });
        }

        return list;
    }
}
