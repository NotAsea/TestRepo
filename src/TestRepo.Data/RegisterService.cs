using Bogus;
using Microsoft.Extensions.DependencyInjection;
using TestRepo.Data.CompiledModels;
using TestRepo.Util;
using TestRepo.Util.Tools;
using Person = TestRepo.Data.Entities.Person;

namespace TestRepo.Data;

internal static class RegisterService
{
    internal static void AddRepository(this IServiceCollection service, string connectionString)
    {
        service.AddDbContext<MyAppContext>(opt =>
        {
            opt.UseModel(MyAppContextModel.Instance);
            opt.UseNpgsql(connectionString);
        });
        service.AddGenericRepository<MyAppContext>();
    }
}

internal static class StartupAction
{
    /// <summary>
    /// Prebuilt setup for <see cref="Person"/>
    /// </summary>
    private static readonly Action<Faker<Person>> PersonSetup = faker =>
    {
        faker
            .Ignore(p => p.Id)
            .RuleFor(p => p.Name, (f, _) => f.Person.FullName)
            .RuleFor(p => p.Description, (f, _) => f.Company.Bs().OrNull(f))
            .RuleFor(p => p.IsDeleted, (_, _) => false)
            .RuleFor(p => p.Email, (f, _) => f.Person.Email.OrNull(f, 0.4f))
            .RuleFor(p => p.CreatedDate, (_, _) => DateTime.UtcNow);
    };

    /// <summary>
    /// Prebuilt setup for <see cref="Entities.Account"/>
    /// </summary>
    private static readonly Action<Faker<Account>> AccountSetup = faker =>
    {
        faker
            .Ignore(p => p.Id)
            .RuleFor(p => p.IsDeleted, (_, _) => false)
            .RuleFor(p => p.UserName, (f, _) => f.Internet.UserName())
            .RuleFor(p => p.Password, (f, _) => f.Internet.Password(10, 14))
            .Ignore(p => p.PersonId);
    };

    internal static async Task InitDb(this IServiceProvider provider)
    {
        var repository = provider.GetRequiredService<IRepository>();
        var context = provider.GetRequiredService<MyAppContext>();
        await context.Database.EnsureCreatedAsync().ConfigureAwait(false);

        if (!await repository.ExistsAsync<Person>().ConfigureAwait(true))
        {
            await context.BulkInsertAsync(SeedData.GenerateFor(PersonSetup)).ConfigureAwait(false);
        }

        if (await repository.ExistsAsync<Account>().ConfigureAwait(true))
        {
            return;
        }

        var accounts = SeedData.GenerateFor(AccountSetup);
        var path =
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)
            + @"\TestRepo\account.csv";
        await FileUtil
            .WriteToFile(
                path,
                accounts,
                static (a) =>
                    [
                        new PropertySelect(nameof(a.UserName), a.UserName),
                        new PropertySelect(nameof(a.Password), a.Password)
                    ]
            )
            .ConfigureAwait(false);

        var newAccounts = accounts.Select(a =>
        {
            a.Password = SecretHasher.Hash(a.Password);
            return a;
        });
        if (await repository.ExistsAsync<Person>().ConfigureAwait(true))
        {
            var peopleId = await repository
                .GetListAsync<Person, int>(p => p.Id)
                .ConfigureAwait(true);
            newAccounts = newAccounts.Select(a =>
            {
                a.PersonId = new Faker().PickRandom(peopleId);
                return a;
            });
        }

        await context.BulkInsertAsync(newAccounts).ConfigureAwait(false);
        Console.WriteLine("Generated Account got wrote in {0}", path);
    }
}
