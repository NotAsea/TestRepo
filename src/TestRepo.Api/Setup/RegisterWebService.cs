using AccountRegisterModelSerializerContext = TestRepo.Api.Models.AccountRegisterModelSerializerContext;

namespace TestRepo.Api.Setup;

internal static class SetupWebApp
{
    /// <summary>
    ///     All App Service should register here to keep main program clean
    /// </summary>
    /// <param name="builder"></param>
    internal static void RegisterService(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer().AddHttpContextAccessor();
        builder.Services.AddAppAuthentication(builder.Configuration);
        builder.Services.AddSwagger();
        builder.Services.AddService(builder.Configuration);
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
    ///     All Startup action like use Middleware, set up Database, check request... should put in here to keep main program
    ///     clean
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
}
