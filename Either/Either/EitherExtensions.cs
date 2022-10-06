using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rem.Core.Utilities.Monads;

/// <summary>
/// General-purpose extension methods for the <see cref="Either{TLeft, TRight}"/> struct.
/// </summary>
public static class EitherExtensions
{
    #region Where
    /// <summary>
    /// Filters the value wrapped in the current <see cref="Either{TLeft, TRight}"/> by the specified predicate.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="either"></param>
    /// <param name="parentPredicate"></param>
    /// <returns></returns>
    public static IEnumerable<TParent> WhereEither<TLeft, TRight, TParent>(
        this Either<TLeft, TRight> either, Func<TParent, bool> parentPredicate)
        where TLeft : TParent
        where TRight : TParent
    {
        if (either.IsRight)
        {
            if (parentPredicate(either._right)) yield return either._right;
        }
        else
        {
            if (parentPredicate(either._left)) yield return either._left;
        }
    }

    /// <summary>
    /// Asynchronously filters the value wrapped in the current <see cref="Either{TLeft, TRight}"/> by the
    /// specified predicate.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="either"></param>
    /// <param name="parentPredicateAsync"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<TParent>> WhereEitherAsync<TLeft, TRight, TParent>(
        this Either<TLeft, TRight> either,
        Func<TParent, Task<bool>> parentPredicateAsync)
        where TLeft : TParent
        where TRight : TParent
    {
        var builder = ImmutableList.CreateBuilder<TParent>();
        if (either.IsRight)
        {
            if (await parentPredicateAsync(either._right)) builder.Add(either._right);
        }
        else
        {
            if (await parentPredicateAsync(either._left)) builder.Add(either._left);
        }
        return builder.ToImmutable();
    }

    /// <summary>
    /// Asynchronously filters the value wrapped in the current <see cref="Either{TLeft, TRight}"/> by the
    /// specified predicate.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="either"></param>
    /// <param name="parentPredicateAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<TParent>> WhereEitherAsync<TLeft, TRight, TParent>(
        this Either<TLeft, TRight> either,
        Func<TParent, CancellationToken, Task<bool>> parentPredicateAsync,
        CancellationToken cancellationToken = default)
        where TLeft : TParent
        where TRight : TParent
    {
        var builder = ImmutableList.CreateBuilder<TParent>();
        if (either.IsRight)
        {
            if (await parentPredicateAsync(either._right, cancellationToken)) builder.Add(either._right);
        }
        else
        {
            if (await parentPredicateAsync(either._left, cancellationToken)) builder.Add(either._left);
        }
        return builder.ToImmutable();
    }
    #endregion
}
