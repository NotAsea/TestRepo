using System.IdentityModel.Tokens.Jwt;

namespace TestRepo.Api.Utils;

public static class TokenExtractor
{
    public static async Task<PersonAccount?> GetPersonFromToken(this HttpContext context)
    {
        var tokenString = context.Request.Headers.Authorization[0];
        if (string.IsNullOrEmpty(tokenString))
            return null;
        var jwtToken = tokenString.Split(" ")[1];
        var token = new JwtSecurityToken(jwtToken);
        var id = token.FindValue("Id") ?? throw new Exception("Not valid token");
        var service = context.RequestServices.GetRequiredService<IAccountService>();
        var result = await service.GetFromPersonId(int.Parse(id));
        return result;
    }

    private static string? FindValue(this JwtSecurityToken token, string type) =>
        token.Claims.FirstOrDefault(c => c.Type == type)?.Value;
}
