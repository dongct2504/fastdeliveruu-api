using FastDeliveruu.Application.Common.Enums;

namespace FastDeliveruu.Application.Dtos.OrderDtos;

public class OrderDto
{
    public Guid Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateTime? OrderDate { get; set; }

    public string PhoneNumber { get; set; } = null!;

    public decimal TotalAmount { get; set; }

    public string DeliveryMethodShortName { get; set; } = null!;

    public decimal ShippingPrice { get; set; }

    public OrderStatusEnum? OrderStatus { get; set; }

    public PaymentMethodsEnum? PaymentMethod { get; set; }

    public string? TransactionId { get; set; }

    public string HouseNumber { get; set; } = null!;
    public string StreetName { get; set; } = null!;

    public int CityId { get; set; }
    public int DistrictId { get; set; }
    public int WardId { get; set; }

    public decimal? Latitude { get; set; }

    public decimal? Longitude { get; set; }
}