using TestRepo.Data.Dtos;

namespace TestRepo.Service.Models;

public record PersonAccount
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public string? Description { get; init; }

    public string? Email { get; init; }
    public bool IsDeleted { get; init; }
    public DateTime CreatedDate { get; init; } = DateTime.UtcNow;
    public string UserName { get; init; } = string.Empty;
    public string Password { get; init; } = "********************************";
}

[Mapper]
internal static partial class PersonAccountMapper
{
    [MapperIgnoreTarget(nameof(PersonDto.Password))]
    public static partial PersonAccount? ToModel(this PersonDto? dto);
}

[JsonSerializable(typeof(PersonAccount))]
public partial class PersonAccountSerializerContext : JsonSerializerContext;
