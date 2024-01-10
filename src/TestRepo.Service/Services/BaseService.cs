using static TestRepo.Data.QueryTransactionStatic;

namespace TestRepo.Service.Services;

// ReSharper disable once SuggestBaseTypeForParameterInConstructor
/// <summary>
///     Base service class contain boilerplate code to add, update and remove entity
/// </summary>
/// <param name="repository"></param>
/// <param name="context"></param>
internal abstract class BaseService(IRepository repository, MyAppContext context)
{
    /// <summary>
    ///     Update this entity to Database
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <remarks>transaction is taking to account, so no trash data will leave in database</remarks>
    protected Task AddToDatabase<T>(T data)
        where T : class =>
        QueryInTransactionScope(
            static async (repo, d) =>
            {
                await repo.AddAsync(d);
                await repo.SaveChangesAsync().ConfigureAwait(false);
            },
            data,
            repository,
            true
        );

    /// <summary>
    ///     Insert multiple entities to database
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <remarks>transaction is taking to account, so no trash data will leave in database</remarks>
    protected Task AddToDatabase<T>(IEnumerable<T> data)
        where T : class =>
        QueryInTransactionScope(
            static async (ctx, d) => await ctx.BulkInsertAsync(d).ConfigureAwait(false),
            data,
            context
        );

    /// <summary>
    ///     Update this entity to database
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <remarks>transaction is taking to account, so no trash data will leave in database</remarks>
    protected Task UpdateToDatabase<T>(T data)
        where T : class =>
        QueryInTransactionScope(
            static async (repo, d) =>
            {
                repo.Update(d);
                await repo.SaveChangesAsync().ConfigureAwait(false);
            },
            data,
            repository,
            true
        );

    /// <summary>
    ///     Update multiple entities to database
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <remarks>transaction is taking to account, so no trash data will leave in database</remarks>
    protected Task UpdateToDatabase<T>(IEnumerable<T> data)
        where T : class =>
        QueryInTransactionScope(
            static async (ctx, d) => await ctx.BulkUpdateAsync(d).ConfigureAwait(false),
            data,
            context
        );

    /// <summary>
    ///     <b>Permanent Delete</b> this entity to database.<br />
    ///     If soft delete is desire, use <see cref="UpdateToDatabase{T}(T)" /> instead
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <remarks>transaction is taking to account, so no trash data will leave in database</remarks>
    protected Task RemoveToDatabase<T>(T data)
        where T : class =>
        QueryInTransactionScope(
            static async (repo, d) =>
            {
                repo.Remove(d);
                await repo.SaveChangesAsync().ConfigureAwait(false);
            },
            data,
            repository,
            false
        );

    /// <summary>
    ///     <b>Permanent Delete</b> multiple entities to database.<br />
    ///     If soft delete is desire, use <see cref="UpdateToDatabase{T}(T)" /> instead. <br />
    ///     <b>Collection expect to be already filter out</b>
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <remarks>transaction is taking to account, so no trash data will leave in database</remarks>
    protected Task RemoveToDatabase<T>(IEnumerable<T> data)
        where T : class =>
        QueryInTransactionScope(
            static async (ctx, d) => await ctx.BulkDeleteAsync(d).ConfigureAwait(false),
            data,
            context
        );
}
