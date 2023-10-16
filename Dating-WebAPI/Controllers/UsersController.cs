using AutoMapper;
using Dating_WebAPI.DTOs;
using Dating_WebAPI.Entities;
using Dating_WebAPI.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dating_WebAPI.Controllers;

[Authorize]
public class UsersController : BaseApiController
{

    private readonly IUserRepository _repo;
    private readonly IMapper _mapper;

    public UsersController(IUserRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MembersDTO>>> GetAllUsersAsync()
    {
        IEnumerable<MembersDTO> userList = await _repo.GetMembersAsync();

        return Ok(userList);
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MembersDTO>> GetUserByIdAsync(int id)
    {
        MembersDTO? member = await _repo.GetMemberByIdAsync(id);
            
        if ( member is null ) return NotFound($"User with ID {id} was not found");

        return Ok(member);
    }

    [AllowAnonymous]
    [HttpGet("{username}")]
    public async Task<ActionResult<MembersDTO>> GetUserByUsernameAsync(string username)
    {
        MembersDTO? member = await _repo.GetMemberByNameAsync(username);
            
        if ( member is null ) return NotFound($"User with ID {username} was not found");
        
        return Ok(member);
    }
}