﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemTest.Core.Utilities.Monads;

/// <summary>
/// Tests of the
/// <see cref="Either{TLeft, TRight}.Select{TLeftResult, TRightResult}(Func{TLeft, TLeftResult}, Func{TRight, TRightResult})"/>
/// and
/// <see cref="Either{TLeft, TRight}.SelectMany{TLeftResult, TRightResult}(Func{TLeft, Either{TLeftResult, TRightResult}}, Func{TRight, Either{TLeftResult, TRightResult}})"/>
/// methods and related overloads.
/// </summary>
[TestClass]
public class SelectTest
{
    #region Constants
    /// <summary>
    /// The timespan to delay in a cancellable asynchronous selector.
    /// </summary>
    private static readonly TimeSpan AsyncCancellableDelay = TimeSpan.FromSeconds(2);

    /// <summary>
    /// The timespan to wait before testing a cancellation of an async method.
    /// </summary>
    private static readonly TimeSpan AsyncCancellationTestWaitPeriod = TimeSpan.FromSeconds(1);
    #endregion

    #region Tests
    #region Select
    #region Synchronous
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.SelectLeft{TLeftResult}(Func{TLeft, TLeftResult})"/> method.
    /// </summary>
    [TestMethod]
    public void TestSelectLeft()
    {
        Assert.That.HasLeft(4, Either<object?, string>.NewLeft(null).SelectLeft(o => o is null ? 4 : 5));
        Assert.That.HasRight("", Either<object?, string>.New("").SelectLeft(o => o is null ? 4 : 5));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.Select{TLeftResult, TRightResult}(Func{TLeft, TLeftResult}, Func{TRight, TRightResult})"/>
    /// method.
    /// </summary>
    [TestMethod]
    public void TestSelect()
    {
        static string intToStr(int i) => i.ToString();
        static int strToInt(string s) => int.Parse(s);

        Assert.That.HasLeft("4", Either<int, string>.New(4).Select(intToStr, strToInt));
        Assert.That.HasRight(4, Either<int, string>.New("4").Select(intToStr, strToInt));
    }

    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.SelectRight{TRightResult}(Func{TRight, TRightResult})"/> method.
    /// </summary>
    [TestMethod]
    public void TestSelectRight()
    {
        Assert.That.HasRight(4, Either<object?, string>.New("").SelectRight(s => string.IsNullOrEmpty(s) ? 4 : 5));
        Assert.That.HasLeft(null, Either<object?, string>.NewLeft(null).SelectRight(s => string.IsNullOrEmpty(s) ? 4 : 5));
    }
    #endregion

    #region Asynchronous
    #region Left
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.SelectLeftAsync{TLeftResult}(Func{TLeft, Task{TLeftResult}})"/>
    /// method and related overloads.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectLeftAsync_NonCancellable()
    {
        Assert.That.HasLeft(
            true,
            await Either<object?, string>.NewLeft(null).SelectLeftAsync(IsNullAsync).ConfigureAwait(false));
        Assert.That.HasRight(
            "",
            await Either<object?, string>.New("").SelectLeftAsync(IsNullAsync).ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectLeftAsync{TLeftResult}(Func{TLeft, CancellationToken, Task{TLeftResult}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectLeftAsync_Cancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            true,
            await Either<object?, string>.NewLeft(null)
                    .SelectLeftAsync(CancellableIsNullAsync, CancellationToken.None)
                    .ConfigureAwait(false));
        Assert.That.HasRight(
            "",
            await Either<object?, string>.New("")
                    .SelectLeftAsync(CancellableIsNullAsync, CancellationToken.None)
                    .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectLeftAsync{TLeftResult}(Func{TLeft, CancellationToken, Task{TLeftResult}}, CancellationToken)"/>
    /// method when canceled.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectLeftAsync_Cancellable_Cancellation()
    {
        await TestAndAssertCanceled(
                    (e, ct) => e.SelectLeftAsync(CancellableIsNullAsync, ct),
                    Either<object?, string>.NewLeft(null))
                .ConfigureAwait(false);
        Assert.That.HasRight(
            "",
            await TestAndAssertNotCanceled(
                (e, ct) => e.SelectLeftAsync(CancellableIsNullAsync, ct), Either<object?, string>.New(""))
                .ConfigureAwait(false));
    }
    #endregion

    #region Both Sides
    #region Only Left Selector Async
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectAsync{TLeftResult, TRightResult}(Func{TLeft, Task{TLeftResult}}, Func{TRight, TRightResult})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectAsync_LeftAsyncOnly_NonCancellable()
    {
        Assert.That.HasLeft(
            true,
            await Either<object?, string>.NewLeft(null).SelectAsync(IsNullAsync, StringLength).ConfigureAwait(false));
        Assert.That.HasRight(
            0,
            await Either<object?, string>.New("").SelectAsync(IsNullAsync, StringLength).ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectAsync{TLeftResult, TRightResult}(Func{TLeft, CancellationToken, Task{TLeftResult}}, Func{TRight, TRightResult}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectAsync_LeftAsyncOnly_Cancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            true,
            await Either<object?, string>.NewLeft(null)
                    .SelectAsync(CancellableIsNullAsync, StringLength, CancellationToken.None)
                    .ConfigureAwait(false));
        Assert.That.HasRight(
            0,
            await Either<object?, string>.New("")
                    .SelectAsync(CancellableIsNullAsync, StringLength, CancellationToken.None)
                    .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectAsync{TLeftResult, TRightResult}(Func{TLeft, CancellationToken, Task{TLeftResult}}, Func{TRight, TRightResult}, CancellationToken)"/>
    /// method and cancels it to ensure cancellation is observed properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectAsync_LeftAsyncOnly_Cancellable_Cancellation()
    {
        await TestAndAssertCanceled(
                (e, ct) => e.SelectAsync(CancellableIsNullAsync, StringLength, ct),
                Either<object?, string>.NewLeft(null)).ConfigureAwait(false);
        Assert.That.HasRight(
            0,
            await TestAndAssertNotCanceled(
                (e, ct) => e.SelectAsync(CancellableIsNullAsync, StringLength, ct), Either<object?, string>.New(""))
                .ConfigureAwait(false));
    }
    #endregion

    #region Both Selectors Async
    #region Non-Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectAsync{TLeftResult, TRightResult}(Func{TLeft, Task{TLeftResult}}, Func{TRight, Task{TRightResult}})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectAsync_BothAsync_NonCancellable()
    {
        Assert.That.HasLeft(
            true,
            await Either<object?, string>.NewLeft(null).SelectAsync(IsNullAsync, StringLengthAsync).ConfigureAwait(false));
        Assert.That.HasRight(
            0,
            await Either<object?, string>.New("").SelectAsync(IsNullAsync, StringLengthAsync).ConfigureAwait(false));
    }
    #endregion

    #region Left Cancellable Only
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectAsync{TLeftResult, TRightResult}(Func{TLeft, CancellationToken, Task{TLeftResult}}, Func{TRight, Task{TRightResult}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectAsync_BothAsync_OnlyLeftCancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            true,
            await Either<object?, string>.NewLeft(null)
                    .SelectAsync(CancellableIsNullAsync, StringLengthAsync)
                    .ConfigureAwait(false));
        Assert.That.HasRight(
            0,
            await Either<object?, string>.New("")
                    .SelectAsync(CancellableIsNullAsync, StringLengthAsync)
                    .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectAsync{TLeftResult, TRightResult}(Func{TLeft, CancellationToken, Task{TLeftResult}}, Func{TRight, Task{TRightResult}}, CancellationToken)"/>
    /// method, cancelling it to make sure cancellation tokens are used properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectAsync_BothAsync_OnlyLeftCancellable_Cancellation()
    {
        await TestAndAssertCanceled(
                (e, ct) => e.SelectAsync(CancellableIsNullAsync, StringLengthAsync, ct),
                Either<object?, string>.NewLeft(null))
            .ConfigureAwait(false);
        Assert.That.HasRight(
            0,
            await TestAndAssertNotCanceled(
                    (e, ct) => e.SelectAsync(CancellableIsNullAsync, StringLengthAsync, ct),
                    Either<object?, string>.New(""))
                .ConfigureAwait(false));
    }
    #endregion

    #region Right Cancellable Only
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectAsync{TLeftResult, TRightResult}(Func{TLeft, Task{TLeftResult}}, Func{TRight, CancellationToken, Task{TRightResult}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectAsync_BothAsync_OnlyRightCancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            true,
            await Either<object?, string>.NewLeft(null)
                    .SelectAsync(IsNullAsync, CancellableStringLengthAsync)
                    .ConfigureAwait(false));
        Assert.That.HasRight(
            0,
            await Either<object?, string>.New("")
                    .SelectAsync(IsNullAsync, CancellableStringLengthAsync)
                    .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectAsync{TLeftResult, TRightResult}(Func{TLeft, Task{TLeftResult}}, Func{TRight, CancellationToken, Task{TRightResult}}, CancellationToken)"/>
    /// method, cancelling it to make sure cancellation tokens are used properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectAsync_BothAsync_OnlyRightCancellable_Cancellation()
    {
        await TestAndAssertCanceled(
                (e, ct) => e.SelectAsync(CancellableIsNullAsync, StringLengthAsync, ct),
                Either<object?, string>.NewLeft(null))
            .ConfigureAwait(false);
        Assert.That.HasRight(
            0,
            await TestAndAssertNotCanceled(
                    (e, ct) => e.SelectAsync(CancellableIsNullAsync, StringLengthAsync, ct),
                    Either<object?, string>.New(""))
                .ConfigureAwait(false));
    }
    #endregion

    #region Both Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectAsync{TLeftResult, TRightResult}(Func{TLeft, CancellationToken, Task{TLeftResult}}, Func{TRight, CancellationToken, Task{TRightResult}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectAsync_BothAsync_BothCancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            true,
            await Either<object?, string>.NewLeft(null)
                    .SelectAsync(CancellableIsNullAsync, CancellableStringLengthAsync)
                    .ConfigureAwait(false));
        Assert.That.HasRight(
            0,
            await Either<object?, string>.New("")
                    .SelectAsync(CancellableIsNullAsync, CancellableStringLengthAsync)
                    .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectAsync{TLeftResult, TRightResult}(Func{TLeft, CancellationToken, Task{TLeftResult}}, Func{TRight, CancellationToken, Task{TRightResult}}, CancellationToken)"/>
    /// method, cancelling it to make sure cancellation tokens are used properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectAsync_BothAsync_BothCancellable_Cancellation()
    {
        await TestAndAssertCanceled(
                (e, ct) => e.SelectAsync(CancellableIsNullAsync, CancellableStringLengthAsync, ct),
                Either<object?, string>.NewLeft(null))
            .ConfigureAwait(false);
        await TestAndAssertCanceled(
                (e, ct) => e.SelectAsync(CancellableIsNullAsync, CancellableStringLengthAsync, ct),
                Either<object?, string>.New(""))
            .ConfigureAwait(false);
    }
    #endregion
    #endregion

    #region Only Right Selector Async
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectAsync{TLeftResult, TRightResult}(Func{TLeft, TLeftResult}, Func{TRight, Task{TRightResult}})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectAsync_RightAsyncOnly_NonCancellable()
    {
        Assert.That.HasLeft(
            true,
            await Either<object?, string>.NewLeft(null).SelectAsync(IsNull, StringLengthAsync).ConfigureAwait(false));
        Assert.That.HasRight(
            0,
            await Either<object?, string>.New("").SelectAsync(IsNull, StringLengthAsync).ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectAsync{TLeftResult, TRightResult}(Func{TLeft, TLeftResult}, Func{TRight, CancellationToken, Task{TRightResult}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectAsync_RightAsyncOnly_Cancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            true,
            await Either<object?, string>.NewLeft(null)
                    .SelectAsync(IsNull, CancellableStringLengthAsync, CancellationToken.None)
                    .ConfigureAwait(false));
        Assert.That.HasRight(
            0,
            await Either<object?, string>.New("")
                    .SelectAsync(IsNull, CancellableStringLengthAsync, CancellationToken.None)
                    .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectAsync{TLeftResult, TRightResult}(Func{TLeft, TLeftResult}, Func{TRight, CancellationToken, Task{TRightResult}}, CancellationToken)"/>
    /// method and cancels it to ensure cancellation is observed properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectAsync_RightAsyncOnly_Cancellable_Cancellation()
    {
        await TestAndAssertCanceled(
                    (e, ct) => e.SelectAsync(IsNull, CancellableStringLengthAsync, ct),
                    Either<object?, string>.New(""))
                .ConfigureAwait(false);

        Assert.That.HasLeft(
            true,
            await TestAndAssertNotCanceled(
                (e, ct) => e.SelectAsync(IsNull, CancellableStringLengthAsync, ct), Either<object?, string>.NewLeft(null))
                .ConfigureAwait(false));
    }
    #endregion
    #endregion

    #region Right
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.SelectRightAsync{TRightResult}(Func{TRight, Task{TRightResult}})"/>
    /// method and related overloads.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectRightAsync_NonCancellable()
    {
        Assert.That.HasRight(
            true,
            await Either<string, object?>.NewRight(null).SelectRightAsync(IsNullAsync).ConfigureAwait(false));
        Assert.That.HasLeft(
            "",
            await Either<string, object?>.New("").SelectRightAsync(IsNullAsync).ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectRightAsync{TRightResult}(Func{TRight, CancellationToken, Task{TRightResult}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectRightAsync_Cancellable_NoCancellation()
    {
        Assert.That.HasRight(
            true,
            await Either<string, object?>.NewRight(null)
                    .SelectRightAsync(CancellableIsNullAsync, CancellationToken.None)
                    .ConfigureAwait(false));
        Assert.That.HasLeft(
            "",
            await Either<string, object?>.New("")
                    .SelectRightAsync(CancellableIsNullAsync, CancellationToken.None)
                    .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectRightAsync{TRightResult}(Func{TRight, CancellationToken, Task{TRightResult}}, CancellationToken)"/>
    /// method when canceled.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectRightAsync_Cancellable_Cancellation()
    {
        await TestAndAssertCanceled(
                (e, ct) => e.SelectRightAsync(CancellableIsNullAsync, ct),
                Either<string, object?>.NewRight(null)).ConfigureAwait(false);
        Assert.That.HasLeft(
            "",
            await TestAndAssertNotCanceled(
                (e, ct) => e.SelectRightAsync(CancellableIsNullAsync, ct), Either<string, object?>.New(""))
                .ConfigureAwait(false));
    }
    #endregion
    #endregion
    #endregion

    #region SelectMany
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectManyLeft{TLeftResult}(Func{TLeft, Either{TLeftResult, TRight}})"/>
    /// method.
    /// </summary>
    [TestMethod]
    public void TestSelectManyLeft()
    {
        // Function is called
        Assert.That.HasLeft(2uL, Either<int, string?>.New(2).SelectManyLeft(SingleManySelectorLeft));
        Assert.That.HasRight("-2", Either<int, string?>.New(-2).SelectManyLeft(SingleManySelectorLeft));
        Assert.That.HasRight(null, Either<int, string?>.New(3).SelectManyLeft(SingleManySelectorLeft));

        // Function is not called
        Assert.That.HasRight("sss", Either<int, string?>.New("sss").SelectManyLeft(SingleManySelectorLeft));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectMany{TLeftResult, TRightResult}(Func{TLeft, Either{TLeftResult, TRightResult}}, Func{TRight, Either{TLeftResult, TRightResult}})"/>
    /// method.
    /// </summary>
    [TestMethod]
    public void TestSelectMany()
    {
        // Left function is called
        Assert.That.HasLeft(
            2uL,
            Either<int, string?>.New(2).SelectMany(BothManySelectorLeft, BothManySelectorRight));
        Assert.That.HasRight(
            float.NegativeInfinity,
            Either<int, string?>.New(-2).SelectMany(BothManySelectorLeft, BothManySelectorRight));
        Assert.That.HasRight(
            float.NaN,
            Either<int, string?>.New(3).SelectMany(BothManySelectorLeft, BothManySelectorRight));

        // Right function is called
        Assert.That.HasRight(
            float.PositiveInfinity,
            Either<int, string?>.New(null).SelectMany(BothManySelectorLeft, BothManySelectorRight));
        Assert.That.HasLeft(
            3uL,
            Either<int, string?>.New("rrr").SelectMany(BothManySelectorLeft, BothManySelectorRight));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectManyRight{TRightResult}(Func{TRight, Either{TLeft, TRightResult}})"/>
    /// method.
    /// </summary>
    [TestMethod]
    public void TestSelectManyRight()
    {
        // Function is called
        Assert.That.HasRight(float.NaN, Either<int, string?>.New(null).SelectManyRight(SingleManySelectorRight));
        Assert.That.HasLeft(0, Either<int, string?>.New("").SelectManyRight(SingleManySelectorRight));

        // Function is not called
        Assert.That.HasLeft(3, Either<int, string?>.New(3).SelectManyRight(SingleManySelectorRight));
    }
    #endregion
    #endregion

    #region Helpers
    #region Testers
    /// <summary>
    /// Runs the task function asynchronously, canceling it before it can complete and ensuring that it throws an
    /// <see cref="OperationCanceledException"/>.
    /// </summary>
    /// <typeparam name="TParameter"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="taskFactory"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    private static async Task<TaskCanceledException> TestAndAssertCanceled<TParameter, TResult>(
        Func<TParameter, CancellationToken, Task<TResult>> taskFactory, TParameter parameter)
    {
        // Start a new cancellation token source that will cancel automatically after a wait period
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(AsyncCancellationTestWaitPeriod);

        // Ensure that the task is canceled before it is complete
        return await Assert.ThrowsExceptionAsync<TaskCanceledException>(() => taskFactory(parameter, cts.Token))
                            .ConfigureAwait(false);
    }

    /// <summary>
    /// Runs the task function asynchronously, triggering a cancelation that should not cause an exception.
    /// </summary>
    /// <typeparam name="TParameter"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    /// <param name="taskFactory"></param>
    /// <param name="parameter"></param>
    /// <returns></returns>
    private static async Task<TResult> TestAndAssertNotCanceled<TParameter, TResult>(
        Func<TParameter, CancellationToken, Task<TResult>> taskFactory, TParameter parameter)
    {
        // Start a new cancellation token source that will cancel automatically after a wait period
        using var cts = new CancellationTokenSource();
        cts.CancelAfter(AsyncCancellationTestWaitPeriod);

        // Ensure that the task was not canceled
        return await taskFactory(parameter, cts.Token).ConfigureAwait(false);
    }
    #endregion

    #region Selectors
    #region Select
    /// <summary>
    /// Determines if the <see cref="object"/> passed in is <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// This predicate is used internally as a selector to test the methods.
    /// </remarks>
    /// <param name="o"></param>
    /// <returns></returns>
    private static bool IsNull(object? o) => o is null;

    /// <summary>
    /// Asynchronously determines if the <see cref="object"/> passed in is <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// This predicate is used internally as a selector to test the methods.
    /// </remarks>
    /// <param name="o"></param>
    /// <returns></returns>
    private static Task<bool> IsNullAsync(object? o) => Task.FromResult(o is null);

    /// <summary>
    /// Asynchronously determines if the <see cref="object"/> passed in is <see langword="null"/>.
    /// </summary>
    /// <remarks>
    /// This predicate is used internally as a selector to test the methods.
    /// </remarks>
    /// <param name="o"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static Task<bool> CancellableIsNullAsync(object? o, CancellationToken cancellationToken)
        => CallCancellable(o is null, cancellationToken);

    /// <summary>
    /// Gets the length of the string passed in.
    /// </summary>
    /// <remarks>
    /// This selector is used internally to test the methods.
    /// </remarks>
    /// <param name="s"></param>
    /// <returns></returns>
    private static int StringLength(string s) => s.Length;

    /// <summary>
    /// Asynchronously gets the length of the string passed in.
    /// </summary>
    /// <remarks>
    /// This selector is used internally to test the methods.
    /// </remarks>
    /// <param name="s"></param>
    /// <returns></returns>
    private static Task<int> StringLengthAsync(string s) => Task.FromResult(s.Length);

    /// <summary>
    /// Asynchronously gets the length of the string passed in.
    /// </summary>
    /// <remarks>
    /// This selector is used internally to test the methods.
    /// </remarks>
    /// <param name="s"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static Task<int> CancellableStringLengthAsync(string s, CancellationToken cancellationToken)
        => CallCancellable(s.Length, cancellationToken);
    #endregion

    #region SelectMany
    /// <summary>
    /// A single-sided many selector for the left side of an <see cref="Either{TLeft, TRight}"/> instance with an
    /// <see cref="int"/> on the left side or a <see cref="string"/> on the right side.
    /// </summary>
    /// <param name="i"></param>
    /// <returns>
    /// An <see cref="Either{TLeft, TRight}"/> with the value of (non-negative) <paramref name="i"/> on the left side
    /// and the string representation of (negative) <paramref name="i"/> on the right side if <paramref name="i"/> is
    /// even, otherwise <see langword="null"/> on the right side.
    /// </returns>
    private static Either<ulong, string?> SingleManySelectorLeft(int i)
    {
        if (i % 2 == 0) return i >= 0 ? (ulong)i : i.ToString();
        else return null;
    }

    /// <summary>
    /// A both-sided many selector for the left side of an <see cref="Either{TLeft, TRight}"/> instance with an
    /// <see cref="int"/> on the left side or a <see cref="string"/> on the right side.
    /// </summary>
    /// <param name="i"></param>
    /// <returns>
    /// An <see cref="Either{TLeft, TRight}"/> with the (even, non-negative) value of <paramref name="i"/> on the left side
    /// or <see cref="float.NegativeInfinity"/> on the right side if <paramref name="i"/> is an even negative value,
    /// otherwise <see cref="float.NaN"/>.
    /// </returns>
    private static Either<ulong, float> BothManySelectorLeft(int i)
    {
        if (i % 2 == 0) return i >= 0 ? Either<ulong, float>.NewLeft((ulong)i) : float.NegativeInfinity;
        else return float.NaN;
    }

    /// <summary>
    /// A both-sided many selector for the right side of an <see cref="Either{TLeft, TRight}"/> instance with an
    /// <see cref="int"/> on the left side or a <see cref="string"/> on the right side.
    /// </summary>
    /// <param name="i"></param>
    /// <returns>
    /// An <see cref="Either{TLeft, TRight}"/> with <see cref="float.PositiveInfinity"/> on the right side if
    /// <paramref name="s"/> is <see langword="null"/>, otherwise the length of <paramref name="s"/> on the left side.
    /// </returns>
    static Either<ulong, float> BothManySelectorRight(string? s)
    {
        if (s is null) return float.PositiveInfinity;
        else return (ulong)s.Length;
    }

    /// <summary>
    /// A single-sided many selector for the right side of an <see cref="Either{TLeft, TRight}"/> instance with an
    /// <see cref="int"/> on the left side or a <see cref="string"/> on the right side.
    /// </summary>
    /// <param name="s"></param>
    /// <returns>
    /// An <see cref="Either{TLeft, TRight}"/> with <see cref="float.NaN"/> on the right side if <paramref name="s"/>
    /// is <see langword="null"/>, or the length of <paramref name="s"/> on the left side otherwise.
    /// </returns>
    private static Either<int, float> SingleManySelectorRight(string? s)
    {
        if (s is null) return float.NaN;
        else return s.Length;
    }
    #endregion
    #endregion

    #region Auxiliary
    /// <summary>
    /// Delays asynchronously before returning the specified value.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="result"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static async Task<T> CallCancellable<T>(T result, CancellationToken cancellationToken)
    {
        await Task.Delay(AsyncCancellableDelay).ConfigureAwait(false);
        return result;
    }
    #endregion
    #endregion
}
