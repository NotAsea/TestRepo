using Bogus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Person = TestRepo.Data.Entities.Person;

namespace TestRepo.Service;

public static class RegisterService
{
    public static void AddService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRepository(configuration.GetConnectionString("default")!);

        #region Service

        services
            .AddScoped<IPersonService, PersonService>()
            .AddScoped<IAccountService, AccountService>();

        #endregion

        #region Validator

        services
            .AddScoped<IValidator<AccountModel>, AccountValidator>()
            .AddScoped<IValidator<PersonModel>, PersonModelValidator>();

        #endregion


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
    public static async Task InitDb(this IServiceProvider provider)
    {
        var repository = provider.GetRequiredService<IRepository>();
        var context = provider.GetRequiredService<MyAppContext>();
        await context.Database.EnsureCreatedAsync();

        if (!await repository.ExistsAsync<Person>())
        {
            await context.BulkInsertAsync(SeedData.GetPeople()).ConfigureAwait(false);
        }

        if (await repository.ExistsAsync<Account>())
        {
            return;
        }

        var accounts = SeedData.GetAcc();
        var folder =
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\TestRepo";
        if (!Directory.Exists(folder))
        {
            Directory.CreateDirectory(folder);
        }

        var path = folder + @"\account.csv";
        if (File.Exists(path))
            File.Delete(path);
        await using var file = File.OpenWrite(path);
        await using var writer = new StreamWriter(file);
        await writer.WriteLineAsync($"{nameof(Account.UserName)},{nameof(Account.Password)}");
        foreach (var acc in accounts)
        {
            await writer.WriteLineAsync($"{acc.UserName},{acc.Password}");
        }

        var newAccounts = accounts.Select(a =>
        {
            a.Password = SecretHasher.Hash(a.Password);
            return a;
        });
        if (await repository.ExistsAsync<Person>())
        {
            var peopleId = await repository.GetListAsync<Person, int>(p => p.Id);
            var faker = new Faker();
            newAccounts = newAccounts.Select(a =>
            {
                a.PersonId = faker.PickRandom(peopleId);
                return a;
            });
        }

        await context.BulkInsertAsync(newAccounts).ConfigureAwait(false);
        Console.WriteLine("Generated Account got wrote in {0}", path);
    }
}