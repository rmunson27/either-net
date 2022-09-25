using Rem.Core.Attributes;
using Rem.Core.ComponentModel;
using System;
using System.Collections;
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
    public bool Contains(
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
    public Either<TLeftResult, TRightResult> Select<TLeftResult, TRightResult>(
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

    #region SelectMany
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
    public Either<TLeftResult, TRightResult> SelectMany<TLeftResult, TRightResult>(
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
    public IEnumerable Enumerate() => (IEnumerable)GetNonGenericEnumerator();

    /// <summary>
    /// Gets an <see cref="IEnumerator"/> enumerating the value wrapped in this instance.
    /// </summary>
    /// <returns></returns>
    [InstanceNotDefault]
    public IEnumerator GetNonGenericEnumerator()
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
    /// Filters the right side of this instance by a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    public IEnumerable<TRight> WhereRight(Func<TRight, bool> predicate)
    {
        if (IsRight && predicate(_right)) yield return _right;
    }
    #endregion

    #region Either (Default Value)
    #region Eager
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

    #region Lazy
    /// <summary>
    /// Filters the right side of this instance by a predicate, returning a new <see cref="Either{TLeft, TRight}"/>
    /// with the default value produced by the specified factory method on the left if the predicate fails.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="defaultValueFactory"></param>
    /// <returns></returns>
    [InstanceNotDefault]
    public Either<TLeft, TRight> WhereLeft(Func<TLeft, bool> predicate, Func<TRight> defaultValueFactory)
        => IsRight ? this : (predicate(_left) ? this : new(defaultValueFactory()));

    /// <summary>
    /// Filters the right side of this instance by a predicate, returning a new <see cref="Either{TLeft, TRight}"/>
    /// with the default value produced by the specified factory method on the left if the predicate fails.
    /// </summary>
    /// <param name="predicate"></param>
    /// <param name="defaultValueFactory"></param>
    /// <returns></returns>
    public Either<TLeft, TRight> WhereRight(Func<TRight, bool> predicate, Func<TLeft> defaultValueFactory)
        => IsRight ? (predicate(_right) ? this : new(defaultValueFactory())) : this;
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
    /// Creates a new <see cref="Either{TLeft, TRight}"/> equivalent to this instance with the right value replaced
    /// with the result of calling the supplied <typeparamref name="TNewRight"/> factory on the right, or the left
    /// value of this instance on the left.
    /// </summary>
    /// <typeparam name="TNewRight"></typeparam>
    /// <param name="newRightFactory"></param>
    /// <returns></returns>
    public Either<TLeft, TNewRight> ReplaceRightLazy<TNewRight>(Func<TNewRight> newRightFactory)
        => IsRight ? new(newRightFactory()) : new(_left);
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
    public bool Matches(Func<TLeft, bool> leftPredicate, Func<TRight, bool> rightPredicate)
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
