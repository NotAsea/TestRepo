using TestRepo.Data.Dtos;
using TestRepo.Data.Entities;
using TestRepo.Util;

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
                    .LeftJoin(
                        context.Set<Person>(),
                        a => a.PersonId,
                        p => p.Id,
                        (a, p) =>
                            new PersonDto
                            {
                                Id = p != null ? p.Id : a.Id,
                                Description = p != null ? p.Description : null,
                                Email = p != null ? p.Email : null,
                                Name = p != null ? p.Name : "",
                                CreatedDate = p != null ? p.CreatedDate : DateTime.UtcNow,
                                IsDeleted = p != null ? p.IsDeleted : a.IsDeleted,
                                UserName = a.UserName
                            }
                    )
                    .FirstOrDefault()
        );

    #endregion
}