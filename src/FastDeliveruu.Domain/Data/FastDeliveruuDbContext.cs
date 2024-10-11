using System;
using System.Collections.Generic;
using FastDeliveruu.Domain.Entities;
using FastDeliveruu.Domain.Entities.AutoGenEntities;
using FastDeliveruu.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FastDeliveruu.Domain.Data
{
    //public partial class FastDeliveruuDbContext : IdentityDbContext<AppUser, AppRole, Guid> // for not dealing with roles
    public partial class FastDeliveruuDbContext : IdentityDbContext<AppUser, AppRole, Guid,
        IdentityUserClaim<Guid>, AppUserRole, IdentityUserLogin<Guid>,
        IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
    {
        public FastDeliveruuDbContext(DbContextOptions<FastDeliveruuDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AddressesCustomer> AddressesCustomers { get; set; } = null!;
        public virtual DbSet<City> Cities { get; set; } = null!;
        public virtual DbSet<Coupon> Coupons { get; set; } = null!;
        public virtual DbSet<DeliveryMethod> DeliveryMethods { get; set; } = null!;
        public virtual DbSet<District> Districts { get; set; } = null!;
        public virtual DbSet<Genre> Genres { get; set; } = null!;
        public virtual DbSet<MenuItem> MenuItems { get; set; } = null!;
        public virtual DbSet<MenuItemReview> MenuItemReviews { get; set; } = null!;
        public virtual DbSet<MenuVariant> MenuVariants { get; set; } = null!;
        public virtual DbSet<MenuItemInventory> MenuItemInventories { get; set; } = null!;
        public virtual DbSet<MenuVariantInventory> MenuVariantInventories { get; set; } = null!;
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; } = null!;
        public virtual DbSet<AppUserNotification> AppUserNotifications { get; set; } = null!;
        public virtual DbSet<ShipperNotification> ShipperNotifications { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDelivery> OrderDeliveries { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<Payment> Payments { get; set; } = null!;
        public virtual DbSet<Restaurant> Restaurants { get; set; } = null!;
        public virtual DbSet<RestaurantHour> RestaurantHours { get; set; } = null!;
        public virtual DbSet<RestaurantReview> RestaurantReviews { get; set; } = null!;
        public virtual DbSet<Ward> Wards { get; set; } = null!;
        public virtual DbSet<WishList> WishLists { get; set; } = null!;
        public virtual DbSet<MessageThread> MessageThreads { get; set; } = null!;
        public virtual DbSet<Chat> Chats { get; set; } = null!;

        // identity related
        public virtual DbSet<Shipper> Shippers { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

        //partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
