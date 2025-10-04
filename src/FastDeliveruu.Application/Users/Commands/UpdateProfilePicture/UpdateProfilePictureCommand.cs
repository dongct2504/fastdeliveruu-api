using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace FastDeliveruu.Application.Users.Commands.UpdateProfilePicture;

public class UpdateProfilePictureCommand : IRequest<Result>
{
    public Guid Id { get; set; }
    public IFormFile ImageFile { get; set; } = null!;
}
