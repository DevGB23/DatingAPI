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

        var userId = resultContext.HttpContext.User.GetUserId();
        var repo =  resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

        var user = await repo.GetAsync(includeProperties: "", tracked: true, u=>u.Id == userId);
        user.LastActive = DateTime.UtcNow;

        await repo.SaveAsync();
    }
}
