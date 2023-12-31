﻿using TanvirArjel.EFCore.GenericRepository;

namespace TestRepo.Data;

internal static class QueryTransactionStatic
{
    /// <summary>
    /// Method for query in transaction scope,
    /// suitable for add, update, remove single entity in Database,
    /// auto create transaction and commit after done,
    /// auto roll back when exception happened
    /// </summary>
    /// <param name="action">action to do, expect static async lambda</param>
    /// <param name="data">entity</param>
    /// <param name="repository">IRepository context, often inject from DI</param>
    /// <param name="isClearTrackerAfterDone">if true, clear change tracker of DbContext</param>
    /// <typeparam name="T">Entity type</typeparam>
    internal static async Task QueryInTransactionScope<T>(
        Func<IRepository, T, Task> action,
        T data,
        IRepository repository,
        bool isClearTrackerAfterDone
    )
        where T : class
    {
        await using var transaction = await repository.BeginTransactionAsync();
        try
        {
            await action(repository, data);
            await transaction.CommitAsync();
            if (isClearTrackerAfterDone)
                repository.ClearChangeTracker();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    /// Method for query in transaction scope, suitable for add, update, remove multiple entity in Database,
    /// auto create transaction and commit after done,
    /// auto roll back when exception happened
    /// </summary>
    /// <param name="action">since this intend for bulk operation, recommend using bulk operator from
    /// Z.EntityFramework.Extension or EFCore.BulkExtensions</param>
    /// <param name="data">List of Entity</param>
    /// <param name="context">this AppContext, often application context derived from DbContext</param>
    /// <typeparam name="T">Entity type</typeparam>
    internal static async Task QueryInTransactionScope<T>(
        Func<DbContext, IEnumerable<T>, Task> action,
        IEnumerable<T> data,
        DbContext context
    )
        where T : class
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        try
        {
            await action(context, data);
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}