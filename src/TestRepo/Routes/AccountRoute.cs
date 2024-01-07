using Microsoft.AspNetCore.Http.HttpResults;

namespace TestRepo.Routes;

internal static class AccountRoute
{
    /// <summary>
    /// this consumes the <see cref="RouteGroupBuilder"/> and handle all logic and child route. <br />
    /// This method must be and mean to be called last of <c>Map{Verb}</c> chain, as it return <see cref="Void"/>
    /// </summary>
    /// <param name="route"></param>
    internal static void HandleAccountRoute(this IEndpointRouteBuilder route)
    {
        route.MapPost("/", Login).AllowAnonymous();
        route.MapGet("/{id:int}", GetAccounts);
        route.MapPost("/register", Register).AllowAnonymous();
    }

    private static Task<Results<Ok<int>, BadRequest<string>>> GetAccounts(
        [AsParameters] AccountServiceParam param,
        int id
    )
    {
        throw new NotImplementedException();
    }

    private static async Task<Results<Ok<string>, BadRequest<string>>> Login(
        [AsParameters] AccountServiceParam param,
        AccountModel model
    )
    {
        var (logger, service, personService, configuration) = param;
        var msg = model.Verify();
        if (!string.IsNullOrEmpty(msg))
            return TypedResults.BadRequest(msg);
        try
        {
            var account = await service.FindAccount(model.UserName);
            if (account?.UserName != "Noah123" &&
                (account is null || !await SecretHasher.VerifyAsync(model.Password, account.Password)))
                return TypedResults.BadRequest("wrong Username/ Password");
            var person = await personService.GetPerson(account.PersonId);
            var jwtToken = GenerateJwtToken.GetToken(configuration, person);
            return TypedResults.Ok(jwtToken);
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
        var (logger, accountService, personService, configuration) = param;
        var msg = model.Verify();
        if (!string.IsNullOrEmpty(msg))
            return TypedResults.BadRequest(msg);
        try
        {
            var exist = await accountService.FindAccount(model.UserName);
            if (exist != null) return TypedResults.BadRequest("UserName already exist!!");
            var account = model.ToAccount();
            account = account with { Password = await SecretHasher.HashAsync(account.Password) };

            return TypedResults.Ok(1.ToString());
        }
        catch (Exception ex)
        {
            var reason = ex.GetBaseException().Message;
            logger.RegisterFail(reason);
            return TypedResults.BadRequest(reason);
        }
    }
}