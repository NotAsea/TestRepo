using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace TestRepo.Util;

public sealed class GenerateJwtToken(IConfiguration configuration)
{
    public ValueTask<string> GetToken(int personId, string personName)
    {
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        var key = Encoding.UTF8.GetBytes(
            configuration["Jwt:Key"]
                ?? throw new Exception("Not found Secret key in appsettings.json")
        );
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
                [
                    new Claim("Id", personId.ToString()),
                    new Claim(JwtRegisteredClaimNames.Name, personName)
                ]
            ),
            Expires = DateTime.UtcNow.AddDays(7),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature
            )
        };
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return ValueTask.FromResult(tokenHandler.WriteToken(token));
    }
}

public static class TokenValueExtractor
{
    public static string? GetFromJwt(this JwtSecurityToken token, string type) =>
        token.Claims.FirstOrDefault(c => c.Type == type)?.Value;
}
