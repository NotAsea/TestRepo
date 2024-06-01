namespace TestRepo.Service.Services;

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
        QueryTransactionStatic.QueryInTransactionScope(
            repository,
            data,
            static async (repo, d) =>
            {
                await repo.AddAsync(d).ConfigureAwait(false);
                await repo.SaveChangesAsync().ConfigureAwait(false);
            }
        );

    /// <summary>
    ///     Insert multiple entities to database
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <remarks>transaction is taking to account, so no trash data will leave in database</remarks>
    protected Task AddToDatabase<T>(IEnumerable<T> data)
        where T : class =>
        QueryTransactionStatic.QueryInTransactionScope(
            context,
            data,
            static (ctx, d) => ctx.BulkInsertAsync(d)
        );

    /// <summary>
    ///     Update this entity to database
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <remarks>transaction is taking to account, so no trash data will leave in database</remarks>
    protected Task UpdateToDatabase<T>(T data)
        where T : class =>
        QueryTransactionStatic.QueryInTransactionScope(
            repository,
            data,
            static (repo, d) =>
            {
                repo.Update(d);
                return repo.SaveChangesAsync();
            }
        );

    /// <summary>
    ///     Update multiple entities to database
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <remarks>transaction is taking to account, so no trash data will leave in database</remarks>
    protected Task UpdateToDatabase<T>(IEnumerable<T> data)
        where T : class =>
        QueryTransactionStatic.QueryInTransactionScope(
            context,
            data,
            static (ctx, d) => ctx.BulkUpdateAsync(d)
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
        QueryTransactionStatic.QueryInTransactionScope(
            repository,
            data,
            static (repo, d) =>
            {
                repo.Remove(d);
                return repo.SaveChangesAsync();
            }
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
        QueryTransactionStatic.QueryInTransactionScope(
            context,
            data,
            static (ctx, d) => ctx.BulkDeleteAsync(d)
        );
}
