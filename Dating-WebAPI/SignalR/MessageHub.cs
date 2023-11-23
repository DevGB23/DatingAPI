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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHubContext<PresenceHub> _presenceHub;
    private readonly PresenceTracker _tracker;

    public MessageHub(
        IMapper mapper, 
        IUnitOfWork unitOfWork,
        IHubContext<PresenceHub> presenceHub,
        PresenceTracker tracker)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _presenceHub = presenceHub;
        _tracker = tracker;
    }

    public override async Task OnConnectedAsync()
    {
        var username = Context.User?.GetUsername();

        Console.WriteLine($"MessageHub: {username} - Connection: {Context.ConnectionId}");

        if (string.IsNullOrEmpty(username)) return;

        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext.Request.Query["user"].ToString();
        var groupName = GetGroupName(username, otherUser);

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        Group group = await AddToGroup(groupName);

        await Clients.Group(groupName).SendAsync("UpdatedGroup", group);
        
        var messages = await _unitOfWork.MessageRepository.GetMessageThread(username, otherUser);

        if (_unitOfWork.HasChanges()) await _unitOfWork.Complete();

        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }


    public async Task SendMessage(CreateMessageDTO createMessageDTO)
    {
        string? username = Context.User?.GetUsername();

        if (string.IsNullOrEmpty(username)) return;
            
        if (username == createMessageDTO?.RecipientUsername?.ToLower())
            throw new HubException("You cannot send messages to yourself");

        AppUser? sender = await _unitOfWork.UserRepository.GetAsync(
            includeProperties: "", tracked: true, u => u.UserName == username);
        AppUser? recipient = await _unitOfWork.UserRepository.GetAsync(
            includeProperties: "", tracked: true, u => u.UserName == createMessageDTO.RecipientUsername);

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

        var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);

        if (group is null) throw new HubException("No messages found in this group");

        if (group.Connections.Any(x => x.Username == recipient.UserName))
        {
            message.DateRead = DateTime.UtcNow;
        }
        else 
        {
            var connections = await PresenceTracker.GetConnectionsForUser(recipient.UserName);

            if (connections is not null)
            {
                await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", 
                    new {username = sender.UserName, knownAs = sender.KnownAs});
            }
        }

        _unitOfWork.MessageRepository.AddMessage(message);

        if (await _unitOfWork.Complete())
        {
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDTO>(message));
        }
    }


    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // if (!string.IsNullOrEmpty(Context.ConnectionId)) return;
        // Console.WriteLine($"OnDisconneted: {Context.ConnectionId}");
        
        Group group = await RemoveFromMessageGroup();
        await Clients.Groups(group.Name).SendAsync("UpdatedGroup");
        await base.OnDisconnectedAsync(exception);
    }


    private string GetGroupName(string caller, string other)
    {
        var stringCompare = string.CompareOrdinal(caller, other) < 0;
        return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
    } 


    private async Task<Group> AddToGroup(string groupName)
    {
        string? username = Context.User?.GetUsername();

        var group = await _unitOfWork.MessageRepository.GetMessageGroup(groupName);
        Connection connection = new Connection(Context.ConnectionId, username);

        if (group is null) {
            group = new Group { Name = groupName };
            _unitOfWork.MessageRepository.AddGroup(group);
            group.Connections.Add(connection);
        }
        else 
        {
            group.Connections.Add(connection);                
        }

        if (await _unitOfWork.Complete()) return group;

        throw new HubException("Failed to add to group");
    }

    private async Task<Group> RemoveFromMessageGroup()
    {
        Group group = await _unitOfWork.MessageRepository.GetGroupforConnection(Context.ConnectionId);
        
        Connection? connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId) 
            ?? throw new HubException("This group does not have this connection");

        _unitOfWork.MessageRepository.RemoveConnection(connection);

        if (await _unitOfWork.Complete()) return group;

        throw new HubException("Failed to remove from Group");
    }

}
