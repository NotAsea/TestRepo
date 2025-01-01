using System.Security.Claims;
using NetEscapades.EnumGenerators;
using TestRepo.Util.Helper;

// ReSharper disable MemberCanBePrivate.Global

namespace TestRepo.Util.Tools;

public readonly record struct TokenBody(AppTokenType Type, string Value);

[EnumExtensions]
public enum AppTokenType
{
    Id,
    Name
}

[RegisterSingleton]
public sealed class TokenUtility(ITokenParameterFactory parameterFactory)
{
    /// <summary>
    /// Generate JWT token which contains <paramref name="dataBody"/>,
    /// the generated token live for 1 week, but can be set through <paramref name="liveDays"/>
    /// </summary>
    /// <param name="liveDays">how many day should this token valid</param>
    /// <param name="dataBody">data is an array of value pairs for its type and value</param>
    /// <returns>generated token as encoded string</returns>
    /// <exception cref="Exception">app setting is not valid</exception>
    public ValueTask<string> GetTokenForDay(TokenBody[] dataBody, int liveDays = 7) =>
        GetToken(dataBody, DateTime.UtcNow.AddDays(liveDays));

    /// <summary>
    /// Generate JWT token which contains <paramref name="dataBody"/>,
    /// the generated token live for 30 minutes, but can be set through <paramref name="liveTime"/>
    /// </summary>
    /// <param name="liveTime">how many minutes should this token valid?</param>
    /// <param name="dataBody">data is an array of value pairs for its type and value</param>
    /// <returns>generated token as encoded string</returns>
    /// <exception cref="Exception">app setting is not valid</exception>
    public ValueTask<string> GetTokenForMinute(TokenBody[] dataBody, int liveTime = 30) =>
        GetToken(dataBody, DateTime.UtcNow.AddMinutes(liveTime));

    /// <summary>
    /// Generate JWT token which contains <paramref name="dataBody"/>,
    /// the generated token live for the date <paramref name="validTo"/>
    /// </summary>
    /// <param name="dataBody">data is an array of value pairs for its type and value</param>
    /// <param name="validTo">The date which token will be expired</param>
    /// <returns>generated token as encoded string</returns>
    /// <exception cref="Exception">app setting is not valid</exception>
    public ValueTask<string> GetToken(TokenBody[] dataBody, DateTime validTo)
    {
        if (dataBody.Length == 0)
            throw new AggregateException("dataBody must have element");
        var tokenDescriptor = parameterFactory.GetSecurityTokenDescriptor();
        var claimIdentity = new ClaimsIdentity(
            dataBody.Select(x =>
            {
                var (type, value) = x;
                if (!AppTokenTypeExtensions.IsDefined(type))
                    throw new("Invalid token type");
                return new Claim(type.ToStringFast(), value);
            })
        );
        tokenDescriptor.Subject = claimIdentity;
        tokenDescriptor.Expires = validTo;
        var tokenHandler = JwtSecurityTokenHandlerContainer.Instance;
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return ValueTask.FromResult(token);
    }
}

public static class TokenValueExtractor
{
    public static string? GetFromJwt(this JwtSecurityToken token, AppTokenType type) =>
        token.Claims.Gen().Where(new TokenTypeFilter(type)).FirstOrDefault()?.Value;
}

file readonly struct TokenTypeFilter(AppTokenType type) : IStructFunction<Claim, bool>
{
    public bool Invoke(Claim arg) => arg.Type == type.ToStringFast();
}
