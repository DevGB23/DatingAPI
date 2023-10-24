using AutoMapper;
using Dating_WebAPI.DTOs;
using Dating_WebAPI.Entities;
using Dating_WebAPI.Extensions;
using Dating_WebAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Dating_WebAPI.SignalR;

[Authorize]
public class MessageHub : Hub
{
    private readonly IMapper _mapper;
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;

    public MessageHub(IMapper mapper, IMessageRepository messageRepository, IUserRepository userRepository)
    {
        _mapper = mapper;
        _messageRepository = messageRepository;
        _userRepository = userRepository;
    }

    public override async Task OnConnectedAsync()
    {
        var username = Context.User?.GetUsername();

        if (string.IsNullOrEmpty(username)) return;

        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext.Request.Query["user"].ToString();
        var groupName = GetGroupName(username, otherUser);

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        //  await Groups.AddToGroupAsync(groupName);
        
        // await Clients.Group(groupName).SendAsync("UpdatedGroup", group);


        var messages = await _messageRepository.
            GetMessageThread(username, otherUser);

        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);

        // return base.OnConnectedAsync();
    }


    public async Task SendMessage(CreateMessageDTO createMessageDTO)
    {
        string? username = Context.User?.GetUsername();

        if (string.IsNullOrEmpty(username)) return;
            
        if (username == createMessageDTO?.RecipientUsername?.ToLower())
            throw new HubException("You cannot send messages to yourself");

        AppUser? sender = await _userRepository.GetAsync(includeProperties: "", tracked: true, u => u.UserName == username);
        AppUser? recipient = await _userRepository.GetAsync(includeProperties: "", tracked: true, u => u.UserName == createMessageDTO.RecipientUsername);

        if (recipient is null) throw new HubException("Not found User");

        Message message = new() 
        {
            Sender = sender,
            SenderUsername = username,
            Recipient = recipient,
            RecipientUsername = recipient.UserName,
            Content = createMessageDTO.Content
        };

        _messageRepository.AddMessage(message);

        if (await _messageRepository.SaveAllAsync())
        {
            var group = GetGroupName(sender.UserName, recipient.UserName);
            await Clients.Group(group).SendAsync("NewMessage", _mapper.Map<MessageDTO>(message));
        }
    }


    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // var group = await RemoveFromMessageGroup();
        // await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
        await base.OnDisconnectedAsync(exception);
    }


    private string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    } 
}
