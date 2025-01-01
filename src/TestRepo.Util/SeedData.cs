using Bogus;
using Cysharp.Text;

namespace TestRepo.Util;

public static class SeedData
{
    /// <summary>
    /// Using Faker to generate read only collection for this <typeparamref name="T"/>
    /// </summary>
    /// <param name="setup">action to config this <typeparamref name="T"/></param>
    /// <param name="amount">amount of element to generate</param>
    /// <typeparam name="T">type of entity or model class</typeparam>
    /// <returns>an <see cref="IReadOnlyList{T}"/></returns>
    public static IReadOnlyList<T> GenerateFor<T>(Action<Faker<T>> setup, int amount = 40)
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

public static class BogusPasswordGenExt
{
    public static string Password(
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
        using var s = ZString.CreateStringBuilder(true);

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

        return new(charsShuffled);
    }
}

#endregion
