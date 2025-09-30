using FastDeliveruu.Domain.Data;
using FastDeliveruu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Infrastructure.SeedData.Seeders;

public class AddressSeeder : IDataSeeder
{
    public int Order => 1;

    public async Task SeedAsync(FastDeliveruuDbContext context)
    {
        var now = DateTime.UtcNow;

        var citiesData = new List<(string CityName, List<(string DistrictName, List<string> Wards)> Districts)>
        {
            ("Hồ Chí Minh", new List<(string, List<string>)>
            {
                ("Quận 1", new List<string>{ "Phường Bến Nghé", "Phường Bến Thành", "Phường Cô Giang" }),
                ("Quận 3", new List<string>{ "Phường Võ Thị Sáu", "Phường 7", "Phường 8" })
            }),
            ("Hà Nội", new List<(string, List<string>)>
            {
                ("Ba Đình", new List<string>{ "Phường Ngọc Hà", "Phường Điện Biên", "Phường Quán Thánh" }),
                ("Hoàn Kiếm", new List<string>{ "Phường Hàng Bài", "Phường Tràng Tiền", "Phường Đồng Xuân" })
            }),
            ("Đà Nẵng", new List<(string, List<string>)>
            {
                ("Hải Châu", new List<string>{ "Phường Hải Châu 1", "Phường Hải Châu 2", "Phường Thạch Thang" }),
                ("Thanh Khê", new List<string>{ "Phường Xuân Hà", "Phường Tân Chính", "Phường Chính Gián" })
            }),
            ("Cần Thơ", new List<(string, List<string>)>
            {
                ("Ninh Kiều", new List<string>{ "Phường An Khánh", "Phường An Bình", "Phường Hưng Lợi" }),
                ("Bình Thủy", new List<string>{ "Phường Bình Thủy", "Phường Trà An", "Phường Trà Nóc" })
            })
        };

        foreach (var (cityName, districts) in citiesData)
        {
            var city = await context.Cities
                .Include(c => c.Districts)
                .ThenInclude(d => d.Wards)
                .FirstOrDefaultAsync(c => c.Name == cityName);

            if (city == null)
            {
                city = new City
                {
                    Name = cityName,
                    CreatedAt = now,
                    Districts = new List<District>()
                };
                context.Cities.Add(city);
            }

            foreach (var (districtName, wards) in districts)
            {
                var district = city.Districts.FirstOrDefault(d => d.Name == districtName);
                if (district == null)
                {
                    district = new District
                    {
                        Name = districtName,
                        CreatedAt = now,
                        Wards = new List<Ward>()
                    };
                    city.Districts.Add(district);
                }

                foreach (var wardName in wards)
                {
                    if (!district.Wards.Any(w => w.Name == wardName))
                    {
                        district.Wards.Add(new Ward
                        {
                            Name = wardName,
                            CreatedAt = now
                        });
                    }
                }
            }
        }

        await context.SaveChangesAsync();
    }
}
