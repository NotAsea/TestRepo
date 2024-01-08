namespace TestRepo.Service.Services.Providers;

internal sealed class AccountService(IRepository repository, MyAppContext context)
    : BaseService(repository, context),
        IAccountService
{
    private readonly IRepository _repository = repository;

    public Task<AccountModel> GetAccount(int id) =>
        _repository.GetAsync<Account, AccountModel>(
            a => a.Id == id && !a.IsDeleted,
            a => a.ToModel()
        );

    public Task<AccountModel?> FindAccount(string username) =>
        _repository.GetAsync<Account, AccountModel?>(
            p => p.UserName == username && !p.IsDeleted,
            p => p.ToModel()
        );

    public async Task<int> SaveAccount(AccountModel model)
    {
        var entity = model.ToEntity();
        if (entity.Id == 0)
            await AddToDatabase(entity);
        else
            await UpdateToDatabase(entity);
        return entity.Id;
    }

    public async Task<int> DeleteAccount(int id, bool isForce)
    {
        var entity = await _repository.GetByIdAsync<Account>(id, true);
        if (entity is null)
            throw new Exception("Not found Account");
        if (isForce)
            await RemoveToDatabase(entity);
        else
        {
            entity.IsDeleted = true;
            await UpdateToDatabase(entity);
        }

        return id;
    }

    public async Task<int> DeleteAccount(int[] id, bool isForce)
    {
        var entities = await _repository.GetListAsync<Account>(p => id.Contains(p.Id), true);
        if (entities is null or { Count: 0 })
            throw new Exception("Not found Account");
        if (isForce)
            await RemoveToDatabase(entities);
        else
            await UpdateToDatabase(
                entities.Select(static a =>
                {
                    a.IsDeleted = true;
                    return a;
                })
            );
        return id[0];
    }
}
