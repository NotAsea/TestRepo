using System.IdentityModel.Tokens.Jwt;

namespace TestRepo.Api.Utils;

public static class TokenExtractor
{
    public static Task<PersonAccount?> GetPersonFromToken(this HttpContext context)
    {
        var rawTokenString = context.Request.Headers.Authorization[0];
        var tokenValidationParameterFactory = context.RequestServices.GetRequiredService<ITokenParameterFactory>();
        var tokenValidationParameter = tokenValidationParameterFactory.GetValidationParameters();
        if (string.IsNullOrEmpty(rawTokenString))
            return Task.FromResult<PersonAccount?>(null);
        
        var tokenString = rawTokenString[7..];
        var handler = new JwtSecurityTokenHandler();
        handler.ValidateToken(tokenString, tokenValidationParameter, out var validatedToken);
        var token = (JwtSecurityToken)validatedToken;
        var id =
            token.GetFromJwt(AppTokenType.Id) ?? throw new Exception("Not found Id from token");
        var service = context.RequestServices.GetRequiredService<IAccountService>();
        return service.GetFromPersonId(int.Parse(id));
    }
}