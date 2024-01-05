// ReSharper disable ClassNeverInstantiated.Global

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
