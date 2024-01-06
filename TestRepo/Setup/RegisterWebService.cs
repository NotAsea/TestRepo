using TestRepo.Service.Models;

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
        builder.Services.AddSwaggerGen();
        builder.Services.AddService(builder.Configuration.GetConnectionString("default")!);
        builder.Services.AddTransient(p =>
        {
            var logFactory = p.GetRequiredService<ILoggerFactory>();
            var httpContext = p.GetRequiredService<IHttpContextAccessor>().HttpContext!;
            return logFactory.CreateLogger(httpContext.Request.Path);
        });
        builder.Services.Configure<JsonOptions>(config =>
        {
            config.JsonSerializerOptions.TypeInfoResolverChain.Add(PersonSerializer.Default);
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
}
