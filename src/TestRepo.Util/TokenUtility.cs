using System.Security.Claims;

// ReSharper disable MemberCanBePrivate.Global

namespace TestRepo.Util;

public sealed class TokenUtility(IConfiguration configuration)
{
    /// <summary>
    /// Generate JWT token which contains <paramref name="personId"/> and <paramref name="personName"/>,
    /// the generated token live for 1 week, but can be set through <paramref name="liveDays"/>
    /// </summary>
    /// <param name="personId">ID of user</param>
    /// <param name="personName">Name of user</param>
    /// <param name="liveDays">how many day should this token valid</param>
    /// <returns>generated token as encoded string</returns>
    /// <exception cref="Exception">app setting is not valid</exception>
    public ValueTask<string> GetTokenForDay(int personId, string personName, int liveDays = 7) =>
        GetToken(personId, personName, DateTime.UtcNow.AddDays(liveDays));
    
    /// <summary>
    /// Generate JWT token which contains <paramref name="personId"/> and <paramref name="personName"/>,
    /// the generated token live for 30 minute, but can be set through <paramref name="liveTime"/>
    /// </summary>
    /// <param name="personId">ID of user</param>
    /// <param name="personName">Name of user</param>
    /// <param name="liveTime">how many minute should this token valid</param>
    /// <returns>generated token as encoded string</returns>
    /// <exception cref="Exception">app setting is not valid</exception>
    public ValueTask<string> GetTokenForMinute(int personId, string personName, int liveTime = 30) =>
        GetToken(personId, personName, DateTime.UtcNow.AddMinutes(liveTime));

    /// <summary>
    /// Generate JWT token which contains <paramref name="personId"/> and <paramref name="personName"/>,
    /// the generated token live for the date <paramref name="validTo"/>
    /// </summary>
    /// <param name="personId">ID of user</param>
    /// <param name="personName">Name of user</param>
    /// <param name="validTo">The date which token will be expired</param>
    /// <returns>generated token as encoded string</returns>
    /// <exception cref="Exception">app setting is not valid</exception>
    public ValueTask<string> GetToken(int personId, string personName, DateTime validTo)
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
            Expires = validTo,
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