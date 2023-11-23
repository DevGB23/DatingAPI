using System;
using System.Threading.Tasks;
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
        var unitOfWork =  resultContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();

        var user = await unitOfWork.UserRepository.GetAsync(includeProperties: "", tracked: true, u=>u.Id == userId);
        user.LastActive = DateTime.UtcNow;

        await unitOfWork.Complete();
    }
}
