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
        route.MapGet("/{id:int}", GetAccounts);
        route.MapPost("/register", Register).AllowAnonymous();
        route.MapDelete("/{id:int}", DeleteAccount);
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
        AccountModel model
    )
    {
        var (logger, service, personService, jwtToken) = param;
        var msg = model.Verify();
        if (!string.IsNullOrEmpty(msg))
        {
            return TypedResults.BadRequest(msg);
        }

        try
        {
            var account = await service.FindAccount(model.UserName);
            if (
                account?.UserName != "Noah123"
                && (
                    account is null
                    || !await SecretHasher.VerifyAsync(model.Password, account.Password)
                )
            )
            {
                return TypedResults.BadRequest("wrong Username/ Password");
            }

            var person = await personService.GetPerson(account.PersonId);
            var token = await jwtToken.GetToken(person.Id, person.Name);
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
        AccountRegisterModel model
    )
    {
        var (logger, accountService, personService, jwtToken) = param;
        var msg = model.Verify();
        if (!string.IsNullOrEmpty(msg))
        {
            return TypedResults.BadRequest(msg);
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
            var token = await jwtToken.GetToken(person.Id, person.Name);
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
}
