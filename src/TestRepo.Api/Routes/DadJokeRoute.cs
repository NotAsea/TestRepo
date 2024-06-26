﻿namespace TestRepo.Api.Routes;

public static class DadJokeRoute
{
    public static void HandleDadJokeRoute(this IEndpointRouteBuilder route)
    {
        route.MapGet("/", GetDadJoke);
        route.MapGet("/search", SearchDadJoke);
    }

    private static async Task<Results<Ok<DadJokeModel>, Ok<string>, BadRequest<string>>> GetDadJoke(
        [AsParameters] DadJokeParam param,
        string? term,
        bool asString
    )
    {
        var (service, logger) = param;
        try
        {
            return asString
                ? TypedResults.Ok(await service.GetDadJokeAsString(term).ConfigureAwait(true))
                : TypedResults.Ok(await service.GetDadJoke(term).ConfigureAwait(true));
        }
        catch (Exception ex)
        {
            var reason = ex.GetBaseException().Message;
            logger.CallApiFail(reason, ex.StackTrace!);
            return TypedResults.BadRequest(reason);
        }
    }

    private static async Task<
        Results<Ok<DadJokeModelList>, Ok<string>, BadRequest<string>>
    > SearchDadJoke(
        [AsParameters] DadJokeParam param,
        bool asString,
        int? page,
        int? limit,
        string? term
    )
    {
        var (service, logger) = param;
        try
        {
            return asString
                ? TypedResults.Ok(
                    await service.SearchDadJokeAsString(page, limit, term).ConfigureAwait(true)
                )
                : TypedResults.Ok(
                    await service.SearchDadJoke(page, limit, term).ConfigureAwait(true)
                );
        }
        catch (Exception ex)
        {
            var reason = ex.GetBaseException().Message;
            logger.CallApiFail(reason, ex.StackTrace!);
            return TypedResults.BadRequest(reason);
        }
    }
}
