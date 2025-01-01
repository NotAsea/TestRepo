using System.Net.Http.Headers;
using System.Net.Http.Json;
using Cysharp.Text;
using TestRepo.Util.Tools;

namespace TestRepo.Service.Services.Providers;

internal sealed class GetDadJokeService(HttpClient httpClient) : IGetDadJokeService
{
    public Task<DadJokeModel?> GetDadJoke(string? id)
    {
        SetHeader(httpClient, "application/json");
        return httpClient.GetFromJsonAsync(
            id.NotNull() ? ZString.Concat("/j/", id) : "/",
            DadJokeSerializerContext.Default.DadJokeModel
        );
    }

    public Task<string> GetDadJokeAsString(string? id)
    {
        SetHeader(httpClient, "text/plain");
        return httpClient.GetStringAsync(id.NotNull() ? ZString.Concat("/j/", id) : "/");
    }

    public Task<DadJokeModelList?> SearchDadJoke(int? page, int? limit, string? term)
    {
        SetHeader(httpClient, "application/json");
        return httpClient.GetFromJsonAsync(
            ZString.Format("/search?page={page}&limit={limit}&term={term}", page, limit, term),
            DadJokeSerializerContext.Default.DadJokeModelList
        );
    }

    public Task<string> SearchDadJokeAsString(int? page, int? limit, string? term)
    {
        SetHeader(httpClient, "text/plain");
        return httpClient.GetStringAsync(
            ZString.Format("/search?page={page}&limit={limit}&term={term}", page, limit, term)
        );
    }

    private static void SetHeader(HttpClient client, string mediaQueryType)
    {
        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add(
            "User-Agent",
            "My Library (https://github.com/NotAsea/TestRepo.git)"
        );
        client.DefaultRequestHeaders.Accept.Add(
            new(mediaQueryType)
        );
    }
}
