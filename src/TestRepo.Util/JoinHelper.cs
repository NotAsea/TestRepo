using System.Linq.Expressions;

// ReSharper disable MemberCanBePrivate.Global

namespace TestRepo.Util;

public static class JoinHelper
{
    /// <summary>
    /// Left join will outer join left to right collection, which mean, record not exist in right will stay null,
    /// to have reverse effect, reverse parameter order
    /// </summary>
    /// <param name="left">base collection</param>
    /// <param name="right">collection to join, will null if member in left not found in right</param>
    /// <param name="leftKey">collection <paramref name="left"/> key to join</param>
    /// <param name="rightKey">collection <paramref name="right"/> key to join</param>
    /// <param name="join">result collection</param>
    /// <typeparam name="TLeft">collection type</typeparam>
    /// <typeparam name="TRight">collection type</typeparam>
    /// <typeparam name="TKey">collection key type</typeparam>
    /// <typeparam name="TOutput">collection type</typeparam>
    /// <exception cref="ArgumentNullException">The base collection <paramref name="left"/> is null</exception>
    /// <returns>result of left outer join <paramref name="left" /> to <paramref name="right"/></returns>
    public static IQueryable<TOutput> LeftJoin<TLeft, TRight, TKey, TOutput>(
        this IQueryable<TLeft> left,
        IEnumerable<TRight> right,
        Expression<Func<TLeft, TKey>> leftKey,
        Expression<Func<TRight, TKey>> rightKey,
        Expression<Func<TLeft, TRight?, TOutput>> join
    ) => LeftJoinImpl(left, right, leftKey, rightKey, join, EqualityComparer<TKey>.Default);

    /// <summary>
    /// Left join will outer join left to right collection, which mean, record not exist in right will stay null,
    /// <paramref name="comparer"/> is used to define key comparer between two collection keys,
    /// to have reverse effect, reverse parameter order
    /// </summary>
    /// <param name="left">base collection</param>
    /// <param name="right">collection to join, will null if member in left not found in right</param>
    /// <param name="leftKey">collection <paramref name="left"/> key to join</param>
    /// <param name="rightKey">collection <paramref name="right"/> key to join</param>
    /// <param name="join">result collection</param>
    /// <param name="comparer">Comparer to use</param>
    /// <typeparam name="TLeft">collection type</typeparam>
    /// <typeparam name="TRight">collection type</typeparam>
    /// <typeparam name="TKey">collection key type</typeparam>
    /// <typeparam name="TOutput">collection type</typeparam>
    /// <exception cref="ArgumentNullException">The base collection <paramref name="left"/> is null</exception>
    /// <returns>result of left outer join <paramref name="left" /> to <paramref name="right"/></returns>
    public static IQueryable<TOutput> LeftJoin<TLeft, TRight, TKey, TOutput>(
        this IQueryable<TLeft> left,
        IEnumerable<TRight> right,
        Expression<Func<TLeft, TKey>> leftKey,
        Expression<Func<TRight, TKey>> rightKey,
        Expression<Func<TLeft, TRight?, TOutput>> join,
        IEqualityComparer<TKey> comparer
    ) => LeftJoinImpl(left, right, leftKey, rightKey, join, comparer);

    #region Left Join Internal

    private static IQueryable<TOutput> LeftJoinImpl<TLeft, TRight, TKey, TOutput>(
        this IQueryable<TLeft> left,
        IEnumerable<TRight> right,
        Expression<Func<TLeft, TKey>> leftKey,
        Expression<Func<TRight, TKey>> rightKey,
        Expression<Func<TLeft, TRight?, TOutput>> join,
        IEqualityComparer<TKey> comparer
    )
    {
        ArgumentNullException.ThrowIfNull(left);
        var paramJ = Expression.Parameter(typeof(LeftJoinInternal<TLeft, TRight>));
        var paramR = Expression.Parameter(typeof(TRight));
        var body = Expression.Invoke(join, Expression.Field(paramJ, "L"), paramR);
        var l = Expression.Lambda<Func<LeftJoinInternal<TLeft, TRight>, TRight, TOutput>>(
            body,
            paramJ,
            paramR
        );
        return left.GroupJoin(
                right,
                leftKey,
                rightKey,
                (le, r) => new LeftJoinInternal<TLeft, TRight> { L = le, R = r },
                comparer
            )
            .SelectMany(j => j.R.DefaultIfEmpty()!, l);
    }

    private sealed class LeftJoinInternal<TLeft, TRight>
    {
        // ReSharper disable once NotAccessedField.Local
        public TLeft L = default!;
        public IEnumerable<TRight> R = default!;
    }

    #endregion
}