namespace TestRepo.Util.Tools;

/// <summary>
/// Service for get <see cref="SecurityTokenDescriptor"/> and <see cref="TokenValidationParameters"/>
/// </summary>
public interface ITokenParameterFactory
{
    /// <summary>
    /// Activate a <see cref="TokenValidationParameters"/> with internal cache
    /// </summary>
    /// <param name="reuseConfig">whether to reuse cache or not default is true</param>
    /// <returns><see cref="TokenValidationParameters"/> with default populated</returns>
    TokenValidationParameters GetValidationParameters(bool reuseConfig = true);

    /// <summary>
    /// Activate a <see cref="SecurityTokenDescriptor"/> with internal cache
    /// </summary>
    /// <param name="reuseConfig">whether to reuse cache or not default is true</param>
    /// <returns><see cref="SecurityTokenDescriptor"/> with Issuer, Audience, SignKey and EncryptKey populated</returns>
    SecurityTokenDescriptor GetSecurityTokenDescriptor(bool reuseConfig = true);
}

[RegisterSingleton]
internal sealed class TokenParameterFactory(IConfiguration configuration) : ITokenParameterFactory
{
    private static TokenValidationParameters? _tokenValidation;
    private static SecurityTokenDescriptor? _tokenDescriptor;

    public TokenValidationParameters GetValidationParameters(bool reuseConfig = true)
    {
        if (_tokenValidation is not null && reuseConfig)
        {
            return _tokenValidation;
        }

        var secKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                configuration["Jwt:Key"]
                    ?? throw new Exception("Not found Secret key in appsettings.json")
            )
        );
        _tokenValidation = new()
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
        return _tokenValidation;
    }

    public SecurityTokenDescriptor GetSecurityTokenDescriptor(bool reuseConfig = true)
    {
        if (_tokenDescriptor is not null && reuseConfig)
        {
            return _tokenDescriptor;
        }
        var issuer = configuration["Jwt:Issuer"];
        var audience = configuration["Jwt:Audience"];
        var key = Encoding.UTF8.GetBytes(
            configuration["Jwt:Key"]
                ?? throw new Exception("Not found Secret key in appsettings.json")
        );
        var secKey = new SymmetricSecurityKey(key);
        _tokenDescriptor = new()
        {
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new(secKey, SecurityAlgorithms.HmacSha512Signature),
            EncryptingCredentials = new(
                secKey,
                JwtConstants.DirectKeyUseAlg,
                SecurityAlgorithms.Aes256CbcHmacSha512
            )
        };
        return _tokenDescriptor;
    }
}
