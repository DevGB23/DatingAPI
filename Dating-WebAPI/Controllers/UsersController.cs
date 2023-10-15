using Dating_WebAPI.Data;
using Dating_WebAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dating_WebAPI.Controllers;

[Authorize]
public class UsersController : BaseApiController
{

    private readonly DataContext _context;

    public UsersController(DataContext context)
    {
        _context = context;               
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppUser>>> GetAllUsersAsync()
    {
        List<AppUser> userList = await _context.Users.ToListAsync();

        return userList;
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<AppUser>> GetUserAsync(int? id)
    {
        AppUser? user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            
        if ( user is null )
        {
            return NotFound($"User with ID {id} was not found");
        }
        return user;
    }
}