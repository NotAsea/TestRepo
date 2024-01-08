namespace TestRepo.Service.Services.Interfaces;

public interface IAccountService
{
    Task<AccountModel> GetAccount(int id);
    Task<AccountModel?> FindAccount(string username);
    Task<int> SaveAccount(AccountModel model);

    Task<int> DeleteAccount(int id, bool isForce);
    Task<int> DeleteAccount(int[] id, bool isForce);
}