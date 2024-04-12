using System.Net;

namespace FastDeliveruu.Application.Dtos;

public class ApiResponse
{
    public HttpStatusCode HttpStatusCode { get; set; }

    public bool IsSuccess { get; set; }

    public List<string> ErrorMessages { get; set; } = new List<string>();

    public object? Result { get; set; }
}
