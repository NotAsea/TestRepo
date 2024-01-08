using Bogus;
using Person = TestRepo.Data.Entities.Person;

namespace TestRepo.Data;

internal static class SeedData
{
    private static readonly Faker<Person> Faker = new Faker<Person>()
        .Ignore(p => p.Id)
        .RuleFor(p => p.Name, (f, _) => f.Person.FullName)
        .RuleFor(p => p.Description, (f, _) => f.Company.Bs().OrNull(f))
        .RuleFor(p => p.IsDeleted, (_, _) => false)
        .RuleFor(p => p.Email, (f, _) => f.Person.Email.OrNull(f, 0.4f))
        .RuleFor(p => p.CreatedDate, (_, _) => DateTime.UtcNow);

    public static IEnumerable<Person> GetPeople(int amount = 40)
    {
        return Enumerable.Range(1, amount).Select(_ => Faker.Generate());
    }
}