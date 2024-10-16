﻿using FastDeliveruu.Domain.Entities.AutoGenEntities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FastDeliveruu.Domain.Entities.Identity;

public class AppUser : IdentityUser<Guid>
{
    [StringLength(50)]
    public string? FirstName { get; set; }

    [StringLength(50)]
    public string? LastName { get; set; }

    [StringLength(255)]
    [Unicode(false)]
    public string? ImageUrl { get; set; }

    [StringLength(256)]
    [Unicode(false)]
    public string? PublicId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreatedAt { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("AppUser")]
    public virtual ICollection<WishList> WishLists { get; set; } = new HashSet<WishList>();
    [InverseProperty("AppUser")]
    public virtual ICollection<AddressesCustomer> AddressesCustomers { get; set; } = new HashSet<AddressesCustomer>();
    [InverseProperty("AppUser")]
    public virtual ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    [InverseProperty("AppUser")]
    public virtual ICollection<RestaurantReview> RestaurantReviews { get; set; } = new HashSet<RestaurantReview>();
    [InverseProperty("AppUser")]
    public virtual ICollection<MenuItemReview> MenuItemReviews { get; set; } = new HashSet<MenuItemReview>();
    [InverseProperty("AppUser")]
    public virtual ICollection<AppUserNotification> AppUserNotifications { get; set; } = new HashSet<AppUserNotification>();

    [InverseProperty("AppUser")]
    public virtual ICollection<Coupon> Coupons { get; set; } = new HashSet<Coupon>();

    [InverseProperty("AppUser")]
    public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new HashSet<ShoppingCart>();

    [InverseProperty("SenderAppUser")]
    public virtual ICollection<MessageThread> SenderMessageThreads { get; set; } = new HashSet<MessageThread>();

    [InverseProperty("RecipientAppUser")]
    public virtual ICollection<MessageThread> RecipientMessageThreads { get; set; } = new HashSet<MessageThread>();

    // identity
    public virtual ICollection<AppUserRole> AppUserRoles { get; set; } = new List<AppUserRole>();
}
