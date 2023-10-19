using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dating_WebAPI.Entities;
using Dating_WebAPI.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace Dating_WebAPI.Services;
public class TokenService : ITokenService
{
    private readonly IConfiguration _config;
    private readonly SymmetricSecurityKey _key;

    public TokenService(IConfiguration config)
    {
        _config = config;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
    }
    public string CreateToken(AppUser user)
    {
        var cp = new ClaimsPrincipal();

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username)
        };

        ClaimsIdentity claimsIdentity = new (claims);

        var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = claimsIdentity,
            Expires = DateTime.Now.AddDays(7),
            SigningCredentials = creds
        };

        var cl = claimsIdentity.Claims;

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}
