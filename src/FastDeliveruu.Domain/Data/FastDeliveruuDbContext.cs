using System;
using System.Collections.Generic;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FastDeliveruu.Domain.Data
{
    public partial class FastDeliveruuDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public FastDeliveruuDbContext(DbContextOptions<FastDeliveruuDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AddressesCustomer> AddressesCustomers { get; set; } = null!;
        public virtual DbSet<City> Cities { get; set; } = null!;
        public virtual DbSet<Coupon> Coupons { get; set; } = null!;
        public virtual DbSet<DeliveryAddress> DeliveryAddresses { get; set; } = null!;
        public virtual DbSet<DeliveryMethod> DeliveryMethods { get; set; } = null!;
        public virtual DbSet<District> Districts { get; set; } = null!;
        public virtual DbSet<Genre> Genres { get; set; } = null!;
        public virtual DbSet<MenuItem> MenuItems { get; set; } = null!;
        public virtual DbSet<MenuItemReview> MenuItemReviews { get; set; } = null!;
        public virtual DbSet<MenuVariant> MenuVariants { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDelivery> OrderDeliveries { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Restaurant> Restaurants { get; set; } = null!;
        public virtual DbSet<RestaurantHour> RestaurantHours { get; set; } = null!;
        public virtual DbSet<RestaurantReview> RestaurantReviews { get; set; } = null!;
        public virtual DbSet<Ward> Wards { get; set; } = null!;
        public virtual DbSet<WishList> WishLists { get; set; } = null!;
        public virtual DbSet<AppUser> AppUsers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AddressesCustomer>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.City)
                    .WithMany(p => p.AddressesCustomers)
                    .HasForeignKey(d => d.CityId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_AddressCustomers_Cities");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.AddressesCustomers)
                    .HasForeignKey(d => d.DistrictId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_AddressCustomers_Districts");

                entity.HasOne(d => d.Ward)
                    .WithMany(p => p.AddressesCustomers)
                    .HasForeignKey(d => d.WardId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_AddressCustomers_Wards");
            });

            modelBuilder.Entity<DeliveryAddress>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.City)
                    .WithMany(p => p.DeliveryAddresses)
                    .HasForeignKey(d => d.CityId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_DeliveryAddresses_Cities");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.DeliveryAddresses)
                    .HasForeignKey(d => d.DistrictId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_DeliveryAddresses_Districts");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.DeliveryAddresses)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_DeliveryAddresses_Orders");

                entity.HasOne(d => d.Ward)
                    .WithMany(p => p.DeliveryAddresses)
                    .HasForeignKey(d => d.WardId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_DeliveryAddresses_Wards");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.OrderStatus).HasDefaultValueSql("((0))");

                entity.Property(e => e.PaymentStatus).HasDefaultValueSql("((0))");

                entity.HasOne(d => d.DeliveryMethod)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.DeliveryMethodId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_Orders_DeliveryMethods");
            });

            modelBuilder.Entity<Restaurant>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.HasOne(d => d.City)
                    .WithMany(p => p.Restaurants)
                    .HasForeignKey(d => d.CityId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_Restaurants_Cities");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Restaurants)
                    .HasForeignKey(d => d.DistrictId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_Restaurants_Districts");

                entity.HasOne(d => d.Ward)
                    .WithMany(p => p.Restaurants)
                    .HasForeignKey(d => d.WardId)
                    .OnDelete(DeleteBehavior.NoAction)
                    .HasConstraintName("FK_Restaurants_Wards");
            });
        }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{
        //    OnModelCreating(modelBuilder);

        //    modelBuilder.Entity<AddressesCustomer>(entity =>
        //    {
        //        entity.Property(e => e.Id).ValueGeneratedNever();

        //        entity.HasOne(d => d.City)
        //            .WithMany(p => p.AddressesCustomers)
        //            .HasForeignKey(d => d.CityId)
        //            .OnDelete(DeleteBehavior.ClientSetNull)
        //            .HasConstraintName("FK_AddressCustomers_Cities");

        //        entity.HasOne(d => d.District)
        //            .WithMany(p => p.AddressesCustomers)
        //            .HasForeignKey(d => d.DistrictId)
        //            .OnDelete(DeleteBehavior.ClientSetNull)
        //            .HasConstraintName("FK_AddressCustomers_Districts");

        //        entity.HasOne(d => d.Ward)
        //            .WithMany(p => p.AddressesCustomers)
        //            .HasForeignKey(d => d.WardId)
        //            .OnDelete(DeleteBehavior.ClientSetNull)
        //            .HasConstraintName("FK_AddressCustomers_Wards");
        //    });

        //    modelBuilder.Entity<Coupon>(entity =>
        //    {
        //        entity.Property(e => e.Id).ValueGeneratedNever();
        //    });

        //    modelBuilder.Entity<DeliveryAddress>(entity =>
        //    {
        //        entity.Property(e => e.Id).ValueGeneratedNever();

        //        entity.HasOne(d => d.City)
        //            .WithMany(p => p.DeliveryAddresses)
        //            .HasForeignKey(d => d.CityId)
        //            .OnDelete(DeleteBehavior.ClientSetNull)
        //            .HasConstraintName("FK_DeliveryAddresses_Cities");

        //        entity.HasOne(d => d.District)
        //            .WithMany(p => p.DeliveryAddresses)
        //            .HasForeignKey(d => d.DistrictId)
        //            .OnDelete(DeleteBehavior.ClientSetNull)
        //            .HasConstraintName("FK_DeliveryAddressess_Districts");

        //        entity.HasOne(d => d.Order)
        //            .WithMany(p => p.DeliveryAddresses)
        //            .HasForeignKey(d => d.OrderId)
        //            .HasConstraintName("FK_DeliveryAddress_Orders");

        //        entity.HasOne(d => d.Ward)
        //            .WithMany(p => p.DeliveryAddresses)
        //            .HasForeignKey(d => d.WardId)
        //            .OnDelete(DeleteBehavior.ClientSetNull)
        //            .HasConstraintName("FK_DeliveryAddresses_Wards");
        //    });

        //    modelBuilder.Entity<District>(entity =>
        //    {
        //        entity.HasOne(d => d.City)
        //            .WithMany(p => p.Districts)
        //            .HasForeignKey(d => d.CityId)
        //            .HasConstraintName("FK_Districts_Cities");
        //    });

        //    modelBuilder.Entity<Genre>(entity =>
        //    {
        //        entity.Property(e => e.Id).ValueGeneratedNever();
        //    });

        //    modelBuilder.Entity<MenuItem>(entity =>
        //    {
        //        entity.Property(e => e.Id).ValueGeneratedNever();

        //        entity.HasOne(d => d.Genre)
        //            .WithMany(p => p.MenuItems)
        //            .HasForeignKey(d => d.GenreId)
        //            .HasConstraintName("FK_MenuItems_Genres");

        //        entity.HasOne(d => d.Restaurant)
        //            .WithMany(p => p.MenuItems)
        //            .HasForeignKey(d => d.RestaurantId)
        //            .HasConstraintName("FK_MenuItems_Restaurants");
        //    });

        //    modelBuilder.Entity<MenuItemReview>(entity =>
        //    {
        //        entity.Property(e => e.Id).ValueGeneratedNever();

        //        entity.HasOne(d => d.MenuItem)
        //            .WithMany(p => p.MenuItemReviews)
        //            .HasForeignKey(d => d.MenuItemId)
        //            .HasConstraintName("FK_MenuItemReviews_MenuItems");

        //        entity.HasOne(d => d.Order)
        //            .WithMany(p => p.MenuItemReviews)
        //            .HasForeignKey(d => d.OrderId)
        //            .HasConstraintName("FK_MenuItemReviews_Orders");
        //    });

        //    modelBuilder.Entity<MenuVariant>(entity =>
        //    {
        //        entity.Property(e => e.Id).ValueGeneratedNever();

        //        entity.HasOne(d => d.MenuItem)
        //            .WithMany(p => p.MenuVariants)
        //            .HasForeignKey(d => d.MenuItemId)
        //            .HasConstraintName("FK_MenuVariants_MenuItems");
        //    });

        //    modelBuilder.Entity<Notification>(entity =>
        //    {
        //        entity.Property(e => e.Id).ValueGeneratedNever();
        //    });

        //    modelBuilder.Entity<Order>(entity =>
        //    {
        //        entity.Property(e => e.Id).ValueGeneratedNever();

        //        entity.Property(e => e.OrderStatus).HasDefaultValueSql("((0))");

        //        entity.Property(e => e.PaymentStatus).HasDefaultValueSql("((0))");

        //        entity.HasOne(d => d.DeliveryMethod)
        //            .WithMany(p => p.Orders)
        //            .HasForeignKey(d => d.DeliveryMethodId)
        //            .OnDelete(DeleteBehavior.SetNull)
        //            .HasConstraintName("FK_Orders_DeliveryMethods");
        //    });

        //    modelBuilder.Entity<OrderDelivery>(entity =>
        //    {
        //        entity.Property(e => e.Id).ValueGeneratedNever();

        //        entity.HasOne(d => d.Order)
        //            .WithMany(p => p.OrderDeliveries)
        //            .HasForeignKey(d => d.OrderId)
        //            .HasConstraintName("FK_OrderDeliveries_Orders");
        //    });

        //    modelBuilder.Entity<OrderDetail>(entity =>
        //    {
        //        entity.Property(e => e.Id).ValueGeneratedNever();

        //        entity.HasOne(d => d.MenuItem)
        //            .WithMany(p => p.OrderDetails)
        //            .HasForeignKey(d => d.MenuItemId)
        //            .HasConstraintName("FK_OrderDetails_MenuItems");

        //        entity.HasOne(d => d.MenuVariant)
        //            .WithMany(p => p.OrderDetails)
        //            .HasForeignKey(d => d.MenuVariantId)
        //            .HasConstraintName("FK_OrderDetails_MenuVariants");

        //        entity.HasOne(d => d.Order)
        //            .WithMany(p => p.OrderDetails)
        //            .HasForeignKey(d => d.OrderId)
        //            .HasConstraintName("FK_OrderDetails_Orders");
        //    });

        //    modelBuilder.Entity<Payment>(entity =>
        //    {
        //        entity.Property(e => e.Id).ValueGeneratedNever();

        //        entity.Property(e => e.PaymentStatus).HasDefaultValueSql("((0))");

        //        entity.HasOne(d => d.Order)
        //            .WithMany(p => p.Payments)
        //            .HasForeignKey(d => d.OrderId)
        //            .HasConstraintName("FK_Payments_Orders");
        //    });

        //    modelBuilder.Entity<Restaurant>(entity =>
        //    {
        //        entity.Property(e => e.Id).ValueGeneratedNever();

        //        entity.HasOne(d => d.City)
        //            .WithMany(p => p.Restaurants)
        //            .HasForeignKey(d => d.CityId)
        //            .OnDelete(DeleteBehavior.ClientSetNull)
        //            .HasConstraintName("FK_Restaurants_Cities");

        //        entity.HasOne(d => d.District)
        //            .WithMany(p => p.Restaurants)
        //            .HasForeignKey(d => d.DistrictId)
        //            .OnDelete(DeleteBehavior.ClientSetNull)
        //            .HasConstraintName("FK_Restaurants_Districts");

        //        entity.HasOne(d => d.Ward)
        //            .WithMany(p => p.Restaurants)
        //            .HasForeignKey(d => d.WardId)
        //            .OnDelete(DeleteBehavior.ClientSetNull)
        //            .HasConstraintName("FK_Restaurants_Wards");
        //    });

        //    modelBuilder.Entity<RestaurantHour>(entity =>
        //    {
        //        entity.Property(e => e.Id).ValueGeneratedNever();

        //        entity.HasOne(d => d.Restaurant)
        //            .WithMany(p => p.RestaurantHours)
        //            .HasForeignKey(d => d.RestaurantId)
        //            .HasConstraintName("FK_RestaurantHours_Restaurants");
        //    });

        //    modelBuilder.Entity<RestaurantReview>(entity =>
        //    {
        //        entity.Property(e => e.Id).ValueGeneratedNever();

        //        entity.HasOne(d => d.Order)
        //            .WithMany(p => p.RestaurantReviews)
        //            .HasForeignKey(d => d.OrderId)
        //            .HasConstraintName("FK_RestaurantReviews_Orders");

        //        entity.HasOne(d => d.Restaurant)
        //            .WithMany(p => p.RestaurantReviews)
        //            .HasForeignKey(d => d.RestaurantId)
        //            .HasConstraintName("FK_RestaurantReviews_Restaurants");
        //    });

        //    modelBuilder.Entity<Ward>(entity =>
        //    {
        //        entity.HasOne(d => d.District)
        //            .WithMany(p => p.Wards)
        //            .HasForeignKey(d => d.DistrictId)
        //            .HasConstraintName("FK_Wards_Districts");
        //    });

        //    modelBuilder.Entity<WishList>(entity =>
        //    {
        //        entity.Property(e => e.Id).ValueGeneratedNever();

        //        entity.HasOne(d => d.MenuItem)
        //            .WithMany(p => p.WishLists)
        //            .HasForeignKey(d => d.MenuItemId)
        //            .HasConstraintName("FK_WishLists_MenuItems");
        //    });

        //    OnModelCreatingPartial(modelBuilder);
        //}

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
