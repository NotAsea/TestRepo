﻿namespace TestRepo.Api.Models.PersonModels;

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
        string[] possibleSortBy = ["name", "email", "id", "createddate"];
        if (
            !string.IsNullOrEmpty(param.SortBy)
            && !possibleSortBy.ContainGenForStringEnumerable(param.SortType)
        )
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
