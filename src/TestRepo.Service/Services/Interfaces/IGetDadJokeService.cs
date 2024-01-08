namespace TestRepo.Service.Services.Interfaces;

public interface IGetDadJokeService
{
    Task<DadJokeModel?> GetDadJoke(string? id);
    Task<string> GetDadJokeAsString(string? id);
    Task<DadJokeModelList?> SearchDadJoke(int? page, int? limit, string? term);
    Task<string> SearchDadJokeAsString(int? page, int? limit, string? term);
}