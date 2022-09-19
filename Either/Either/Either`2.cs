using Rem.Core.Attributes;
using Rem.Core.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Rem.Core.Utilities.Monads;

/// <summary>
/// Represents a single value of one of two possible types.
/// </summary>
[DefaultableIfTypeParamsNonDefaultable(nameof(TLeft))]
public readonly record struct Either<TLeft, TRight> : IDefaultableStruct
{
    #region Properties And Fields
    /// <inheritdoc/>
    public bool IsDefault => !IsRight && Defaults<TLeft>.IsDefault(_left);

    /// <summary>
    /// Gets whether or not this object wraps a value of type <typeparamref name="TLeft"/>.
    /// </summary>
    public bool IsLeft => !IsRight;

    /// <summary>
    /// Gets the <typeparamref name="TLeft"/> value wrapped in this instance, or the default value of type
    /// <typeparamref name="TLeft"/> if this instance wraps a value of type <typeparamref name="TRight"/>.
    /// </summary>
    [MaybeNull, MaybeDefault] public TLeft LeftOrDefault => _left;

    /// <summary>
    /// Gets the <typeparamref name="TLeft"/> value wrapped in this instance.
    /// </summary>
    /// <exception cref="EitherException">
    /// This instance wraps an instance of <typeparamref name="TRight"/>.
    /// </exception>
    public TLeft Left
        => IsRight
            ? throw new EitherException($"Invalid left access of {nameof(Either<TLeft, TRight>)}.")
            : _left;

    /// <summary>
    /// Gets the <typeparamref name="TRight"/> value wrapped in this instance, or the default value of type
    /// <typeparamref name="TRight"/> if this instance wraps a value of type <typeparamref name="TLeft"/>.
    /// </summary>
    [MaybeNull, MaybeDefault] public TRight RightOrDefault => _right;

    /// <summary>
    /// Gets the <typeparamref name="TRight"/> value wrapped in this instance.
    /// </summary>
    /// <exception cref="EitherException">
    /// This instance wraps an instance of <typeparamref name="TLeft"/>.
    /// </exception>
    public TRight Right
        => IsRight
            ? _right
            : throw new EitherException($"Invalid right access of {nameof(Either<TRight, TRight>)}.");

    /// <summary>
    /// Gets whether or not this object wraps a value of type <typeparamref name="TRight"/>.
    /// </summary>
    public bool IsRight { get; }

    [AllowNull, AllowDefault] internal readonly TLeft _left;
    [AllowNull, AllowDefault] internal readonly TRight _right;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new <see cref="Either{TLeft, TRight}"/> wrapping the <typeparamref name="TLeft"/> value passed in.
    /// </summary>
    /// <param name="Value"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Either(TLeft Value) : this(Value, default, IsRight: false) { }

    /// <summary>
    /// Constructs a new <see cref="Either{TLeft, TRight}"/> wrapping the <typeparamref name="TRight"/> value passed in.
    /// </summary>
    /// <param name="Value"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Either(TRight Value) : this(default, Value, IsRight: true) { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal Either([AllowNull, AllowDefault] TLeft LeftValue, [AllowNull, AllowDefault] TRight RightValue, bool IsRight)
    {
        _left = LeftValue;
        _right = RightValue;
        this.IsRight = IsRight;
    }
    #endregion

    #region Methods
    #region CombineSides
    /// <summary>
    /// Combines the sides of this instance into a single value using the combiner methods passed in.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="leftCombiner"></param>
    /// <param name="rightCombiner"></param>
    /// <returns></returns>
    public TResult CombineSides<TResult>(Func<TLeft, TResult> leftCombiner, Func<TRight, TResult> rightCombiner)
        => IsRight ? rightCombiner(_right) : leftCombiner(_left);
    #endregion

    #region Conversion
    /// <summary>
    /// Implicitly converts a <typeparamref name="TLeft"/> to an instance of <see cref="Either{TLeft, TRight}"/>.
    /// </summary>
    /// <param name="left"></param>
    public static implicit operator Either<TLeft, TRight>(TLeft left) => new(left);

    /// <summary>
    /// Implicitly converts a <typeparamref name="TRight"/> to an instance of <see cref="Either{TLeft, TRight}"/>.
    /// </summary>
    /// <param name="right"></param>
    public static implicit operator Either<TLeft, TRight>(TRight right) => new(right);

    /// <summary>
    /// Explicitly converts an <see cref="Either{TLeft, TRight}"/> to an instance of <typeparamref name="TLeft"/>.
    /// </summary>
    /// <param name="either"></param>
    /// <exception cref="EitherException">The either was right.</exception>
    public static explicit operator TLeft(Either<TLeft, TRight> either) => either.Left;
    
    /// <summary>
    /// Explicitly converts an <see cref="Either{TLeft, TRight}"/> to an instance of <typeparamref name="TRight"/>.
    /// </summary>
    /// <param name="either"></param>
    /// <exception cref="EitherException">The either was left.</exception>
    public static explicit operator TRight(Either<TLeft, TRight> either) => either.Right;
    #endregion

    #region Select
    /// <summary>
    /// Maps a selector over the left side of this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <param name="selector"></param>
    /// <returns></returns>
    public Either<TLeftResult, TRight> SelectLeft<TLeftResult>(Func<TLeft, TLeftResult> selector)
        => IsRight ? new(_right) : new(selector(_left));

    /// <summary>
    /// Maps a side-dependent selector over this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="leftSelector"></param>
    /// <param name="rightSelector"></param>
    /// <returns></returns>
    public Either<TLeftResult, TRightResult> Select<TLeftResult, TRightResult>(
        Func<TLeft, TLeftResult> leftSelector, Func<TRight, TRightResult> rightSelector)
        => IsRight ? new(rightSelector(_right)) : new(leftSelector(_left));

    /// <summary>
    /// Maps a selector over the right side of this instance.
    /// </summary>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="selector"></param>
    /// <returns></returns>
    public Either<TLeft, TRightResult> SelectRight<TRightResult>(Func<TRight, TRightResult> selector)
        => IsRight ? new(selector(_right)) : new(_left);
    #endregion

    #region SelectMany
    /// <summary>
    /// Maps a selector over the left side of this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <param name="selector"></param>
    /// <returns></returns>
    public Either<TLeftResult, TRight> SelectManyLeft<TLeftResult>(Func<TLeft, Either<TLeftResult, TRight>> selector)
        => IsRight ? new(_right) : selector(_left);

    /// <summary>
    /// Maps a side-specific selector over this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="leftSelector"></param>
    /// <param name="rightSelector"></param>
    /// <returns></returns>
    public Either<TLeftResult, TRightResult> SelectMany<TLeftResult, TRightResult>(
        Func<TLeft, Either<TLeftResult, TRightResult>> leftSelector,
        Func<TRight, Either<TLeftResult, TRightResult>> rightSelector)
        => IsRight ? rightSelector(_right) : leftSelector(_left);

    /// <summary>
    /// Maps a selector over the right side of this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <param name="selector"></param>
    /// <returns></returns>
    public Either<TLeft, TRightResult> SelectManyRight<TRightResult>(
        Func<TRight, Either<TLeft, TRightResult>> selector)
        => IsRight ? selector(_right) : new(_left);
    #endregion

    #region Factory
    /// <summary>
    /// Creates a new <see cref="Either{TLeft, TRight}"/> containing the <typeparamref name="TLeft"/> value passed in
    /// on the left.
    /// </summary>
    /// <param name="left"></param>
    /// <returns></returns>
    public static Either<TLeft, TRight> NewLeft(TLeft left) => new(left);

    /// <summary>
    /// Creates a new <see cref="Either{TLeft, TRight}"/> containing the <typeparamref name="TLeft"/> value passed in
    /// on the left.
    /// </summary>
    /// <param name="left"></param>
    /// <returns></returns>
    public static Either<TLeft, TRight> New(TLeft left) => new(left);

    /// <summary>
    /// Creates a new <see cref="Either{TLeft, TRight}"/> containing the <typeparamref name="TRight"/> value passed in
    /// on the right.
    /// </summary>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Either<TLeft, TRight> New(TRight right) => new(right);

    /// <summary>
    /// Creates a new <see cref="Either{TLeft, TRight}"/> containing the <typeparamref name="TRight"/> value passed in
    /// on the right.
    /// </summary>
    /// <param name="right"></param>
    /// <returns></returns>
    public static Either<TLeft, TRight> NewRight(TRight right) => new(right);
    #endregion

    #region TryGet
    /// <summary>
    /// Tries to get the <typeparamref name="TLeft"/> value wrapped in this instance.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Whether or not this instance wraps an instance of type <typeparamref name="TLeft"/>.</returns>
    public bool TryGetLeft([MaybeNullWhen(false), MaybeDefaultWhen(false)] out TLeft value)
    {
        value = _left;
        return !IsRight;
    }

    /// <summary>
    /// Tries to get the <typeparamref name="TRight"/> value wrapped in this instance.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Whether or not this instance wraps an instance of type <typeparamref name="TRight"/>.</returns>
    public bool TryGetRight([MaybeNullWhen(false), MaybeDefaultWhen(false)] out TRight value)
    {
        value = _right;
        return IsRight;
    }

    /// <summary>
    /// Tries to get the <typeparamref name="TRight"/> value wrapped in this instance, getting the
    /// <typeparamref name="TLeft"/> value wrapped in this instance otherwise.
    /// </summary>
    /// <remarks>
    /// This method is essentially a deconstructor; it can be used to describe the entire structure of the object
    /// with a single call.
    /// </remarks>
    /// <param name="leftValue"></param>
    /// <param name="rightValue"></param>
    /// <returns>Whether or not this instance wraps an instance of type <typeparamref name="TRight"/>.</returns>
    public bool TryGetRight(
        [MaybeNullWhen(true), MaybeDefaultWhen(true)] out TLeft leftValue,
        [MaybeNullWhen(false), MaybeDefaultWhen(false)] out TRight rightValue)
    {
        leftValue = _left;
        rightValue = _right;
        return IsRight;
    }
    #endregion
    #endregion
}
