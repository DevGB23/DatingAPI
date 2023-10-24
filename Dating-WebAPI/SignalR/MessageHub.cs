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
        
        await AddToGroup(groupName);

        var messages = await _messageRepository.
            GetMessageThread(username, otherUser);

        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);
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

        var groupName = GetGroupName(sender.UserName, recipient.UserName);

        var group = await _messageRepository.GetMessageGroup(groupName);

        if (group is null) throw new HubException("No messages found in this group");

        if (group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }

        _messageRepository.AddMessage(message);

        if (await _messageRepository.SaveAllAsync())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDTO>(message));
        }
    }


    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await RemoveFromMessageGroup();
        await base.OnDisconnectedAsync(exception);
    }


    private string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    } 


    private async Task<bool> AddToGroup(string groupName)
    {
        string? username = Context.User?.GetUsername();

        var group = await _messageRepository.GetMessageGroup(groupName);
        var connection = new Connection(Context.ConnectionId, username);

        if (group is null) {
            group = new Group { Name = groupName };
            _messageRepository.AddGroup(group);
            await _messageRepository.SaveAllAsync();
            group.Connections.Add(connection);
        }else
        {
            group.Connections.Add(connection);
        }

        return true;
    }

    private async Task RemoveFromMessageGroup()
    {
        var connection = await _messageRepository.GetConnection(Context.ConnectionId);
        _messageRepository.RemoveConnection(connection);
        await _messageRepository.SaveAllAsync();
    }

}
