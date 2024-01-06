using Microsoft.Extensions.DependencyInjection;

namespace TestRepo.Service;

public static class RegisterService
{
    public static IServiceCollection AddService(this IServiceCollection services, string connectionString)
    {
        services.AddRepository(connectionString);
        services.AddScoped<IPersonService, PersonService>();
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