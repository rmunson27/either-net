using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemTest.Core.Utilities.Monads;

/// <summary>
/// Tests of the <see cref="Either{TLeft, TRight}.SelectEither"/> method and related overloads.
/// </summary>
[TestClass]
public class SelectTest
{
    #region Tests
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
    /// <see cref="Either{TLeft, TRight}.SelectEither{TLeftResult, TRightResult}(Func{TLeft, TLeftResult}, Func{TRight, TRightResult})"/>
    /// method.
    /// </summary>
    [TestMethod]
    public void TestSelectEither()
    {
        Assert.That.HasLeft(false, Either<object?, string>.New(4).SelectEither(IsNull.Invoke, StringLength.Invoke));
        Assert.That.HasRight(1, Either<object?, string>.New("4").SelectEither(IsNull.Invoke, StringLength.Invoke));
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
        await Assert.That.IsCanceledAsync(
                    (e, ct) => e.SelectLeftAsync(IsNull.InvokeCancellableAsync, ct),
                    Either<object?, string>.NewLeft(null))
                .ConfigureAwait(false);
        Assert.That.HasRight(
            "",
            await Assert.That.IsNotCanceledAsync(
                (e, ct) => e.SelectLeftAsync(IsNull.InvokeCancellableAsync, ct), Either<object?, string>.New(""))
                .ConfigureAwait(false));
    }
    #endregion

    #region Both Sides
    #region Only Left Selector Async
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectEitherAsync{TLeftResult, TRightResult}(Func{TLeft, Task{TLeftResult}}, Func{TRight, TRightResult})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectEitherAsync_LeftAsyncOnly_NonCancellable()
    {
        Assert.That.HasLeft(
            true,
            await Either<object?, string>.NewLeft(null)
                    .SelectEitherAsync(IsNull.InvokeAsync, StringLength.Invoke)
                .ConfigureAwait(false));
        Assert.That.HasRight(
            0,
            await Either<object?, string>.New("")
                    .SelectEitherAsync(IsNull.InvokeAsync, StringLength.Invoke)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectEitherAsync{TLeftResult, TRightResult}(Func{TLeft, CancellationToken, Task{TLeftResult}}, Func{TRight, TRightResult}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectEitherAsync_LeftAsyncOnly_Cancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            true,
            await Either<object?, string>.NewLeft(null)
                    .SelectEitherAsync(IsNull.InvokeCancellableAsync, StringLength.Invoke, CancellationToken.None)
                    .ConfigureAwait(false));
        Assert.That.HasRight(
            0,
            await Either<object?, string>.New("")
                    .SelectEitherAsync(IsNull.InvokeCancellableAsync, StringLength.Invoke, CancellationToken.None)
                    .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectEitherAsync{TLeftResult, TRightResult}(Func{TLeft, CancellationToken, Task{TLeftResult}}, Func{TRight, TRightResult}, CancellationToken)"/>
    /// method and cancels it to ensure cancellation is observed properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectEitherAsync_LeftAsyncOnly_Cancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                    (e, ct) => e.SelectEitherAsync(IsNull.InvokeCancellableAsync, StringLength.Invoke, ct),
                    Either<object?, string>.NewLeft(null))
                .ConfigureAwait(false);
        Assert.That.HasRight(
            0,
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.SelectEitherAsync(IsNull.InvokeCancellableAsync, StringLength.Invoke, ct),
                    Either<object?, string>.New(""))
                .ConfigureAwait(false));
    }
    #endregion

    #region Both Selectors Async
    #region Non-Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectEitherAsync{TLeftResult, TRightResult}(Func{TLeft, Task{TLeftResult}}, Func{TRight, Task{TRightResult}})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectEitherAsync_BothAsync_NonCancellable()
    {
        Assert.That.HasLeft(
            true,
            await Either<object?, string>.NewLeft(null)
                    .SelectEitherAsync(IsNull.InvokeAsync, StringLength.InvokeAsync)
                .ConfigureAwait(false));
        Assert.That.HasRight(
            0,
            await Either<object?, string>.New("")
                    .SelectEitherAsync(IsNull.InvokeAsync, StringLength.InvokeAsync)
                .ConfigureAwait(false));
    }
    #endregion

    #region Left Cancellable Only
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectEitherAsync{TLeftResult, TRightResult}(Func{TLeft, CancellationToken, Task{TLeftResult}}, Func{TRight, Task{TRightResult}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectEitherAsync_BothAsync_OnlyLeftCancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            true,
            await Either<object?, string>.NewLeft(null)
                    .SelectEitherAsync(IsNull.InvokeCancellableAsync, StringLength.InvokeAsync)
                    .ConfigureAwait(false));
        Assert.That.HasRight(
            0,
            await Either<object?, string>.New("")
                    .SelectEitherAsync(IsNull.InvokeCancellableAsync, StringLength.InvokeAsync)
                    .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectEitherAsync{TLeftResult, TRightResult}(Func{TLeft, CancellationToken, Task{TLeftResult}}, Func{TRight, Task{TRightResult}}, CancellationToken)"/>
    /// method, cancelling it to make sure cancellation tokens are used properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectEitherAsync_BothAsync_OnlyLeftCancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.SelectEitherAsync(IsNull.InvokeCancellableAsync, StringLength.InvokeAsync, ct),
                Either<object?, string>.NewLeft(null))
            .ConfigureAwait(false);
        Assert.That.HasRight(
            0,
            await Assert.That.IsNotCanceledAsync(
                (e, ct) => e.SelectEitherAsync(IsNull.InvokeCancellableAsync, StringLength.InvokeAsync, ct),
                    Either<object?, string>.New(""))
                .ConfigureAwait(false));
    }
    #endregion

    #region Right Cancellable Only
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectEitherAsync{TLeftResult, TRightResult}(Func{TLeft, Task{TLeftResult}}, Func{TRight, CancellationToken, Task{TRightResult}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectEitherAsync_BothAsync_OnlyRightCancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            true,
            await Either<object?, string>.NewLeft(null)
                    .SelectEitherAsync(IsNull.InvokeAsync, StringLength.InvokeCancellableAsync)
                    .ConfigureAwait(false));
        Assert.That.HasRight(
            0,
            await Either<object?, string>.New("")
                    .SelectEitherAsync(IsNull.InvokeAsync, StringLength.InvokeCancellableAsync)
                    .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectEitherAsync{TLeftResult, TRightResult}(Func{TLeft, Task{TLeftResult}}, Func{TRight, CancellationToken, Task{TRightResult}}, CancellationToken)"/>
    /// method, cancelling it to make sure cancellation tokens are used properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectEitherAsync_BothAsync_OnlyRightCancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.SelectEitherAsync(IsNull.InvokeAsync, StringLength.InvokeCancellableAsync, ct),
                Either<object?, string>.New(""))
            .ConfigureAwait(false);
        Assert.That.HasLeft(
            true,
            await Assert.That.IsNotCanceledAsync(
                (e, ct) => e.SelectEitherAsync(IsNull.InvokeAsync, StringLength.InvokeCancellableAsync, ct),
                    Either<object?, string>.NewLeft(null))
                .ConfigureAwait(false));
    }
    #endregion

    #region Both Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectEitherAsync{TLeftResult, TRightResult}(Func{TLeft, CancellationToken, Task{TLeftResult}}, Func{TRight, CancellationToken, Task{TRightResult}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectEitherAsync_BothAsync_BothCancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            true,
            await Either<object?, string>.NewLeft(null)
                    .SelectEitherAsync(IsNull.InvokeCancellableAsync, StringLength.InvokeCancellableAsync)
                    .ConfigureAwait(false));
        Assert.That.HasRight(
            0,
            await Either<object?, string>.New("")
                    .SelectEitherAsync(IsNull.InvokeCancellableAsync, StringLength.InvokeCancellableAsync)
                    .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectEitherAsync{TLeftResult, TRightResult}(Func{TLeft, CancellationToken, Task{TLeftResult}}, Func{TRight, CancellationToken, Task{TRightResult}}, CancellationToken)"/>
    /// method, cancelling it to make sure cancellation tokens are used properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectEitherAsync_BothAsync_BothCancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.SelectEitherAsync(IsNull.InvokeCancellableAsync, StringLength.InvokeCancellableAsync, ct),
                Either<object?, string>.NewLeft(null))
            .ConfigureAwait(false);
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.SelectEitherAsync(IsNull.InvokeCancellableAsync, StringLength.InvokeCancellableAsync, ct),
                Either<object?, string>.New(""))
            .ConfigureAwait(false);
    }
    #endregion
    #endregion

    #region Only Right Selector Async
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectEitherAsync{TLeftResult, TRightResult}(Func{TLeft, TLeftResult}, Func{TRight, Task{TRightResult}})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectEitherAsync_RightAsyncOnly_NonCancellable()
    {
        Assert.That.HasLeft(
            true,
            await Either<object?, string>.NewLeft(null)
                .SelectEitherAsync(IsNull.Invoke, StringLength.InvokeAsync)
                .ConfigureAwait(false));
        Assert.That.HasRight(
            0,
            await Either<object?, string>.New("")
                .SelectEitherAsync(IsNull.Invoke, StringLength.InvokeAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectEitherAsync{TLeftResult, TRightResult}(Func{TLeft, TLeftResult}, Func{TRight, CancellationToken, Task{TRightResult}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectEitherAsync_RightAsyncOnly_Cancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            true,
            await Either<object?, string>.NewLeft(null)
                    .SelectEitherAsync(IsNull.Invoke, StringLength.InvokeCancellableAsync, CancellationToken.None)
                    .ConfigureAwait(false));
        Assert.That.HasRight(
            0,
            await Either<object?, string>.New("")
                    .SelectEitherAsync(IsNull.Invoke, StringLength.InvokeCancellableAsync, CancellationToken.None)
                    .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectEitherAsync{TLeftResult, TRightResult}(Func{TLeft, TLeftResult}, Func{TRight, CancellationToken, Task{TRightResult}}, CancellationToken)"/>
    /// method and cancels it to ensure cancellation is observed properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestSelectEitherAsync_RightAsyncOnly_Cancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                    (e, ct) => e.SelectEitherAsync(IsNull.Invoke, StringLength.InvokeCancellableAsync, ct),
                    Either<object?, string>.New(""))
                .ConfigureAwait(false);

        Assert.That.HasLeft(
            true,
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.SelectEitherAsync(IsNull.Invoke, StringLength.InvokeCancellableAsync, ct),
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
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.SelectRightAsync(IsNull.InvokeCancellableAsync, ct),
                Either<string, object?>.NewRight(null)).ConfigureAwait(false);
        Assert.That.HasLeft(
            "",
            await Assert.That.IsNotCanceledAsync(
                (e, ct) => e.SelectRightAsync(IsNull.InvokeCancellableAsync, ct), Either<string, object?>.New(""))
                .ConfigureAwait(false));
    }
    #endregion
    #endregion
    #endregion

    #region Helper Selectors
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
}
