using System.Security.Cryptography;
using System.Text;
using Dating_WebAPI.Data;
using Dating_WebAPI.DTOs;
using Dating_WebAPI.Entities;
using Dating_WebAPI.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dating_WebAPI.Controllers;
public class AccountController : BaseApiController
{
    private readonly DataContext _context;
    private readonly ITokenService _tokenService;

    public AccountController(DataContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    [HttpPost("register")] // Post: api.account/register
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
    {
        if (await UserExists(registerDTO.Username)) return BadRequest("Username is already taken");

        using var hmac = new HMACSHA512();

        AppUser user = new() {
            Username = registerDTO.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
            PasswordSalt = hmac.Key
        };  

        _context.Add(user);
        await _context.SaveChangesAsync();

        return new UserDTO
        {
            Username = user.Username,
            Token = _tokenService.CreateToken(user)
        };
    }

    [HttpPost("login")] // Post: api.account/register
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
    {
        AppUser? user = await _context.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.Username == loginDTO.Username);

        if ( user is null) return Unauthorized("Invalid Username");

        if ( user.PasswordSalt is not null && user.PasswordHash is not null )
        {
            using var hmac = new HMACSHA512(user.PasswordSalt);
            var computeHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for ( int i = 0; i < computeHash.Length; i++)
            {
                if (computeHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }
        }
        
         return new UserDTO
        {
            Username = user.Username,
            Token = _tokenService.CreateToken(user),
            PhotoUrl = user.Photos?.FirstOrDefault(p => p.IsMain)?.ImageUrl
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await _context.Users.AnyAsync(x => x.Username == username.ToLower());
    }


}
