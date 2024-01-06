using TestRepo.Data.Entities;
// ReSharper disable ReturnTypeCanBeEnumerable.Global

namespace TestRepo.Data;

public class MyAppContext(DbContextOptions<MyAppContext> options) : DbContext(options)
{
    internal DbSet<Person> Persons => Set<Person>();
    internal DbSet<Account> Accounts => Set<Account>();
}
