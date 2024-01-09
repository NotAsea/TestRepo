using System.Text;
using System.Text.Json.Serialization;

// ReSharper disable ClassNeverInstantiated.Global

namespace TestRepo.Service.Models;

public record PersonModel
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public string? Email { get; init; }
    public bool IsDeleted { get; init; }
    public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
}

public static class PersonModelVerifier
{
    public static string Verify(this PersonModel person, bool isAdd = false)
    {
        var msg = new StringBuilder();
        if (string.IsNullOrEmpty(person.Name))
        {
            msg.Append("Name cannot be null or Empty,");
        }

        if (!string.IsNullOrEmpty(person.Email) && !RegexService.VerifyEmail(person.Email))
        {
            msg.Append("Wrong format Email,");
        }

        if (!isAdd && person.Id == 0)
        {
            msg.Append("Id cannot be zero,");
        }

        return msg.Length > 0 ? msg.ToString().TrimEnd(',') : string.Empty;
    }
}

public record ListReturn(List<PersonModel> Data, long Count);

[Mapper]
internal static partial class PersonModelMapper
{
    internal static partial PersonModel ToModel(this Person entity);

    internal static partial Person ToEntity(this PersonModel model);
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(ListReturn))]
[JsonSerializable(typeof(List<PersonModel>))]
[JsonSerializable(typeof(PersonModel))]
public partial class PersonSerializer : JsonSerializerContext;
