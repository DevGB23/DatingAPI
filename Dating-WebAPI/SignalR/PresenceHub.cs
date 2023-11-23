using Dating_WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;


namespace Dating_WebAPI.SignalR;

[Authorize]
public class PresenceHub : Hub
{
    private readonly PresenceTracker _tracker;
    public PresenceHub(PresenceTracker tracker)
    {
        _tracker = tracker;
        
    }
    public override async Task OnConnectedAsync() 
    {
        var username = Context.User?.GetUsername();

        if (string.IsNullOrEmpty(username)) return;

        bool isOnline = await _tracker.UserConnected(username, Context.ConnectionId);

        if (isOnline) await Clients.Others.SendAsync("UserIsOnline", username);

        var currentUsers = await _tracker.GetOnlineUsers();

        await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

        await base.OnConnectedAsync();
    }


    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var username = Context.User?.GetUsername();
        await Clients.Others.SendAsync("UserIsOffline", username);

        if (string.IsNullOrEmpty(username)) return;

        var isOffline = await _tracker.UserDisconnected(username, Context.ConnectionId);
        var currentUsers = await _tracker.GetOnlineUsers();


        // await Clients.All.SendAsync("GetOnlineUsers", currentUsers);

        foreach (var item in currentUsers)
        {
            Console.WriteLine($"CurrentOnlineUsers: {item}");
                        
        }

        if (isOffline) await Clients.All.SendAsync("GetOnlineUsers", currentUsers);


        if (exception == null)
        {
            Console.WriteLine("Connection closed without error.");
        }
        else
        {
            
            Console.WriteLine($"Connection closed due to an error: {exception}");
        }

        await base.OnDisconnectedAsync(exception);
    }
}
