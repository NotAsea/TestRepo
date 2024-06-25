using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Open.Text;

namespace TestRepo.Api.Utils;

public static class TokenExtractor
{
    public static Task<PersonAccount?> GetPersonFromToken(this HttpContext context)
    {
        var tokenString = context.Request.Headers.Authorization[0];
        var configuration = context.RequestServices.GetRequiredService<IConfiguration>();

        if (string.IsNullOrEmpty(tokenString))
            return Task.FromResult<PersonAccount?>(null);
        tokenString = tokenString.Split(' ')[1];

        var secKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                configuration["Jwt:Key"]
                    ?? throw new Exception("Not found Secret key in appsettings.json")
            )
        );

        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = configuration["Jwt:Issuer"],
            ValidAudience = configuration["Jwt:Audience"],
            IssuerSigningKey = secKey,
            TokenDecryptionKey = secKey,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true
        };
        var handler = new JwtSecurityTokenHandler();
        handler.ValidateToken(tokenString, tokenValidationParameters, out var validatedToken);
        var token = (JwtSecurityToken)validatedToken;
        var id =
            token.GetFromJwt(AppTokenType.Id) ?? throw new Exception("Not found Id from token");
        var service = context.RequestServices.GetRequiredService<IAccountService>();
        return service.GetFromPersonId(int.Parse(id));
    }
}
