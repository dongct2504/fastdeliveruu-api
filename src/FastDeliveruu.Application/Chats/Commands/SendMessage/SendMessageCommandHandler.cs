using FastDeliveruu.Common.Constants;
using FastDeliveruu.Common.Enums;
using FastDeliveruu.Application.Common.Helpers;
using FastDeliveruu.Application.Dtos.ChatDtos;
using FastDeliveruu.Application.Interfaces;
using FastDeliveruu.Domain.Entities.AutoGenEntities;
using FastDeliveruu.Domain.Entities.Identity;
using FastDeliveruu.Domain.Interfaces;
using FastDeliveruu.Domain.Specifications.Chats;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace FastDeliveruu.Application.Chats.Commands.SendMessage;

public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, MessageDto>
{
    private readonly IFastDeliveruuUnitOfWork _unitOfWork;
    private readonly ICacheService _cacheService;
    private readonly UserManager<AppUser> _appUserManager;
    private readonly UserManager<Shipper> _ShipperManager;
    private readonly IDateTimeProvider _dateTimeProvider;

    public SendMessageCommandHandler(
        IFastDeliveruuUnitOfWork unitOfWork,
        ICacheService cacheService,
        UserManager<AppUser> appUserManager,
        UserManager<Shipper> shipperManager,
        IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _cacheService = cacheService;
        _appUserManager = appUserManager;
        _ShipperManager = shipperManager;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<MessageDto> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        Shipper? shipperSender = null;
        Shipper? shipperRecipient = null;
        AppUser? appUserSender = null;
        AppUser? appUserRecipient = null;

        Guid senderId = request.SenderId;
        Guid recipientId = request.RecipientId;

        SenderRecipientTypeEnum senderType = request.SenderType;
        SenderRecipientTypeEnum recipientType = request.RecipientType;
        string content = request.Content;

        switch (request.SenderType)
        {
            case SenderRecipientTypeEnum.Shipper:
                shipperSender = await _ShipperManager.FindByIdAsync(senderId.ToString());
                if (shipperSender == null)
                {
                    throw new Exception("Shipper not found");
                }
                break;

            case SenderRecipientTypeEnum.Customer:
                appUserSender = await _appUserManager.FindByIdAsync(senderId.ToString());
                if (appUserSender == null)
                {
                    throw new Exception("User not found");
                }
                break;

            default:
                throw new Exception("Invalid sender enum");
        }

        switch (recipientType)
        {
            case SenderRecipientTypeEnum.Shipper:
                shipperRecipient = await _ShipperManager.FindByIdAsync(recipientId.ToString());
                if (shipperRecipient == null)
                {
                    throw new Exception("Shipper not found");
                }
                break;

            case SenderRecipientTypeEnum.Customer:
                appUserRecipient = await _appUserManager.FindByIdAsync(recipientId.ToString());
                if (appUserRecipient == null)
                {
                    throw new Exception("User not found");
                }
                break;

            default:
                throw new Exception("Invalid recipient enum");
        }

        MessageThread? messageThread = await _unitOfWork.MessageThreads
            .GetWithSpecAsync(new MessageThreadsBySenderIdOrRecipientIdSpecification(senderId, recipientId));
        if (messageThread == null)
        {
            messageThread = new MessageThread
            {
                Id = Guid.NewGuid(),
                Title = recipientType == SenderRecipientTypeEnum.Shipper
                    ? shipperRecipient!.UserName
                    : appUserRecipient!.UserName,
                SenderShipperId = shipperSender != null ? shipperSender.Id : null,
                SenderAppUserId = appUserSender != null ? appUserSender.Id : null,

                RecipientShipperId = shipperRecipient != null ? shipperRecipient.Id : null,
                RecipientAppUserId = appUserRecipient != null ? appUserRecipient.Id : null,

                SenderType = (byte)(senderType == SenderRecipientTypeEnum.Shipper
                    ? SenderRecipientTypeEnum.Shipper
                    : SenderRecipientTypeEnum.Customer),
                RecipientType = (byte)(recipientType == SenderRecipientTypeEnum.Shipper
                    ? SenderRecipientTypeEnum.Shipper
                    : SenderRecipientTypeEnum.Customer),
                CreatedAt = _dateTimeProvider.VietnamDateTimeNow
            };
            _unitOfWork.MessageThreads.Add(messageThread);
        }
        else
        {
            messageThread.UpdatedAt = _dateTimeProvider.VietnamDateTimeNow;
        }

        Chat chat = new Chat()
        {
            Id = Guid.NewGuid(),
            ThreadId = messageThread.Id,
            Content = content,
            DateSent = _dateTimeProvider.VietnamDateTimeNow,
            CreatedAt = _dateTimeProvider.VietnamDateTimeNow,
        };

        string groupName = Utils.GetGroupName(senderId, recipientId);

        string key = $"{CacheConstants.Group}-{groupName}";
        List<Guid> groupMembers = await _cacheService.GetAsync<List<Guid>>(key) ?? new List<Guid>();

        if (messageThread.RecipientShipperId.HasValue)
        {
            if (groupMembers.Contains(messageThread.RecipientShipperId.Value))
            {
                chat.DateRead = _dateTimeProvider.VietnamDateTimeNow;
            }
            //else
            //{
            //    // send notification if the user is online AND not in the chat group
            //    List<string>? connectionIds = await _onlineTrackerService
            //        .GetConnectionIdsForUserAsync(messageThread.RecipientId);
            //    if (connectionIds != null)
            //    {
            //        await _onlineHubContext.Clients.Clients(connectionIds).SendAsync("NewMessage",
            //            new
            //            {
            //                senderId = messageThread.SenderId,
            //                senderUserName = senderEnum == SenderRecipientTypeEnum.Shipper
            //                        ? shipperRecipient!.UserName
            //                        : appUserRecipient!.UserName
            //            });
            //    }
            //}
        }
        else if (messageThread.RecipientAppUserId.HasValue)
        {
            if (groupMembers.Contains(messageThread.RecipientAppUserId.Value))
            {
                chat.DateRead = _dateTimeProvider.VietnamDateTimeNow;
            }
            //else
            //{
            //    // send notification if the user is online AND not in the chat group
            //    List<string>? connectionIds = await _onlineTrackerService
            //        .GetConnectionIdsForUserAsync(messageThread.RecipientId);
            //    if (connectionIds != null)
            //    {
            //        await _onlineHubContext.Clients.Clients(connectionIds).SendAsync("NewMessage",
            //            new
            //            {
            //                senderId = messageThread.SenderId,
            //                senderUserName = senderEnum == SenderRecipientTypeEnum.Shipper
            //                        ? shipperRecipient!.UserName
            //                        : appUserRecipient!.UserName
            //            });
            //    }
            //}
        }

        _unitOfWork.Chats.Add(chat);

        await _unitOfWork.SaveChangesAsync();

        MessageDto messageDto = new MessageDto();

        messageDto.Content = chat.Content;
        messageDto.DateRead = chat.DateRead;
        messageDto.DateSent = chat.DateSent;

        messageDto.SenderId = senderType == SenderRecipientTypeEnum.Shipper
            ? shipperSender!.Id
            : appUserSender!.Id;
        messageDto.SenderUserName = senderType == SenderRecipientTypeEnum.Shipper
            ? shipperSender!.UserName
            : appUserSender!.UserName;
        messageDto.SenderImageUrl = senderType == SenderRecipientTypeEnum.Shipper
            ? shipperSender!.ImageUrl
            : appUserSender!.ImageUrl;

        messageDto.RecipientId = recipientType == SenderRecipientTypeEnum.Shipper
            ? shipperRecipient!.Id
            : appUserRecipient!.Id;
        messageDto.RecipientUserName = recipientType == SenderRecipientTypeEnum.Shipper
            ? shipperRecipient!.UserName
            : appUserRecipient!.UserName;
        messageDto.RecipientImageUrl = recipientType == SenderRecipientTypeEnum.Shipper
            ? shipperRecipient!.ImageUrl
            : appUserRecipient!.ImageUrl;

        return messageDto;
    }
}
