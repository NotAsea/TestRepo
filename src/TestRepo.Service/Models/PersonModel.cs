using TestRepo.Util.Tools;
using ValidationResult = FluentValidation.Results.ValidationResult;

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

#region Validator, Mapper, Serializer

public static class PersonValidatorExtensions
{
    /// <summary>
    /// Validate <see cref="PersonModel"/>
    /// </summary>
    /// <param name="model">data to validate</param>
    /// <returns>Validation Result</returns>
    public static ValidationResult Validate(this PersonModel model) =>
        new InlineValidator<PersonModel>
        {
            v => v.RuleFor(x => x.Name).NotEmpty().WithMessage(Constant.ValueIsNull),
            v =>
                v.RuleFor(x => x.Email)
                    .Must(RegexUtility.VerifyEmail!)
                    .WithMessage(Constant.WrongEmailFormat)
                    .When(x => x.Email.NotNull()),
        }.Validate(model);
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

#endregion
