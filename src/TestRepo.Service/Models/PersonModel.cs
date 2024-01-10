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

public sealed class PersonModelValidator : AbstractValidator<PersonModel>
{
    public PersonModelValidator()
    {
        RuleFor(x => x.Name).NotEmpty().WithMessage(Constant.ValueIsNull);
        RuleFor(x => x.Email)
            .Must(RegexService.VerifyEmail!)
            .WithMessage(Constant.WrongEmailFormat)
            .When(x => !string.IsNullOrEmpty(x.Email));
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
