﻿using Rem.Core.Attributes;
using Rem.Core.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rem.Core.Utilities.Monads;

/// <summary>
/// Represents a single value of one of two possible types.
/// </summary>
public readonly record struct Either<TLeft, TRight> : IDefaultableStruct
{
    #region Properties And Fields
    /// <inheritdoc/>
    public bool IsDefault => !IsRight;

    /// <summary>
    /// Gets whether or not this object wraps a value of type <typeparamref name="TLeft"/>.
    /// </summary>
    public bool IsLeft => !_isRight;

    /// <summary>
    /// Gets the <typeparamref name="TLeft"/> value wrapped in this instance, or the default value of type
    /// <typeparamref name="TLeft"/> if this instance wraps a value of type <typeparamref name="TRight"/>.
    /// </summary>
    [MaybeNull, MaybeDefault] public TLeft LeftOrDefault => _isRight ? default : _left;

    /// <summary>
    /// Gets the <typeparamref name="TLeft"/> value wrapped in this instance.
    /// </summary>
    /// <exception cref="EitherException">
    /// This instance wraps an instance of <typeparamref name="TRight"/>.
    /// </exception>
    public TLeft Left
        => _isRight
            ? throw new EitherException($"Invalid left access of {nameof(Either<TLeft, TRight>)}.")
            : _left;

    /// <summary>
    /// Gets the <typeparamref name="TRight"/> value wrapped in this instance, or the default value of type
    /// <typeparamref name="TRight"/> if this instance wraps a value of type <typeparamref name="TLeft"/>.
    /// </summary>
    [MaybeNull, MaybeDefault] public TRight RightOrDefault => _isRight ? _right : default;

    /// <summary>
    /// Gets the <typeparamref name="TRight"/> value wrapped in this instance.
    /// </summary>
    /// <exception cref="EitherException">
    /// This instance wraps an instance of <typeparamref name="TLeft"/>.
    /// </exception>
    public TRight Right
        => _isRight
            ? _right
            : throw new EitherException($"Invalid right access of {nameof(Either<TRight, TRight>)}.");

    /// <summary>
    /// Gets whether or not this object wraps a value of type <typeparamref name="TRight"/>.
    /// </summary>
    public bool IsRight => _isRight;
    private readonly bool _isRight;

    [AllowNull, AllowDefault] private readonly TLeft _left;
    [AllowNull, AllowDefault] private readonly TRight _right;
    #endregion

    #region Constructors
    /// <summary>
    /// Constructs a new <see cref="Either{TLeft, TRight}"/> wrapping the <typeparamref name="TLeft"/> value passed in.
    /// </summary>
    /// <param name="Value"></param>
    public Either(TLeft Value) : this(Value, default, IsRight: false) { }

    /// <summary>
    /// Constructs a new <see cref="Either{TLeft, TRight}"/> wrapping the <typeparamref name="TRight"/> value passed in.
    /// </summary>
    /// <param name="Value"></param>
    public Either(TRight Value) : this(default, Value, IsRight: true) { }

    private Either([AllowNull, AllowDefault] TLeft LeftValue, [AllowNull, AllowDefault] TRight RightValue, bool IsRight)
    {
        _left = LeftValue;
        _right = RightValue;
        _isRight = IsRight;
    }
    #endregion

    #region Methods
    #region TryGet
    /// <summary>
    /// Tries to get the <typeparamref name="TLeft"/> value wrapped in this instance.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Whether or not this instance wraps an instance of type <typeparamref name="TLeft"/>.</returns>
    public bool TryGetLeft([MaybeNullWhen(false), MaybeDefaultWhen(false)] out TLeft value)
    {
        value = _left;
        return !_isRight;
    }

    /// <summary>
    /// Tries to get the <typeparamref name="TRight"/> value wrapped in this instance.
    /// </summary>
    /// <param name="value"></param>
    /// <returns>Whether or not this instance wraps an instance of type <typeparamref name="TRight"/>.</returns>
    public bool TryGetRight([MaybeNullWhen(false), MaybeDefaultWhen(false)] out TRight value)
    {
        value = _right;
        return _isRight;
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
        return _isRight;
    }
    #endregion
    #endregion
}
