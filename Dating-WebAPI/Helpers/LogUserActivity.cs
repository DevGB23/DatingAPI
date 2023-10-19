using Dating_WebAPI.Extensions;
using Dating_WebAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Dating_WebAPI.Helpers;
public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

        var username = resultContext.HttpContext.User.GetUsername();

        var repo =  resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

        var user = await repo.GetAsync(includeProperties: "", tracked: true, u=>u.Username == username);
        user.LastActive = DateTime.UtcNow;

        await repo.SaveAsync();


    }
}
