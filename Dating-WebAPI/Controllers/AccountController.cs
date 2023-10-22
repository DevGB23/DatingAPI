using System.Security.Cryptography;
using System.Text;
using AutoMapper;
using Dating_WebAPI.Data;
using Dating_WebAPI.DTOs;
using Dating_WebAPI.Entities;
using Dating_WebAPI.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Dating_WebAPI.Controllers;
public class AccountController : BaseApiController
{
    private readonly UserManager<AppUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AccountController(UserManager<AppUser> userManager, ITokenService tokenService, IMapper mapper)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    [HttpPost("register")] // Post: api.account/register
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO)
    {
        if (await UserExists(registerDTO.Username)) return BadRequest("Username is already taken");

        AppUser? user = _mapper.Map<AppUser>(registerDTO);

        user.UserName = registerDTO.Username.ToLower();

        var result = await _userManager.CreateAsync(user, registerDTO.Password);

        if (!result.Succeeded) return BadRequest(result.Errors);

        var roleResult = await _userManager.AddToRoleAsync(user, "Member");

        if (!roleResult.Succeeded) return BadRequest(result.Errors);
    
        return new UserDTO
        {
            Username = user.UserName,
            Token = await _tokenService.CreateToken(user),
            KnownAs = user.KnownAs,
            PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain)?.ImageUrl
        };
    }

    [HttpPost("login")] // Post: api.account/login
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
    {
        AppUser? user = await _userManager.Users
            .Include(p => p.Photos)
            .SingleOrDefaultAsync(x => x.UserName == loginDTO.Username);

        if ( user is null) return Unauthorized("Invalid Username");

        var result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

        if (!result) return Unauthorized("Invalid password");

         return new UserDTO
        {
            Username = user.UserName,
            Token = await _tokenService.CreateToken(user),
            PhotoUrl = user.Photos?.FirstOrDefault(p => p.IsMain)?.ImageUrl,
            Gender = user.Gender
        };
    }

    private async Task<bool> UserExists(string username)
    {
        return await _userManager.Users.AnyAsync(x => x.UserName == username.ToLower());
    }


}
