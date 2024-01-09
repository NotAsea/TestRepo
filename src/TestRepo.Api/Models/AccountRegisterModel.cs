using System.Text.Json.Serialization;
using Riok.Mapperly.Abstractions;

namespace TestRepo.Api.Models;

public record AccountRegisterModel(
    string UserName,
    string Password,
    string Name,
    string? Email,
    string? Description
);

public static class AccountRegisterModelVerifier
{
    public static string Verify(this AccountRegisterModel model)
    {
        var sb = new StringBuilder();
        if (string.IsNullOrEmpty(model.UserName))
        {
            sb.Append("Username cannot be null,");
        }

        if (string.IsNullOrEmpty(model.Password))
        {
            sb.Append("Password cannot be null or empty,");
        }

        if (string.IsNullOrEmpty(model.Name))
        {
            sb.Append("Name cannot be null or Empty,");
        }

        if (!string.IsNullOrEmpty(model.Email) && !CompileRegex.VerifyEmail(model.Email))
        {
            sb.Append("Wrong format Email,");
        }

        return sb.Length > 0 ? sb.ToString().TrimEnd(',') : string.Empty;
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