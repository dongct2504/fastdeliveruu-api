using FastDeliveruu.Common.Helpers;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Entities.AutoGenEntities;
using FastDeliveruu.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FastDeliveruu.Domain.Data
{
    public partial class FastDeliveruuDbContext
    {
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            // addresses configuration
            modelBuilder.Entity<AddressesCustomer>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.City)
                    .WithMany(p => p.AddressesCustomers)
                    .HasForeignKey(d => d.CityId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_AddressCustomers_Cities_CityId");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.AddressesCustomers)
                    .HasForeignKey(d => d.DistrictId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_AddressCustomers_Districts_DistrictId");

                entity.HasOne(d => d.Ward)
                    .WithMany(p => p.AddressesCustomers)
                    .HasForeignKey(d => d.WardId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_AddressCustomers_Wards_WardId");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.DeliveryMethod)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.DeliveryMethodId)
                    .OnDelete(DeleteBehavior.NoAction) // manually handle null 
                    .HasConstraintName("FK_Orders_DeliveryMethods_DeliveryMethodId");

                entity.HasOne(d => d.City)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.CityId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_Orders_Cities_CityId");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.DistrictId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_Orders_Districts_DistrictId");

                entity.HasOne(d => d.Ward)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.WardId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_Orders_Wards_WardId");
            });

            modelBuilder.Entity<Restaurant>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.City)
                    .WithMany(p => p.Restaurants)
                    .HasForeignKey(d => d.CityId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_Restaurants_Cities_CityId");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Restaurants)
                    .HasForeignKey(d => d.DistrictId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_Restaurants_Districts_DistrictId");

                entity.HasOne(d => d.Ward)
                    .WithMany(p => p.Restaurants)
                    .HasForeignKey(d => d.WardId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_Restaurants_Wards_WardId");
            });

            modelBuilder.Entity<Shipper>(entity =>
            {
                entity.HasOne(d => d.City)
                    .WithMany(p => p.Shippers)
                    .HasForeignKey(d => d.CityId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_Shippers_Cities_CityId");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Shippers)
                    .HasForeignKey(d => d.DistrictId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_Shippers_Districts_DistrictId");

                entity.HasOne(d => d.Ward)
                    .WithMany(p => p.Shippers)
                    .HasForeignKey(d => d.WardId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_Shippers_Wards_WardId");
            });

            // order detail configuration for menu variant (delete menu variant affect order detail)
            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.MenuVariant)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.MenuVariantId)
                    .OnDelete(DeleteBehavior.NoAction) // manually handle null 
                    .HasConstraintName("FK_OrderDetails_MenuVariants_MenuVariantId");
            });

            // order configuration for shipper (delete shipper affect order)
            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.Shipper)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ShipperId)
                    .OnDelete(DeleteBehavior.NoAction) // manually handle null
                    .HasConstraintName("FK_Orders_Shippers_ShipperId");
            });

            // config deletion behavior for sender and receipient in MessageThread table
            modelBuilder.Entity<MessageThread>()
                .HasOne(c => c.SenderAppUser)
                .WithMany(u => u.SenderMessageThreads)
                .HasForeignKey(c => c.SenderAppUserId)
                .OnDelete(DeleteBehavior.Cascade); // delete cascade for user sender
            modelBuilder.Entity<MessageThread>()
                .HasOne(c => c.RecipientAppUser)
                .WithMany(u => u.RecipientMessageThreads)
                .HasForeignKey(c => c.RecipientAppUserId)
                .OnDelete(DeleteBehavior.NoAction); // can't delete for recipient

            modelBuilder.Entity<MessageThread>()
                .HasOne(c => c.SenderShipper)
                .WithMany(s => s.SenderMessageThreads)
                .HasForeignKey(c => c.SenderShipperId)
                .OnDelete(DeleteBehavior.Cascade); // delete cascade for shiper sender
            modelBuilder.Entity<MessageThread>()
                .HasOne(c => c.RecipientShipper)
                .WithMany(s => s.RecipientMessageThreads)
                .HasForeignKey(c => c.RecipientShipperId)
                .OnDelete(DeleteBehavior.NoAction); // can't delete for recipient

            // fix asp.net auto generate FK for AppUserRoles
            modelBuilder.Entity<AppUser>()
                .HasMany(ur => ur.AppUserRoles)
                .WithOne(u => u.AppUser)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            modelBuilder.Entity<AppRole>()
                .HasMany(ur => ur.AppUserRoles)
                .WithOne(u => u.AppRole)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            // encrypt, decrypt
            DataEncryptionHelper encryptionHelper = new DataEncryptionHelper(_configuration["Encryption:SecretKey"]);

            ValueConverter<string?, string?> converter = new ValueConverter<string?, string?>(
                v => v == null ? null : encryptionHelper.Encrypt(v),
                v => v == null ? null : encryptionHelper.Decrypt(v));

            modelBuilder.Entity<AppUser>()
                .Property(u => u.PhoneNumber)
                .HasConversion(converter);

            modelBuilder.Entity<AddressesCustomer>()
                .Property(a => a.HouseNumber)
                .HasConversion(converter);

            modelBuilder.Entity<AddressesCustomer>()
                .Property(a => a.StreetName)
                .HasConversion(converter);
        }
    }
}
