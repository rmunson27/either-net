using Rem.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rem.Core.Utilities.Monads.RightEitherMonad;

/// <summary>
/// Extension methods providing selectors and other query methods for <see cref="Either{TLeft, TRight}"/> instances
/// through the right side.
/// </summary>
public static class RightEitherQueryExtensions
{
    /// <summary>
    /// Maps a selector over the right side of the current <see cref="Either{TLeft, TRight}"/>.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="either"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    [return: NotDefaultIfNotDefault("either")]
    public static Either<TLeft, TRightResult> Select<TLeft, TRight, TRightResult>(
        in this Either<TLeft, TRight> either, Func<TRight, TRightResult> selector)
        => either.SelectRight(selector);

    /// <summary>
    /// Maps a selector over the right side of the current <see cref="Either{TLeft, TRight}"/>.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="either"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    [return: MaybeDefaultIfDefault("either")]
    public static Either<TLeft, TRightResult> SelectMany<TLeft, TRight, TRightResult>(
        in this Either<TLeft, TRight> either, Func<TRight, Either<TLeft, TRightResult>> selector)
        => either.SelectManyRight(selector);

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> containing the <typeparamref name="TRight"/> value wrapped in the
    /// current <see cref="Either{TLeft, TRight}"/> on the right, or an empty <see cref="IEnumerable{T}"/> if the
    /// current <see cref="Either{TLeft, TRight}"/> is left.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <param name="either"></param>
    /// <returns></returns>
    public static IEnumerable<TRight> AsEnumerable<TLeft, TRight>(in this Either<TLeft, TRight> either)
        => either.EnumerateRight();

    /// <summary>
    /// Gets an <see cref="IEnumerator{T}"/> that enumerates over the right side of the current
    /// <see cref="Either{TLeft, TRight}"/>.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <param name="either"></param>
    /// <returns></returns>
    public static IEnumerator<TRight> GetEnumerator<TLeft, TRight>(in this Either<TLeft, TRight> either)
        => either.GetRightEnumerator();

    /// <summary>
    /// Filters the right side of the current <see cref="Either{TLeft, TRight}"/> based on a predicate.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <param name="either"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static IEnumerable<TRight> Where<TLeft, TRight>(
        in this Either<TLeft, TRight> either, Func<TRight, bool> predicate)
        => either.WhereRight(predicate);
}
