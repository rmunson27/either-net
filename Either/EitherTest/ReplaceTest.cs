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
        Assert.That.HasLeft(2.0, Either<int, string>.New(4).ReplaceLeft(2.0));
        Assert.That.HasRight("", Either<int, string>.New("").ReplaceLeft(2.0));
    }

    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.ReplaceLeftLazy{TNewLeft}(Func{TNewLeft})"/> method.
    /// </summary>
    [TestMethod]
    public void TestReplaceLeftLazy()
    {
        Assert.That.HasLeft(2.0f, Either<int, string>.New(4).ReplaceLeftLazy(Get2.Invoke));
        Assert.That.HasRight("", Either<int, string>.New("").ReplaceLeftLazy(Get2.Invoke));
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
            2.0f,
            await Either<int, string>.New(4).ReplaceLeftLazyAsync(Get2.InvokeAsync).ConfigureAwait(false));
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
            2.0f,
            await Either<int, string>.New(4).ReplaceLeftLazyAsync(Get2.InvokeCancellableAsync).ConfigureAwait(false));
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
                (e, ct) => e.ReplaceLeftLazyAsync(Get2.InvokeCancellableAsync, ct),
                Either<int, string>.New(4))
            .ConfigureAwait(false);
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
        Assert.That.HasLeft(2.0, Either<int, string>.New(4).ReplaceEither(2.0, DateTime.Now));
        Assert.That.HasRight(StartOfMillennium3, Either<int, string>.New("").ReplaceEither(2.0, StartOfMillennium3));
    }

    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.ReplaceEitherLazy{TNewLeft, TNewRight}(Func{TNewLeft}, TNewRight)"/>
    /// method.
    /// </summary>
    [TestMethod]
    public void TestReplaceEitherLazy_LazyLeftOnly()
    {
        Assert.That.HasLeft(2.0f, Either<int, string>.New(4).ReplaceEitherLazy(Get2.Invoke, DateTime.Now));
        Assert.That.HasRight(
            StartOfMillennium3, Either<int, string>.New("").ReplaceEitherLazy(Get2.Invoke, StartOfMillennium3));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceEitherLazy{TNewLeft, TNewRight}(Func{TNewLeft}, Func{TNewRight})"/> method.
    /// </summary>
    [TestMethod]
    public void TestReplaceEitherLazy()
    {
        Assert.That.HasLeft(
            2.0f, Either<int, string>.New(4).ReplaceEitherLazy(Get2.Invoke, GetStartOfMillennium3.Invoke));
        Assert.That.HasRight(
            StartOfMillennium3,
            Either<int, string>.New("rr").ReplaceEitherLazy(Get2.Invoke, GetStartOfMillennium3.Invoke));
    }

    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.ReplaceEitherLazy{TNewLeft, TNewRight}(TNewLeft, Func{TNewRight})"/>
    /// method.
    /// </summary>
    [TestMethod]
    public void TestReplaceEitherLazy_LazyRightOnly()
    {
        Assert.That.HasLeft(2.0, Either<int, string>.New(4).ReplaceEitherLazy(2.0, GetStartOfMillennium3.Invoke));
        Assert.That.HasRight(
            StartOfMillennium3, Either<int, string>.New("").ReplaceEitherLazy(2.0, GetStartOfMillennium3.Invoke));
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
            2.0f,
            await Either<int, string>.New(4).ReplaceEitherLazyAsync(Get2.InvokeAsync, StartOfMillennium3)
                    .ConfigureAwait(false));

        Assert.That.HasRight(
            StartOfMillennium3,
            await Either<int, string>.New("   ").ReplaceEitherLazyAsync(Get2.InvokeAsync, StartOfMillennium3)
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
            2.0f,
            await Either<int, string>.New(4).ReplaceEitherLazyAsync(Get2.InvokeCancellableAsync, StartOfMillennium3)
                    .ConfigureAwait(false));

        Assert.That.HasRight(
            StartOfMillennium3,
            await Either<int, string>.New(" ").ReplaceEitherLazyAsync(Get2.InvokeCancellableAsync, StartOfMillennium3)
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
                (e, ct) => e.ReplaceEitherLazyAsync(Get2.InvokeCancellableAsync, StartOfMillennium3, ct),
                Either<int, string>.New(4))
            .ConfigureAwait(false);

        Assert.That.HasRight(
            StartOfMillennium3,
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.ReplaceEitherLazyAsync(Get2.InvokeCancellableAsync, StartOfMillennium3, ct),
                    Either<int, string>.New(""))
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
            2.0f,
            await LeftEither.ReplaceEitherLazyAsync(Get2.InvokeAsync, GetStartOfMillennium3.Invoke)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            StartOfMillennium3,
            await RightEither.ReplaceEitherLazyAsync(Get2.InvokeAsync, GetStartOfMillennium3.Invoke)
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
                (e, ct) => e.ReplaceEitherLazyAsync(Get2.InvokeCancellableAsync, GetStartOfMillennium3.Invoke, ct),
                LeftEither)
            .ConfigureAwait(false);

        Assert.That.HasRight(
            RightReplacement,
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.ReplaceEitherLazyAsync(Get2.InvokeCancellableAsync, GetStartOfMillennium3.Invoke, ct),
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
            2.0f,
            await Either<int, string>.New(4).ReplaceEitherLazyAsync(2.0, GetStartOfMillennium3.InvokeAsync)
                    .ConfigureAwait(false));

        Assert.That.HasRight(
            StartOfMillennium3,
            await Either<int, string>.New("   ").ReplaceEitherLazyAsync(2.0, GetStartOfMillennium3.InvokeAsync)
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
            2.0f,
            await Either<int, string>.New(4).ReplaceEitherLazyAsync(2.0, GetStartOfMillennium3.InvokeCancellableAsync)
                    .ConfigureAwait(false));

        Assert.That.HasRight(
            StartOfMillennium3,
            await Either<int, string>.New("").ReplaceEitherLazyAsync(2.0, GetStartOfMillennium3.InvokeCancellableAsync)
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
            2.0,
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.ReplaceEitherLazyAsync(2.0, GetStartOfMillennium3.InvokeCancellableAsync, ct),
                    Either<int, string>.New(4))
                .ConfigureAwait(false));

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.ReplaceEitherLazyAsync(2.0, GetStartOfMillennium3.InvokeCancellableAsync, ct),
                Either<int, string>.New(""))
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
            2.0f,
            await LeftEither.ReplaceEitherLazyAsync(Get2.Invoke, GetStartOfMillennium3.InvokeAsync)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            StartOfMillennium3,
            await RightEither.ReplaceEitherLazyAsync(Get2.Invoke, GetStartOfMillennium3.InvokeAsync)
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
        Assert.That.HasLeft(2, Either<int, string>.New(2).ReplaceRight(3.0));
        Assert.That.HasRight(3.0, Either<int, string>.New("").ReplaceRight(3.0));
    }

    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.ReplaceRightLazy{TNewRight}(Func{TNewRight})"/> method.
    /// </summary>
    [TestMethod]
    public void TestReplaceRightLazy()
    {
        Assert.That.HasLeft(4, Either<int, string>.New(4).ReplaceRightLazy(GetStartOfMillennium3.Invoke));
        Assert.That.HasRight(StartOfMillennium3, Either<int, string>.New("").ReplaceRightLazy(GetStartOfMillennium3.Invoke));
    }
    #endregion

    #region Asynchronous
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.ReplaceRightLazyAsync{TNewRight}(Func{Task{TNewRight}})"/> method.
    /// </summary>
    [TestMethod]
    public async Task TestReplaceRightLazyAsync_NonCancellable()
    {
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
    /// A factory method that gets the number 2.
    /// </summary>
    private static readonly FunctionOptions<float> Get2 = new(() => 2);

    /// <summary>
    /// A factory method that gets the first instant of the third millennium.
    /// </summary>
    private static readonly FunctionOptions<DateTime> GetStartOfMillennium3 = new(() => StartOfMillennium3);

    /// <summary>
    /// The first instant of the third millennium.
    /// </summary>
    private static readonly DateTime StartOfMillennium3 = DateTime.Parse("January 1, 2000");

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
