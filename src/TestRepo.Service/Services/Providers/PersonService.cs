namespace TestRepo.Service.Services.Providers;

// ReSharper disable once SuggestBaseTypeForParameterInConstructor
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
            OrderByDynamic = (sortBy, sortType)
        };

        spec.Conditions.Add(
            p =>
                (
                    (
                        string.IsNullOrEmpty(nameSearch)
                        || p.Name.Contains(nameSearch)
                        || !string.IsNullOrEmpty(p.Email) && p.Email.Contains(nameSearch)
                    ) && !p.IsDeleted
                )
        );
        spec.Conditions.Add(p => !p.IsDeleted);
        var res = await _repository.GetListAsync(spec, x => x.ToModel());
        return new ListReturn(res.Items, res.TotalItems);
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
            await AddToDatabase(entity);
        else
            await UpdateToDatabase(entity);
        return entity.Id;
    }

    public async Task DeletePeople(int[] peopleId, bool isForce)
    {
        var people = await _repository.GetListAsync<Person>(p => peopleId.Contains(p.Id), true);
        if (people is null or { Count: 0 })
            throw new Exception("Not found People");
        if (isForce)
            await RemoveToDatabase(people);
        else
            await UpdateToDatabase(
                people.Select(static p =>
                {
                    p.IsDeleted = true;
                    return p;
                })
            );
    }

    public async Task DeletePerson(int id, bool isForce)
    {
        var person = await _repository.GetAsync<Person>(p => p.Id == id, true);
        if (person is null)
            throw new Exception("Not found Person");
        if (isForce)
            await RemoveToDatabase(person);
        else
        {
            person.IsDeleted = true;
            await UpdateToDatabase(person);
        }
    }

    public async Task ActivatePerson(int id)
    {
        var person = await _repository.GetByIdAsync<Person>(id);
        if (person is null)
            throw new Exception("No person found");

        person.IsDeleted = false;
        await UpdateToDatabase(person);
    }

    public async Task ActivatePeople(int[] ids)
    {
        var people = await _repository.GetListAsync<Person>(p => ids.Contains(p.Id), true);
        if (people is null or { Count: 0 })
            throw new Exception("No person found");
        await UpdateToDatabase(
            people.Select(static p =>
            {
                p.IsDeleted = false;
                return p;
            })
        );
    }
}
