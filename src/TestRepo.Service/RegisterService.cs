using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TestRepo.Service;

public static class RegisterService
{
    public static void AddAppService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRepository(configuration.GetConnectionString("default")!);
        services.AddMemoryCache();
        services.AutoRegisterFromTestRepoService();

        services
            .AddHttpClient<IGetDadJokeService, GetDadJokeService>(config =>
            {
                config.BaseAddress = new Uri(
                    configuration["DadJokeUrl"] ?? throw new Exception("Not found Url")
                );
            })
            .ConfigurePrimaryHttpMessageHandler(
                () => new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(2) }
            )
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
    }
}

public static class StartupAction
{
    public static Task InitDb(this AsyncServiceScope scope) => scope.ServiceProvider.InitDb();
}