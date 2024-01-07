using System.Text.Json.Serialization;
using Riok.Mapperly.Abstractions;

namespace TestRepo.Models;

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
            sb.Append("Username cannot be null,");
        if (string.IsNullOrEmpty(model.Password))
            sb.Append("Password cannot be null or empty,");
        if (string.IsNullOrEmpty(model.Name))
            sb.Append("Name cannot be null or Empty,");
        if (!string.IsNullOrEmpty(model.Email) && !CompileRegex.VerifyEmail(model.Email))
            sb.Append("Wrong format Email,");
        return sb.Length > 0 ? sb.ToString().TrimEnd(',') : string.Empty;
    }
}

[Mapper]
public static partial class AccountRegisterModelMapper
{
    public static partial AccountModel ToAccount(this AccountRegisterModel model);

    public static partial PersonModel ToPerson(this AccountRegisterModel model);
}

[JsonSerializable(typeof(AccountRegisterModel))]
public partial class AccountRegisterModelSerializerContext : JsonSerializerContext;
