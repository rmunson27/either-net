using Rem.Core.Attributes;
using Rem.Core.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
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
    public bool IsDefault => !IsRight && TypeInformation<TLeft>.IsDefault(_left);

    /// <summary>
    /// Determines whether or not this instance wraps a <see langword="null"/> value.
    /// </summary>
    public bool WrapsNull => IsRight ? _right is null : _left is null;

    /// <summary>
    /// Gets the value wrapped in this instance typed as an <see cref="object"/>.
    /// </summary>
    [NotDefaultIfTypeParamsNonNullable(nameof(TLeft), nameof(TRight))]
    public object? Value => IsRight ? _right : _left;

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
    #region Containment
    /// <summary>
    /// Determines whether or not the current instance contains the supplied left value.
    /// </summary>
    /// <param name="value">The value to check for.</param>
    /// <param name="comparer">
    /// An <see cref="IEqualityComparer{T}"/> to use to check for equality, or <see langword="null"/> to use the
    /// default comparer for type <typeparamref name="TRight"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the current instance contains the supplied left value, or <see langword="false"/>
    /// if this instance contains a different left value or is right.
    /// </returns>
    public bool ContainsLeft(TLeft value, IEqualityComparer<TLeft>? comparer = null)
        => IsLeft && comparer.DefaultIfNull().Equals(_left, value);

    /// <summary>
    /// Determines whether or not the current instance contains -either- the left value passed in on the left -or- the
    /// right value passed in on the right.
    /// </summary>
    /// <param name="left">The left value to check for.</param>
    /// <param name="right">The right value to check for.</param>
    /// <param name="leftComparer">
    /// An <see cref="IEqualityComparer{T}"/> to use to check for <typeparamref name="TRight"/> equality, or
    /// <see langword="null"/> to use the default comparer for type <typeparamref name="TRight"/>.
    /// </param>
    /// <param name="rightComparer">
    /// An <see cref="IEqualityComparer{T}"/> to use to check for <typeparamref name="TLeft"/> equality, or
    /// <see langword="null"/> to use the default comparer for type <typeparamref name="TLeft"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the current instance contains the supplied left value or the supplied right value,
    /// or <see langword="false"/> if it contains neither.
    /// </returns>
    public bool ContainsEither(
        TLeft left, TRight right,
        IEqualityComparer<TLeft>? leftComparer = null, IEqualityComparer<TRight>? rightComparer = null)
        => IsRight
            ? rightComparer.DefaultIfNull().Equals(_right, right)
            : leftComparer.DefaultIfNull().Equals(_left, left);

    /// <summary>
    /// Determines whether or not the current instance contains the supplied right value.
    /// </summary>
    /// <param name="value">The value to check for.</param>
    /// <param name="comparer">
    /// An <see cref="IEqualityComparer{T}"/> to use to check for equality, or <see langword="null"/> to use the
    /// default comparer for type <typeparamref name="TRight"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the current instance contains the supplied right value, or <see langword="false"/>
    /// if this instance contains a different right value or is left.
    /// </returns>
    public bool ContainsRight(TRight value, IEqualityComparer<TRight>? comparer = null)
        => IsRight && comparer.DefaultIfNull().Equals(_right, value);
    #endregion

    #region Conversions
    #region CombineSides
    /// <summary>
    /// Combines the sides of this instance into a single value using the combiner methods passed in.
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="leftCombiner"></param>
    /// <param name="rightCombiner"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public TResult CombineSides<TResult>(Func<TLeft, TResult> leftCombiner, Func<TRight, TResult> rightCombiner)
        => IsRight ? rightCombiner(_right) : leftCombiner(_left);
    #endregion

    #region Operators
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
    [return: MaybeDefaultIfInstanceDefault]
    public static explicit operator TLeft(Either<TLeft, TRight> either) => either.Left;
    
    /// <summary>
    /// Explicitly converts an <see cref="Either{TLeft, TRight}"/> to an instance of <typeparamref name="TRight"/>.
    /// </summary>
    /// <param name="either"></param>
    /// <exception cref="EitherException">The either was left.</exception>
    public static explicit operator TRight(Either<TLeft, TRight> either) => either.Right;
    #endregion

    #region Polymorphism
    #region Up Casts
    /// <summary>
    /// Converts the left (<typeparamref name="TLeftChild"/>) type of the <see cref="Either{TLeft, TRight}"/> passed in
    /// to <typeparamref name="TLeft"/>.
    /// </summary>
    /// <typeparam name="TChild"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <param name="either"></param>
    /// <returns></returns>
    [return: NotDefaultIfNotDefault("either")]
    public static Either<TLeft, TRight> FromLeftChild<TLeftChild>(in Either<TLeftChild, TRight> either)
        where TLeftChild : TLeft
        => new(either._left, either._right, either.IsRight);

    /// <summary>
    /// Converts the left (<typeparamref name="TLeftChild"/>) and right (<typeparamref name="TRightChild"/>) types of
    /// the <see cref="Either{TLeft, TRight}"/> passed in to <typeparamref name="TLeft"/> and
    /// <typeparamref name="TRight"/>, respectively.
    /// </summary>
    /// <typeparam name="TLeftChild"></typeparam>
    /// <typeparam name="TRightChild"></typeparam>
    /// <param name="either"></param>
    /// <returns></returns>
    [return: NotDefaultIfNotDefault("either")]
    public static Either<TLeft, TRight> FromChild<TLeftChild, TRightChild>(in Either<TLeftChild, TRightChild> either)
        where TLeftChild : TLeft
        where TRightChild : TRight
        => new(either._left, either._right, either.IsRight);

    /// <summary>
    /// Converts the right (<typeparamref name="TRightChild"/>) type of the <see cref="Either{TLeft, TRight}"/> passed in
    /// to <typeparamref name="TRight"/>.
    /// </summary>
    /// <typeparam name="TChild"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <param name="either"></param>
    /// <returns></returns>
    [return: NotDefaultIfNotDefault("either")]
    public static Either<TLeft, TRight> FromRightChild<TRightChild>(in Either<TLeft, TRightChild> either)
        where TRightChild : TRight
        => new(either._left, either._right, either.IsRight);
    #endregion

    #region Down Casts
    /// <summary>
    /// Casts the left type of the current instance to <typeparamref name="TLeftChild"/>.
    /// </summary>
    /// <typeparam name="TLeftChild"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidCastException">The cast was invalid.</exception>
    [return: MaybeDefaultIfInstanceDefault]
    public Either<TLeftChild, TRight> CastToLeftChild<TLeftChild>()
        where TLeftChild : TLeft
        => IsRight ? new(_right) : new((TLeftChild)_left!);

    /// <summary>
    /// Casts the left type of the current instance to <typeparamref name="TLeftChild"/> and the right type of the
    /// current instance to <typeparamref name="TRightChild"/>.
    /// </summary>
    /// <typeparam name="TLeftChild"></typeparam>
    /// <typeparam name="TRightChild"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidCastException">The cast was invalid.</exception>
    [return: MaybeDefaultIfInstanceDefault]
    public Either<TLeftChild, TRightChild> CastToChild<TLeftChild, TRightChild>()
        where TLeftChild : TLeft
        where TRightChild : TRight
        => IsRight ? new((TRightChild)_right!) : new((TLeftChild)_left!);

    /// <summary>
    /// Casts the right type of the current instance to <typeparamref name="TRightChild"/>.
    /// </summary>
    /// <typeparam name="TRightChild"></typeparam>
    /// <returns></returns>
    /// <exception cref="InvalidCastException">The cast was invalid.</exception>
    [return: MaybeDefaultIfInstanceDefault]
    public Either<TLeft, TRightChild> CastToRightChild<TRightChild>()
        where TRightChild : TRight
        => IsRight ? new((TRightChild)_right!) : new(_left);
    #endregion
    #endregion
    #endregion

    #region Equality
    /// <summary>
    /// Determines if the current instance is equal to another object of the same type.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(Either<TLeft, TRight> other) => Equals(other, null, null);

    /// <summary>
    /// Determines if the current instance is equal to another object of the same type, using the specified equality
    /// comparers to compare equality.
    /// </summary>
    /// <param name="other">The other object to compare with.</param>
    /// <param name="leftComparer">
    /// An <see cref="IEqualityComparer{T}"/> to use to compare equality of <typeparamref name="TLeft"/> instances, or
    /// <see langword="null"/> to use the default comparer for type <typeparamref name="TLeft"/>.
    /// </param>
    /// <param name="rightComparer">
    /// An <see cref="IEqualityComparer{T}"/> to use to compare equality of <typeparamref name="TRight"/> instances, or
    /// <see langword="null"/> to use the default comparer for type <typeparamref name="TRight"/>.
    /// </param>
    /// <returns></returns>
    public bool Equals(
        in Either<TLeft, TRight> other,
        IEqualityComparer<TLeft>? leftComparer, IEqualityComparer<TRight>? rightComparer)
        => IsRight
            ? other.IsRight && rightComparer.DefaultIfNull().Equals(_right, other._right)
            : other.IsLeft && leftComparer.DefaultIfNull().Equals(_left, other._left);

    /// <summary>
    /// Gets a hash code for the current instance.
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
        => IsRight ? HashCode.Combine(true, _right) : HashCode.Combine(false, _left);

    /// <summary>
    /// Gets a hash code for the current instance, 
    /// </summary>
    /// <param name="leftComparer">
    /// An <see cref="IEqualityComparer{T}"/> to use to get hash codes for <typeparamref name="TLeft"/> instances, or
    /// <see langword="null"/> to use the default comparer for type <typeparamref name="TLeft"/>.
    /// </param>
    /// <param name="rightComparer">
    /// An <see cref="IEqualityComparer{T}"/> to use to get hash codes for <typeparamref name="TRight"/> instances, or
    /// <see langword="null"/> to use the default comparer for type <typeparamref name="TRight"/>.
    /// </param>
    /// <returns></returns>
    public int GetHashCode(IEqualityComparer<TLeft>? leftComparer, IEqualityComparer<TRight>? rightComparer)
        => IsRight
            ? HashCode.Combine(true, rightComparer.DefaultIfNull().GetHashCode(_right))
            : HashCode.Combine(false, leftComparer.DefaultIfNull().GetHashCode(_left));
    #endregion

    #region Factories
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

    #region Getters
    #region GetType
    /// <summary>
    /// Gets the type of the value wrapped in this instance, or <see langword="null"/> if the instance wraps
    /// <see langword="null"/>.
    /// </summary>
    /// <returns></returns>
    [return: NotDefaultIfTypeParamsNonNullable(nameof(TLeft), nameof(TRight))]
    public Type? GetWrappedType() => IsRight ? _right?.GetType() : _left?.GetType();
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

    #region Linq-Like
    #region Select
    #region Synchronous
    /// <summary>
    /// Maps a selector over the left side of this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <param name="selector"></param>
    /// <returns></returns>
    [InstanceNotDefault]
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
    [InstanceNotDefault]
    public Either<TLeftResult, TRightResult> SelectEither<TLeftResult, TRightResult>(
        Func<TLeft, TLeftResult> leftSelector, Func<TRight, TRightResult> rightSelector)
        => IsRight ? new(rightSelector(_right)) : new(leftSelector(_left));

    /// <summary>
    /// Maps a selector over the right side of this instance.
    /// </summary>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="selector"></param>
    /// <returns></returns>
    [return: MaybeDefaultIfInstanceDefault]
    public Either<TLeft, TRightResult> SelectRight<TRightResult>(Func<TRight, TRightResult> selector)
        => IsRight ? new(selector(_right)) : new(_left);
    #endregion

    #region Asyncronous
    #region Left
    /// <summary>
    /// Asynchronously maps a selector over the left side of this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <param name="selectorAsync"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<Either<TLeftResult, TRight>> SelectLeftAsync<TLeftResult>(
        Func<TLeft, Task<TLeftResult>> selectorAsync)
        => IsRight ? new(_right) : new(await selectorAsync(_left));

    /// <summary>
    /// Asynchronously maps a selector over the left side of this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <param name="selectorAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<Either<TLeftResult, TRight>> SelectLeftAsync<TLeftResult>(
        Func<TLeft, CancellationToken, Task<TLeftResult>> selectorAsync, CancellationToken cancellationToken = default)
        => IsRight ? new(_right) : new(await selectorAsync(_left, cancellationToken));
    #endregion

    #region Either Side
    #region One Async
    /// <summary>
    /// Asynchronously maps a side-dependent selector over this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="leftSelectorAsync"></param>
    /// <param name="rightSelector"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<Either<TLeftResult, TRightResult>> SelectEitherAsync<TLeftResult, TRightResult>(
        Func<TLeft, Task<TLeftResult>> leftSelectorAsync, Func<TRight, TRightResult> rightSelector)
        => IsRight ? new(rightSelector(_right)) : new(await leftSelectorAsync(_left));

    /// <summary>
    /// Asynchronously maps a side-dependent selector over this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="leftSelectorAsync"></param>
    /// <param name="rightSelector"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<Either<TLeftResult, TRightResult>> SelectEitherAsync<TLeftResult, TRightResult>(
        Func<TLeft, CancellationToken, Task<TLeftResult>> leftSelectorAsync, Func<TRight, TRightResult> rightSelector,
        CancellationToken cancellationToken = default)
        => IsRight ? new(rightSelector(_right)) : new(await leftSelectorAsync(_left, cancellationToken));

    /// <summary>
    /// Asynchronously maps a side-dependent selector over this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="leftSelector"></param>
    /// <param name="rightSelectorAsync"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<Either<TLeftResult, TRightResult>> SelectEitherAsync<TLeftResult, TRightResult>(
        Func<TLeft, TLeftResult> leftSelector, Func<TRight, Task<TRightResult>> rightSelectorAsync)
        => IsRight ? new(await rightSelectorAsync(_right)) : new(leftSelector(_left));

    /// <summary>
    /// Asynchronously maps a side-dependent selector over this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="leftSelector"></param>
    /// <param name="rightSelectorAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<Either<TLeftResult, TRightResult>> SelectEitherAsync<TLeftResult, TRightResult>(
        Func<TLeft, TLeftResult> leftSelector, Func<TRight, CancellationToken, Task<TRightResult>> rightSelectorAsync,
        CancellationToken cancellationToken = default)
        => IsRight ? new(await rightSelectorAsync(_right, cancellationToken)) : new(leftSelector(_left));
    #endregion

    #region Both Async
    /// <summary>
    /// Asynchronously maps a side-dependent selector over this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="leftSelectorAsync"></param>
    /// <param name="rightSelectorAsync"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<Either<TLeftResult, TRightResult>> SelectEitherAsync<TLeftResult, TRightResult>(
        Func<TLeft, Task<TLeftResult>> leftSelectorAsync, Func<TRight, Task<TRightResult>> rightSelectorAsync)
        => IsRight ? new(await rightSelectorAsync(_right)) : new(await leftSelectorAsync(_left));

    /// <summary>
    /// Asynchronously maps a side-dependent selector over this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="leftSelectorAsync"></param>
    /// <param name="rightSelectorAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<Either<TLeftResult, TRightResult>> SelectEitherAsync<TLeftResult, TRightResult>(
        Func<TLeft, CancellationToken, Task<TLeftResult>> leftSelectorAsync,
        Func<TRight, Task<TRightResult>> rightSelectorAsync,
        CancellationToken cancellationToken = default)
        => IsRight ? new(await rightSelectorAsync(_right)) : new(await leftSelectorAsync(_left, cancellationToken));

    /// <summary>
    /// Asynchronously maps a side-dependent selector over this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="leftSelectorAsync"></param>
    /// <param name="rightSelectorAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<Either<TLeftResult, TRightResult>> SelectEitherAsync<TLeftResult, TRightResult>(
        Func<TLeft, Task<TLeftResult>> leftSelectorAsync,
        Func<TRight, CancellationToken, Task<TRightResult>> rightSelectorAsync,
        CancellationToken cancellationToken = default)
        => IsRight ? new(await rightSelectorAsync(_right, cancellationToken)) : new(await leftSelectorAsync(_left));

    /// <summary>
    /// Asynchronously maps a side-dependent selector over this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="leftSelectorAsync"></param>
    /// <param name="rightSelectorAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<Either<TLeftResult, TRightResult>> SelectEitherAsync<TLeftResult, TRightResult>(
        Func<TLeft, CancellationToken, Task<TLeftResult>> leftSelectorAsync,
        Func<TRight, CancellationToken, Task<TRightResult>> rightSelectorAsync,
        CancellationToken cancellationToken = default)
        => IsRight
            ? new(await rightSelectorAsync(_right, cancellationToken))
            : new(await leftSelectorAsync(_left, cancellationToken));
    #endregion
    #endregion

    #region Right
    /// <summary>
    /// Asynchronously maps a selector over the right side of this instance.
    /// </summary>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="selector"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<Either<TLeft, TRightResult>> SelectRightAsync<TRightResult>(
        Func<TRight, Task<TRightResult>> selector)
        => IsRight ? new(await selector(_right)) : new(_left);

    /// <summary>
    /// Asynchronously maps a selector over the right side of this instance.
    /// </summary>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="selector"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<Either<TLeft, TRightResult>> SelectRightAsync<TRightResult>(
        Func<TRight, CancellationToken, Task<TRightResult>> selector, CancellationToken cancellationToken = default)
        => IsRight ? new(await selector(_right, cancellationToken)) : new(_left);
    #endregion
    #endregion
    #endregion

    #region SelectMany
    #region Synchronous
    /// <summary>
    /// Maps a selector over the left side of this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <param name="selector"></param>
    /// <returns></returns>
    [InstanceNotDefault]
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
    [InstanceNotDefault]
    public Either<TLeftResult, TRightResult> SelectManyEither<TLeftResult, TRightResult>(
        Func<TLeft, Either<TLeftResult, TRightResult>> leftSelector,
        Func<TRight, Either<TLeftResult, TRightResult>> rightSelector)
        => IsRight ? rightSelector(_right) : leftSelector(_left);

    /// <summary>
    /// Maps a selector over the right side of this instance.
    /// </summary>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="selector"></param>
    /// <returns></returns>
    [return: MaybeDefaultIfInstanceDefault]
    public Either<TLeft, TRightResult> SelectManyRight<TRightResult>(
        Func<TRight, Either<TLeft, TRightResult>> selector)
        => IsRight ? selector(_right) : new(_left);
    #endregion

    #region Asynchronous
    #region Left
    /// <summary>
    /// Maps a selector over the left side of this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <param name="selectorAsync"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public Task<Either<TLeftResult, TRight>> SelectManyLeftAsync<TLeftResult>(
        Func<TLeft, Task<Either<TLeftResult, TRight>>> selectorAsync)
        => IsRight ? Task.FromResult<Either<TLeftResult, TRight>>(new(_right)) : selectorAsync(_left);

    /// <summary>
    /// Maps a selector over the left side of this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <param name="selectorAsync"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public Task<Either<TLeftResult, TRight>> SelectManyLeftAsync<TLeftResult>(
        Func<TLeft, CancellationToken, Task<Either<TLeftResult, TRight>>> selectorAsync,
        CancellationToken cancellationToken = default)
        => IsRight ? Task.FromResult<Either<TLeftResult, TRight>>(new(_right)) : selectorAsync(_left, cancellationToken);
    #endregion

    #region Either Side
    #region One Async
    /// <summary>
    /// Asynchronously maps a side-specific selector over this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="leftSelector"></param>
    /// <param name="rightSelectorAsync"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public Task<Either<TLeftResult, TRightResult>> SelectManyEitherAsync<TLeftResult, TRightResult>(
        Func<TLeft, Either<TLeftResult, TRightResult>> leftSelector,
        Func<TRight, Task<Either<TLeftResult, TRightResult>>> rightSelectorAsync)
        => IsRight ? rightSelectorAsync(_right) : Task.FromResult(leftSelector(_left));

    /// <summary>
    /// Asynchronously maps a side-specific selector over this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="leftSelector"></param>
    /// <param name="rightSelectorAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public Task<Either<TLeftResult, TRightResult>> SelectManyEitherAsync<TLeftResult, TRightResult>(
        Func<TLeft, Either<TLeftResult, TRightResult>> leftSelector,
        Func<TRight, CancellationToken, Task<Either<TLeftResult, TRightResult>>> rightSelectorAsync,
        CancellationToken cancellationToken = default)
        => IsRight ? rightSelectorAsync(_right, cancellationToken) : Task.FromResult(leftSelector(_left));

    /// <summary>
    /// Asynchronously maps a side-specific selector over this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="leftSelectorAsync"></param>
    /// <param name="rightSelector"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public Task<Either<TLeftResult, TRightResult>> SelectManyEitherAsync<TLeftResult, TRightResult>(
        Func<TLeft, Task<Either<TLeftResult, TRightResult>>> leftSelectorAsync,
        Func<TRight, Either<TLeftResult, TRightResult>> rightSelector)
        => IsRight ? Task.FromResult(rightSelector(_right)) : leftSelectorAsync(_left);

    /// <summary>
    /// Asynchronously maps a side-specific selector over this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="leftSelectorAsync"></param>
    /// <param name="rightSelector"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public Task<Either<TLeftResult, TRightResult>> SelectManyEitherAsync<TLeftResult, TRightResult>(
        Func<TLeft, CancellationToken, Task<Either<TLeftResult, TRightResult>>> leftSelectorAsync,
        Func<TRight, Either<TLeftResult, TRightResult>> rightSelector,
        CancellationToken cancellationToken = default)
        => IsRight ? Task.FromResult(rightSelector(_right)) : leftSelectorAsync(_left, cancellationToken);
    #endregion

    #region Both Async
    /// <summary>
    /// Asynchronously maps a side-specific selector over this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="leftSelectorAsync"></param>
    /// <param name="rightSelectorAsync"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public Task<Either<TLeftResult, TRightResult>> SelectManyEitherAsync<TLeftResult, TRightResult>(
        Func<TLeft, Task<Either<TLeftResult, TRightResult>>> leftSelectorAsync,
        Func<TRight, Task<Either<TLeftResult, TRightResult>>> rightSelectorAsync)
        => IsRight ? rightSelectorAsync(_right) : leftSelectorAsync(_left);

    /// <summary>
    /// Asynchronously maps a side-specific selector over this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="leftSelectorAsync"></param>
    /// <param name="rightSelectorAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public Task<Either<TLeftResult, TRightResult>> SelectManyEitherAsync<TLeftResult, TRightResult>(
        Func<TLeft, CancellationToken, Task<Either<TLeftResult, TRightResult>>> leftSelectorAsync,
        Func<TRight, Task<Either<TLeftResult, TRightResult>>> rightSelectorAsync,
        CancellationToken cancellationToken = default)
        => IsRight ? rightSelectorAsync(_right) : leftSelectorAsync(_left, cancellationToken);

    /// <summary>
    /// Asynchronously maps a side-specific selector over this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="leftSelectorAsync"></param>
    /// <param name="rightSelectorAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public Task<Either<TLeftResult, TRightResult>> SelectManyEitherAsync<TLeftResult, TRightResult>(
        Func<TLeft, Task<Either<TLeftResult, TRightResult>>> leftSelectorAsync,
        Func<TRight, CancellationToken, Task<Either<TLeftResult, TRightResult>>> rightSelectorAsync,
        CancellationToken cancellationToken = default)
        => IsRight ? rightSelectorAsync(_right, cancellationToken) : leftSelectorAsync(_left);

    /// <summary>
    /// Asynchronously maps a side-specific selector over this instance.
    /// </summary>
    /// <typeparam name="TLeftResult"></typeparam>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="leftSelectorAsync"></param>
    /// <param name="rightSelectorAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public Task<Either<TLeftResult, TRightResult>> SelectManyEitherAsync<TLeftResult, TRightResult>(
        Func<TLeft, CancellationToken, Task<Either<TLeftResult, TRightResult>>> leftSelectorAsync,
        Func<TRight, CancellationToken, Task<Either<TLeftResult, TRightResult>>> rightSelectorAsync,
        CancellationToken cancellationToken = default)
        => IsRight ? rightSelectorAsync(_right, cancellationToken) : leftSelectorAsync(_left, cancellationToken);
    #endregion
    #endregion

    #region Right
    /// <summary>
    /// Maps a selector over the right side of this instance.
    /// </summary>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="selectorAsync"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public Task<Either<TLeft, TRightResult>> SelectManyRightAsync<TRightResult>(
        Func<TRight, Task<Either<TLeft, TRightResult>>> selectorAsync)
        => IsRight ? selectorAsync(_right) : Task.FromResult(new Either<TLeft, TRightResult>(_left));

    /// <summary>
    /// Maps a selector over the right side of this instance.
    /// </summary>
    /// <typeparam name="TRightResult"></typeparam>
    /// <param name="selectorAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public Task<Either<TLeft, TRightResult>> SelectManyRightAsync<TRightResult>(
        Func<TRight, CancellationToken, Task<Either<TLeft, TRightResult>>> selectorAsync,
        CancellationToken cancellationToken = default)
        => IsRight ? selectorAsync(_right, cancellationToken) : Task.FromResult(new Either<TLeft, TRightResult>(_left));
    #endregion
    #endregion
    #endregion

    #region Enumerator / Enumerable
    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> wrapping the right value wrapped in this instance, or an empty
    /// <see cref="IEnumerable{T}"/> if this instance is left.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<TRight> EnumerateRight() => (IEnumerable<TRight>)GetRightEnumerator();

    /// <summary>
    /// Gets an <see cref="IEnumerator{T}"/> that enumerates the right value wrapped in this instance, or an empty
    /// <see cref="IEnumerator{T}"/> if this instance is left.
    /// </summary>
    /// <returns></returns>
    public IEnumerator<TRight> GetRightEnumerator()
    {
        if (IsRight) yield return _right;
    }

    /// <summary>
    /// Gets an <see cref="IEnumerable"/> wrapping the value wrapped in this instance.
    /// </summary>
    /// <returns></returns>
    [InstanceNotDefault]
    public IEnumerable EnumerateEither() => (IEnumerable)GetEitherEnumerator();

    /// <summary>
    /// Gets an <see cref="IEnumerator"/> enumerating the value wrapped in this instance.
    /// </summary>
    /// <returns></returns>
    [InstanceNotDefault]
    public IEnumerator GetEitherEnumerator()
    {
        yield return IsRight ? _right : _left;
    }

    /// <summary>
    /// Gets an <see cref="IEnumerable{T}"/> wrapping the left value wrapped in this instance, or an empty
    /// <see cref="IEnumerable{T}"/> if this instance is right.
    /// </summary>
    /// <returns></returns>
    [InstanceNotDefault]
    public IEnumerable<TLeft> EnumerateLeft() => (IEnumerable<TLeft>)GetLeftEnumerator();

    /// <summary>
    /// Gets an <see cref="IEnumerator{T}"/> that enumerates the left value wrapped in this instance, or an empty
    /// <see cref="IEnumerator{T}"/> if this instance is right.
    /// </summary>
    /// <returns></returns>
    [InstanceNotDefault]
    public IEnumerator<TLeft> GetLeftEnumerator()
    {
        if (IsLeft) yield return _left;
    }
    #endregion

    #region Where
    #region IEnumerable (No Default Value)
    #region Left
    /// <summary>
    /// Filters the left side of this instance by a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public IEnumerable<TLeft> WhereLeft(Func<TLeft, bool> predicate)
    {
        if (IsLeft && predicate(_left)) yield return _left;
    }

    /// <summary>
    /// Asynchronously filters the left side of this instance by a predicate.
    /// </summary>
    /// <param name="predicateAsync"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<IEnumerable<TLeft>> WhereLeftAsync(Func<TLeft, Task<bool>> predicateAsync)
        => IsLeft && await predicateAsync(_left) ? ImmutableList.Create(_left) : ImmutableList<TLeft>.Empty;

    /// <summary>
    /// Asynchronously filters the left side of this instance by a predicate.
    /// </summary>
    /// <param name="predicateAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<IEnumerable<TLeft>> WhereLeftAsync(
        Func<TLeft, CancellationToken, Task<bool>> predicateAsync, CancellationToken cancellationToken = default)
        => IsLeft && await predicateAsync(_left, cancellationToken)
            ? ImmutableList.Create(_left)
            : ImmutableList<TLeft>.Empty;
    #endregion

    #region Either Side
    #region Synchronous
    /// <summary>
    /// Filters this instance by a predicate.
    /// </summary>
    /// <param name="leftPredicate"></param>
    /// <param name="rightPredicate"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public IEnumerable WhereEither(Func<TLeft, bool> leftPredicate, Func<TRight, bool> rightPredicate)
    {
        if (IsRight)
        {
            if (rightPredicate(_right)) yield return _right;
        }
        else
        {
            if (leftPredicate(_left)) yield return _left;
        }

        yield break;
    }
    #endregion

    #region Left Async Only
    /// <summary>
    /// Asynchronously filters this instance by a predicate.
    /// </summary>
    /// <param name="leftPredicateAsync"></param>
    /// <param name="rightPredicate"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<IEnumerable> WhereEitherAsync(
        Func<TLeft, Task<bool>> leftPredicateAsync, Func<TRight, bool> rightPredicate)
    {
        if (IsRight)
        {
            if (rightPredicate(_right)) return ImmutableList.Create(_right);
        }
        else
        {
            if (await leftPredicateAsync(_left)) return ImmutableList.Create(_left);
        }

        return ImmutableList<object>.Empty;
    }

    /// <summary>
    /// Asynchronously filters this instance by a predicate.
    /// </summary>
    /// <param name="leftPredicateAsync"></param>
    /// <param name="rightPredicate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<IEnumerable> WhereEitherAsync(
        Func<TLeft, CancellationToken, Task<bool>> leftPredicateAsync, Func<TRight, bool> rightPredicate,
        CancellationToken cancellationToken = default)
    {
        if (IsRight)
        {
            if (rightPredicate(_right)) return ImmutableList.Create(_right);
        }
        else
        {
            if (await leftPredicateAsync(_left, cancellationToken)) return ImmutableList.Create(_left);
        }

        return ImmutableList<object>.Empty;
    }
    #endregion

    #region Right Async Only
    /// <summary>
    /// Asynchronously filters this instance by a predicate.
    /// </summary>
    /// <param name="leftPredicate"></param>
    /// <param name="rightPredicateAsync"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<IEnumerable> WhereEitherAsync(
        Func<TLeft, bool> leftPredicate, Func<TRight, Task<bool>> rightPredicateAsync)
    {
        if (IsRight)
        {
            if (await rightPredicateAsync(_right)) return ImmutableList.Create(_right);
        }
        else
        {
            if (leftPredicate(_left)) return ImmutableList.Create(_left);
        }

        return ImmutableList<object>.Empty;
    }

    /// <summary>
    /// Asynchronously filters this instance by a predicate.
    /// </summary>
    /// <param name="leftPredicate"></param>
    /// <param name="rightPredicateAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<IEnumerable> WhereEitherAsync(
        Func<TLeft, bool> leftPredicate, Func<TRight, CancellationToken, Task<bool>> rightPredicateAsync,
        CancellationToken cancellationToken = default)
    {
        if (IsRight)
        {
            if (await rightPredicateAsync(_right, cancellationToken)) return ImmutableList.Create(_right);
        }
        else
        {
            if (leftPredicate(_left)) return ImmutableList.Create(_left);
        }

        return ImmutableList<object>.Empty;
    }
    #endregion

    #region Both Async
    /// <summary>
    /// Asynchronously filters this instance by a predicate.
    /// </summary>
    /// <param name="leftPredicateAsync"></param>
    /// <param name="rightPredicateAsync"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<IEnumerable> WhereEitherAsync(
        Func<TLeft, Task<bool>> leftPredicateAsync, Func<TRight, Task<bool>> rightPredicateAsync)
    {
        if (IsRight)
        {
            if (await rightPredicateAsync(_right)) return ImmutableList.Create(_right);
        }
        else
        {
            if (await leftPredicateAsync(_left)) return ImmutableList.Create(_left);
        }

        return ImmutableList<object>.Empty;
    }

    /// <summary>
    /// Asynchronously filters this instance by a predicate.
    /// </summary>
    /// <param name="leftPredicateAsync"></param>
    /// <param name="rightPredicateAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<IEnumerable> WhereEitherAsync(
        Func<TLeft, CancellationToken, Task<bool>> leftPredicateAsync, Func<TRight, Task<bool>> rightPredicateAsync,
        CancellationToken cancellationToken = default)
    {
        if (IsRight)
        {
            if (await rightPredicateAsync(_right)) return ImmutableList.Create(_right);
        }
        else
        {
            if (await leftPredicateAsync(_left, cancellationToken)) return ImmutableList.Create(_left);
        }

        return ImmutableList<object>.Empty;
    }

    /// <summary>
    /// Asynchronously filters this instance by a predicate.
    /// </summary>
    /// <param name="leftPredicateAsync"></param>
    /// <param name="rightPredicateAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<IEnumerable> WhereEitherAsync(
        Func<TLeft, Task<bool>> leftPredicateAsync, Func<TRight, CancellationToken, Task<bool>> rightPredicateAsync,
        CancellationToken cancellationToken = default)
    {
        if (IsRight)
        {
            if (await rightPredicateAsync(_right, cancellationToken)) return ImmutableList.Create(_right);
        }
        else
        {
            if (await leftPredicateAsync(_left)) return ImmutableList.Create(_left);
        }

        return ImmutableList<object>.Empty;
    }

    /// <summary>
    /// Asynchronously filters this instance by a predicate.
    /// </summary>
    /// <param name="leftPredicateAsync"></param>
    /// <param name="rightPredicateAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<IEnumerable> WhereEitherAsync(
        Func<TLeft, CancellationToken, Task<bool>> leftPredicateAsync,
        Func<TRight, CancellationToken, Task<bool>> rightPredicateAsync,
        CancellationToken cancellationToken = default)
    {
        if (IsRight)
        {
            if (await rightPredicateAsync(_right, cancellationToken)) return ImmutableList.Create(_right);
        }
        else
        {
            if (await leftPredicateAsync(_left, cancellationToken)) return ImmutableList.Create(_left);
        }

        return ImmutableList<object>.Empty;
    }
    #endregion
    #endregion

    #region Right
    /// <summary>
    /// Filters the right side of this instance by a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IEnumerable<TRight> WhereRight(Func<TRight, bool> predicate)
    {
        if (IsRight && predicate(_right)) yield return _right;
    }

    /// <summary>
    /// Asynchronously filters the right side of this instance by a predicate.
    /// </summary>
    /// <param name="predicateAsync"></param>
    /// <returns></returns>
    public async Task<IEnumerable<TRight>> WhereRightAsync(Func<TRight, Task<bool>> predicateAsync)
        => IsRight && await predicateAsync(_right) ? ImmutableList.Create(_right) : ImmutableList<TRight>.Empty;

    /// <summary>
    /// Asynchronously filters the right side of this instance by a predicate.
    /// </summary>
    /// <param name="predicateAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<IEnumerable<TRight>> WhereRightAsync(
        Func<TRight, CancellationToken, Task<bool>> predicateAsync, CancellationToken cancellationToken = default)
        => IsRight && await predicateAsync(_right, cancellationToken)
            ? ImmutableList.Create(_right)
            : ImmutableList<TRight>.Empty;
    #endregion
    #endregion

    #region Either (Default Value)
    #region Eager
    #region Synchronous
    /// <summary>
    /// Filters the left side of this instance by a predicate, returning a new <see cref="Either{TLeft, TRight}"/>
    /// with the specified default value on the right if the predicate fails.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public Either<TLeft, TRight> WhereLeft(Func<TLeft, bool> predicate, TRight defaultValue)
        => IsRight ? this : (predicate(_left) ? this : new(defaultValue));

    /// <summary>
    /// Filters the right side of this instance by a predicate, returning a new <see cref="Either{TLeft, TRight}"/>
    /// with the specified default value on the left if the predicate fails.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public Either<TLeft, TRight> WhereRight(Func<TRight, bool> predicate, TLeft defaultValue)
        => IsRight ? (predicate(_right) ? this : new(defaultValue)) : this;
    #endregion

    #region Asynchronous
    /// <summary>
    /// Asynchronously filters the left side of this instance by a predicate, returning a new
    /// <see cref="Either{TLeft, TRight}"/> with the specified default value on the right if the predicate fails.
    /// </summary>
    /// <param name="predicateAsync"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<Either<TLeft, TRight>> WhereLeftAsync(
        Func<TLeft, Task<bool>> predicateAsync, TRight defaultValue)
        => IsRight ? this : (await predicateAsync(_left) ? this : new(defaultValue));

    /// <summary>
    /// Asynchronously filters the left side of this instance by a predicate, returning a new
    /// <see cref="Either{TLeft, TRight}"/> with the specified default value on the right if the predicate fails.
    /// </summary>
    /// <param name="predicateAsync"></param>
    /// <param name="defaultValue"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<Either<TLeft, TRight>> WhereLeftAsync(
        Func<TLeft, CancellationToken, Task<bool>> predicateAsync, TRight defaultValue,
        CancellationToken cancellationToken = default)
        => IsRight ? this : (await predicateAsync(_left, cancellationToken) ? this : new(defaultValue));

    /// <summary>
    /// Filters the right side of this instance by a predicate, returning a new <see cref="Either{TLeft, TRight}"/>
    /// with the specified default value on the left if the predicate fails.
    /// </summary>
    /// <param name="predicateAsync"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public async Task<Either<TLeft, TRight>> WhereRightAsync(
        Func<TRight, Task<bool>> predicateAsync, TLeft defaultValue)
        => IsRight ? (await predicateAsync(_right) ? this : new(defaultValue)) : this;

    /// <summary>
    /// Filters the right side of this instance by a predicate, returning a new <see cref="Either{TLeft, TRight}"/>
    /// with the specified default value on the left if the predicate fails.
    /// </summary>
    /// <param name="predicateAsync"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public async Task<Either<TLeft, TRight>> WhereRightAsync(
        Func<TRight, CancellationToken, Task<bool>> predicateAsync, TLeft defaultValue,
        CancellationToken cancellationToken = default)
        => IsRight ? (await predicateAsync(_right, cancellationToken) ? this : new(defaultValue)) : this;
    #endregion
    #endregion

    #region Lazy
    #region Synchronous
    /// <summary>
    /// Filters the left side of this instance by a predicate, returning a new <see cref="Either{TLeft, TRight}"/>
    /// with the default value produced by the specified factory method on the right if the predicate fails.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="defaultFactory"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public Either<TLeft, TRight> WhereLeftLazy(Func<TLeft, bool> predicate, Func<TRight> defaultFactory)
        => IsRight ? this : (predicate(_left) ? this : new(defaultFactory()));

    /// <summary>
    /// Filters the right side of this instance by a predicate, returning a new <see cref="Either{TLeft, TRight}"/>
    /// with the default value produced by the specified factory method on the left if the predicate fails.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="defaultFactory"></param>
    /// <returns></returns>
    public Either<TLeft, TRight> WhereRightLazy(Func<TRight, bool> predicate, Func<TLeft> defaultFactory)
        => IsRight ? (predicate(_right) ? this : new(defaultFactory())) : this;
    #endregion

    #region Asynchronous
    #region Only Predicate Async
    /// <summary>
    /// Asynchronously filters the left side of this instance by a predicate, returning a new
    /// <see cref="Either{TLeft, TRight}"/> with the default value produced by the specified factory method on the
    /// right if the predicate fails.
    /// </summary>
    /// <param name="predicateAsync"></param>
    /// <param name="defaultFactory"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<Either<TLeft, TRight>> WhereLeftLazyAsync(
        Func<TLeft, Task<bool>> predicateAsync, Func<TRight> defaultFactory)
        => IsRight ?this : (await predicateAsync(_left) ? this : new(defaultFactory()));

    /// <summary>
    /// Asynchronously filters the left side of this instance by a predicate, returning a new
    /// <see cref="Either{TLeft, TRight}"/> with the default value produced by the specified factory method on the
    /// right if the predicate fails.
    /// </summary>
    /// <param name="predicateAsync"></param>
    /// <param name="defaultFactory"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<Either<TLeft, TRight>> WhereLeftLazyAsync(
        Func<TLeft, CancellationToken, Task<bool>> predicateAsync, Func<TRight> defaultFactory,
        CancellationToken cancellationToken = default)
        => IsRight ? this : (await predicateAsync(_left, cancellationToken) ? this : new(defaultFactory()));

    /// <summary>
    /// Asynchronously filters the right side of this instance by a predicate, returning a new
    /// <see cref="Either{TLeft, TRight}"/> with the default value produced by the specified factory method on the
    /// left if the predicate fails.
    /// </summary>
    /// <param name="predicateAsync"></param>
    /// <param name="defaultFactory"></param>
    /// <returns></returns>
    public async Task<Either<TLeft, TRight>> WhereRightLazyAsync(
        Func<TRight, Task<bool>> predicateAsync, Func<TLeft> defaultFactory)
        => IsRight ? (await predicateAsync(_right) ? this : defaultFactory()) : this;

    /// <summary>
    /// Asynchronously filters the right side of this instance by a predicate, returning a new
    /// <see cref="Either{TLeft, TRight}"/> with the default value produced by the specified factory method on the
    /// left if the predicate fails.
    /// </summary>
    /// <param name="predicateAsync"></param>
    /// <param name="defaultFactory"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Either<TLeft, TRight>> WhereRightLazyAsync(
        Func<TRight, CancellationToken, Task<bool>> predicateAsync, Func<TLeft> defaultFactory,
        CancellationToken cancellationToken = default)
        => IsRight ? (await predicateAsync(_right, cancellationToken) ? this : defaultFactory()) : this;
    #endregion

    #region Only Factory Async
    /// <summary>
    /// Asynchronously filters the left side of this instance by a predicate, returning a new
    /// <see cref="Either{TLeft, TRight}"/> with the default value produced by the specified factory method on the
    /// right if the predicate fails.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="defaultFactoryAsync"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<Either<TLeft, TRight>> WhereLeftLazyAsync(
        Func<TLeft, bool> predicate, Func<Task<TRight>> defaultFactoryAsync)
        => IsRight ? this : (predicate(_left) ? this : new(await defaultFactoryAsync()));

    /// <summary>
    /// Asynchronously filters the left side of this instance by a predicate, returning a new
    /// <see cref="Either{TLeft, TRight}"/> with the default value produced by the specified factory method on the
    /// right if the predicate fails.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="defaultFactoryAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<Either<TLeft, TRight>> WhereLeftLazyAsync(
        Func<TLeft, bool> predicate, Func<CancellationToken, Task<TRight>> defaultFactoryAsync,
        CancellationToken cancellationToken = default)
        => IsRight ? this : (predicate(_left) ? this : new(await defaultFactoryAsync(cancellationToken)));

    /// <summary>
    /// Asynchronously filters the right side of this instance by a predicate, returning a new
    /// <see cref="Either{TLeft, TRight}"/> with the default value produced by the specified factory method on the
    /// left if the predicate fails.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="defaultFactoryAsync"></param>
    /// <returns></returns>
    public async Task<Either<TLeft, TRight>> WhereRightLazyAsync(
        Func<TRight, bool> predicate, Func<Task<TLeft>> defaultFactoryAsync)
        => IsRight ? (predicate(_right) ? this : await defaultFactoryAsync()) : this;

    /// <summary>
    /// Asynchronously filters the right side of this instance by a predicate, returning a new
    /// <see cref="Either{TLeft, TRight}"/> with the default value produced by the specified factory method on the
    /// left if the predicate fails.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="defaultFactoryAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Either<TLeft, TRight>> WhereRightLazyAsync(
        Func<TRight, bool> predicate, Func<CancellationToken, Task<TLeft>> defaultFactoryAsync,
        CancellationToken cancellationToken = default)
        => IsRight ? (predicate(_right) ? this : await defaultFactoryAsync(cancellationToken)) : this;
    #endregion

    #region Both Async
    #region Non-Cancellable
    /// <summary>
    /// Asynchronously filters the left side of this instance by a predicate, returning a new
    /// <see cref="Either{TLeft, TRight}"/> with the default value produced by the specified factory method on the
    /// right if the predicate fails.
    /// </summary>
    /// <param name="predicateAsync"></param>
    /// <param name="defaultFactoryAsync"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<Either<TLeft, TRight>> WhereLeftLazyAsync(
        Func<TLeft, Task<bool>> predicateAsync, Func<Task<TRight>> defaultFactoryAsync)
        => IsRight ? this : (await predicateAsync(_left) ? this : new(await defaultFactoryAsync()));

    /// <summary>
    /// Asynchronously filters the right side of this instance by a predicate, returning a new
    /// <see cref="Either{TLeft, TRight}"/> with the default value produced by the specified factory method on the
    /// left if the predicate fails.
    /// </summary>
    /// <param name="predicateAsync"></param>
    /// <param name="defaultFactoryAsync"></param>
    /// <returns></returns>
    public async Task<Either<TLeft, TRight>> WhereRightLazyAsync(
        Func<TRight, Task<bool>> predicateAsync, Func<Task<TLeft>> defaultFactoryAsync)
        => IsRight ? (await predicateAsync(_right) ? this : await defaultFactoryAsync()) : this;
    #endregion

    #region Only Predicate Cancellable
    /// <summary>
    /// Asynchronously filters the left side of this instance by a predicate, returning a new
    /// <see cref="Either{TLeft, TRight}"/> with the default value produced by the specified factory method on the
    /// right if the predicate fails.
    /// </summary>
    /// <param name="predicateAsync"></param>
    /// <param name="defaultFactoryAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<Either<TLeft, TRight>> WhereLeftLazyAsync(
        Func<TLeft, CancellationToken, Task<bool>> predicateAsync, Func<Task<TRight>> defaultFactoryAsync,
        CancellationToken cancellationToken = default)
        => IsRight ? this : (await predicateAsync(_left, cancellationToken) ? this : new(await defaultFactoryAsync()));

    /// <summary>
    /// Asynchronously filters the right side of this instance by a predicate, returning a new
    /// <see cref="Either{TLeft, TRight}"/> with the default value produced by the specified factory method on the
    /// left if the predicate fails.
    /// </summary>
    /// <param name="predicateAsync"></param>
    /// <param name="defaultFactoryAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Either<TLeft, TRight>> WhereRightLazyAsync(
        Func<TRight, CancellationToken, Task<bool>> predicateAsync, Func<Task<TLeft>> defaultFactoryAsync,
        CancellationToken cancellationToken = default)
        => IsRight ? (await predicateAsync(_right, cancellationToken) ? this : new(await defaultFactoryAsync())) : this;
    #endregion

    #region Only Factory Cancellable
    /// <summary>
    /// Asynchronously filters the left side of this instance by a predicate, returning a new
    /// <see cref="Either{TLeft, TRight}"/> with the default value produced by the specified factory method on the
    /// right if the predicate fails.
    /// </summary>
    /// <param name="predicateAsync"></param>
    /// <param name="defaultFactoryAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<Either<TLeft, TRight>> WhereLeftLazyAsync(
        Func<TLeft, Task<bool>> predicateAsync, Func<CancellationToken, Task<TRight>> defaultFactoryAsync,
        CancellationToken cancellationToken = default)
        => IsRight ? this : (await predicateAsync(_left) ? this : new(await defaultFactoryAsync(cancellationToken)));

    /// <summary>
    /// Asynchronously filters the right side of this instance by a predicate, returning a new
    /// <see cref="Either{TLeft, TRight}"/> with the default value produced by the specified factory method on the
    /// left if the predicate fails.
    /// </summary>
    /// <param name="predicateAsync"></param>
    /// <param name="defaultFactoryAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Either<TLeft, TRight>> WhereRightLazyAsync(
        Func<TRight, Task<bool>> predicateAsync, Func<CancellationToken, Task<TLeft>> defaultFactoryAsync,
        CancellationToken cancellationToken = default)
        => IsRight ? (await predicateAsync(_right) ? this : await defaultFactoryAsync(cancellationToken)) : this;
    #endregion

    #region Both Cancellable
    /// <summary>
    /// Asynchronously filters the left side of this instance by a predicate, returning a new
    /// <see cref="Either{TLeft, TRight}"/> with the default value produced by the specified factory method on the
    /// right if the predicate fails.
    /// </summary>
    /// <param name="predicateAsync"></param>
    /// <param name="defaultFactoryAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<Either<TLeft, TRight>> WhereLeftLazyAsync(
        Func<TLeft, CancellationToken, Task<bool>> predicateAsync,
        Func<CancellationToken, Task<TRight>> defaultFactoryAsync,
        CancellationToken cancellationToken = default)
        => IsRight
            ? this
            : (await predicateAsync(_left, cancellationToken) ? this : await defaultFactoryAsync(cancellationToken));

    /// <summary>
    /// Asynchronously filters the right side of this instance by a predicate, returning a new
    /// <see cref="Either{TLeft, TRight}"/> with the default value produced by the specified factory method on the
    /// left if the predicate fails.
    /// </summary>
    /// <param name="predicateAsync"></param>
    /// <param name="defaultFactoryAsync"></param>
    /// <param name="cancellationToken"></param
    /// <returns></returns>
    public async Task<Either<TLeft, TRight>> WhereRightLazyAsync(
        Func<TRight, CancellationToken, Task<bool>> predicateAsync,
        Func<CancellationToken, Task<TLeft>> defaultFactoryAsync,
        CancellationToken cancellationToken = default)
        => IsRight
            ? (await predicateAsync(_right, cancellationToken) ? this : await defaultFactoryAsync(cancellationToken))
            : this;
    #endregion
    #endregion
    #endregion
    #endregion
    #endregion
    #endregion
    #endregion

    #region Modifications
    #region Replace
    #region Eager
    /// <summary>
    /// Creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the left value replaced
    /// with the supplied <typeparamref name="TNewLeft"/> instance on the left, or the right value of this instance
    /// on the right.
    /// </summary>
    /// <typeparam name="TNewLeft"></typeparam>
    /// <param name="newLeft"></param>
    /// <returns></returns>
    public Either<TNewLeft, TRight> ReplaceLeft<TNewLeft>(TNewLeft newLeft)
        => IsRight ? new(_right) : new(newLeft);

    /// <summary>
    /// Creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the left value replaced
    /// with the supplied <typeparamref name="TNewLeft"/> instance on the left, or the right value replaced with the
    /// supplied <typeparamref name="TNewRight"/> instance on the right.
    /// </summary>
    /// <typeparam name="TNewLeft"></typeparam>
    /// <typeparam name="TNewRight"></typeparam>
    /// <param name="newLeft"></param>
    /// <param name="newRight"></param>
    /// <returns></returns>
    public Either<TNewLeft, TNewRight> ReplaceEither<TNewLeft, TNewRight>(TNewLeft newLeft, TNewRight newRight)
        => IsRight ? newRight : newLeft;

    /// <summary>
    /// Creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the right value replaced
    /// with the supplied <typeparamref name="TNewRight"/> instance on the right, or the left value of this instance
    /// on the left.
    /// </summary>
    /// <typeparam name="TNewRight"></typeparam>
    /// <param name="newRight"></param>
    /// <returns></returns>
    public Either<TLeft, TNewRight> ReplaceRight<TNewRight>(TNewRight newRight)
        => IsRight ? new(newRight) : new(_left);
    #endregion

    #region Lazy
    #region Left
    /// <summary>
    /// Creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the left value replaced
    /// with the result of calling the supplied <typeparamref name="TNewLeft"/> factory on the left, or the right
    /// value of this instance on the right.
    /// </summary>
    /// <typeparam name="TNewLeft"></typeparam>
    /// <param name="newLeftFactory"></param>
    /// <returns></returns>
    public Either<TNewLeft, TRight> ReplaceLeftLazy<TNewLeft>(Func<TNewLeft> newLeftFactory)
        => IsRight ? new(_right) : new(newLeftFactory());

    /// <summary>
    /// Asynchronously creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the left
    /// value replaced with the result of calling the supplied <typeparamref name="TNewLeft"/> factory on the left, or
    /// the right value of this instance on the right.
    /// </summary>
    /// <typeparam name="TNewLeft"></typeparam>
    /// <param name="newLeftFactoryAsync"></param>
    /// <returns></returns>
    public async Task<Either<TNewLeft, TRight>> ReplaceLeftLazyAsync<TNewLeft>(
        Func<Task<TNewLeft>> newLeftFactoryAsync)
        => IsRight ? _right : await newLeftFactoryAsync();

    /// <summary>
    /// Asynchronously creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the left
    /// value replaced with the result of calling the supplied <typeparamref name="TNewLeft"/> factory on the left, or
    /// the right value of this instance on the right.
    /// </summary>
    /// <typeparam name="TNewLeft"></typeparam>
    /// <param name="newLeftFactoryAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Either<TNewLeft, TRight>> ReplaceLeftLazyAsync<TNewLeft>(
        Func<CancellationToken, Task<TNewLeft>> newLeftFactoryAsync,
        CancellationToken cancellationToken = default)
        => IsRight ? _right : await newLeftFactoryAsync(cancellationToken);
    #endregion

    #region Both Sides
    #region Synchronous
    /// <summary>
    /// Creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the left value replaced
    /// with the result of calling the supplied <typeparamref name="TNewLeft"/> factory on the left, or the right
    /// value replaced with the supplied <typeparamref name="TNewRight"/> instance on the right.
    /// </summary>
    /// <typeparam name="TNewLeft"></typeparam>
    /// <typeparam name="TNewRight"></typeparam>
    /// <param name="newLeftFactory"></param>
    /// <param name="newRight"></param>
    /// <returns></returns>
    public Either<TNewLeft, TNewRight> ReplaceEitherLazy<TNewLeft, TNewRight>(
        Func<TNewLeft> newLeftFactory, TNewRight newRight)
        => IsRight ? newRight : newLeftFactory();

    /// <summary>
    /// Creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the left value replaced
    /// with the result of calling the supplied <typeparamref name="TNewLeft"/> factory on the left, or the right value
    /// replaced with the result of calling the supplied <typeparamref name="TNewRight"/> factory on the right.
    /// </summary>
    /// <typeparam name="TNewLeft"></typeparam>
    /// <typeparam name="TNewRight"></typeparam>
    /// <param name="newLeftFactory"></param>
    /// <param name="newRightFactory"></param>
    /// <returns></returns>
    public Either<TNewLeft, TNewRight> ReplaceEitherLazy<TNewLeft, TNewRight>(
        Func<TNewLeft> newLeftFactory, Func<TNewRight> newRightFactory)
        => IsRight ? newRightFactory() : newLeftFactory();

    /// <summary>
    /// Creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the left value replaced
    /// with the supplied <typeparamref name="TNewLeft"/> instance on the left, or the right value replaced with the
    /// result of calling the supplied <typeparamref name="TNewRight"/> factory on the right.
    /// </summary>
    /// <typeparam name="TNewLeft"></typeparam>
    /// <typeparam name="TNewRight"></typeparam>
    /// <param name="newLeft"></param>
    /// <param name="newRightFactory"></param>
    /// <returns></returns>
    public Either<TNewLeft, TNewRight> ReplaceEitherLazy<TNewLeft, TNewRight>(
        TNewLeft newLeft, Func<TNewRight> newRightFactory)
        => IsRight ? newRightFactory() : newLeft;
    #endregion

    #region Only Left Asynchronous
    #region Right Eager
    /// <summary>
    /// Asynchronously creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the left
    /// value replaced with the result of calling the supplied <typeparamref name="TNewLeft"/> factory on the left, or
    /// the right value replaced with the supplied <typeparamref name="TNewRight"/> instance on the right.
    /// </summary>
    /// <typeparam name="TNewLeft"></typeparam>
    /// <typeparam name="TNewRight"></typeparam>
    /// <param name="newLeftFactoryAsync"></param>
    /// <param name="newRight"></param>
    /// <returns></returns>
    public async Task<Either<TNewLeft, TNewRight>> ReplaceEitherLazyAsync<TNewLeft, TNewRight>(
        Func<Task<TNewLeft>> newLeftFactoryAsync, TNewRight newRight)
        => IsRight ? newRight : await newLeftFactoryAsync();

    /// <summary>
    /// Asynchronously creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the left
    /// value replaced with the result of calling the supplied <typeparamref name="TNewLeft"/> factory on the left, or
    /// the right value replaced with the supplied <typeparamref name="TNewRight"/> instance on the right.
    /// </summary>
    /// <typeparam name="TNewLeft"></typeparam>
    /// <typeparam name="TNewRight"></typeparam>
    /// <param name="newLeftFactoryAsync"></param>
    /// <param name="newRight"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Either<TNewLeft, TNewRight>> ReplaceEitherLazyAsync<TNewLeft, TNewRight>(
        Func<CancellationToken, Task<TNewLeft>> newLeftFactoryAsync, TNewRight newRight,
        CancellationToken cancellationToken = default)
        => IsRight ? newRight : await newLeftFactoryAsync(cancellationToken);
    #endregion Right Eager

    #region Right Lazy
    /// <summary>
    /// Asynchronously creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the left
    /// value replaced with the result of calling the supplied <typeparamref name="TNewLeft"/> factory on the left, or
    /// the right value replaced with the result of calling the supplied <typeparamref name="TNewRight"/> factory
    /// on the right.
    /// </summary>
    /// <typeparam name="TNewLeft"></typeparam>
    /// <typeparam name="TNewRight"></typeparam>
    /// <param name="newLeftFactoryAsync"></param>
    /// <param name="newRightFactory"></param>
    /// <returns></returns>
    public async Task<Either<TNewLeft, TNewRight>> ReplaceEitherLazyAsync<TNewLeft, TNewRight>(
        Func<Task<TNewLeft>> newLeftFactoryAsync, Func<TNewRight> newRightFactory)
        => IsRight ? newRightFactory() : await newLeftFactoryAsync();

    /// <summary>
    /// Asynchronously creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the left
    /// value replaced with the result of calling the supplied <typeparamref name="TNewLeft"/> factory on the left, or
    /// the right value replaced with the result of calling the supplied <typeparamref name="TNewRight"/> factory
    /// on the right.
    /// </summary>
    /// <typeparam name="TNewLeft"></typeparam>
    /// <typeparam name="TNewRight"></typeparam>
    /// <param name="newLeftFactoryAsync"></param>
    /// <param name="newRightFactory"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Either<TNewLeft, TNewRight>> ReplaceEitherLazyAsync<TNewLeft, TNewRight>(
        Func<CancellationToken, Task<TNewLeft>> newLeftFactoryAsync, Func<TNewRight> newRightFactory,
        CancellationToken cancellationToken = default)
        => IsRight ? newRightFactory() : await newLeftFactoryAsync(cancellationToken);
    #endregion Right Lazy
    #endregion Only Left Asynchronous

    #region Only Right Asynchronous
    #region Left Eager
    /// <summary>
    /// Asynchronously creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the left
    /// value replaced with the supplied <typeparamref name="TNewLeft"/> instance on the left, or the right value
    /// replaced with the result of calling the supplied <typeparamref name="TNewRight"/> factory on the right.
    /// </summary>
    /// <typeparam name="TNewLeft"></typeparam>
    /// <typeparam name="TNewRight"></typeparam>
    /// <param name="newLeft"></param>
    /// <param name="newRightFactoryAsync"></param>
    /// <returns></returns>
    public async Task<Either<TNewLeft, TNewRight>> ReplaceEitherLazyAsync<TNewLeft, TNewRight>(
        TNewLeft newLeft, Func<Task<TNewRight>> newRightFactoryAsync)
        => IsRight ? await newRightFactoryAsync() : newLeft;

    /// <summary>
    /// Asynchronously creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the left
    /// value replaced with the supplied <typeparamref name="TNewLeft"/> instance on the left, or the right value
    /// replaced with the result of calling the supplied <typeparamref name="TNewRight"/> factory on the right.
    /// </summary>
    /// <typeparam name="TNewLeft"></typeparam>
    /// <typeparam name="TNewRight"></typeparam>
    /// <param name="newLeft"></param>
    /// <param name="newRightFactoryAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Either<TNewLeft, TNewRight>> ReplaceEitherLazyAsync<TNewLeft, TNewRight>(
        TNewLeft newLeft, Func<CancellationToken, Task<TNewRight>> newRightFactoryAsync,
        CancellationToken cancellationToken = default)
        => IsRight ? await newRightFactoryAsync(cancellationToken) : newLeft;
    #endregion Left Eager

    #region Left Lazy
    /// <summary>
    /// Asynchronously creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the left
    /// value replaced with the result of calling the supplied <typeparamref name="TNewLeft"/> factory on the left, or
    /// the right value replaced with the result of calling the supplied <typeparamref name="TNewRight"/> factory
    /// on the right.
    /// </summary>
    /// <typeparam name="TNewLeft"></typeparam>
    /// <typeparam name="TNewRight"></typeparam>
    /// <param name="newLeftFactory"></param>
    /// <param name="newRightFactoryAsync"></param>
    /// <returns></returns>
    public async Task<Either<TNewLeft, TNewRight>> ReplaceEitherLazyAsync<TNewLeft, TNewRight>(
        Func<TNewLeft> newLeftFactory, Func<Task<TNewRight>> newRightFactoryAsync)
        => IsRight ? await newRightFactoryAsync() : newLeftFactory();

    /// <summary>
    /// Asynchronously creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the left
    /// value replaced with the result of calling the supplied <typeparamref name="TNewLeft"/> factory on the left, or
    /// the right value replaced with the result of calling the supplied <typeparamref name="TNewRight"/> factory
    /// on the right.
    /// </summary>
    /// <typeparam name="TNewLeft"></typeparam>
    /// <typeparam name="TNewRight"></typeparam>
    /// <param name="newLeftFactory"></param>
    /// <param name="newRightFactoryAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Either<TNewLeft, TNewRight>> ReplaceEitherLazyAsync<TNewLeft, TNewRight>(
        Func<TNewLeft> newLeftFactory, Func<CancellationToken, Task<TNewRight>> newRightFactoryAsync,
        CancellationToken cancellationToken = default)
        => IsRight ? await newRightFactoryAsync(cancellationToken) : newLeftFactory();
    #endregion Left Lazy
    #endregion Only Right Asynchronous

    #region Both Asynchronous
    /// <summary>
    /// Asynchronously creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the left
    /// value replaced with the result of calling the supplied <typeparamref name="TNewLeft"/> factory on the left, or
    /// the right value replaced with the result of calling the supplied <typeparamref name="TNewRight"/> factory
    /// on the right.
    /// </summary>
    /// <typeparam name="TNewLeft"></typeparam>
    /// <typeparam name="TNewRight"></typeparam>
    /// <param name="newLeftFactoryAsync"></param>
    /// <param name="newRightFactoryAsync"></param>
    /// <returns></returns>
    public async Task<Either<TNewLeft, TNewRight>> ReplaceEitherLazyAsync<TNewLeft, TNewRight>(
        Func<Task<TNewLeft>> newLeftFactoryAsync, Func<Task<TNewRight>> newRightFactoryAsync)
        => IsRight ? await newRightFactoryAsync() : await newLeftFactoryAsync();

    /// <summary>
    /// Asynchronously creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the left
    /// value replaced with the result of calling the supplied <typeparamref name="TNewLeft"/> factory on the left, or
    /// the right value replaced with the result of calling the supplied <typeparamref name="TNewRight"/> factory
    /// on the right.
    /// </summary>
    /// <typeparam name="TNewLeft"></typeparam>
    /// <typeparam name="TNewRight"></typeparam>
    /// <param name="newLeftFactoryAsync"></param>
    /// <param name="newRightFactoryAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Either<TNewLeft, TNewRight>> ReplaceEitherLazyAsync<TNewLeft, TNewRight>(
        Func<CancellationToken, Task<TNewLeft>> newLeftFactoryAsync,
        Func<Task<TNewRight>> newRightFactoryAsync,
        CancellationToken cancellationToken = default)
        => IsRight ? await newRightFactoryAsync() : await newLeftFactoryAsync(cancellationToken);

    /// <summary>
    /// Asynchronously creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the left
    /// value replaced with the result of calling the supplied <typeparamref name="TNewLeft"/> factory on the left, or
    /// the right value replaced with the result of calling the supplied <typeparamref name="TNewRight"/> factory
    /// on the right.
    /// </summary>
    /// <typeparam name="TNewLeft"></typeparam>
    /// <typeparam name="TNewRight"></typeparam>
    /// <param name="newLeftFactoryAsync"></param>
    /// <param name="newRightFactoryAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Either<TNewLeft, TNewRight>> ReplaceEitherLazyAsync<TNewLeft, TNewRight>(
        Func<Task<TNewLeft>> newLeftFactoryAsync,
        Func<CancellationToken, Task<TNewRight>> newRightFactoryAsync,
        CancellationToken cancellationToken = default)
        => IsRight ? await newRightFactoryAsync(cancellationToken) : await newLeftFactoryAsync();

    /// <summary>
    /// Asynchronously creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the left
    /// value replaced with the result of calling the supplied <typeparamref name="TNewLeft"/> factory on the left, or
    /// the right value replaced with the result of calling the supplied <typeparamref name="TNewRight"/> factory
    /// on the right.
    /// </summary>
    /// <typeparam name="TNewLeft"></typeparam>
    /// <typeparam name="TNewRight"></typeparam>
    /// <param name="newLeftFactoryAsync"></param>
    /// <param name="newRightFactoryAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Either<TNewLeft, TNewRight>> ReplaceEitherLazyAsync<TNewLeft, TNewRight>(
        Func<CancellationToken, Task<TNewLeft>> newLeftFactoryAsync,
        Func<CancellationToken, Task<TNewRight>> newRightFactoryAsync,
        CancellationToken cancellationToken = default)
        => IsRight ? await newRightFactoryAsync(cancellationToken) : await newLeftFactoryAsync(cancellationToken);
    #endregion Both Asynchronous
    #endregion Both Sides

    #region Right
    /// <summary>
    /// Creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the right value replaced
    /// with the result of calling the supplied <typeparamref name="TNewRight"/> factory on the right, or the left
    /// value of this instance on the left.
    /// </summary>
    /// <typeparam name="TNewRight"></typeparam>
    /// <param name="newRightFactory"></param>
    /// <returns></returns>
    public Either<TLeft, TNewRight> ReplaceRightLazy<TNewRight>(Func<TNewRight> newRightFactory)
        => IsRight ? new(newRightFactory()) : new(_left);

    /// <summary>
    /// Asynchronously creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the right
    /// value replaced with the result of calling the supplied <typeparamref name="TNewRight"/> factory on the right,
    /// or the left value of this instance on the left.
    /// </summary>
    /// <typeparam name="TNewRight"></typeparam>
    /// <param name="newRightFactory"></param>
    /// <returns></returns>
    public async Task<Either<TLeft, TNewRight>> ReplaceRightLazyAsync<TNewRight>(Func<Task<TNewRight>> newRightFactory)
        => IsRight ? await newRightFactory() : _left;

    /// <summary>
    /// Asynchronously creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the right
    /// value replaced with the result of calling the supplied <typeparamref name="TNewRight"/> factory on the right,
    /// or the left value of this instance on the left.
    /// </summary>
    /// <typeparam name="TNewRight"></typeparam>
    /// <param name="newRightFactory"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<Either<TLeft, TNewRight>> ReplaceRightLazyAsync<TNewRight>(
        Func<CancellationToken, Task<TNewRight>> newRightFactory,
        CancellationToken cancellationToken = default)
        => IsRight ? await newRightFactory(cancellationToken) : _left;
    #endregion
    #endregion
    #endregion

    #region SwapSides
    /// <summary>
    /// Creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the
    /// <typeparamref name="TLeft"/> and <typeparamref name="TRight"/> sides swapped.
    /// </summary>
    /// <returns></returns>
    [InstanceNotDefault]
    public Either<TRight, TLeft> SwapSides() => new(_right, _left, !IsRight);
    #endregion
    #endregion

    #region Predicates
    #region Synchronous
    /// <summary>
    /// Determines whether or not the current instance contains a left value that matches the specified predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>
    /// <see langword="true"/> if the current instance wraps a left value that matches <paramref name="predicate"/>,
    /// or <see langword="false"/> if this instance either contains a left value that does not match
    /// <paramref name="predicate"/> or is right.
    /// </returns>
    [InstanceNotDefault]
    public bool LeftMatches(Func<TLeft, bool> predicate) => IsLeft && predicate(_left);

    /// <summary>
    /// Determines whether or not the current instance comtains -either- a left value that matches the specified left
    /// predicate -or- a right value that matches the specified right predicate.
    /// </summary>
    /// <param name="leftPredicate"></param>
    /// <param name="rightPredicate"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public bool EitherMatches(Func<TLeft, bool> leftPredicate, Func<TRight, bool> rightPredicate)
        => IsRight ? rightPredicate(_right) : leftPredicate(_left);

    /// <summary>
    /// Determines whether or not the current instance contains a right value that matches the specified predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns>
    /// <see langword="true"/> if the current instance wraps a right value that matches <paramref name="predicate"/>,
    /// or <see langword="false"/> if this instance either contains a right value that does not match
    /// <paramref name="predicate"/> or is left.
    /// </returns>
    public bool RightMatches(Func<TRight, bool> predicate) => IsRight && predicate(_right);
    #endregion

    #region Asynchronous
    #region Left
    /// <summary>
    /// Asynchronously determines whether or not the current instance contains a left value that matches the
    /// specified predicate.
    /// </summary>
    /// <param name="predicateAsync"></param>
    /// <returns>
    /// A task wrapping <see langword="true"/> if the current instance wraps a left value that matches
    /// <paramref name="predicateAsync"/>, or <see langword="false"/> if this instance either contains a left value
    /// that does not match <paramref name="predicateAsync"/> or is right.
    /// </returns>
    [InstanceNotDefault]
    public async Task<bool> LeftMatchesAsync(Func<TLeft, Task<bool>> predicateAsync)
        => IsLeft && await predicateAsync(_left);

    /// <summary>
    /// Asynchronously determines whether or not the current instance contains a left value that matches the
    /// specified predicate.
    /// </summary>
    /// <param name="predicateAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// A task wrapping <see langword="true"/> if the current instance wraps a left value that matches
    /// <paramref name="predicateAsync"/>, or <see langword="false"/> if this instance either contains a left value
    /// that does not match <paramref name="predicateAsync"/> or is right.
    /// </returns>
    [InstanceNotDefault]
    public async Task<bool> LeftMatchesAsync(
        Func<TLeft, CancellationToken, Task<bool>> predicateAsync, CancellationToken cancellationToken = default)
        => IsLeft && await predicateAsync(_left, cancellationToken);
    #endregion

    #region Either Side
    #region Left Async Only
    /// <summary>
    /// Asynchronously determines whether or not the current instance comtains -either- a left value that matches the
    /// specified left predicate -or- a right value that matches the specified right predicate.
    /// </summary>
    /// <param name="leftPredicateAsync"></param>
    /// <param name="rightPredicate"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<bool> EitherMatchesAsync(
        Func<TLeft, Task<bool>> leftPredicateAsync, Func<TRight, bool> rightPredicate)
        => IsRight ? rightPredicate(_right) : await leftPredicateAsync(_left);

    /// <summary>
    /// Asynchronously determines whether or not the current instance comtains -either- a left value that matches the
    /// specified left predicate -or- a right value that matches the specified right predicate.
    /// </summary>
    /// <param name="leftPredicateAsync"></param>
    /// <param name="rightPredicate"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<bool> EitherMatchesAsync(
        Func<TLeft, CancellationToken, Task<bool>> leftPredicateAsync, Func<TRight, bool> rightPredicate,
        CancellationToken cancellationToken = default)
        => IsRight ? rightPredicate(_right) : await leftPredicateAsync(_left, cancellationToken);
    #endregion

    #region Both Async
    /// <summary>
    /// Asynchronously determines whether or not the current instance contains a left value that matches the
    /// specified predicate.
    /// </summary>
    /// <param name="leftPredicateAsync"></param>
    /// <param name="rightPredicateAsync"></param>
    /// <returns></returns>
    public async Task<bool> EitherMatchesAsync(
        Func<TLeft, Task<bool>> leftPredicateAsync, Func<TRight, Task<bool>> rightPredicateAsync)
        => IsRight ? await rightPredicateAsync(_right) : await leftPredicateAsync(_left);

    /// <summary>
    /// Asynchronously determines whether or not the current instance contains a left value that matches the
    /// specified predicate.
    /// </summary>
    /// <param name="leftPredicateAsync"></param>
    /// <param name="rightPredicateAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> EitherMatchesAsync(
        Func<TLeft, CancellationToken, Task<bool>> leftPredicateAsync, Func<TRight, Task<bool>> rightPredicateAsync,
        CancellationToken cancellationToken = default)
        => IsRight ? await rightPredicateAsync(_right) : await leftPredicateAsync(_left, cancellationToken);

    /// <summary>
    /// Asynchronously determines whether or not the current instance contains a left value that matches the
    /// specified predicate.
    /// </summary>
    /// <param name="leftPredicateAsync"></param>
    /// <param name="rightPredicateAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> EitherMatchesAsync(
        Func<TLeft, Task<bool>> leftPredicateAsync, Func<TRight, CancellationToken, Task<bool>> rightPredicateAsync,
        CancellationToken cancellationToken = default)
        => IsRight ? await rightPredicateAsync(_right, cancellationToken) : await leftPredicateAsync(_left);

    /// <summary>
    /// Asynchronously determines whether or not the current instance contains a left value that matches the
    /// specified predicate.
    /// </summary>
    /// <param name="leftPredicateAsync"></param>
    /// <param name="rightPredicateAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<bool> EitherMatchesAsync(
        Func<TLeft, CancellationToken, Task<bool>> leftPredicateAsync,
        Func<TRight, CancellationToken, Task<bool>> rightPredicateAsync,
        CancellationToken cancellationToken = default)
        => IsRight
            ? await rightPredicateAsync(_right, cancellationToken)
            : await leftPredicateAsync(_left, cancellationToken);
    #endregion

    #region Right Async Only
    /// <summary>
    /// Asynchronously determines whether or not the current instance comtains -either- a left value that matches the
    /// specified left predicate -or- a right value that matches the specified right predicate.
    /// </summary>
    /// <param name="leftPredicate"></param>
    /// <param name="rightPredicateAsync"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<bool> EitherMatchesAsync(
        Func<TLeft, bool> leftPredicate, Func<TRight, Task<bool>> rightPredicateAsync)
        => IsRight ? await rightPredicateAsync(_right) : leftPredicate(_left);

    /// <summary>
    /// Asynchronously determines whether or not the current instance comtains -either- a left value that matches the
    /// specified left predicate -or- a right value that matches the specified right predicate.
    /// </summary>
    /// <param name="leftPredicate"></param>
    /// <param name="rightPredicateAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public async Task<bool> EitherMatchesAsync(
        Func<TLeft, bool> leftPredicate, Func<TRight, CancellationToken, Task<bool>> rightPredicateAsync,
        CancellationToken cancellationToken = default)
        => IsRight ? await rightPredicateAsync(_right, cancellationToken) : leftPredicate(_left);
    #endregion
    #endregion

    #region Right
    /// <summary>
    /// Asynchronously determines whether or not the current instance contains a right value that matches the
    /// specified predicate.
    /// </summary>
    /// <param name="predicateAsync"></param>
    /// <returns>
    /// A task wrapping <see langword="true"/> if the current instance wraps a right value that matches
    /// <paramref name="predicateAsync"/>, or <see langword="false"/> if this instance either contains a right value
    /// that does not match <paramref name="predicateAsync"/> or is left.
    /// </returns>
    public async Task<bool> RightMatchesAsync(Func<TRight, Task<bool>> predicateAsync)
        => IsRight && await predicateAsync(_right);

    /// <summary>
    /// Asynchronously determines whether or not the current instance contains a right value that matches the
    /// specified predicate.
    /// </summary>
    /// <param name="predicateAsync"></param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// A task wrapping <see langword="true"/> if the current instance wraps a right value that matches
    /// <paramref name="predicateAsync"/>, or <see langword="false"/> if this instance either contains a right value
    /// that does not match <paramref name="predicateAsync"/> or is left.
    /// </returns>
    public async Task<bool> RightMatchesAsync(
        Func<TRight, CancellationToken, Task<bool>> predicateAsync, CancellationToken cancellationToken = default)
        => IsRight && await predicateAsync(_right, cancellationToken);
    #endregion
    #endregion
    #endregion

    #region ToString
    /// <summary>
    /// Gets a string that represents the current instance.
    /// </summary>
    /// <returns></returns>
    public override string ToString()
        => $"{nameof(Either)} {{ {(IsRight ? $"Right = {_right}" : $"Left = {_left}")} }}";
    #endregion
    #endregion
}
