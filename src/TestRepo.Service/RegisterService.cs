using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TestRepo.Service;

public static class RegisterService
{
    public static IServiceCollection AddService(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.AddRepository(configuration.GetConnectionString("default")!);
        services.AddScoped<IPersonService, PersonService>();
        services.AddScoped<IAccountService, AccountService>();
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
        return services;
    }
}

public static class StartupAction
{
    public static async Task InitDb(this IServiceProvider provider)
    {
        var repository = provider.GetRequiredService<IRepository>();
        var context = provider.GetRequiredService<MyAppContext>();
        await context.Database.EnsureCreatedAsync();
        if (!await repository.ExistsAsync<Person>())
        {
            await context.BulkInsertAsync(SeedData.GetPeople());
        }
    }
}