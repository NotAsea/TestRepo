using System.IdentityModel.Tokens.Jwt;
using TestRepo.Util.Helper;

namespace TestRepo.Api.Middlewares;

internal sealed class JwtExtractMiddleware(
    IAccountService accountService,
    ITokenParameterFactory tokenValidationParameterFactory
) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
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
        var handler = JwtSecurityTokenHandlerContainer.Instance;
        var validationResult = await handler
            .ValidateTokenAsync(tokenString, tokenValidationParameter)
            .ConfigureAwait(false);
        if (!validationResult.IsValid)
            throw new InvalidDataException("Verify token failed");
        var token = (JwtSecurityToken)validationResult.SecurityToken;
        var id =
            token.GetFromJwt(AppTokenType.Id) ?? throw new("Not found Id from token");
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
