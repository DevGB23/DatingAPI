using Dating_WebAPI.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Dating_WebAPI.SignalR;

[Authorize]
public class PresenceHub : Hub
{
    public override async Task OnConnectedAsync() 
    {
        await Clients.Others.SendAsync("UsersOnline", Context.User.GetUsername);
    }

    public override async Task OnDisconnectedAsync(Exception exception)
        {
            await Clients.Others.SendAsync("UserOffLine", Context.User.GetUsername());

            await base.OnDisconnectedAsync(exception);
        }
}
