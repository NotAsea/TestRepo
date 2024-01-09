using Bogus;
using TestRepo.Data.Entities;
using TestRepo.Util;
using Person = TestRepo.Data.Entities.Person;

namespace TestRepo.Data;

internal static class SeedData
{
    private class PeronSeeder : IDisposable
    {
        private Faker<Person> _fakeUser = new Faker<Person>()
            .Ignore(p => p.Id)
            .RuleFor(p => p.Name, (f, _) => f.Person.FullName)
            .RuleFor(p => p.Description, (f, _) => f.Company.Bs().OrNull(f))
            .RuleFor(p => p.IsDeleted, (_, _) => false)
            .RuleFor(p => p.Email, (f, _) => f.Person.Email.OrNull(f, 0.4f))
            .RuleFor(p => p.CreatedDate, (_, _) => DateTime.UtcNow);

        public void Dispose()
        {
            _fakeUser = null!;
        }

        public IEnumerable<Person> GenPeople(int amount = 40) =>
            Enumerable.Range(1, amount).Select(_ => _fakeUser.Generate()).ToList();
    }

    private class AccountSeeder : IDisposable
    {
        private Faker<Account> _fakeAccount = new Faker<Account>()
            .Ignore(p => p.Id)
            .RuleFor(p => p.IsDeleted, (_, _) => false)
            .RuleFor(p => p.UserName, (f, _) => f.Internet.UserName())
            .RuleFor(
                p => p.Password,
                (f, _) => f.Internet.Password(14, true, Constant.PasswordRegex)
            )
            .Ignore(p => p.PersonId);

        public void Dispose()
        {
            _fakeAccount = null!;
        }

        public IReadOnlyList<Account> GenAcc(int amount = 40) =>
            Enumerable.Range(1, amount).Select(_ => _fakeAccount.Generate()).ToList();
    }

    public static IEnumerable<Person> GetPeople(int amount = 40)
    {
        using var seeder = new PeronSeeder();
        return seeder.GenPeople(amount);
    }

    public static IReadOnlyList<Account> GetAcc(int amount = 40)
    {
        using var seeder = new AccountSeeder();
        return seeder.GenAcc(amount);
    }
}
