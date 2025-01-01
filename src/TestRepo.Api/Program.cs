using TestRepo.Api.Routes;

var builder = WebApplication.CreateBuilder(args);

builder.RegisterService();

var app = builder.Build();

await app.StartupActionAsync().ConfigureAwait(false);

app.MapGroup("/person").WithTags("Person").HandlePersonRoute();
app.MapGroup("/account").WithTags("Account").HandleAccountRoute();
app.MapGroup("/dad-joke").WithTags("DadJoke").HandleDadJokeRoute();

app.Run();