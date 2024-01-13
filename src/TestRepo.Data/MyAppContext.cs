using TestRepo.Data.Dtos;

// ReSharper disable ReturnTypeCanBeEnumerable.Global

namespace TestRepo.Data;

internal class MyAppContext(DbContextOptions<MyAppContext> options) : DbContext(options)
{
    internal required DbSet<Person> Persons { get; set; }
    internal required DbSet<Account> Accounts { get; set; }

    #region Compile query for caching

    internal Task<PersonDto?> GetPersonAccount(int id) => GetPersonAccountInternal(this, id);

    private static readonly Func<MyAppContext, int, Task<PersonDto?>> GetPersonAccountInternal =
        EF.CompileAsyncQuery(
            (MyAppContext context, int id) =>
                context
                    .Set<Account>()
                    .Where(a => !a.IsDeleted && a.PersonId == id)
                    .Join(
                        context.Set<Person>(),
                        a => a.PersonId,
                        p => p.Id,
                        (a, p) =>
                            new PersonDto
                            {
                                Id = p.Id,
                                Description = p.Description,
                                Email = p.Email,
                                Name = p.Name,
                                CreatedDate = p.CreatedDate,
                                IsDeleted = p.IsDeleted,
                                UserName = a.UserName
                            }
                    )
                    .FirstOrDefault()
        );

    #endregion
}
