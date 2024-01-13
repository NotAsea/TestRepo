using System.IdentityModel.Tokens.Jwt;

namespace TestRepo.Api.Utils;

public static class TokenExtractor
{
    public static Task<PersonAccount?> GetPersonFromToken(this HttpContext context)
    {
        var tokenString = context.Request.Headers.Authorization[0];
        if (string.IsNullOrEmpty(tokenString))
            return Task.FromResult<PersonAccount?>(null);
        var jwtToken = tokenString.Split(" ")[1];
        var token = new JwtSecurityToken(jwtToken);
        var id =
            token.GetFromJwt(AppTokenType.Id) ?? throw new Exception("Not found Id from token");
        var service = context.RequestServices.GetRequiredService<IAccountService>();
        return service.GetFromPersonId(int.Parse(id));
    }
}
