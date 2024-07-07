namespace TestRepo.Api.Utils;

public static class TokenExtractor
{
    public static PersonAccount? GetPersonFromToken(this HttpContext context) =>
        context.Items["AppAccount"] as PersonAccount;
}
