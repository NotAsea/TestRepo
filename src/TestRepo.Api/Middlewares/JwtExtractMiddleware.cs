using System.IdentityModel.Tokens.Jwt;

namespace TestRepo.Api.Middlewares;

file sealed class JwtExtractMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(
        HttpContext context,
        IAccountService accountService,
        ITokenParameterFactory tokenValidationParameterFactory
    )
    {
        if (context.User.Identity is null or { IsAuthenticated: false })
        {
            await next(context).ConfigureAwait(false);
            return;
        }

        var rawTokenString = context.Request.Headers.Authorization[0];
        var tokenValidationParameter = tokenValidationParameterFactory.GetValidationParameters();
        if (string.IsNullOrEmpty(rawTokenString))
            return;
        var tokenString = rawTokenString[7..];
        var handler = new JwtSecurityTokenHandler();
        handler.ValidateToken(tokenString, tokenValidationParameter, out var validatedToken);
        var token = (JwtSecurityToken)validatedToken;
        var id =
            token.GetFromJwt(AppTokenType.Id) ?? throw new Exception("Not found Id from token");
        context.Items["AppAccount"] = await accountService
            .GetFromPersonId(int.Parse(id))
            .ConfigureAwait(false);

        await next(context).ConfigureAwait(false);
    }
}

internal static class MiddlewareExtension
{
    public static void UseAppAccountMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<JwtExtractMiddleware>();
    }
}
