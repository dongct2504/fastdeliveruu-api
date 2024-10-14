﻿namespace FastDeliveruu.Application.Dtos.OrderDtos;

public class OrderHeaderDetailDto
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime? OrderDate { get; set; }

    public string? OrderDescription { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public string? TrackingNumber { get; set; }

    public string? OrderStatus { get; set; }

    public string? PaymentMethod { get; set; }

    public string? TransactionId { get; set; }

    public string Address { get; set; } = null!;

    public int WardId { get; set; }

    public int DistrictId { get; set; }

    public int CityId { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }

    public DeliveryMethodDto DeliveryMethodDto { get; set; } = null!;

    public IEnumerable<OrderDetailDto> OrderDetailDtos { get; set; } = null!;
}
