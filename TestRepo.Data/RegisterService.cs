using Microsoft.Extensions.DependencyInjection;
using TanvirArjel.EFCore.GenericRepository;

namespace TestRepo.Data;

internal static class RegisterService
{
    internal static void AddRepository(this IServiceCollection service, string connectionString)
    {
        service.AddDbContext<MyAppContext>(opt => opt.UseNpgsql(connectionString));
        service.AddGenericRepository<MyAppContext>();
    }
}
