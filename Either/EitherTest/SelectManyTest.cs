using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RemTest.Core.Utilities.Monads;

/// <summary>
/// Tests of the <see cref="Either{TLeft, TRight}.SelectMany"/> function and related overloads.
/// </summary>
[TestClass]
public class SelectManyTest
{
    #region Tests
    #region Synchronous
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectManyLeft{TLeftResult}(Func{TLeft, Either{TLeftResult, TRight}})"/>
    /// method.
    /// </summary>
    [TestMethod]
    public void TestSelectManyLeft()
    {
        // Function is called
        Assert.That.HasLeft(2uL, Either<int, string?>.New(2).SelectManyLeft(SingleSelectorLeft.Invoke));
        Assert.That.HasRight("-2", Either<int, string?>.New(-2).SelectManyLeft(SingleSelectorLeft.Invoke));
        Assert.That.HasRight(null, Either<int, string?>.New(3).SelectManyLeft(SingleSelectorLeft.Invoke));

        // Function is not called
        Assert.That.HasRight("sss", Either<int, string?>.New("sss").SelectManyLeft(SingleSelectorLeft.Invoke));
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
            Either<int, string?>.New(2).SelectMany(BothSelectorLeft.Invoke, BothSelectorRight));
        Assert.That.HasRight(
            float.NegativeInfinity,
            Either<int, string?>.New(-2).SelectMany(BothSelectorLeft.Invoke, BothSelectorRight));
        Assert.That.HasRight(
            float.NaN,
            Either<int, string?>.New(3).SelectMany(BothSelectorLeft.Invoke, BothSelectorRight));

        // Right function is called
        Assert.That.HasRight(
            float.PositiveInfinity,
            Either<int, string?>.New(null).SelectMany(BothSelectorLeft.Invoke, BothSelectorRight));
        Assert.That.HasLeft(
            3uL,
            Either<int, string?>.New("rrr").SelectMany(BothSelectorLeft.Invoke, BothSelectorRight));
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
            float.NaN, Either<int, string?>.New(null).SelectManyRight(SingleSelectorRight.Invoke));
        Assert.That.HasLeft(0, Either<int, string?>.New("").SelectManyRight(SingleSelectorRight.Invoke));

        // Function is not called
        Assert.That.HasLeft(3, Either<int, string?>.New(3).SelectManyRight(SingleSelectorRight.Invoke));
    }
    #endregion

    #region Asynchronous
    #region Left
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectManyLeftAsync{TLeftResult}(Func{TLeft, Task{Either{TLeftResult, TRight}}})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectManyLeftAsync_NonCancellable()
    {
        Assert.That.HasLeft(
            2uL,
            await Either<int, string?>.New(2)
                    .SelectManyLeftAsync(SingleSelectorLeft.InvokeAsync)
                    .ConfigureAwait(false));

        Assert.That.HasRight(
            "eee",
            await Either<int, string?>.New("eee")
                    .SelectManyLeftAsync(SingleSelectorLeft.InvokeAsync)
                    .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectManyLeftAsync{TLeftResult}(Func{TLeft, CancellationToken, Task{Either{TLeftResult, TRight}}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectManyLeftAsync_Cancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            2uL,
            await Either<int, string?>.New(2)
                    .SelectManyLeftAsync(SingleSelectorLeft.InvokeCancellableAsync)
                    .ConfigureAwait(false));

        Assert.That.HasRight(
            "eee",
            await Either<int, string?>.New("eee")
                    .SelectManyLeftAsync(SingleSelectorLeft.InvokeCancellableAsync)
                    .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectManyLeftAsync{TLeftResult}(Func{TLeft, CancellationToken, Task{Either{TLeftResult, TRight}}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectManyLeftAsync_Cancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.SelectManyLeftAsync(SingleSelectorLeft.InvokeCancellableAsync, ct),
                Either<int, string?>.New(2))
            .ConfigureAwait(false);

        Assert.That.HasRight(
            "eee",
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.SelectManyLeftAsync(SingleSelectorLeft.InvokeCancellableAsync, ct),
                    Either<int, string?>.New("eee"))
                .ConfigureAwait(false));
    }
    #endregion

    #region Both Sides
    #region Only Left Selector Async
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectManyAsync{TLeftResult, TRightResult}(Func{TLeft, Task{Either{TLeftResult, TRightResult}}}, Func{TRight, Either{TLeftResult, TRightResult}})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectManyAsync_LeftOnlyAsync_NonCancellable()
    {
        Assert.That.HasRight(
            float.NaN,
            await Either<int, string?>.New(3).SelectManyAsync(BothSelectorLeft.InvokeAsync, BothSelectorRight.Invoke)
                    .ConfigureAwait(false));

        Assert.That.HasLeft(
            1uL,
            await Either<int, string?>.New(" ").SelectManyAsync(BothSelectorLeft.InvokeAsync, BothSelectorRight.Invoke)
                    .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectManyAsync{TLeftResult, TRightResult}(Func{TLeft, CancellationToken, Task{Either{TLeftResult, TRightResult}}}, Func{TRight, Either{TLeftResult, TRightResult}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectManyAsync_LeftOnlyAsync_Cancellable_NoCancellation()
    {
        Assert.That.HasRight(
            float.NaN,
            await Either<int, string?>.New(3)
                    .SelectManyAsync(BothSelectorLeft.InvokeCancellableAsync, BothSelectorRight.Invoke)
                    .ConfigureAwait(false));

        Assert.That.HasLeft(
            1uL,
            await Either<int, string?>.New(" ")
                    .SelectManyAsync(BothSelectorLeft.InvokeCancellableAsync, BothSelectorRight.Invoke)
                    .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectManyAsync{TLeftResult, TRightResult}(Func{TLeft, CancellationToken, Task{Either{TLeftResult, TRightResult}}}, Func{TRight, Either{TLeftResult, TRightResult}}, CancellationToken)"/>
    /// method, cancelling it to ensure cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectManyAsync_LeftOnlyAsync_Cancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                    (e, ct) => e.SelectManyAsync(BothSelectorLeft.InvokeCancellableAsync, BothSelectorRight.Invoke, ct),
                    Either<int, string?>.New(3))
                .ConfigureAwait(false);

        Assert.That.HasLeft(
            1uL,
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.SelectManyAsync(BothSelectorLeft.InvokeCancellableAsync, BothSelectorRight.Invoke, ct),
                    Either<int, string?>.New(" "))
                .ConfigureAwait(false));
    }
    #endregion

    #region Only Right Selector Async
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectManyAsync{TLeftResult, TRightResult}(Func{TLeft, Either{TLeftResult, TRightResult}}, Func{TRight, Task{Either{TLeftResult, TRightResult}}})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectManyAsync_RightOnlyAsync_NonCancellable()
    {
        Assert.That.HasRight(
            float.NaN,
            await Either<int, string?>.New(3).SelectManyAsync(BothSelectorLeft.Invoke, BothSelectorRight.InvokeAsync)
                    .ConfigureAwait(false));

        Assert.That.HasLeft(
            1uL,
            await Either<int, string?>.New(" ").SelectManyAsync(BothSelectorLeft.Invoke, BothSelectorRight.InvokeAsync)
                    .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectManyAsync{TLeftResult, TRightResult}(Func{TLeft, Either{TLeftResult, TRightResult}}, Func{TRight, CancellationToken, Task{Either{TLeftResult, TRightResult}}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectManyAsync_RightOnlyAsync_Cancellable_NoCancellation()
    {
        Assert.That.HasRight(
            float.NaN,
            await Either<int, string?>.New(3)
                    .SelectManyAsync(BothSelectorLeft.Invoke, BothSelectorRight.InvokeCancellableAsync)
                    .ConfigureAwait(false));

        Assert.That.HasLeft(
            1uL,
            await Either<int, string?>.New(" ")
                    .SelectManyAsync(BothSelectorLeft.Invoke, BothSelectorRight.InvokeCancellableAsync)
                    .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectManyAsync{TLeftResult, TRightResult}(Func{TLeft, Either{TLeftResult, TRightResult}}, Func{TRight, CancellationToken, Task{Either{TLeftResult, TRightResult}}}, CancellationToken)"/>
    /// method, cancelling it to ensure cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectManyAsync_RightOnlyAsync_Cancellable_Cancellation()
    {
        Assert.That.HasRight(
            float.NaN,
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.SelectManyAsync(BothSelectorLeft.Invoke, BothSelectorRight.InvokeCancellableAsync, ct),
                    Either<int, string?>.New(3))
                .ConfigureAwait(false));

        await Assert.That.IsCanceledAsync(
                    (e, ct) => e.SelectManyAsync(BothSelectorLeft.Invoke, BothSelectorRight.InvokeCancellableAsync, ct),
                    Either<int, string?>.New(" "))
                .ConfigureAwait(false);
    }
    #endregion

    #region Both Selectors Async
    #region Non-Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectManyAsync{TLeftResult, TRightResult}(Func{TLeft, Task{Either{TLeftResult, TRightResult}}}, Func{TRight, Task{Either{TLeftResult, TRightResult}}})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectManyAsync_BothAsync_NonCancellable()
    {
        Assert.That.HasRight(
            float.NaN,
            await Either<int, string?>.New(3)
                    .SelectManyAsync(BothSelectorLeft.InvokeAsync, BothSelectorRight.InvokeAsync)
                    .ConfigureAwait(false));

        Assert.That.HasLeft(
            1uL,
            await Either<int, string?>.New(" ")
                    .SelectManyAsync(BothSelectorLeft.InvokeAsync, BothSelectorRight.InvokeAsync)
                    .ConfigureAwait(false));
    }
    #endregion

    #region Left Only Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectManyAsync{TLeftResult, TRightResult}(Func{TLeft, CancellationToken, Task{Either{TLeftResult, TRightResult}}}, Func{TRight, Task{Either{TLeftResult, TRightResult}}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectManyAsync_BothAsync_LeftOnlyCancellable_NoCancellation()
    {
        Assert.That.HasRight(
            float.NaN,
            await Either<int, string?>.New(3)
                    .SelectManyAsync(BothSelectorLeft.InvokeCancellableAsync, BothSelectorRight.InvokeAsync)
                    .ConfigureAwait(false));

        Assert.That.HasLeft(
            1uL,
            await Either<int, string?>.New(" ")
                    .SelectManyAsync(BothSelectorLeft.InvokeCancellableAsync, BothSelectorRight.InvokeAsync)
                    .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectManyAsync{TLeftResult, TRightResult}(Func{TLeft, CancellationToken, Task{Either{TLeftResult, TRightResult}}}, Func{TRight, Task{Either{TLeftResult, TRightResult}}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectManyAsync_BothAsync_LeftOnlyCancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                    (e, ct) => e.SelectManyAsync(
                                    BothSelectorLeft.InvokeCancellableAsync, BothSelectorRight.InvokeAsync, ct),
                    Either<int, string?>.New(3))
                .ConfigureAwait(false);

        Assert.That.HasLeft(
            1uL,
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.SelectManyAsync(
                                    BothSelectorLeft.InvokeCancellableAsync, BothSelectorRight.InvokeAsync, ct),
                    Either<int, string?>.New(" "))
                .ConfigureAwait(false));
    }
    #endregion

    #region Right Only Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectManyAsync{TLeftResult, TRightResult}(Func{TLeft, Task{Either{TLeftResult, TRightResult}}}, Func{TRight, CancellationToken, Task{Either{TLeftResult, TRightResult}}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectManyAsync_BothAsync_RightOnlyCancellable_NoCancellation()
    {
        Assert.That.HasRight(
            float.NaN,
            await Either<int, string?>.New(3)
                    .SelectManyAsync(BothSelectorLeft.InvokeAsync, BothSelectorRight.InvokeCancellableAsync)
                    .ConfigureAwait(false));

        Assert.That.HasLeft(
            1uL,
            await Either<int, string?>.New(" ")
                    .SelectManyAsync(BothSelectorLeft.InvokeAsync, BothSelectorRight.InvokeCancellableAsync)
                    .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectManyAsync{TLeftResult, TRightResult}(Func{TLeft, Task{Either{TLeftResult, TRightResult}}}, Func{TRight, CancellationToken, Task{Either{TLeftResult, TRightResult}}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectManyAsync_BothAsync_RightOnlyCancellable_Cancellation()
    {
        Assert.That.HasRight(
            float.NaN,
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.SelectManyAsync(
                                    BothSelectorLeft.InvokeAsync, BothSelectorRight.InvokeCancellableAsync, ct),
                    Either<int, string?>.New(3))
                .ConfigureAwait(false));

        await Assert.That.IsCanceledAsync(
                    (e, ct) => e.SelectManyAsync(
                                    BothSelectorLeft.InvokeAsync, BothSelectorRight.InvokeCancellableAsync, ct),
                    Either<int, string?>.New(" "))
                .ConfigureAwait(false);
    }
    #endregion

    #region Both Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectManyAsync{TLeftResult, TRightResult}(Func{TLeft, CancellationToken, Task{Either{TLeftResult, TRightResult}}}, Func{TRight, CancellationToken, Task{Either{TLeftResult, TRightResult}}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectManyAsync_BothAsync_BothCancellable_NoCancellation()
    {
        Assert.That.HasRight(
            float.NaN,
            await Either<int, string?>.New(3)
                    .SelectManyAsync(BothSelectorLeft.InvokeCancellableAsync, BothSelectorRight.InvokeCancellableAsync)
                    .ConfigureAwait(false));

        Assert.That.HasLeft(
            1uL,
            await Either<int, string?>.New(" ")
                    .SelectManyAsync(BothSelectorLeft.InvokeCancellableAsync, BothSelectorRight.InvokeCancellableAsync)
                    .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectManyAsync{TLeftResult, TRightResult}(Func{TLeft, CancellationToken, Task{Either{TLeftResult, TRightResult}}}, Func{TRight, CancellationToken, Task{Either{TLeftResult, TRightResult}}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectManyAsync_BothAsync_BothCancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                    (e, ct) => e.SelectManyAsync(
                                    BothSelectorLeft.InvokeCancellableAsync, BothSelectorRight.InvokeCancellableAsync,
                                    ct),
                    Either<int, string?>.New(3))
                .ConfigureAwait(false);

        await Assert.That.IsCanceledAsync(
                    (e, ct) => e.SelectManyAsync(
                                    BothSelectorLeft.InvokeCancellableAsync, BothSelectorRight.InvokeCancellableAsync,
                                    ct),
                    Either<int, string?>.New(" "))
                .ConfigureAwait(false);
    }
    #endregion
    #endregion
    #endregion

    #region Right
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectManyRightAsync{TRightResult}(Func{TRight, Task{Either{TLeft, TRightResult}}})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectManyRightAsync_NonCancellable()
    {
        Assert.That.HasLeft(
            2,
            await Either<int, string?>.New(2)
                    .SelectManyRightAsync(SingleSelectorRight.InvokeAsync)
                    .ConfigureAwait(false));

        Assert.That.HasLeft(
            3,
            await Either<int, string?>.New("eee")
                    .SelectManyRightAsync(SingleSelectorRight.InvokeAsync)
                    .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectManyRightAsync{TRightResult}(Func{TRight, CancellationToken, Task{Either{TLeft, TRightResult}}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectManyRightAsync_Cancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            2,
            await Either<int, string?>.New(2)
                    .SelectManyRightAsync(SingleSelectorRight.InvokeCancellableAsync)
                    .ConfigureAwait(false));

        Assert.That.HasLeft(
            3,
            await Either<int, string?>.New("eee")
                    .SelectManyRightAsync(SingleSelectorRight.InvokeCancellableAsync)
                    .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectManyRightAsync{TRightResult}(Func{TRight, CancellationToken, Task{Either{TLeft, TRightResult}}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectManyRightAsync_Cancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.SelectManyRightAsync(SingleSelectorRight.InvokeCancellableAsync, ct),
                Either<int, string?>.New(" "))
            .ConfigureAwait(false);

        Assert.That.HasLeft(
            2,
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.SelectManyRightAsync(SingleSelectorRight.InvokeCancellableAsync, ct),
                    Either<int, string?>.New(2))
                .ConfigureAwait(false));
    }
    #endregion
    #endregion
    #endregion

    #region Helper Selectors
    /// <summary>
    /// A single-sided many selector for the left side of an <see cref="Either{TLeft, TRight}"/> instance with an
    /// <see cref="int"/> on the left side or a <see cref="string"/> on the right side.
    /// </summary>
    /// <returns>
    /// An <see cref="Either{TLeft, TRight}"/> with the (non-negative) value of the parameter on the left side
    /// and the string representation of the (negative) parameter on the right side if the parameter is even,
    /// otherwise <see langword="null"/> on the right side.
    /// </returns>
    private static readonly FunctionOptions<int, Either<ulong, string?>> SingleSelectorLeft = new(i =>
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
    private static readonly FunctionOptions<int, Either<ulong, float>> BothSelectorLeft = new(i =>
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
    private static readonly FunctionOptions<string?, Either<ulong, float>> BothSelectorRight = new(s =>
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
    private static readonly FunctionOptions<string?, Either<int, float>> SingleSelectorRight = new(s =>
    {
        if (s is null) return float.NaN;
        else return s.Length;
    });
    #endregion
}
