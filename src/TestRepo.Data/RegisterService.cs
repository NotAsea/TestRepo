using Microsoft.Extensions.DependencyInjection;
using TanvirArjel.EFCore.GenericRepository;
using TestRepo.Data.CompiledModels;

namespace TestRepo.Data;

internal static class RegisterService
{
    internal static void AddRepository(this IServiceCollection service, string connectionString)
    {
        service.AddDbContext<MyAppContext>(opt =>
        {
            opt.UseModel(MyAppContextModel.Instance);
            opt.UseNpgsql(connectionString);
        });
        service.AddGenericRepository<MyAppContext>();
    }
}
