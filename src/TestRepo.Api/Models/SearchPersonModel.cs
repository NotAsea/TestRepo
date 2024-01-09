namespace TestRepo.Api.Models;

// ReSharper disable once ClassNeverInstantiated.Global
public record SearchPersonModel(
    int Index = 1,
    int Size = 1000,
    string SortBy = "Id",
    string SortType = "asc",
    string NameSearch = ""
);

internal static class SearchPersonModelVerifier
{
    public static SearchPersonModel Verify(this SearchPersonModel param)
    {
        var sortType =
            string.Equals(param.SortType, "asc", StringComparison.OrdinalIgnoreCase)
            || string.Equals(param.SortType, "desc", StringComparison.CurrentCultureIgnoreCase)
                ? param.SortType.ToLower()
                : "asc";
        var possibleSortBy = new[] { "name", "email", "id", "createddate" };
        if (!string.IsNullOrEmpty(param.SortBy) && !possibleSortBy.Contains(param.SortBy.ToLower()))
        {
            throw new InvalidDataException("Unknown SortBy");
        }

        return param with
        {
            SortBy = !string.IsNullOrEmpty(param.SortBy) ? param.SortBy.ToUpper() : "Id",
            SortType = sortType
        };
    }
}