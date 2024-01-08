using System.Text;
using System.Text.Json.Serialization;

namespace TestRepo.Service.Models;

public record AccountModel(int Id, string UserName, string Password, int PersonId)
{
    public AccountModel() : this(0, "", "", 0)
    {
    }
}


public static class AccountVerifier
{
    public static string Verify(this AccountModel model)
    {
        var sb = new StringBuilder();
        if (string.IsNullOrEmpty(model.UserName))
            sb.Append("Username cannot be null,");
        if (string.IsNullOrEmpty(model.Password))
            sb.Append("Password cannot be null or empty,");
        return sb.Length > 0 ? sb.ToString().TrimEnd(',') : string.Empty;
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