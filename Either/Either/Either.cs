using Rem.Core.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rem.Core.Utilities.Monads;

/// <summary>
/// Extension methods and other static functionality relating to generic <see cref="Either{TLeft, TRight}"/> instances.
/// </summary>
public static class Either
{
    /// <summary>
    /// Fixes the type of a common parent of both the left and right types of the <see cref="Either{TLeft, TRight}"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class TCommonParent<T>
    {
        /// <summary>
        /// Gets the value wrapped in the <see cref="Either{TLeft, TRight}"/> passed in typed as an instance
        /// of <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="either"></param>
        /// <returns></returns>
        public static T UnwrapChild<TLeft, TRight>(in Either<TLeft, TRight> either)
            where TLeft : T
            where TRight : T
            => either.IsRight ? either._right : either._left;
    }

    /// <summary>
    /// Fixes the left type of the <see cref="Either{TLeft, TRight}"/> to allow type inference on the right type.
    /// </summary>
    /// <typeparam name="T">The left type to fix.</typeparam>
    public static class TLeft<T>
    {
        /// <summary>
        /// Creates a new <see cref="Either{TLeft, TRight}"/> with the specified value on the right.
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Either<T, TRight> Right<TRight>(TRight Value) => new(Value);

        /// <summary>
        /// Converts the <typeparamref name="TChild"/> left type of the <see cref="Either{TLeft, TRight}"/> passed in
        /// to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="TChild"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="either"></param>
        /// <returns></returns>
        [return: NotDefaultIfNotDefault("either")]
        public static Either<T, TRight> FromChild<TChild, TRight>(in Either<TChild, TRight> either)
            where TChild : T
            => Either<T, TRight>.FromLeftChild(in either);
    }

    /// <summary>
    /// Fixes the right type of the <see cref="Either{TLeft, TRight}"/> to allow type inference on the left type.
    /// </summary>
    /// <typeparam name="T">The right type to fix.</typeparam>
    public static class TRight<T>
    {
        /// <summary>
        /// Creates a new <see cref="Either{TLeft, TRight}"/> with the specified value on the left.
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static Either<TLeft, T> Left<TLeft>(TLeft Value) => new(Value);

        /// <summary>
        /// Converts the <typeparamref name="TChild"/> right type of the <see cref="Either{TLeft, TRight}"/> passed in
        /// to <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TChild"></typeparam>
        /// <param name="either"></param>
        /// <returns></returns>
        [return: NotDefaultIfNotDefault("either")]
        public static Either<TLeft, T> FromChild<TLeft, TChild>(in Either<TLeft, TChild> either)
            where TChild : T
            => Either<TLeft, T>.FromRightChild(in either);
    }
}
