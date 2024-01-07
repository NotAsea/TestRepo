namespace TestRepo.Service.Services.Providers;

// ReSharper disable once SuggestBaseTypeForParameterInConstructor
public sealed class PersonService(IRepository repository, MyAppContext context) : IPersonService
{
    public async Task<ListReturn> GetPeople(
        int index = 1,
        int size = 1000,
        string sortBy = "Id",
        string sortType = "asc",
        string nameSearch = ""
    )
    {
        var spec = new PaginationSpecification<Person>
        {
            PageIndex = index,
            PageSize = size,
            OrderByDynamic = (sortBy, sortType)
        };
        if (!string.IsNullOrEmpty(nameSearch))
            spec.Conditions.Add(
                p =>
                    p.Name.Contains(nameSearch)
                    || (!string.IsNullOrEmpty(p.Email) && p.Email.Contains(nameSearch))
            );
        var res = await repository.GetListAsync(spec, x => x.ToModel());
        return new(res.Items, res.TotalItems);
    }

    public Task<PersonModel> GetPerson(int id) =>
        repository.GetAsync<Person, PersonModel>(p => p.Id == id && !p.IsDeleted, x => x.ToModel());

    public async Task<int> CreatePerson(PersonModel model)
    {
        var entity = model.ToEntity();
        await using var transaction = await repository.BeginTransactionAsync();
        try
        {
            await repository.AddAsync(entity);
            await repository.SaveChangesAsync();
            await transaction.CommitAsync();
            return entity.Id;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task DeletePeople(int[] peopleId, bool isForce)
    {
        var people = await repository.GetListAsync<Person>(p => peopleId.Contains(p.Id), true);
        if (people is null or { Count: 0 })
            throw new Exception("Not found People");
        if (isForce)
            await repository.ExecuteDeleteAsync<Person>(p => peopleId.Contains(p.Id));
        else
            await context.BulkUpdateAsync(
                people.Select(p =>
                {
                    p.IsDeleted = true;
                    return p;
                })
            );
    }

    public async Task DeletePerson(int id, bool isForce)
    {
        var person = await repository.GetAsync<Person>(p => p.Id == id, true);
        if (person is null)
            throw new Exception("Not found Person");
        if (isForce)
            repository.Remove(person);
        else
        {
            person.IsDeleted = true;
            repository.Update(person);
        }

        await using var transaction = await repository.BeginTransactionAsync();
        try
        {
            await repository.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task UpdatePerson(PersonModel model)
    {
        var person = model.ToEntity();
        await using var transaction = await repository.BeginTransactionAsync();
        try
        {
            repository.Update(person);
            await repository.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task ActivatePerson(int id)
    {
        var person = await repository.GetByIdAsync<Person>(id);
        if (person is null)
            throw new Exception("No person found");

        person.IsDeleted = false;
        await using var transaction = await repository.BeginTransactionAsync();
        try
        {
            repository.Update(person);
            await repository.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task ActivatePeople(int[] ids)
    {
        var people = await repository.GetListAsync<Person>(p => ids.Contains(p.Id), true);
        if (people is null or { Count: 0 })
            throw new Exception("No person found");
        await context.BulkUpdateAsync(
            people.Select(p =>
            {
                p.IsDeleted = false;
                return p;
            })
        );
    }
}