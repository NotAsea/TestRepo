namespace TestRepo.Service.Services;

// ReSharper disable once SuggestBaseTypeForParameterInConstructor
/// <summary>
///     Base service class contain boiler plate code to add, update and remove entity
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
    protected async Task AddToDatabase<T>(T data)
        where T : class
    {
        await using var transaction = await repository.BeginTransactionAsync();
        try
        {
            await repository.AddAsync(data);
            await repository.SaveChangesAsync();
            await transaction.CommitAsync();
            repository.ClearChangeTracker();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    ///     Insert multiple entities to database
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <remarks>transaction is taking to account, so no trash data will leave in database</remarks>
    protected async Task AddToDatabase<T>(IEnumerable<T> data)
        where T : class
    {
        await using var transaction = await repository.BeginTransactionAsync();
        try
        {
            await context.BulkInsertAsync(data);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    ///     Update this entity to database
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <remarks>transaction is taking to account, so no trash data will leave in database</remarks>
    protected async Task UpdateToDatabase<T>(T data)
        where T : class
    {
        await using var transaction = await repository.BeginTransactionAsync();
        try
        {
            repository.Update(data);
            await repository.SaveChangesAsync();
            await transaction.CommitAsync();
            repository.ClearChangeTracker();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    ///     Update multiple entities to database
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <remarks>transaction is taking to account, so no trash data will leave in database</remarks>
    protected async Task UpdateToDatabase<T>(IEnumerable<T> data)
        where T : class
    {
        await using var transaction = await repository.BeginTransactionAsync();
        try
        {
            await context.BulkUpdateAsync(data);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    ///     <b>Permanent Delete</b> this entity to database.<br />
    ///     If soft delete is desire, use <see cref="UpdateToDatabase{T}(T)" /> instead
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <remarks>transaction is taking to account, so no trash data will leave in database</remarks>
    protected async Task RemoveToDatabase<T>(T data)
        where T : class
    {
        await using var transaction = await repository.BeginTransactionAsync();
        try
        {
            repository.Remove(data);
            await repository.SaveChangesAsync();
            await transaction.CommitAsync();
            repository.ClearChangeTracker();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    ///     <b>Permanent Delete</b> multiple entities to database.<br />
    ///     If soft delete is desire, use <see cref="UpdateToDatabase{T}(T)" /> instead. <br />
    ///     <b>Collection expect to be already filter out</b>
    /// </summary>
    /// <param name="data"></param>
    /// <typeparam name="T"></typeparam>
    /// <remarks>transaction is taking to account, so no trash data will leave in database</remarks>
    protected async Task RemoveToDatabase<T>(IEnumerable<T> data)
        where T : class
    {
        await using var transaction = await repository.BeginTransactionAsync();
        try
        {
            await context.BulkDeleteAsync(data);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}