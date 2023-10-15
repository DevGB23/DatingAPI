using Dating_WebAPI.Entities;

namespace Dating_WebAPI.Interfaces;
public interface ITokenService
{
    string CreateToken(AppUser user);
}
