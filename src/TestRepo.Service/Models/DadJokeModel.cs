using System.Text.Json.Serialization;

namespace TestRepo.Service.Models;

public record DadJokeModel(string Id, string Joke);

public record DadJokeModelList(
    int CurrentPage,
    int Limit,
    int NextPage,
    int PreviousPage,
    List<DadJokeModel> Results,
    string? SearchTerm,
    int TotalJokes,
    int TotalPages
);

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.SnakeCaseLower)]
[JsonSerializable(typeof(DadJokeModel))]
[JsonSerializable(typeof(List<DadJokeModel>))]
[JsonSerializable(typeof(DadJokeModelList))]
public partial class DadJokeSerializerContext : JsonSerializerContext;