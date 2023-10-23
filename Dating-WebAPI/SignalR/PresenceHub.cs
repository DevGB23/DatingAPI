using Dating_WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;


namespace Dating_WebAPI.SignalR;

[Authorize]
public class PresenceHub : Hub
{
    public override async Task OnConnectedAsync() 
    {
        var username = Context.User?.GetUsername();
        await Clients.Others.SendAsync("UserIsOnline", username);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var username = Context.User?.GetUsername();
        await Clients.Others.SendAsync("UserIsOffline", username);

        if (exception == null)
        {
            await base.OnDisconnectedAsync(exception);
            Console.WriteLine("Connection closed without error.");
        }
        else
        {
            Console.WriteLine($"Connection closed due to an error: {exception}");
        }

    }
}
