namespace TestRepo.Service.Models;

public record AccountModel(int Id, string UserName, string Password, int PersonId)
{
    public AccountModel()
        : this(0, "", "", 0)
    {
    }
}

[RegisterScoped(typeof(IValidator<AccountModel>))]
public sealed class AccountValidator : AbstractValidator<AccountModel>
{
    public AccountValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage(Constant.ValueIsNull);
        RuleFor(x => x.Password).NotEmpty().WithMessage(Constant.ValueIsNull);
    }
}

public record ListAccount(List<AccountModel> Data, long Count);

[Mapper]
internal static partial class AccountMapper
{
    [MapperIgnoreTarget(nameof(Account.IsDeleted))]
    internal static partial Account ToEntity(this AccountModel model);

    [MapperIgnoreSource(nameof(Account.IsDeleted))]
    internal static partial AccountModel ToModel(this Account account);
}

[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(ListAccount))]
[JsonSerializable(typeof(List<AccountModel>))]
[JsonSerializable(typeof(AccountModel))]
public partial class AccountSerializerContext : JsonSerializerContext;