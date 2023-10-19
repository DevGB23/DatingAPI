using Dating_WebAPI.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Dating_WebAPI.Controllers;

[ServiceFilter(typeof(LogUserActivity))]
[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
    
}
