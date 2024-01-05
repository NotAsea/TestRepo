// ReSharper disable ClassNeverInstantiated.Global

using System.Text;
using TestRepo.Routes;

namespace TestRepo.Models;

internal record ListReturn(List<Person> Data, long Count);

internal record PersonRouteDefaultParam(ILogger Logger, IRepository Repository);

internal record SearchPersonParam(
    int Index = 1,
    int Size = 1000,
    string SortBy = "Id",
    string SortType = "asc",
    string NameSearch = ""
);

internal static class SearchPersonParamVerifier
{
    public static SearchPersonParam Verify(this SearchPersonParam param)
    {
        var sortType =
            string.Equals(param.SortType, "asc", StringComparison.OrdinalIgnoreCase)
            || string.Equals(param.SortType, "desc", StringComparison.CurrentCultureIgnoreCase)
                ? param.SortType.ToLower()
                : "asc";
        var possibleSortBy = new[] { "name", "email", "id", "createddate" };
        if (!string.IsNullOrEmpty(param.SortBy) && !possibleSortBy.Contains(param.SortBy.ToLower()))
            throw new InvalidDataException("Unknown SortBy");
        return param with
        {
            SortBy = !string.IsNullOrEmpty(param.SortBy) ? param.SortBy.ToUpper() : "Id",
            SortType = sortType
        };
    }
}

internal static class PersonVerifier
{
    internal static string Verify(this Person person)
    {
        var msg = new StringBuilder();
        if (string.IsNullOrEmpty(person.Name))
            msg.Append("Name cannot be null or Empty,");
        if (!string.IsNullOrEmpty(person.Email) && !CompileRegex.VerifyEmail(person.Email))
            msg.Append("Wrong format Email,");
        if (person.Id == 0)
            msg.Append("Id cannot be zero,");
        return msg.Length > 0 ? msg.ToString().TrimEnd(',') : string.Empty;
    }
}
