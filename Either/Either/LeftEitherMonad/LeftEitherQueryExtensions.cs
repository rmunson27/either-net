using Rem.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rem.Core.Utilities.Monads.LeftEitherMonad;

/// <summary>
/// Extension methods providing selectors and other query methods for <see cref="Either{TLeft, TRight}"/> instances
/// through the left side.
/// </summary>
public static class LeftEitherQueryExtensions
{
    /// <summary>
    /// Maps a selector over the left side of the current <see cref="Either{TLeft, TRight}"/>.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <param name="either"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static Either<TLeftResult, TRight> Select<TLeft, TLeftResult, TRight>(
        [NonDefaultableStruct] in this Either<TLeft, TRight> either, Func<TLeft, TLeftResult> selector)
        => either.SelectLeft(selector);

    /// <summary>
    /// Maps a selector over the left side of the current <see cref="Either{TLeft, TRight}"/>.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <param name="either"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static Either<TLeftResult, TRight> SelectMany<TLeft, TLeftResult, TRight>(
        [NonDefaultableStruct] in this Either<TLeft, TRight> either, Func<TLeft, Either<TLeftResult, TRight>> selector)
        => either.SelectManyLeft(selector);

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> containing the <typeparamref name="TLeft"/> value wrapped in the
    /// current <see cref="Either{TLeft, TRight}"/> on the left, or an empty <see cref="IEnumerable{T}"/> if the
    /// current <see cref="Either{TLeft, TRight}"/> is right.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <param name="either"></param>
    /// <returns></returns>
    public static IEnumerable<TLeft> AsEnumerable<TLeft, TRight>(
        [NonDefaultableStruct] in this Either<TLeft, TRight> either)
        => either.EnumerateLeft();

    /// <summary>
    /// Gets an <see cref="IEnumerator{T}"/> that enumerates over the left side of the current
    /// <see cref="Either{TLeft, TRight}"/>.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <param name="either"></param>
    /// <returns></returns>
    public static IEnumerator<TLeft> GetEnumerator<TLeft, TRight>(
        [NonDefaultableStruct] in this Either<TLeft, TRight> either)
        => either.GetLeftEnumerator();

    /// <summary>
    /// Filters the left side of the current <see cref="Either{TLeft, TRight}"/> based on a predicate.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <param name="either"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static IEnumerable<TLeft> Where<TLeft, TRight>(
        [NonDefaultableStruct] in this Either<TLeft, TRight> either, Func<TLeft, bool> predicate)
        => either.WhereLeft(predicate);
}
