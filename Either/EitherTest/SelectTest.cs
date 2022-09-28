using System;
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
        Assert.That.HasLeft(true, Either<object?, string>.NewLeft(null).SelectLeft(IsNull.Invoke));
        Assert.That.HasRight("", Either<object?, string>.New("").SelectLeft(IsNull.Invoke));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.Select{TLeftResult, TRightResult}(Func{TLeft, TLeftResult}, Func{TRight, TRightResult})"/>
    /// method.
    /// </summary>
    [TestMethod]
    public void TestSelect()
    {
        Assert.That.HasLeft(false, Either<object?, string>.New(4).Select(IsNull.Invoke, StringLength.Invoke));
        Assert.That.HasRight(1, Either<object?, string>.New("4").Select(IsNull.Invoke, StringLength.Invoke));
    }

    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.SelectRight{TRightResult}(Func{TRight, TRightResult})"/> method.
    /// </summary>
    [TestMethod]
    public void TestSelectRight()
    {
        Assert.That.HasRight(0, Either<object?, string>.New("").SelectRight(StringLength.Invoke));
        Assert.That.HasLeft(null, Either<object?, string>.NewLeft(null).SelectRight(StringLength.Invoke));
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
            await Either<object?, string>.NewLeft(null).SelectLeftAsync(IsNull.InvokeAsync).ConfigureAwait(false));
        Assert.That.HasRight(
            "",
            await Either<object?, string>.New("").SelectLeftAsync(IsNull.InvokeAsync).ConfigureAwait(false));
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
                    .SelectLeftAsync(IsNull.InvokeCancellableAsync, CancellationToken.None)
                    .ConfigureAwait(false));
        Assert.That.HasRight(
            "",
            await Either<object?, string>.New("")
                    .SelectLeftAsync(IsNull.InvokeCancellableAsync, CancellationToken.None)
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
                    (e, ct) => e.SelectLeftAsync(IsNull.InvokeCancellableAsync, ct),
                    Either<object?, string>.NewLeft(null))
                .ConfigureAwait(false);
        Assert.That.HasRight(
            "",
            await TestAndAssertNotCanceled(
                (e, ct) => e.SelectLeftAsync(IsNull.InvokeCancellableAsync, ct), Either<object?, string>.New(""))
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
            await Either<object?, string>.NewLeft(null)
                    .SelectAsync(IsNull.InvokeAsync, StringLength.Invoke)
                .ConfigureAwait(false));
        Assert.That.HasRight(
            0,
            await Either<object?, string>.New("")
                    .SelectAsync(IsNull.InvokeAsync, StringLength.Invoke)
                .ConfigureAwait(false));
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
                    .SelectAsync(IsNull.InvokeCancellableAsync, StringLength.Invoke, CancellationToken.None)
                    .ConfigureAwait(false));
        Assert.That.HasRight(
            0,
            await Either<object?, string>.New("")
                    .SelectAsync(IsNull.InvokeCancellableAsync, StringLength.Invoke, CancellationToken.None)
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
                    (e, ct) => e.SelectAsync(IsNull.InvokeCancellableAsync, StringLength.Invoke, ct),
                    Either<object?, string>.NewLeft(null))
                .ConfigureAwait(false);
        Assert.That.HasRight(
            0,
            await TestAndAssertNotCanceled(
                    (e, ct) => e.SelectAsync(IsNull.InvokeCancellableAsync, StringLength.Invoke, ct),
                    Either<object?, string>.New(""))
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
            await Either<object?, string>.NewLeft(null)
                    .SelectAsync(IsNull.InvokeAsync, StringLength.InvokeAsync)
                .ConfigureAwait(false));
        Assert.That.HasRight(
            0,
            await Either<object?, string>.New("")
                    .SelectAsync(IsNull.InvokeAsync, StringLength.InvokeAsync)
                .ConfigureAwait(false));
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
                    .SelectAsync(IsNull.InvokeCancellableAsync, StringLength.InvokeAsync)
                    .ConfigureAwait(false));
        Assert.That.HasRight(
            0,
            await Either<object?, string>.New("")
                    .SelectAsync(IsNull.InvokeCancellableAsync, StringLength.InvokeAsync)
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
                (e, ct) => e.SelectAsync(IsNull.InvokeCancellableAsync, StringLength.InvokeAsync, ct),
                Either<object?, string>.NewLeft(null))
            .ConfigureAwait(false);
        Assert.That.HasRight(
            0,
            await TestAndAssertNotCanceled(
                (e, ct) => e.SelectAsync(IsNull.InvokeCancellableAsync, StringLength.InvokeAsync, ct),
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
                    .SelectAsync(IsNull.InvokeAsync, StringLength.InvokeCancellableAsync)
                    .ConfigureAwait(false));
        Assert.That.HasRight(
            0,
            await Either<object?, string>.New("")
                    .SelectAsync(IsNull.InvokeAsync, StringLength.InvokeCancellableAsync)
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
                (e, ct) => e.SelectAsync(IsNull.InvokeAsync, StringLength.InvokeCancellableAsync, ct),
                Either<object?, string>.New(""))
            .ConfigureAwait(false);
        Assert.That.HasLeft(
            true,
            await TestAndAssertNotCanceled(
                (e, ct) => e.SelectAsync(IsNull.InvokeAsync, StringLength.InvokeCancellableAsync, ct),
                    Either<object?, string>.NewLeft(null))
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
                    .SelectAsync(IsNull.InvokeCancellableAsync, StringLength.InvokeCancellableAsync)
                    .ConfigureAwait(false));
        Assert.That.HasRight(
            0,
            await Either<object?, string>.New("")
                    .SelectAsync(IsNull.InvokeCancellableAsync, StringLength.InvokeCancellableAsync)
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
                (e, ct) => e.SelectAsync(IsNull.InvokeCancellableAsync, StringLength.InvokeCancellableAsync, ct),
                Either<object?, string>.NewLeft(null))
            .ConfigureAwait(false);
        await TestAndAssertCanceled(
                (e, ct) => e.SelectAsync(IsNull.InvokeCancellableAsync, StringLength.InvokeCancellableAsync, ct),
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
            await Either<object?, string>.NewLeft(null)
                .SelectAsync(IsNull.Invoke, StringLength.InvokeAsync)
                .ConfigureAwait(false));
        Assert.That.HasRight(
            0,
            await Either<object?, string>.New("")
                .SelectAsync(IsNull.Invoke, StringLength.InvokeAsync)
                .ConfigureAwait(false));
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
                    .SelectAsync(IsNull.Invoke, StringLength.InvokeCancellableAsync, CancellationToken.None)
                    .ConfigureAwait(false));
        Assert.That.HasRight(
            0,
            await Either<object?, string>.New("")
                    .SelectAsync(IsNull.Invoke, StringLength.InvokeCancellableAsync, CancellationToken.None)
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
                    (e, ct) => e.SelectAsync(IsNull.Invoke, StringLength.InvokeCancellableAsync, ct),
                    Either<object?, string>.New(""))
                .ConfigureAwait(false);

        Assert.That.HasLeft(
            true,
            await TestAndAssertNotCanceled(
                    (e, ct) => e.SelectAsync(IsNull.Invoke, StringLength.InvokeCancellableAsync, ct),
                    Either<object?, string>.NewLeft(null))
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
            await Either<string, object?>.NewRight(null).SelectRightAsync(IsNull.InvokeAsync).ConfigureAwait(false));
        Assert.That.HasLeft(
            "",
            await Either<string, object?>.New("").SelectRightAsync(IsNull.InvokeAsync).ConfigureAwait(false));
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
                    .SelectRightAsync(IsNull.InvokeCancellableAsync, CancellationToken.None)
                    .ConfigureAwait(false));
        Assert.That.HasLeft(
            "",
            await Either<string, object?>.New("")
                    .SelectRightAsync(IsNull.InvokeCancellableAsync, CancellationToken.None)
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
                (e, ct) => e.SelectRightAsync(IsNull.InvokeCancellableAsync, ct),
                Either<string, object?>.NewRight(null)).ConfigureAwait(false);
        Assert.That.HasLeft(
            "",
            await TestAndAssertNotCanceled(
                (e, ct) => e.SelectRightAsync(IsNull.InvokeCancellableAsync, ct), Either<string, object?>.New(""))
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
        Assert.That.HasLeft(2uL, Either<int, string?>.New(2).SelectManyLeft(SingleManySelectorLeft.Invoke));
        Assert.That.HasRight("-2", Either<int, string?>.New(-2).SelectManyLeft(SingleManySelectorLeft.Invoke));
        Assert.That.HasRight(null, Either<int, string?>.New(3).SelectManyLeft(SingleManySelectorLeft.Invoke));

        // Function is not called
        Assert.That.HasRight("sss", Either<int, string?>.New("sss").SelectManyLeft(SingleManySelectorLeft.Invoke));
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
            Either<int, string?>.New(2).SelectMany(BothManySelectorLeft.Invoke, BothManySelectorRight));
        Assert.That.HasRight(
            float.NegativeInfinity,
            Either<int, string?>.New(-2).SelectMany(BothManySelectorLeft.Invoke, BothManySelectorRight));
        Assert.That.HasRight(
            float.NaN,
            Either<int, string?>.New(3).SelectMany(BothManySelectorLeft.Invoke, BothManySelectorRight));

        // Right function is called
        Assert.That.HasRight(
            float.PositiveInfinity,
            Either<int, string?>.New(null).SelectMany(BothManySelectorLeft.Invoke, BothManySelectorRight));
        Assert.That.HasLeft(
            3uL,
            Either<int, string?>.New("rrr").SelectMany(BothManySelectorLeft.Invoke, BothManySelectorRight));
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
        Assert.That.HasRight(
            float.NaN, Either<int, string?>.New(null).SelectManyRight(SingleManySelectorRight.Invoke));
        Assert.That.HasLeft(0, Either<int, string?>.New("").SelectManyRight(SingleManySelectorRight.Invoke));

        // Function is not called
        Assert.That.HasLeft(3, Either<int, string?>.New(3).SelectManyRight(SingleManySelectorRight.Invoke));
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
    /// <returns></returns>
    private static readonly FunctionOptions<object?, bool> IsNull = new(o => o is null);

    /// <summary>
    /// Gets the length of the string passed in.
    /// </summary>
    /// <remarks>
    /// This selector is used internally to test the methods.
    /// </remarks>
    /// <returns></returns>
    private static readonly FunctionOptions<string, int> StringLength = new(s => s.Length);
    #endregion

    #region SelectMany
    /// <summary>
    /// A single-sided many selector for the left side of an <see cref="Either{TLeft, TRight}"/> instance with an
    /// <see cref="int"/> on the left side or a <see cref="string"/> on the right side.
    /// </summary>
    /// <returns>
    /// An <see cref="Either{TLeft, TRight}"/> with the (non-negative) value of the parameter on the left side
    /// and the string representation of the (negative) parameter on the right side if the parameter is even,
    /// otherwise <see langword="null"/> on the right side.
    /// </returns>
    private static readonly FunctionOptions<int, Either<ulong, string?>> SingleManySelectorLeft = new(i =>
    {
        if (i % 2 == 0) return i >= 0 ? (ulong)i : i.ToString();
        else return null;
    });

    /// <summary>
    /// A both-sided many selector for the left side of an <see cref="Either{TLeft, TRight}"/> instance with an
    /// <see cref="int"/> on the left side or a <see cref="string"/> on the right side.
    /// </summary>
    /// <returns>
    /// An <see cref="Either{TLeft, TRight}"/> with the (even, non-negative) value of the parameter on the left side
    /// or <see cref="float.NegativeInfinity"/> on the right side if the parameter is an even negative value,
    /// otherwise <see cref="float.NaN"/>.
    /// </returns>
    private static readonly FunctionOptions<int, Either<ulong, float>> BothManySelectorLeft = new(i =>
    {
        if (i % 2 == 0) return i >= 0 ? Either<ulong, float>.NewLeft((ulong)i) : float.NegativeInfinity;
        else return float.NaN;
    });

    /// <summary>
    /// A both-sided many selector for the right side of an <see cref="Either{TLeft, TRight}"/> instance with an
    /// <see cref="int"/> on the left side or a <see cref="string"/> on the right side.
    /// </summary>
    /// <returns>
    /// An <see cref="Either{TLeft, TRight}"/> with <see cref="float.PositiveInfinity"/> on the right side if the
    /// parameter is <see langword="null"/>, otherwise the length of the parameter on the left side.
    /// </returns>
    private static readonly FunctionOptions<string?, Either<ulong, float>> BothManySelectorRight = new(s =>
    {
        if (s is null) return float.PositiveInfinity;
        else return (ulong)s.Length;
    });

    /// <summary>
    /// A single-sided many selector for the right side of an <see cref="Either{TLeft, TRight}"/> instance with an
    /// <see cref="int"/> on the left side or a <see cref="string"/> on the right side.
    /// </summary>
    /// <returns>
    /// An <see cref="Either{TLeft, TRight}"/> with <see cref="float.NaN"/> on the right side if the parameter
    /// is <see langword="null"/>, or the length of the parameter on the left side otherwise.
    /// </returns>
    private static readonly FunctionOptions<string?, Either<int, float>> SingleManySelectorRight = new(s =>
    {
        if (s is null) return float.NaN;
        else return s.Length;
    });
    #endregion
    #endregion

    #region Auxiliary
    /// <summary>
    /// Stores synchronous and asynchronous versions of a function needed for testing of the select methods.
    /// </summary>
    public sealed class FunctionOptions<TArg, TResult>
    {
        /// <summary>
        /// The function for which options are being constructed.
        /// </summary>
        private Func<TArg, TResult> Delegate { get; }

        /// <summary>
        /// Constructs a new instance of the <see cref="FunctionOptions{TArg, TResult}"/> class set up to allow calls
        /// to the function passed in.
        /// </summary>
        /// <param name="Function"></param>
        public FunctionOptions(Func<TArg, TResult> Function)
        {
            Delegate = Function;
        }

        /// <summary>
        /// Calls the method synchronously.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public TResult Invoke(TArg arg) => Delegate(arg);

        /// <summary>
        /// Calls the method asynchronously.
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public Task<TResult> InvokeAsync(TArg arg) => Task.FromResult(Delegate(arg));

        /// <summary>
        /// Calls the method asynchronously with cancellation.
        /// </summary>
        /// <remarks>
        /// This will delay the task by <see cref="AsyncCancellableDelay"/> before returning the result.
        /// </remarks>
        /// <param name="arg"></param>
        /// <returns></returns>
        public async Task<TResult> InvokeCancellableAsync(TArg arg, CancellationToken cancellationToken)
        {
            await Task.Delay(AsyncCancellableDelay, cancellationToken).ConfigureAwait(false);
            return Delegate(arg);
        }

        public static implicit operator Func<TArg, TResult>(FunctionOptions<TArg, TResult> opts) => opts.Invoke;
        public static implicit operator Func<TArg, Task<TResult>>(FunctionOptions<TArg, TResult> opts)
            => opts.InvokeAsync;
        public static implicit operator Func<TArg, CancellationToken, Task<TResult>>(
            FunctionOptions<TArg, TResult> opts)
            => opts.InvokeCancellableAsync;
    }
    #endregion
    #endregion
}
