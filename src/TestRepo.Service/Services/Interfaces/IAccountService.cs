namespace TestRepo.Service.Services.Interfaces;

public interface IAccountService
{
    Task<AccountModel> GetAccount(int id);
    Task<AccountModel?> FindAccount(string username);
}