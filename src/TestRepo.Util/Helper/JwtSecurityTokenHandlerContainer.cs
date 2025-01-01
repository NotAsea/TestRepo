using Microsoft.IdentityModel.JsonWebTokens;

namespace TestRepo.Util.Helper;

public static class JwtSecurityTokenHandlerContainer
{
    public static JsonWebTokenHandler Instance { get; } = new();
}
