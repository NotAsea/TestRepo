namespace TestRepo.Models;

// ReSharper disable once ClassNeverInstantiated.Global
public record AccountServiceParam(
    ILogger Logger,
    IAccountService Service,
    IPersonService PersonService,
    GenerateJwtToken JwtToken
);
