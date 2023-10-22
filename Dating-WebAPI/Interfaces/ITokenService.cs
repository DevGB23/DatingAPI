using Dating_WebAPI.Entities;

namespace Dating_WebAPI.Interfaces;
public interface ITokenService
{
    Task<string> CreateToken(AppUser user);
}
