namespace TestRepo.Service.Services.Providers;

public sealed class AccountService(IRepository repository) : IAccountService
{
    public Task<AccountModel> GetAccount(int id)
    {
        throw new NotImplementedException();
    }

    public Task<AccountModel?> FindAccount(string username) =>
        repository.GetAsync<Account, AccountModel?>(p => p.UserName == username,
            p => p.ToModel());
}