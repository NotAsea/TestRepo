using System.Text.Json.Serialization;
using Riok.Mapperly.Abstractions;

namespace TestRepo.Api.Models.AccountModels;

public record AccountRegisterModel(
    string UserName,
    string Password,
    string Name,
    string? Email,
    string? Description
);

public sealed class AccountRegisterModelValidator : AbstractValidator<AccountRegisterModel>
{
    public AccountRegisterModelValidator()
    {
        RuleFor(x => x.UserName).NotEmpty().WithMessage(Constant.ValueIsNull);
        RuleFor(x => x.Name).NotEmpty().WithMessage(Constant.ValueIsNull);
        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(Constant.ValueIsNull)
            .Must(RegexService.VerifyPassword)
            .WithMessage(Constant.WrongPasswordFormat)
            .When(x => !string.IsNullOrEmpty(x.Password), ApplyConditionTo.CurrentValidator);
        RuleFor(x => x.Email)
            .Must(RegexService.VerifyEmail!)
            .WithMessage(Constant.WrongEmailFormat)
            .When(x => !string.IsNullOrEmpty(x.Email));
    }
}

[Mapper]
public static partial class AccountRegisterModelMapper
{
    [MapperIgnoreTarget(nameof(AccountModel.PersonId))]
    [MapperIgnoreTarget(nameof(AccountModel.Id))]
    [MapperIgnoreSource(nameof(AccountRegisterModel.Email))]
    [MapperIgnoreSource(nameof(AccountRegisterModel.Description))]
    [MapperIgnoreSource(nameof(AccountRegisterModel.Name))]
    public static partial AccountModel ToAccount(this AccountRegisterModel model);

    [MapperIgnoreSource(nameof(AccountRegisterModel.Password))]
    [MapperIgnoreSource(nameof(AccountRegisterModel.UserName))]
    [MapperIgnoreTarget(nameof(PersonModel.Id))]
    [MapperIgnoreTarget(nameof(PersonModel.CreatedDate))]
    [MapperIgnoreTarget(nameof(PersonModel.IsDeleted))]
    public static partial PersonModel ToPerson(this AccountRegisterModel model);
}

[JsonSerializable(typeof(AccountRegisterModel))]
public partial class AccountRegisterModelSerializerContext : JsonSerializerContext;