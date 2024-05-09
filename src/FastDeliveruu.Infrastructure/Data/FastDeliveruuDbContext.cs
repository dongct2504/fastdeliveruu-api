using System;
using System.Collections.Generic;
using FastDeliveruu.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace FastDeliveruu.Infrastructure.Data
{
    public partial class FastDeliveruuDbContext : DbContext
    {
        public FastDeliveruuDbContext()
        {
        }

        public FastDeliveruuDbContext(DbContextOptions<FastDeliveruuDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Genre> Genres { get; set; } = null!;
        public virtual DbSet<LocalUser> LocalUsers { get; set; } = null!;
        public virtual DbSet<MenuItem> MenuItems { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public virtual DbSet<Restaurant> Restaurants { get; set; } = null!;
        public virtual DbSet<Shipper> Shippers { get; set; } = null!;
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Genre>(entity =>
            {
                entity.Property(e => e.GenreId).ValueGeneratedNever();
            });

            modelBuilder.Entity<LocalUser>(entity =>
            {
                entity.Property(e => e.LocalUserId).ValueGeneratedNever();

                entity.Property(e => e.PhoneNumber).IsFixedLength();
            });

            modelBuilder.Entity<MenuItem>(entity =>
            {
                entity.Property(e => e.MenuItemId).ValueGeneratedNever();

                entity.HasOne(d => d.Genre)
                    .WithMany(p => p.MenuItems)
                    .HasForeignKey(d => d.GenreId)
                    .HasConstraintName("FK_MENUITEM_MENUITEMG_GENRES");

                entity.HasOne(d => d.Restaurant)
                    .WithMany(p => p.MenuItems)
                    .HasForeignKey(d => d.RestaurantId)
                    .HasConstraintName("FK_MENUITEM_MENUITEMR_RESTAURA");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderId).ValueGeneratedNever();

                entity.Property(e => e.PhoneNumber).IsFixedLength();

                entity.HasOne(d => d.LocalUser)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.LocalUserId)
                    .HasConstraintName("FK_ORDERS_ORDERLOCA_LOCALUSE");

                entity.HasOne(d => d.Shipper)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ShipperId)
                    .HasConstraintName("FK_ORDERS_ORDERSHIP_SHIPPERS");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => new { e.MenuItemId, e.OrderId })
                    .HasName("PK_ORDERDETAILS");

                entity.HasOne(d => d.MenuItem)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.MenuItemId)
                    .HasConstraintName("FK_ORDERDET_ORDERDETA_MENUITEM");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_ORDERDET_ORDERDETA_ORDERS");
            });

            modelBuilder.Entity<Restaurant>(entity =>
            {
                entity.Property(e => e.RestaurantId).ValueGeneratedNever();

                entity.Property(e => e.PhoneNumber).IsFixedLength();
            });

            modelBuilder.Entity<Shipper>(entity =>
            {
                entity.Property(e => e.ShipperId).ValueGeneratedNever();

                entity.Property(e => e.PhoneNumber).IsFixedLength();
            });

            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.HasKey(e => new { e.MenuItemId, e.LocalUserId })
                    .HasName("PK_SHOPPINGCARTS");

                entity.HasOne(d => d.LocalUser)
                    .WithMany(p => p.ShoppingCarts)
                    .HasForeignKey(d => d.LocalUserId)
                    .HasConstraintName("FK_SHOPPING_SHOPPINGC_LOCALUSE");

                entity.HasOne(d => d.MenuItem)
                    .WithMany(p => p.ShoppingCarts)
                    .HasForeignKey(d => d.MenuItemId)
                    .HasConstraintName("FK_SHOPPING_SHOPPINGC_MENUITEM");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
