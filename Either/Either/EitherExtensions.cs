using System;
using System.Collections.Generic;
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
    #region IEnumerable (No Default Value)
    /// <summary>
    /// Filters the value wrapped in the current <see cref="Either{TLeft, TRight}"/> by the specified predicate.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="either"></param>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public static IEnumerable<TParent> Where<TLeft, TRight, TParent>(
        this Either<TLeft, TRight> either, Func<TParent, bool> predicate)
        where TLeft : TParent
        where TRight : TParent
    {
        if (either.IsRight)
        {
            if (predicate(either._right)) yield return either._right;
        }
        else
        {
            if (predicate(either._left)) yield return either._left;
        }
    }
    #endregion

    #region Either (Default Values)
    #region Eager
    /// <summary>
    /// Filters the value wrapped in the current <see cref="Either{TLeft, TRight}"/> by the specified predicate.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="either"></param>
    /// <param name="predicate"></param>
    /// <param name="leftDefaultValue"></param>
    /// <param name="rightDefaultValue"></param>
    /// <returns>
    /// The current instance if the predicate passes,
    /// -or-
    /// A new instance wrapping <paramref name="leftDefaultValue"/> on the left if the current instance is right and
    /// the predicate fails,
    /// -or-
    /// A new instance wrapping <paramref name="rightDefaultValue"/> on the right if the current instance is left and
    /// the predicate fails.
    public static Either<TLeft, TRight> Where<TLeft, TRight, TParent>(
        in this Either<TLeft, TRight> either,
        Func<TParent, bool> predicate, TLeft leftDefaultValue, TRight rightDefaultValue)
        where TLeft : TParent
        where TRight : TParent
        => either.IsRight
            ? (predicate(either._right) ? either : leftDefaultValue)
            : (predicate(either._left) ? either : rightDefaultValue);
    #endregion

    #region Lazy
    /// <summary>
    /// Filters the value wrapped in the current <see cref="Either{TLeft, TRight}"/> by the specified predicate.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="either"></param>
    /// <param name="predicate"></param>
    /// <param name="leftDefaultValueFactory"></param>
    /// <param name="rightDefaultValue"></param>
    /// <returns>
    /// The current instance if the predicate passes,
    /// -or-
    /// A new instance wrapping <paramref name="leftDefaultValueFactory"/> on the left if the current instance is
    /// right and the predicate fails,
    /// -or-
    /// A new instance wrapping <paramref name="rightDefaultValue"/> on the right if the current instance is left and
    /// the predicate fails.
    public static Either<TLeft, TRight> Where<TLeft, TRight, TParent>(
        in this Either<TLeft, TRight> either,
        Func<TParent, bool> predicate, Func<TLeft> leftDefaultValueFactory, TRight rightDefaultValue)
        where TLeft : TParent
        where TRight : TParent
        => either.IsRight
            ? (predicate(either._right) ? either : leftDefaultValueFactory())
            : (predicate(either._left) ? either : rightDefaultValue);

    /// <summary>
    /// Filters the value wrapped in the current <see cref="Either{TLeft, TRight}"/> by the specified predicate.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="either"></param>
    /// <param name="predicate"></param>
    /// <param name="leftDefaultValue"></param>
    /// <param name="rightDefaultValueFactory"></param>
    /// <returns>
    /// The current instance if the predicate passes,
    /// -or-
    /// A new instance wrapping <paramref name="leftDefaultValue"/> on the left if the current instance is right and
    /// the predicate fails,
    /// -or-
    /// A new instance wrapping <paramref name="rightDefaultValueFactory"/> on the right if the current instance is
    /// left and the predicate fails.
    public static Either<TLeft, TRight> Where<TLeft, TRight, TParent>(
        in this Either<TLeft, TRight> either,
        Func<TParent, bool> predicate, TLeft leftDefaultValue, Func<TRight> rightDefaultValueFactory)
        where TLeft : TParent
        where TRight : TParent
        => either.IsRight
            ? (predicate(either._right) ? either : leftDefaultValue)
            : (predicate(either._left) ? either : rightDefaultValueFactory());

    /// <summary>
    /// Filters the value wrapped in the current <see cref="Either{TLeft, TRight}"/> by the specified predicate.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="either"></param>
    /// <param name="predicate"></param>
    /// <param name="leftDefaultValueFactory"></param>
    /// <param name="rightDefaultValueFactory"></param>
    /// <returns>
    /// The current instance if the predicate passes,
    /// -or-
    /// A new instance wrapping <paramref name="leftDefaultValueFactory"/> on the left if the current instance is
    /// right and the predicate fails,
    /// -or-
    /// A new instance wrapping <paramref name="rightDefaultValueFactory"/> on the right if the current instance is
    /// left and the predicate fails.
    public static Either<TLeft, TRight> Where<TLeft, TRight, TParent>(
        in this Either<TLeft, TRight> either,
        Func<TParent, bool> predicate, Func<TLeft> leftDefaultValueFactory, Func<TRight> rightDefaultValueFactory)
        where TLeft : TParent
        where TRight : TParent
        => either.IsRight
            ? (predicate(either._right) ? either : leftDefaultValueFactory())
            : (predicate(either._left) ? either : rightDefaultValueFactory());
    #endregion
    #endregion
    #endregion
}
