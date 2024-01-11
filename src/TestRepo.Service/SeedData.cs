using System.Text;
using Bogus;
using Person = TestRepo.Data.Entities.Person;

namespace TestRepo.Service;

public static class SeedData
{
    /// <summary>
    /// Prebuilt setup for <see cref="Person"/>
    /// </summary>
    internal static readonly Action<Faker<Person>> PersonSetup = faker =>
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
    /// Prebuilt setup for <see cref="Account"/>
    /// </summary>
    internal static readonly Action<Faker<Account>> AccountSetup = faker =>
    {
        faker
            .Ignore(p => p.Id)
            .RuleFor(p => p.IsDeleted, (_, _) => false)
            .RuleFor(p => p.UserName, (f, _) => f.Internet.UserName())
            .RuleFor(p => p.Password, (f, _) => f.Internet.Password(10, 14))
            .Ignore(p => p.PersonId);
    };

    /// <summary>
    /// Using Faker to generate read only collection for this <typeparamref name="T"/>
    /// </summary>
    /// <param name="setup">action to config this <typeparamref name="T"/></param>
    /// <param name="amount">amount of element to generate</param>
    /// <typeparam name="T">type of entity or model class</typeparam>
    /// <returns>an <see cref="IReadOnlyCollection{T}"/></returns>
    public static IReadOnlyCollection<T> GenerateFor<T>(Action<Faker<T>> setup, int amount = 40)
        where T : class
    {
        //force faker lifetime to die in this scope
        var faker = new Faker<T>();
        setup(faker);
        var result = faker.Generate(amount);
        return result;
    }

    /// <summary>
    /// Variant of <see cref="GenerateFor{T}"/> but return <see cref="IEnumerable{T}"/> and
    /// require to call ToList or ToArray, or call GetEnumerator to actual generate data
    /// </summary>
    /// <param name="setup">action to config this <typeparamref name="T"/></param>
    /// <param name="amount">amount of element to generate</param>
    /// <typeparam name="T">type of entity or model class</typeparam>
    /// <returns>an <see cref="IEnumerable{T}"/></returns>
    public static IEnumerable<T> GenerateLazyFor<T>(Action<Faker<T>> setup, int amount = 40)
        where T : class
    {
        //force faker lifetime to die in this scope
        var faker = new Faker<T>();
        setup(faker);
        var result = faker.GenerateLazy(amount);
        return result;
    }
}

#region Seeder

internal static class BogusPasswordGenExt
{
    internal static string Password(
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
}

#endregion