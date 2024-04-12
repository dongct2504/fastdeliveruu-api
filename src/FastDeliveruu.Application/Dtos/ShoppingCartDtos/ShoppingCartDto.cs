namespace FastDeliveruu.Application.Dtos.ShoppingCartDtos;

public class ShoppingCartDto
{
    public int ShoppingCartId { get; set; }

    public int MenuItemId { get; set; }

    public int LocalUserId { get; set; }

    public int Quantity { get; set; }
}