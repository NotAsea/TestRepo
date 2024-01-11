using System.Text;
using Bogus;
using TestRepo.Data.Entities;
using Person = TestRepo.Data.Entities.Person;

namespace TestRepo.Data;

internal static class SeedData
{
    #region Seeder

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
            .RuleFor(p => p.Password, (f, _) => f.Internet.Password(10, 14))
            .Ignore(p => p.PersonId);

        public void Dispose()
        {
            _fakeAccount = null!;
        }

        public IReadOnlyList<Account> GenAcc(int amount = 40) =>
            Enumerable.Range(1, amount).Select(_ => _fakeAccount.Generate()).ToList();
    }

    private static string Password(
        this DataSet internet,
        int minLength,
        int maxLength,
        bool includeUppercase = true,
        bool includeNumber = true,
        bool includeSymbol = true
    )
    {
        ArgumentNullException.ThrowIfNull(internet);
        ArgumentOutOfRangeException.ThrowIfLessThan(minLength, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxLength, minLength);

        var r = internet.Random;
        var s = new StringBuilder(maxLength);

        s.Append(r.Char('a', 'z'));
        if (s.Length < maxLength)
        {
            if (includeUppercase)
                s.Append(r.Char('A', 'Z'));
            if (includeNumber)
                s.Append(r.Char('0', '9'));
            if (includeSymbol)
                s.Append(r.Char('#', '&'));
        }

        if (s.Length < minLength)
            s.Append(r.String2(minLength - s.Length)); // pad up to min
        if (s.Length < maxLength)
            s.Append(r.String2(r.Number(0, maxLength - s.Length))); // random extra padding in range min..max

        var chars = s.ToString();
        var charsShuffled = r.Shuffle(chars).ToArray();

        return new string(charsShuffled);
    }

    #endregion

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