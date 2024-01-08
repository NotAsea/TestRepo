using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace TestRepo.Service.Services.Providers;

internal sealed class GetDadJokeService(HttpClient httpClient) : IGetDadJokeService
{
    public Task<DadJokeModel?> GetDadJoke(string? id)
    {
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add(
            "User-Agent",
            "My Library (https://github.com/NotAsea/TestRepo.git)"
        );
        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );
        return httpClient.GetFromJsonAsync(
            string.IsNullOrEmpty(id) ? "/" : $"/j/{id}",
            DadJokeSerializerContext.Default.DadJokeModel
        );
    }

    public Task<string> GetDadJokeAsString(string? id)
    {
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add(
            "User-Agent",
            "My Library (https://github.com/NotAsea/TestRepo.git)"
        );
        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("text/plain")
        );
        return httpClient.GetStringAsync(string.IsNullOrEmpty(id) ? "/" : $"/j/{id}");
    }

    public Task<DadJokeModelList?> SearchDadJoke(int? page, int? limit, string? term)
    {
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add(
            "User-Agent",
            "My Library (https://github.com/NotAsea/TestRepo.git)"
        );
        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );
        return httpClient.GetFromJsonAsync(
            $"/search?page={page}&limit={limit}&term={term}",
            DadJokeSerializerContext.Default.DadJokeModelList
        );
    }

    public Task<string> SearchDadJokeAsString(int? page, int? limit, string? term)
    {
        httpClient.DefaultRequestHeaders.Clear();
        httpClient.DefaultRequestHeaders.Add(
            "User-Agent",
            "My Library (https://github.com/NotAsea/TestRepo.git)"
        );
        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("text/plain")
        );
        return httpClient.GetStringAsync($"/search?page={page}&limit={limit}&term={term}");
    }
}
