using TestRepo.Api.Models.AccountModels;

namespace TestRepo.Api.Routes;

internal static class AccountRoute
{
    /// <summary>
    ///     this consumes the <see cref="RouteGroupBuilder" /> and handle all logic and child route. <br />
    ///     This method must be and mean to be called last of <c>Map{Verb}</c> chain, as it return <see cref="Void" />
    /// </summary>
    /// <param name="route"></param>
    internal static void HandleAccountRoute(this IEndpointRouteBuilder route)
    {
        route.MapPost("/", Login).AllowAnonymous();
        route.MapGet("/{id:int}", GetAccounts).RequireAuthorization();
        route.MapPost("/register", Register).AllowAnonymous();
        route.MapDelete("/{id:int}", DeleteAccount).RequireAuthorization();
        route.MapGet("/current", CurrentAccountInfo).RequireAuthorization();
    }

    private static async Task<Results<Ok<AccountModel>, BadRequest<string>>> GetAccounts(
        [AsParameters] AccountServiceParam param,
        int id
    )
    {
        var (logger, accountService, _, _) = param;
        try
        {
            return TypedResults.Ok(await accountService.GetAccount(id));
        }
        catch (Exception ex)
        {
            var reason = ex.GetBaseException().Message;
            logger.ReadFromDatabaseFail("Account", reason);
            return TypedResults.BadRequest(reason);
        }
    }

    private static async Task<Results<Ok<string>, BadRequest<string>>> Login(
        [AsParameters] AccountServiceParam param,
        IValidator<AccountModel> validator,
        AccountModel model
    )
    {
        var (logger, service, personService, jwtToken) = param;

        var msg = await validator.ValidateAsync(model);
        if (!msg.IsValid)
        {
            return TypedResults.BadRequest(msg.ToString("-"));
        }

        try
        {
            var account = await service.FindAccount(model.UserName);
            if (
                account is null
                || !await SecretHasher.VerifyAsync(model.Password, account.Password)
            )
            {
                return TypedResults.BadRequest("wrong Username/ Password");
            }

            var person = await personService.GetPerson(account.PersonId);
            var token = await jwtToken.GetTokenForDay(
                [new TokenBody(AppTokenType.Id, person.Id.ToString())]
            );
            return TypedResults.Ok(token);
        }
        catch (Exception ex)
        {
            var reason = ex.GetBaseException().Message;
            logger.AuthenticateFail(reason, ex.StackTrace!);
            return TypedResults.BadRequest(reason);
        }
    }

    private static async Task<Results<Ok<string>, BadRequest<string>>> Register(
        [AsParameters] AccountServiceParam param,
        IValidator<AccountRegisterModel> validator,
        AccountRegisterModel model
    )
    {
        var (logger, accountService, personService, jwtToken) = param;
        var msg = await validator.ValidateAsync(model);
        if (!msg.IsValid)
        {
            return TypedResults.BadRequest(msg.ToString("-"));
        }

        try
        {
            var exist = await accountService.FindAccount(model.UserName);
            if (exist != null)
            {
                return TypedResults.BadRequest("UserName already exist!!");
            }

            var account = model.ToAccount();
            account = account with { Password = await SecretHasher.HashAsync(account.Password) };
            var accountId = await accountService.SaveAccount(account);
            var person = model.ToPerson();
            var id = await personService.SavePerson(person);
            person = person with { Id = id };
            account = account with { PersonId = id, Id = accountId };
            await accountService.SaveAccount(account);
            var token = await jwtToken.GetTokenForDay(
                [
                    new TokenBody(AppTokenType.Id, person.Id.ToString()),
                    new TokenBody(AppTokenType.Name, person.Name)
                ]
            );
            return TypedResults.Ok(token);
        }
        catch (Exception ex)
        {
            var reason = ex.GetBaseException().Message;
            logger.RegisterFail(reason);
            return TypedResults.BadRequest(reason);
        }
    }

    private static async Task<Results<Ok<int>, BadRequest<string>>> DeleteAccount(
        [AsParameters] AccountServiceParam param,
        int id,
        bool isForce
    )
    {
        var (logger, accountService, _, _) = param;
        try
        {
            return TypedResults.Ok(await accountService.DeleteAccount(id, isForce));
        }
        catch (Exception ex)
        {
            var reason = ex.GetBaseException().Message;
            logger.WriteToDatabaseFail("Account", reason);
            return TypedResults.BadRequest(reason);
        }
    }

    private static async Task<Results<Ok<PersonAccount>, BadRequest<string>>> CurrentAccountInfo(
        [AsParameters] AccountServiceParam param,
        HttpContext context
    )
    {
        var (logger, _, _, _) = param;
        try
        {
            return TypedResults.Ok(await context.GetPersonFromToken());
        }
        catch (Exception ex)
        {
            var reason = ex.GetBaseException().Message;
            logger.ReadTokenFail("Account", reason);
            return TypedResults.BadRequest(reason);
        }
    }
}
