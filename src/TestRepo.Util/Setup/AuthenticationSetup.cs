using Microsoft.AspNetCore.Authentication.JwtBearer;
using TestRepo.Util.Tools;

namespace TestRepo.Util.Setup;

public static class AuthenticationSetup
{
    public static void AddAppAuthentication(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services
            .AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(opt =>
            {
                var secKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(
                        configuration["Jwt:Key"]
                        ?? throw new Exception("Not found Secret key in appsettings.json")
                    )
                );
                opt.TokenValidationParameters = new TokenValidationParameters
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
            });
        services.AddAuthorization();
        services.AddScoped<TokenUtility>();
    }
}