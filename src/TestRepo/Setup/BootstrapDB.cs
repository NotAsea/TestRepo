namespace TestRepo.Setup;

public static class BootstrapDb
{
    public static async Task InitialDb(this WebApplication app)
    {
        try
        {
            await using var scope = app.Services.CreateAsyncScope();
            await scope.ServiceProvider.InitDb();
        }
        catch (Exception ex)
        {
            app.Logger.InitializeDatabaseFail(ex.GetBaseException().Message);
        }
    }
}
