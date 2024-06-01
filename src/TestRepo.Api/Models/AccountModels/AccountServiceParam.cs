namespace TestRepo.Api.Models.AccountModels;

// ReSharper disable once ClassNeverInstantiated.Global
public record AccountServiceParam(
    ILogger Logger,
    IAccountService Service,
    IPersonService PersonService,
    TokenUtility JwtTokenUtility
);
