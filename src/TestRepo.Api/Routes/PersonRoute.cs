using TestRepo.Api.Models.PersonModels;

namespace TestRepo.Api.Routes;

internal static class PersonRoute
{
    /// <summary>
    ///     this consumes the <see cref="RouteGroupBuilder" /> and handle all logic and child route. <br />
    ///     This method must be and mean to be called last of <c>Map{Verb}</c> chain, as it return <see cref="Void" />
    /// </summary>
    /// <param name="route"></param>
    internal static void HandlePersonRoute(this IEndpointRouteBuilder route)
    {
        route.MapGet("/", GetAllPerson).RequireAuthorization();
        route.MapGet("/{id:int}", GetPerson).RequireAuthorization();
        route.MapPost("/", CreatePerson).RequireAuthorization();
        route.MapDelete("/delete", DeleteList).RequireAuthorization();
        route.MapDelete("/delete/{id:int}", DeletePerson).RequireAuthorization();
        route.MapPatch("/", UpdatePerson).RequireAuthorization();
        route.MapPut("/active/{id:int}", ActivatePerson).RequireAuthorization();
        route.MapPut("/active", ActivatePeople).RequireAuthorization();
    }

    private static async Task<Results<Ok<int>, BadRequest<string>>> CreatePerson(
        [AsParameters] PersonRouteDefaultParam param,
        IValidator<PersonModel> validator,
        PersonModel person
    )
    {
        var (logger, service, _) = param;
        var msg = await validator.ValidateAsync(person);
        if (!msg.IsValid)
        {
            return TypedResults.BadRequest(msg.ToString(","));
        }

        try
        {
            return TypedResults.Ok(await service.SavePerson(person));
        }
        catch (Exception ex)
        {
            var reason = ex.GetBaseException().Message;
            logger.WriteToDatabaseFail("Person", reason);
            return TypedResults.BadRequest(reason);
        }
    }

    private static async Task<Results<Ok<int>, NotFound<string>>> DeletePerson(
        [AsParameters] PersonRouteDefaultParam param,
        int id,
        bool force = false
    )
    {
        var (logger, service, _) = param;
        try
        {
            await service.DeletePerson(id, force);
            return TypedResults.Ok(id);
        }
        catch (Exception ex)
        {
            logger.WriteToDatabaseFail("Person", ex.GetBaseException().Message);
            return TypedResults.NotFound("Person");
        }
    }

    private static async Task<Results<Ok<int>, NotFound<string>>> DeleteList(
        [AsParameters] PersonRouteDefaultParam param,
        [FromBody] int[] peopleId,
        bool force = false
    )
    {
        var (logger, service, _) = param;
        try
        {
            await service.DeletePeople(peopleId, force);
            return TypedResults.Ok(peopleId[0]);
        }
        catch (Exception ex)
        {
            logger.WriteToDatabaseFail("Person", ex.GetBaseException().Message);
            return TypedResults.NotFound("Person");
        }
    }

    private static async Task<Results<JsonHttpResult<PersonModel>, NotFound<string>>> GetPerson(
        [AsParameters] PersonRouteDefaultParam param,
        int id
    )
    {
        var (logger, service, _) = param;
        try
        {
            var res = await service.GetPerson(id);
            return TypedResults.Json(res, PersonSerializer.Default.PersonModel);
        }
        catch (Exception ex)
        {
            logger.ReadFromDatabaseFail("Person", ex.GetBaseException().Message);
            return TypedResults.NotFound("Person");
        }
    }

    private static async Task<Results<JsonHttpResult<ListReturn>, NotFound<string>>> GetAllPerson(
        [AsParameters] PersonRouteDefaultParam param,
        [AsParameters] SearchPersonModel search
    )
    {
        var (logger, service, _) = param;
        try
        {
            search = search.Verify();
            return TypedResults.Json(
                await service.GetPeople(
                    search.Index,
                    search.Size,
                    search.SortBy,
                    search.SortType,
                    search.NameSearch
                ),
                PersonSerializer.Default.ListReturn
            );
        }
        catch (Exception ex)
        {
            logger.ReadFromDatabaseFail("Person", ex.GetBaseException().Message);
            return TypedResults.NotFound("Person");
        }
    }

    private static async Task<Results<Ok<int>, BadRequest<string>>> UpdatePerson(
        [AsParameters] PersonRouteDefaultParam param,
        IValidator<PersonModel> validator,
        PersonModel person
    )
    {
        var (logger, service, _) = param;
        var msg = await validator.ValidateAsync(person);
        if (!msg.IsValid)
        {
            return TypedResults.BadRequest(msg.ToString(","));
        }

        try
        {
            await service.SavePerson(person);
            return TypedResults.Ok(person.Id);
        }
        catch (Exception ex)
        {
            var reason = ex.GetBaseException().Message;
            logger.WriteToDatabaseFail("Person", reason);
            return TypedResults.BadRequest(reason);
        }
    }

    private static async Task<Results<Ok<int>, BadRequest<string>>> ActivatePerson(
        [AsParameters] PersonRouteDefaultParam param,
        int id
    )
    {
        var (logger, service, _) = param;
        try
        {
            await service.ActivatePerson(id);
            return TypedResults.Ok(id);
        }
        catch (Exception ex)
        {
            var reason = ex.GetBaseException().Message;
            logger.WriteToDatabaseFail("Person", reason);
            return TypedResults.BadRequest(reason);
        }
    }

    private static async Task<Results<Ok<int>, BadRequest<string>>> ActivatePeople(
        [AsParameters] PersonRouteDefaultParam param,
        [FromBody] int[] ids
    )
    {
        var (logger, service, _) = param;
        try
        {
            await service.ActivatePeople(ids);
            return TypedResults.Ok(ids[0]);
        }
        catch (Exception ex)
        {
            var reason = ex.GetBaseException().Message;
            logger.WriteToDatabaseFail("Person", reason);
            return TypedResults.BadRequest(reason);
        }
    }
}