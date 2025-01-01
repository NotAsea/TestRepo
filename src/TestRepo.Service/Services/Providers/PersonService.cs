using TestRepo.Util.Tools;

namespace TestRepo.Service.Services.Providers;

[RegisterScoped]
internal sealed class PersonService(IRepository repository, MyAppContext context)
    : BaseService(repository, context),
        IPersonService
{
    private readonly IRepository _repository = repository;

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
            OrderByDynamic = (sortBy, sortType),
        };
        if (nameSearch.NotNull())
        {
            spec.Conditions.Add(p =>
                p.Name.Contains(nameSearch)
                || (!string.IsNullOrEmpty(p.Email) && p.Email.Contains(nameSearch))
            );
        }

        spec.Conditions.Add(p => !p.IsDeleted);
        var res = await _repository.GetListAsync(spec, x => x.ToModel()).ConfigureAwait(true);
        return new(res.Items, res.TotalItems);
    }

    public Task<PersonModel> GetPerson(int id) =>
        _repository.GetAsync<Person, PersonModel>(
            p => p.Id == id && !p.IsDeleted,
            x => x.ToModel()
        );

    public async Task<int> SavePerson(PersonModel model)
    {
        var entity = model.ToEntity();
        if (entity.Id == 0)
        {
            await AddToDatabase(entity).ConfigureAwait(false);
        }
        else
        {
            await UpdateToDatabase(entity).ConfigureAwait(false);
        }

        return entity.Id;
    }

    public async Task DeletePeople(int[] peopleId, bool isForce)
    {
        var people = await _repository
            .GetListAsync<Person>(p => peopleId.Contains(p.Id), true)
            .ConfigureAwait(true);
        if (people is null or { Count: 0 })
        {
            throw new("Not found People");
        }

        if (isForce)
        {
            await RemoveToDatabase<Person>(people).ConfigureAwait(false);
        }
        else
        {
            await UpdateToDatabase(
                    people.Select(static p =>
                    {
                        p.IsDeleted = true;
                        return p;
                    })
                )
                .ConfigureAwait(false);
        }
    }

    public async Task DeletePerson(int id, bool isForce)
    {
        var person =
            await _repository.GetByIdAsync<Person>(id, true).ConfigureAwait(true)
            ?? throw new("Not found Person");
        if (isForce)
        {
            await RemoveToDatabase(person).ConfigureAwait(false);
        }
        else
        {
            person.IsDeleted = true;
            await UpdateToDatabase(person).ConfigureAwait(false);
        }
    }

    public async Task ActivatePerson(int id)
    {
        var person =
            await _repository.GetByIdAsync<Person>(id, true).ConfigureAwait(true)
            ?? throw new("No person found");
        person.IsDeleted = false;
        await UpdateToDatabase(person).ConfigureAwait(false);
    }

    public async Task ActivatePeople(int[] ids)
    {
        var people = await _repository
            .GetListAsync<Person>(p => ids.Contains(p.Id), true)
            .ConfigureAwait(true);
        if (people is null or { Count: 0 })
        {
            throw new("No person found");
        }

        await UpdateToDatabase(
                people.Select(static p =>
                {
                    p.IsDeleted = false;
                    return p;
                })
            )
            .ConfigureAwait(false);
    }
}