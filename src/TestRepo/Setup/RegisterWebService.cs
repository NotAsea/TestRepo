using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace TestRepo.Setup;

internal static class SetupWebApp
{
    /// <summary>
    /// All App Service should register here to keep main program clean
    /// </summary>
    /// <param name="builder"></param>
    internal static void RegisterService(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer().AddHttpContextAccessor();
        builder.Services.AddAppAuthentication(builder.Configuration);
        builder.Services.AddSwaggerGen();
        builder.Services.AddService(builder.Configuration.GetConnectionString("default")!);
        builder.Services.AddTransient(p =>
        {
            var logFactory = p.GetRequiredService<ILoggerFactory>();
            var httpContext = p.GetRequiredService<IHttpContextAccessor>().HttpContext!;
            return logFactory.CreateLogger(httpContext.Request.Path);
        });
        builder.Services.ConfigureHttpJsonOptions(config =>
        {
            config.SerializerOptions.TypeInfoResolverChain.Add(PersonSerializer.Default);
            config.SerializerOptions.TypeInfoResolverChain.Add(AccountSerializerContext.Default);
            config.SerializerOptions.TypeInfoResolverChain.Add(
                AccountRegisterModelSerializerContext.Default
            );
        });
    }

    /// <summary>
    /// All Startup action like use Middleware, set up Database, check request... should put in here to keep main program clean
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    internal static async Task StartupAction(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        await app.InitialDb();
    }

    private static async Task InitialDb(this WebApplication app)
    {
        try
        {
            await using var scope = app.Services.CreateAsyncScope();
            await scope.ServiceProvider.InitDb();
        }
        catch (Exception ex)
        {
            app.Logger.InitializeDatabaseFail(ex.GetBaseException().Message);
        }
    }

    private static void AddAppAuthentication(
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
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            configuration["Jwt:Key"]
                                ?? throw new Exception("Not found Secret key in appsettings.json")
                        )
                    ),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = true
                };
            });
        services.AddAuthorization();
    }

    private static void AddSwaggerGen(this IServiceCollection services)
    {
        services.AddSwaggerGen(option =>
        {
            option.AddSecurityDefinition(
                "Bearer",
                new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description =
                        // ReSharper disable once StringLiteralTypo
                        "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
                }
            );
            option.AddSecurityRequirement(
                new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                }
            );
        });
    }
}
