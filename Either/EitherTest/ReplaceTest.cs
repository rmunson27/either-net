using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemTest.Core.Utilities.Monads;

/// <summary>
/// Tests the replace methods of the <see cref="Either{TLeft, TRight}"/> struct.
/// </summary>
[TestClass]
public class ReplaceTest
{
    #region Constants
    /// <summary>
    /// The value wrapped in <see cref="LeftEither"/>.
    /// </summary>
    private const int LeftValue = 3;

    /// <summary>
    /// The value wrapped in <see cref="RightEither"/>.
    /// </summary>
    private const string RightValue = "";

    /// <summary>
    /// A left <see cref="Either{TLeft, TRight}"/> to use for testing.
    /// </summary>
    private static readonly Either<int, string> LeftEither = LeftValue;

    /// <summary>
    /// A right <see cref="Either{TLeft, TRight}"/> to use for testing.
    /// </summary>
    private static readonly Either<int, string> RightEither = RightValue;

    /// <summary>
    /// The value that left eithers in testing will be replaced with.
    /// </summary>
    private const float LeftReplacement = 2.5f;

    /// <summary>
    /// The value that right eithers in testing will be replaced with.
    /// </summary>
    /// <remarks>
    /// This is currently the first instant of the third millennium (i.e. the first second of January 1st, 2000).
    /// </remarks>
    private static readonly DateTime RightReplacement = DateTime.Parse("January 1, 2000");
    #endregion

    #region Tests
    #region Left
    #region Synchronous
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.ReplaceLeft{TNewLeft}(TNewLeft)"/> method.
    /// </summary>
    [TestMethod]
    public void TestReplaceLeft()
    {
        Assert.That.HasLeft(LeftReplacement, LeftEither.ReplaceLeft(LeftReplacement));
        Assert.That.HasRight(RightValue, RightEither.ReplaceLeft(LeftReplacement));
    }

    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.ReplaceLeftLazy{TNewLeft}(Func{TNewLeft})"/> method.
    /// </summary>
    [TestMethod]
    public void TestReplaceLeftLazy()
    {
        Assert.That.HasLeft(LeftReplacement, LeftEither.ReplaceLeftLazy(LeftReplacer.Invoke));
        Assert.That.HasRight(RightValue, RightEither.ReplaceLeftLazy(LeftReplacer.Invoke));
    }
    #endregion Synchronous

    #region Asynchronous
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.ReplaceLeftLazyAsync{TNewLeft}(Func{Task{TNewLeft}})"/> method.
    /// </summary>
    [TestMethod]
    public async Task TestReplaceLeftLazyAsync_NonCancellable()
    {
        Assert.That.HasLeft(
            LeftReplacement,
            await LeftEither.ReplaceLeftLazyAsync(LeftReplacer.InvokeAsync).ConfigureAwait(false));

        Assert.That.HasRight(
            RightValue,
            await RightEither.ReplaceLeftLazyAsync(LeftReplacer.InvokeAsync).ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceLeftLazyAsync{TNewLeft}(Func{CancellationToken, Task{TNewLeft}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    [TestMethod]
    public async Task TestReplaceLeftLazyAsync_Cancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            LeftReplacement,
            await LeftEither.ReplaceLeftLazyAsync(LeftReplacer.InvokeCancellableAsync).ConfigureAwait(false));

        Assert.That.HasRight(
            RightValue,
            await RightEither.ReplaceLeftLazyAsync(LeftReplacer.InvokeCancellableAsync).ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceLeftLazyAsync{TNewLeft}(Func{CancellationToken, Task{TNewLeft}}, CancellationToken)"/>
    /// method, cancelling it to ensure cancellation tokens are used properly.
    /// </summary>
    [TestMethod]
    public async Task TestReplaceLeftLazyAsync_Cancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.ReplaceLeftLazyAsync(LeftReplacer.InvokeCancellableAsync, ct),
                LeftEither)
            .ConfigureAwait(false);

        Assert.That.HasRight(
            RightValue,
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.ReplaceLeftLazyAsync(LeftReplacer.InvokeCancellableAsync, ct),
                    RightEither)
                .ConfigureAwait(false));
    }
    #endregion Asynchronous
    #endregion

    #region Both Sides
    #region Synchronous
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.ReplaceEither{TNewLeft, TNewRight}(TNewLeft, TNewRight)"/> method.
    /// </summary>
    [TestMethod]
    public void TestReplaceEither()
    {
        Assert.That.HasLeft(LeftReplacement, LeftEither.ReplaceEither(LeftReplacement, RightReplacement));
        Assert.That.HasRight(RightReplacement, RightEither.ReplaceEither(LeftReplacement, RightReplacement));
    }

    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.ReplaceEitherLazy{TNewLeft, TNewRight}(Func{TNewLeft}, TNewRight)"/>
    /// method.
    /// </summary>
    [TestMethod]
    public void TestReplaceEitherLazy_LazyLeftOnly()
    {
        Assert.That.HasLeft(LeftReplacement, LeftEither.ReplaceEitherLazy(LeftReplacer.Invoke, RightReplacement));
        Assert.That.HasRight(RightReplacement, RightEither.ReplaceEitherLazy(LeftReplacer.Invoke, RightReplacement));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceEitherLazy{TNewLeft, TNewRight}(Func{TNewLeft}, Func{TNewRight})"/> method.
    /// </summary>
    [TestMethod]
    public void TestReplaceEitherLazy()
    {
        Assert.That.HasLeft(LeftReplacement, LeftEither.ReplaceEitherLazy(LeftReplacer.Invoke, RightReplacer.Invoke));
        Assert.That.HasRight(
            RightReplacement, RightEither.ReplaceEitherLazy(LeftReplacer.Invoke, RightReplacer.Invoke));
    }

    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.ReplaceEitherLazy{TNewLeft, TNewRight}(TNewLeft, Func{TNewRight})"/>
    /// method.
    /// </summary>
    [TestMethod]
    public void TestReplaceEitherLazy_LazyRightOnly()
    {
        Assert.That.HasLeft(LeftReplacement, LeftEither.ReplaceEitherLazy(LeftReplacement, RightReplacer.Invoke));
        Assert.That.HasRight(RightReplacement, RightEither.ReplaceEitherLazy(LeftReplacement, RightReplacer.Invoke));
    }
    #endregion Synchronous

    #region Asynchronous
    #region Left Async Only
    #region Right Eager
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceEitherLazyAsync{TNewLeft, TNewRight}(Func{Task{TNewLeft}}, TNewRight)"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestReplaceEitherLazyAsync_LazyLeftOnly_NonCancellable()
    {
        Assert.That.HasLeft(
            LeftReplacement,
            await LeftEither.ReplaceEitherLazyAsync(LeftReplacer.InvokeAsync, RightReplacement)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            RightReplacement,
            await RightEither.ReplaceEitherLazyAsync(LeftReplacer.InvokeAsync, RightReplacement)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceEitherLazyAsync{TNewLeft, TNewRight}(Func{CancellationToken, Task{TNewLeft}}, TNewRight, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestReplaceEitherLazyAsync_LazyLeftOnly_Cancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            LeftReplacement,
            await LeftEither.ReplaceEitherLazyAsync(LeftReplacer.InvokeCancellableAsync, RightReplacement)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            RightReplacement,
            await RightEither.ReplaceEitherLazyAsync(LeftReplacer.InvokeCancellableAsync, RightReplacement)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceEitherLazyAsync{TNewLeft, TNewRight}(Func{CancellationToken, Task{TNewLeft}}, TNewRight, CancellationToken)"/>
    /// method, cancelling it to ensure cancellation tokens are used properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestReplaceEitherLazyAsync_LazyLeftOnly_Cancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.ReplaceEitherLazyAsync(LeftReplacer.InvokeCancellableAsync, RightReplacement, ct),
                LeftEither)
            .ConfigureAwait(false);

        Assert.That.HasRight(
            RightReplacement,
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.ReplaceEitherLazyAsync(LeftReplacer.InvokeCancellableAsync, RightReplacement, ct),
                    RightEither)
                .ConfigureAwait(false));
    }
    #endregion

    #region Right Lazy
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceEitherLazyAsync{TNewLeft, TNewRight}(Func{Task{TNewLeft}}, Func{TNewRight})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestReplaceEitherLazyAsync_BothLazy_LeftAsyncOnly_NonCancellable()
    {
        Assert.That.HasLeft(
            LeftReplacement,
            await LeftEither.ReplaceEitherLazyAsync(LeftReplacer.InvokeAsync, RightReplacer.Invoke)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            RightReplacement,
            await RightEither.ReplaceEitherLazyAsync(LeftReplacer.InvokeAsync, RightReplacer.Invoke)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceEitherLazyAsync{TNewLeft, TNewRight}(Func{CancellationToken, Task{TNewLeft}}, Func{TNewRight}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestReplaceEitherLazyAsync_BothLazy_LeftAsyncOnly_Cancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            LeftReplacement,
            await LeftEither.ReplaceEitherLazyAsync(LeftReplacer.InvokeCancellableAsync, RightReplacer.Invoke)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            RightReplacement,
            await RightEither.ReplaceEitherLazyAsync(LeftReplacer.InvokeCancellableAsync, RightReplacer.Invoke)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceEitherLazyAsync{TNewLeft, TNewRight}(Func{CancellationToken, Task{TNewLeft}}, Func{TNewRight}, CancellationToken)"/>
    /// method, cancelling it to make sure cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestReplaceEitherLazyAsync_BothLazy_LeftAsyncOnly_Cancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.ReplaceEitherLazyAsync(LeftReplacer.InvokeCancellableAsync, RightReplacer.Invoke, ct),
                LeftEither)
            .ConfigureAwait(false);

        Assert.That.HasRight(
            RightReplacement,
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.ReplaceEitherLazyAsync(LeftReplacer.InvokeCancellableAsync, RightReplacer.Invoke, ct),
                    RightEither)
                .ConfigureAwait(false));
    }
    #endregion
    #endregion Left Async Only

    #region Right Async Only
    #region Left Eager
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceEitherLazyAsync{TNewLeft, TNewRight}(TNewLeft, Func{Task{TNewRight}})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestReplaceEitherLazyAsync_LazyRightOnly_NonCancellable()
    {
        Assert.That.HasLeft(
            LeftReplacement,
            await LeftEither.ReplaceEitherLazyAsync(LeftReplacement, RightReplacer.InvokeAsync)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            RightReplacement,
            await RightEither.ReplaceEitherLazyAsync(LeftReplacement, RightReplacer.InvokeAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceEitherLazyAsync{TNewLeft, TNewRight}(TNewLeft, Func{CancellationToken, Task{TNewRight}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestReplaceEitherLazyAsync_LazyRightOnly_Cancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            LeftReplacement,
            await LeftEither.ReplaceEitherLazyAsync(LeftReplacement, RightReplacer.InvokeCancellableAsync)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            RightReplacement,
            await RightEither.ReplaceEitherLazyAsync(LeftReplacement, RightReplacer.InvokeCancellableAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceEitherLazyAsync{TNewLeft, TNewRight}(TNewLeft, Func{CancellationToken, Task{TNewRight}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestReplaceEitherLazyAsync_LazyRightOnly_Cancellable_Cancellation()
    {
        Assert.That.HasLeft(
            LeftReplacement,
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.ReplaceEitherLazyAsync(LeftReplacement, RightReplacer.InvokeCancellableAsync, ct),
                    LeftEither)
                .ConfigureAwait(false));

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.ReplaceEitherLazyAsync(LeftReplacement, RightReplacer.InvokeCancellableAsync, ct),
                RightEither)
            .ConfigureAwait(false);
    }
    #endregion

    #region Left Lazy
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceEitherLazyAsync{TNewLeft, TNewRight}(Func{TNewLeft}, Func{Task{TNewRight}})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestReplaceEitherLazyAsync_BothLazy_RightAsyncOnly_NonCancellable()
    {
        Assert.That.HasLeft(
            LeftReplacement,
            await LeftEither.ReplaceEitherLazyAsync(LeftReplacer.Invoke, RightReplacer.InvokeAsync)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            RightReplacement,
            await RightEither.ReplaceEitherLazyAsync(LeftReplacer.Invoke, RightReplacer.InvokeAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceEitherLazyAsync{TNewLeft, TNewRight}(Func{TNewLeft}, Func{CancellationToken, Task{TNewRight}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestReplaceEitherLazyAsync_BothLazy_RightAsyncOnly_Cancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            LeftReplacement,
            await LeftEither.ReplaceEitherLazyAsync(LeftReplacer.Invoke, RightReplacer.InvokeCancellableAsync)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            RightReplacement,
            await RightEither.ReplaceEitherLazyAsync(LeftReplacer.Invoke, RightReplacer.InvokeCancellableAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceEitherLazyAsync{TNewLeft, TNewRight}(Func{TNewLeft}, Func{CancellationToken, Task{TNewRight}}, CancellationToken)"/>
    /// method, cancelling it to make sure cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestReplaceEitherLazyAsync_BothLazy_RightAsyncOnly_Cancellable_Cancellation()
    {
        Assert.That.HasLeft(
            LeftReplacement,
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.ReplaceEitherLazyAsync(LeftReplacer.Invoke, RightReplacer.InvokeCancellableAsync, ct),
                    LeftEither)
                .ConfigureAwait(false));

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.ReplaceEitherLazyAsync(LeftReplacer.Invoke, RightReplacer.InvokeCancellableAsync, ct),
                RightEither)
            .ConfigureAwait(false);
    }
    #endregion
    #endregion

    #region Both Async
    #region Non-Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceEitherLazyAsync{TNewLeft, TNewRight}(Func{Task{TNewLeft}}, Func{Task{TNewRight}})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestReplaceEitherLazyAsync_BothLazyAsync_NonCancellable()
    {
        Assert.That.HasLeft(
            LeftReplacement,
            await LeftEither.ReplaceEitherLazyAsync(LeftReplacer.InvokeAsync, RightReplacer.InvokeAsync)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            RightReplacement,
            await RightEither.ReplaceEitherLazyAsync(LeftReplacer.InvokeAsync, RightReplacer.InvokeAsync)
                .ConfigureAwait(false));
    }
    #endregion

    #region Only Left Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceEitherLazyAsync{TNewLeft, TNewRight}(Func{CancellationToken, Task{TNewLeft}}, Func{Task{TNewRight}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestReplaceEitherLazyAsync_BothLazyAsync_OnlyLeftCancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            LeftReplacement,
            await LeftEither.ReplaceEitherLazyAsync(LeftReplacer.InvokeCancellableAsync, RightReplacer.InvokeAsync)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            RightReplacement,
            await RightEither.ReplaceEitherLazyAsync(LeftReplacer.InvokeCancellableAsync, RightReplacer.InvokeAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceEitherLazyAsync{TNewLeft, TNewRight}(Func{CancellationToken, Task{TNewLeft}}, Func{Task{TNewRight}}, CancellationToken)"/>
    /// method, cancelling it to make sure cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestReplaceEitherLazyAsync_BothLazyAsync_OnlyLeftCancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.ReplaceEitherLazyAsync(
                                LeftReplacer.InvokeCancellableAsync, RightReplacer.InvokeAsync, ct),
                LeftEither)
            .ConfigureAwait(false);

        Assert.That.HasRight(
            RightReplacement,
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.ReplaceEitherLazyAsync(
                                    LeftReplacer.InvokeCancellableAsync, RightReplacer.InvokeAsync, ct),
                    RightEither)
                .ConfigureAwait(false));
    }
    #endregion

    #region Only Right Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceEitherLazyAsync{TNewLeft, TNewRight}(Func{Task{TNewLeft}}, Func{CancellationToken, Task{TNewRight}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestReplaceEitherLazyAsync_BothLazyAsync_OnlyRightCancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            LeftReplacement,
            await LeftEither.ReplaceEitherLazyAsync(LeftReplacer.InvokeAsync, RightReplacer.InvokeCancellableAsync)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            RightReplacement,
            await RightEither.ReplaceEitherLazyAsync(LeftReplacer.InvokeAsync, RightReplacer.InvokeCancellableAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceEitherLazyAsync{TNewLeft, TNewRight}(Func{Task{TNewLeft}}, Func{CancellationToken, Task{TNewRight}}, CancellationToken)"/>
    /// method, cancelling it to make sure cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestReplaceEitherLazyAsync_BothLazyAsync_OnlyRightCancellable_Cancellation()
    {
        Assert.That.HasLeft(
            LeftReplacement,
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.ReplaceEitherLazyAsync(LeftReplacer.InvokeAsync, RightReplacer.InvokeCancellableAsync, ct),
                    LeftEither)
                .ConfigureAwait(false));

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.ReplaceEitherLazyAsync(LeftReplacer.InvokeAsync, RightReplacer.InvokeCancellableAsync, ct),
                RightEither)
            .ConfigureAwait(false);
    }
    #endregion

    #region Both Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceEitherLazyAsync{TNewLeft, TNewRight}(Func{CancellationToken, Task{TNewLeft}}, Func{CancellationToken, Task{TNewRight}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestReplaceEitherLazyAsync_BothLazyAsyncCancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            LeftReplacement,
            await LeftEither.ReplaceEitherLazyAsync(
                    LeftReplacer.InvokeCancellableAsync, RightReplacer.InvokeCancellableAsync)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            RightReplacement,
            await RightEither.ReplaceEitherLazyAsync(
                    LeftReplacer.InvokeCancellableAsync, RightReplacer.InvokeCancellableAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceEitherLazyAsync{TNewLeft, TNewRight}(Func{CancellationToken, Task{TNewLeft}}, Func{CancellationToken, Task{TNewRight}}, CancellationToken)"/>
    /// method, cancelling it to make sure cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestReplaceEitherLazyAsync_BothLazyAsyncCancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.ReplaceEitherLazyAsync(
                                LeftReplacer.InvokeCancellableAsync, RightReplacer.InvokeCancellableAsync, ct),
                LeftEither)
            .ConfigureAwait(false);

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.ReplaceEitherLazyAsync(
                                LeftReplacer.InvokeCancellableAsync, RightReplacer.InvokeCancellableAsync, ct),
                RightEither)
            .ConfigureAwait(false);
    }
    #endregion
    #endregion
    #endregion
    #endregion

    #region Right
    #region Synchronous
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.ReplaceRight{TNewRight}(TNewRight)"/> method.
    /// </summary>
    [TestMethod]
    public void TestReplaceRight()
    {
        Assert.That.HasLeft(LeftValue, LeftEither.ReplaceRight(RightReplacement));
        Assert.That.HasRight(RightReplacement, RightEither.ReplaceRight(RightReplacement));
    }

    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.ReplaceRightLazy{TNewRight}(Func{TNewRight})"/> method.
    /// </summary>
    [TestMethod]
    public void TestReplaceRightLazy()
    {
        Assert.That.HasLeft(LeftValue, LeftEither.ReplaceRightLazy(RightReplacer.Invoke));
        Assert.That.HasRight(RightReplacement, RightEither.ReplaceRightLazy(RightReplacer.Invoke));
    }
    #endregion

    #region Asynchronous
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.ReplaceRightLazyAsync{TNewRight}(Func{Task{TNewRight}})"/> method.
    /// </summary>
    [TestMethod]
    public async Task TestReplaceRightLazyAsync_NonCancellable()
    {
        Assert.That.HasLeft(
            LeftValue,
            await LeftEither.ReplaceRightLazyAsync(RightReplacer.InvokeAsync).ConfigureAwait(false));

        Assert.That.HasRight(
            RightReplacement,
            await RightEither.ReplaceRightLazyAsync(RightReplacer.InvokeAsync).ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceRightLazyAsync{TNewRight}(Func{CancellationToken, Task{TNewRight}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    [TestMethod]
    public async Task TestReplaceRightLazyAsync_Cancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            LeftValue,
            await LeftEither.ReplaceRightLazyAsync(RightReplacer.InvokeCancellableAsync).ConfigureAwait(false));

        Assert.That.HasRight(
            RightReplacement,
            await RightEither.ReplaceRightLazyAsync(RightReplacer.InvokeCancellableAsync).ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceRightLazyAsync{TNewRight}(Func{CancellationToken, Task{TNewRight}}, CancellationToken)"/>
    /// method, cancelling it to ensure cancellation tokens are used properly.
    /// </summary>
    [TestMethod]
    public async Task TestReplaceRightLazyAsync_Cancellable_Cancellation()
    {
        Assert.That.HasLeft(
            LeftValue,
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.ReplaceRightLazyAsync(RightReplacer.InvokeCancellableAsync, ct),
                    LeftEither)
                .ConfigureAwait(false));

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.ReplaceRightLazyAsync(RightReplacer.InvokeCancellableAsync, ct),
                RightEither)
            .ConfigureAwait(false);
    }
    #endregion
    #endregion
    #endregion

    #region Helper Factories
    /// <summary>
    /// A thunk that gets <see cref="LeftReplacement"/>.
    /// </summary>
    private static readonly FunctionOptions<float> LeftReplacer = new(() => LeftReplacement);

    /// <summary>
    /// A thunk that gets <see cref="RightReplacement"/>.
    /// </summary>
    private static readonly FunctionOptions<DateTime> RightReplacer = new(() => RightReplacement);
    #endregion
}
