using System;
using System.Collections.Generic;
using FastDeliveruu.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FastDeliveruu.Infrastructure.Data;

public partial class FastdeliveruuContext : DbContext
{
    public FastdeliveruuContext()
    {
    }

    public FastdeliveruuContext(DbContextOptions<FastdeliveruuContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Genre> Genres { get; set; }

    public virtual DbSet<LocalUser> LocalUsers { get; set; }

    public virtual DbSet<MenuItem> MenuItems { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderDetail> OrderDetails { get; set; }

    public virtual DbSet<Restaurant> Restaurants { get; set; }

    public virtual DbSet<Shipper> Shippers { get; set; }

    public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Genre>(entity =>
        {
            entity.HasKey(e => e.GenreId).HasName("PK_GENRES");
        });

        modelBuilder.Entity<LocalUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_LOCALUSERS");

            entity.Property(e => e.PhoneNumber).IsFixedLength();
        });

        modelBuilder.Entity<MenuItem>(entity =>
        {
            entity.HasKey(e => e.MenuItemId).HasName("PK_MENUITEMS");

            entity.HasOne(d => d.Genre).WithMany(p => p.MenuItems)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_MENUITEM_MENUITEMG_GENRES");

            entity.HasOne(d => d.Restaurant).WithMany(p => p.MenuItems)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_MENUITEM_MENUITEMR_RESTAURA");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId).HasName("PK_ORDERS");

            entity.Property(e => e.PhoneNumber).IsFixedLength();

            entity.HasOne(d => d.Shipper).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ORDERS_ORDERSHIP_SHIPPERS");

            entity.HasOne(d => d.User).WithMany(p => p.Orders)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ORDERS_ORDERLOCA_LOCALUSE");
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => e.OrderDetailId).HasName("PK_ORDERDETAILS");

            entity.HasOne(d => d.MenuItem).WithMany(p => p.OrderDetails)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ORDERDET_ORDERDETA_MENUITEM");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderDetails)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_ORDERDET_ORDERDETA_ORDERS");
        });

        modelBuilder.Entity<Restaurant>(entity =>
        {
            entity.HasKey(e => e.RestaurantId).HasName("PK_RESTAURANTS");

            entity.Property(e => e.PhoneNumber).IsFixedLength();
        });

        modelBuilder.Entity<Shipper>(entity =>
        {
            entity.HasKey(e => e.ShipperId).HasName("PK_SHIPPERS");

            entity.Property(e => e.PhoneNumber).IsFixedLength();
        });

        modelBuilder.Entity<ShoppingCart>(entity =>
        {
            entity.HasKey(e => e.ShoppingCartId).HasName("PK_SHOPPINGCARTS");

            entity.HasOne(d => d.MenuItem).WithMany(p => p.ShoppingCarts)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SHOPPING_SHOPPINGC_MENUITEM");

            entity.HasOne(d => d.User).WithMany(p => p.ShoppingCarts)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_SHOPPING_SHOPPINGC_LOCALUSE");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
