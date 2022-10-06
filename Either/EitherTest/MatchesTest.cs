using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemTest.Core.Utilities.Monads;

/// <summary>
/// Tests the <see cref="Either{TLeft, TRight}.EitherMatches(Func{TLeft, bool}, Func{TRight, bool})"/> and side-specific
/// analogs.
/// </summary>
[TestClass]
public class MatchesTest
{
    #region Constants
    /// <summary>
    /// An <see cref="Either{TLeft, TRight}"/> containing an even integer on the left.
    /// </summary>
    private static readonly Either<int, string> EvenLeftEither = Either<int, string>.New(4);

    /// <summary>
    /// An <see cref="Either{TLeft, TRight}"/> containing an odd integer on the left.
    /// </summary>
    private static readonly Either<int, string> OddLeftEither = Either<int, string>.New(3);

    /// <summary>
    /// An <see cref="Either{TLeft, TRight}"/> containing an even-length string on the right.
    /// </summary>
    private static readonly Either<int, string> EvenLengthRightEither = Either<int, string>.New("");

    /// <summary>
    /// An <see cref="Either{TLeft, TRight}"/> containing an odd-length string on the right.
    /// </summary>
    private static readonly Either<int, string> OddLengthRightEither = Either<int, string>.New(".");

    /// <summary>
    /// An <see cref="Either{TLeft, TRight}"/> containing contact information to use to test
    /// <see cref="Either{TLeft, TRight}"/> extension methods.
    /// </summary>
    private static readonly Either<Email, Phone> PersonalEmailLeftEither
                                                    = Either<Email, Phone>.New(new Email("r@s.t", true)),
                                                 NonPersonalEmailLeftEither
                                                    = Either<Email, Phone>.New(new Email("u@v.w", false)),
                                                 PersonalPhoneRightEither
                                                    = Either<Email, Phone>.New(new Phone(1234567890, true)),
                                                 NonPersonalPhoneRightEither
                                                    = Either<Email, Phone>.New(new Phone(2345678901, false));
    #endregion

    #region Tests
    #region Instance
    #region Synchronous
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.LeftMatches(Func{TLeft, bool})"/> method.
    /// </summary>
    [TestMethod]
    public void TestLeftMatches()
    {
        Assert.IsTrue(EvenLeftEither.LeftMatches(IsEven));
        Assert.IsFalse(OddLeftEither.LeftMatches(IsEven));
        Assert.IsFalse(EvenLengthRightEither.LeftMatches(IsEven));
    }

    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.EitherMatches(Func{TLeft, bool}, Func{TRight, bool})"/> method.
    /// </summary>
    [TestMethod]
    public void TestEitherMatches()
    {
        Assert.IsTrue(EvenLeftEither.EitherMatches(IsEven, IsLengthEven));
        Assert.IsTrue(EvenLengthRightEither.EitherMatches(IsEven, IsLengthEven));
        Assert.IsFalse(OddLeftEither.EitherMatches(IsEven, IsLengthEven));
        Assert.IsFalse(OddLengthRightEither.EitherMatches(IsEven, IsLengthEven));
    }

    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.RightMatches(Func{TRight, bool})"/> method.
    /// </summary>
    [TestMethod]
    public void TestRightMatches()
    {
        Assert.IsTrue(EvenLengthRightEither.RightMatches(IsLengthEven));
        Assert.IsFalse(OddLengthRightEither.RightMatches(IsLengthEven));
        Assert.IsFalse(OddLeftEither.RightMatches(IsLengthEven));
    }
    #endregion

    #region Asynchronous
    #region Left
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.LeftMatchesAsync(Func{TLeft, Task{bool}})"/> method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestLeftMatchesAsync_NonCancellable()
    {
        Assert.IsTrue(await EvenLeftEither.LeftMatchesAsync(IsEven.InvokeAsync).ConfigureAwait(false));
        Assert.IsFalse(await OddLeftEither.LeftMatchesAsync(IsEven.InvokeAsync).ConfigureAwait(false));
        Assert.IsFalse(await EvenLengthRightEither.LeftMatchesAsync(IsEven.InvokeAsync).ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.LeftMatchesAsync(Func{TLeft, CancellationToken, Task{bool}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestLeftMatchesAsync_Cancellable_NoCancellation()
    {
        Assert.IsTrue(await EvenLeftEither.LeftMatchesAsync(IsEven.InvokeCancellableAsync).ConfigureAwait(false));
        Assert.IsFalse(await OddLeftEither.LeftMatchesAsync(IsEven.InvokeCancellableAsync).ConfigureAwait(false));
        Assert.IsFalse(
            await EvenLengthRightEither.LeftMatchesAsync(IsEven.InvokeCancellableAsync).ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.LeftMatchesAsync(Func{TLeft, CancellationToken, Task{bool}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are used properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestLeftMatchesAsync_Cancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.LeftMatchesAsync(IsEven.InvokeCancellableAsync, ct),
                EvenLeftEither)
            .ConfigureAwait(false);
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.LeftMatchesAsync(IsEven.InvokeCancellableAsync, ct),
                OddLeftEither)
            .ConfigureAwait(false);

        Assert.IsFalse(
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.LeftMatchesAsync(IsEven.InvokeCancellableAsync, ct),
                    EvenLengthRightEither)
                .ConfigureAwait(false));
    }
    #endregion

    #region Either Side
    #region Left Async Only
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.EitherMatchesAsync(Func{TLeft, Task{bool}}, Func{TRight, bool})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestEitherMatchesAsync_LeftAsyncOnly_NonCancellable()
    {
        Assert.IsTrue(
            await EvenLeftEither.EitherMatchesAsync(IsEven.InvokeAsync, IsLengthEven.Invoke).ConfigureAwait(false));
        Assert.IsFalse(
            await OddLeftEither.EitherMatchesAsync(IsEven.InvokeAsync, IsLengthEven.Invoke).ConfigureAwait(false));
        Assert.IsTrue(
            await EvenLengthRightEither.EitherMatchesAsync(IsEven.InvokeAsync, IsLengthEven.Invoke)
                .ConfigureAwait(false));
        Assert.IsFalse(
            await OddLengthRightEither.EitherMatchesAsync(IsEven.InvokeAsync, IsLengthEven.Invoke)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.EitherMatchesAsync(Func{TLeft, CancellationToken, Task{bool}}, Func{TRight, bool}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestEitherMatchesAsync_LeftAsyncOnly_Cancellable_NoCancellation()
    {
        Assert.IsTrue(
            await EvenLeftEither.EitherMatchesAsync(IsEven.InvokeCancellableAsync, IsLengthEven.Invoke)
                .ConfigureAwait(false));
        Assert.IsFalse(
            await OddLeftEither.EitherMatchesAsync(IsEven.InvokeCancellableAsync, IsLengthEven.Invoke)
                .ConfigureAwait(false));
        Assert.IsTrue(
            await EvenLengthRightEither.EitherMatchesAsync(IsEven.InvokeCancellableAsync, IsLengthEven.Invoke)
                .ConfigureAwait(false));
        Assert.IsFalse(
            await OddLengthRightEither.EitherMatchesAsync(IsEven.InvokeCancellableAsync, IsLengthEven.Invoke)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.EitherMatchesAsync(Func{TLeft, CancellationToken, Task{bool}}, Func{TRight, bool}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are used properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestEitherMatchesAsync_LeftAsyncOnly_Cancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.EitherMatchesAsync(IsEven.InvokeCancellableAsync, IsLengthEven.Invoke, ct),
                EvenLeftEither)
            .ConfigureAwait(false);
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.EitherMatchesAsync(IsEven.InvokeCancellableAsync, IsLengthEven.Invoke, ct),
                OddLeftEither)
            .ConfigureAwait(false);
        Assert.IsTrue(
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.EitherMatchesAsync(IsEven.InvokeCancellableAsync, IsLengthEven.Invoke, ct),
                    EvenLengthRightEither)
                .ConfigureAwait(false));
        Assert.IsFalse(
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.EitherMatchesAsync(IsEven.InvokeCancellableAsync, IsLengthEven.Invoke, ct),
                    OddLengthRightEither)
                .ConfigureAwait(false));
    }
    #endregion

    #region Right Async Only
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.EitherMatchesAsync(Func{TLeft, bool}, Func{TRight, Task{bool}})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestEitherMatchesAsync_RightAsyncOnly_NonCancellable()
    {
        Assert.IsTrue(
            await EvenLeftEither.EitherMatchesAsync(IsEven.Invoke, IsLengthEven.InvokeAsync).ConfigureAwait(false));
        Assert.IsFalse(
            await OddLeftEither.EitherMatchesAsync(IsEven.Invoke, IsLengthEven.InvokeAsync).ConfigureAwait(false));
        Assert.IsTrue(
            await EvenLengthRightEither.EitherMatchesAsync(IsEven.Invoke, IsLengthEven.InvokeAsync)
                .ConfigureAwait(false));
        Assert.IsFalse(
            await OddLengthRightEither.EitherMatchesAsync(IsEven.Invoke, IsLengthEven.InvokeAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.EitherMatchesAsync(Func{TLeft, bool}, Func{TRight, CancellationToken, Task{bool}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestEitherMatchesAsync_RightAsyncOnly_Cancellable_NoCancellation()
    {
        Assert.IsTrue(
            await EvenLeftEither.EitherMatchesAsync(IsEven.Invoke, IsLengthEven.InvokeCancellableAsync)
                .ConfigureAwait(false));
        Assert.IsFalse(
            await OddLeftEither.EitherMatchesAsync(IsEven.Invoke, IsLengthEven.InvokeCancellableAsync)
                .ConfigureAwait(false));
        Assert.IsTrue(
            await EvenLengthRightEither.EitherMatchesAsync(IsEven.Invoke, IsLengthEven.InvokeCancellableAsync)
                .ConfigureAwait(false));
        Assert.IsFalse(
            await OddLengthRightEither.EitherMatchesAsync(IsEven.Invoke, IsLengthEven.InvokeCancellableAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.EitherMatchesAsync(Func{TLeft, bool}, Func{TRight, CancellationToken, Task{bool}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are used properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestEitherMatchesAsync_RightAsyncOnly_Cancellable_Cancellation()
    {
        Assert.IsTrue(
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.EitherMatchesAsync(IsEven.Invoke, IsLengthEven.InvokeCancellableAsync, ct),
                    EvenLeftEither)
                .ConfigureAwait(false));
        Assert.IsFalse(
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.EitherMatchesAsync(IsEven.Invoke, IsLengthEven.InvokeCancellableAsync, ct),
                    OddLeftEither)
                .ConfigureAwait(false));

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.EitherMatchesAsync(IsEven.Invoke, IsLengthEven.InvokeCancellableAsync, ct),
                EvenLengthRightEither)
            .ConfigureAwait(false);
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.EitherMatchesAsync(IsEven.Invoke, IsLengthEven.InvokeCancellableAsync, ct),
                OddLengthRightEither)
            .ConfigureAwait(false);
    }
    #endregion

    #region Both Async
    #region Non-Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.EitherMatchesAsync(Func{TLeft, Task{bool}}, Func{TRight, Task{bool}})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestEitherMatchesAsync_BothAsync_NonCancellable()
    {
        Assert.IsTrue(
            await EvenLeftEither.EitherMatchesAsync(IsEven.InvokeAsync, IsLengthEven.InvokeAsync)
                .ConfigureAwait(false));
        Assert.IsFalse(
            await OddLeftEither.EitherMatchesAsync(IsEven.InvokeAsync, IsLengthEven.InvokeAsync)
                .ConfigureAwait(false));
        Assert.IsTrue(
            await EvenLengthRightEither.EitherMatchesAsync(IsEven.InvokeAsync, IsLengthEven.InvokeAsync)
                .ConfigureAwait(false));
        Assert.IsFalse(
            await OddLengthRightEither.EitherMatchesAsync(IsEven.InvokeAsync, IsLengthEven.InvokeAsync)
                .ConfigureAwait(false));
    }
    #endregion

    #region Only Left Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.EitherMatchesAsync(Func{TLeft, CancellationToken, Task{bool}}, Func{TRight, Task{bool}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestEitherMatchesAsync_BothAsync_LeftOnlyCancellable_NoCancellation()
    {
        Assert.IsTrue(
            await EvenLeftEither.EitherMatchesAsync(IsEven.InvokeCancellableAsync, IsLengthEven.InvokeAsync)
                .ConfigureAwait(false));
        Assert.IsFalse(
            await OddLeftEither.EitherMatchesAsync(IsEven.InvokeCancellableAsync, IsLengthEven.InvokeAsync)
                .ConfigureAwait(false));
        Assert.IsTrue(
            await EvenLengthRightEither.EitherMatchesAsync(IsEven.InvokeCancellableAsync, IsLengthEven.InvokeAsync)
                .ConfigureAwait(false));
        Assert.IsFalse(
            await OddLengthRightEither.EitherMatchesAsync(IsEven.InvokeCancellableAsync, IsLengthEven.InvokeAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.EitherMatchesAsync(Func{TLeft, CancellationToken, Task{bool}}, Func{TRight, Task{bool}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestEitherMatchesAsync_BothAsync_LeftOnlyCancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.EitherMatchesAsync(IsEven.InvokeCancellableAsync, IsLengthEven.InvokeAsync, ct),
                EvenLeftEither)
            .ConfigureAwait(false);
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.EitherMatchesAsync(IsEven.InvokeCancellableAsync, IsLengthEven.InvokeAsync, ct),
                OddLeftEither)
            .ConfigureAwait(false);
        Assert.IsTrue(
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.EitherMatchesAsync(IsEven.InvokeCancellableAsync, IsLengthEven.InvokeAsync, ct),
                    EvenLengthRightEither)
                .ConfigureAwait(false));
        Assert.IsFalse(
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.EitherMatchesAsync(IsEven.InvokeCancellableAsync, IsLengthEven.InvokeAsync, ct),
                    OddLengthRightEither)
                .ConfigureAwait(false));
    }
    #endregion

    #region Only Right Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.EitherMatchesAsync(Func{TLeft, Task{bool}}, Func{TRight, CancellationToken, Task{bool}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestEitherMatchesAsync_BothAsync_RightOnlyCancellable_NoCancellation()
    {
        Assert.IsTrue(
            await EvenLeftEither.EitherMatchesAsync(IsEven.InvokeAsync, IsLengthEven.InvokeCancellableAsync)
                .ConfigureAwait(false));
        Assert.IsFalse(
            await OddLeftEither.EitherMatchesAsync(IsEven.InvokeAsync, IsLengthEven.InvokeCancellableAsync)
                .ConfigureAwait(false));
        Assert.IsTrue(
            await EvenLengthRightEither.EitherMatchesAsync(IsEven.InvokeAsync, IsLengthEven.InvokeCancellableAsync)
                .ConfigureAwait(false));
        Assert.IsFalse(
            await OddLengthRightEither.EitherMatchesAsync(IsEven.InvokeAsync, IsLengthEven.InvokeCancellableAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.EitherMatchesAsync(Func{TLeft, Task{bool}}, Func{TRight, CancellationToken, Task{bool}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestEitherMatchesAsync_BothAsync_RightOnlyCancellable_Cancellation()
    {
        Assert.IsTrue(
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.EitherMatchesAsync(IsEven.InvokeAsync, IsLengthEven.InvokeCancellableAsync, ct),
                    EvenLeftEither)
                .ConfigureAwait(false));
        Assert.IsFalse(
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.EitherMatchesAsync(IsEven.InvokeAsync, IsLengthEven.InvokeCancellableAsync, ct),
                    OddLeftEither)
                .ConfigureAwait(false));

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.EitherMatchesAsync(IsEven.InvokeAsync, IsLengthEven.InvokeCancellableAsync, ct),
                EvenLengthRightEither)
            .ConfigureAwait(false);
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.EitherMatchesAsync(IsEven.InvokeAsync, IsLengthEven.InvokeCancellableAsync, ct),
                OddLengthRightEither)
            .ConfigureAwait(false);
    }
    #endregion

    #region Both Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.EitherMatchesAsync(Func{TLeft, CancellationToken, Task{bool}}, Func{TRight, CancellationToken, Task{bool}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestEitherMatchesAsync_BothAsyncCancellable_NoCancellation()
    {
        Assert.IsTrue(
            await EvenLeftEither.EitherMatchesAsync(IsEven.InvokeCancellableAsync, IsLengthEven.InvokeCancellableAsync)
                .ConfigureAwait(false));
        Assert.IsFalse(
            await OddLeftEither.EitherMatchesAsync(IsEven.InvokeCancellableAsync, IsLengthEven.InvokeCancellableAsync)
                .ConfigureAwait(false));
        Assert.IsTrue(
            await EvenLengthRightEither.EitherMatchesAsync(IsEven.InvokeCancellableAsync, IsLengthEven.InvokeCancellableAsync)
                .ConfigureAwait(false));
        Assert.IsFalse(
            await OddLengthRightEither.EitherMatchesAsync(IsEven.InvokeCancellableAsync, IsLengthEven.InvokeCancellableAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.EitherMatchesAsync(Func{TLeft, CancellationToken, Task{bool}}, Func{TRight, CancellationToken, Task{bool}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestEitherMatchesAsync_BothAsyncCancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.EitherMatchesAsync(
                                IsEven.InvokeCancellableAsync, IsLengthEven.InvokeCancellableAsync, ct),
                EvenLeftEither)
            .ConfigureAwait(false);
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.EitherMatchesAsync(
                                IsEven.InvokeCancellableAsync, IsLengthEven.InvokeCancellableAsync, ct),
                OddLeftEither)
            .ConfigureAwait(false);

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.EitherMatchesAsync(
                                IsEven.InvokeCancellableAsync, IsLengthEven.InvokeCancellableAsync, ct),
                EvenLengthRightEither)
            .ConfigureAwait(false);
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.EitherMatchesAsync(
                                IsEven.InvokeCancellableAsync, IsLengthEven.InvokeCancellableAsync, ct),
                OddLengthRightEither)
            .ConfigureAwait(false);
    }
    #endregion
    #endregion
    #endregion

    #region Right
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.RightMatchesAsync(Func{TRight, Task{bool}})"/> method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestRightMatchesAsync_NonCancellable()
    {
        Assert.IsTrue(await EvenLengthRightEither.RightMatchesAsync(IsLengthEven.InvokeAsync).ConfigureAwait(false));
        Assert.IsFalse(await OddLengthRightEither.RightMatchesAsync(IsLengthEven.InvokeAsync).ConfigureAwait(false));
        Assert.IsFalse(await EvenLeftEither.RightMatchesAsync(IsLengthEven.InvokeAsync).ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.RightMatchesAsync(Func{TRight, CancellationToken, Task{bool}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestRightMatchesAsync_Cancellable_NoCancellation()
    {
        Assert.IsTrue(
            await EvenLengthRightEither.RightMatchesAsync(IsLengthEven.InvokeCancellableAsync).ConfigureAwait(false));
        Assert.IsFalse(
            await OddLengthRightEither.RightMatchesAsync(IsLengthEven.InvokeCancellableAsync).ConfigureAwait(false));
        Assert.IsFalse(
            await EvenLeftEither.RightMatchesAsync(IsLengthEven.InvokeCancellableAsync).ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.RightMatchesAsync(Func{TRight, CancellationToken, Task{bool}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are used properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestRightMatchesAsync_Cancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.RightMatchesAsync(IsLengthEven.InvokeCancellableAsync, ct),
                EvenLengthRightEither)
            .ConfigureAwait(false);
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.RightMatchesAsync(IsLengthEven.InvokeCancellableAsync, ct),
                OddLengthRightEither)
            .ConfigureAwait(false);

        Assert.IsFalse(
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.RightMatchesAsync(IsLengthEven.InvokeCancellableAsync, ct),
                    EvenLeftEither)
                .ConfigureAwait(false));
    }
    #endregion
    #endregion
    #endregion

    #region Extension
    /// <summary>
    /// Tests the
    /// <see cref="EitherExtensions.MatchesEither{TLeft, TRight, TParent}(Either{TLeft, TRight}, Func{TParent, bool})"/>
    /// method.
    /// </summary>
    [TestMethod]
    public void TestMatchesEitherExtension()
    {
        Assert.IsTrue(PersonalEmailLeftEither.MatchesEither(IsPersonal.Delegate));
        Assert.IsFalse(NonPersonalEmailLeftEither.MatchesEither(IsPersonal.Delegate));
        Assert.IsTrue(PersonalPhoneRightEither.MatchesEither(IsPersonal.Delegate));
        Assert.IsFalse(NonPersonalPhoneRightEither.MatchesEither(IsPersonal.Delegate));
    }

    /// <summary>
    /// Tests the
    /// <see cref="EitherExtensions.MatchesEitherAsync{TLeft, TRight, TParent}(Either{TLeft, TRight}, Func{TParent, Task{bool}})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestMatchesEitherExtensionAsync_NonCancellable()
    {
        Assert.IsTrue(
            await PersonalEmailLeftEither.MatchesEitherAsync(IsPersonal.AsyncDelegate).ConfigureAwait(false));
        Assert.IsFalse(
            await NonPersonalEmailLeftEither.MatchesEitherAsync(IsPersonal.AsyncDelegate).ConfigureAwait(false));
        Assert.IsTrue(
            await PersonalPhoneRightEither.MatchesEitherAsync(IsPersonal.AsyncDelegate).ConfigureAwait(false));
        Assert.IsFalse(
            await NonPersonalPhoneRightEither.MatchesEitherAsync(IsPersonal.AsyncDelegate).ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="EitherExtensions.MatchesEitherAsync{TLeft, TRight, TParent}(Either{TLeft, TRight}, Func{TParent, CancellationToken, Task{bool}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestMatchesEitherExtensionAsync_Cancellable_NoCancellation()
    {
        Assert.IsTrue(
            await PersonalEmailLeftEither.MatchesEitherAsync(IsPersonal.CancellableAsyncDelegate)
                .ConfigureAwait(false));
        Assert.IsFalse(
            await NonPersonalEmailLeftEither.MatchesEitherAsync(IsPersonal.CancellableAsyncDelegate)
                .ConfigureAwait(false));
        Assert.IsTrue(
            await PersonalPhoneRightEither.MatchesEitherAsync(IsPersonal.CancellableAsyncDelegate)
                .ConfigureAwait(false));
        Assert.IsFalse(
            await NonPersonalPhoneRightEither.MatchesEitherAsync(IsPersonal.CancellableAsyncDelegate)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="EitherExtensions.MatchesEitherAsync{TLeft, TRight, TParent}(Either{TLeft, TRight}, Func{TParent, CancellationToken, Task{bool}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestMatchesEitherExtensionAsync_Cancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.MatchesEitherAsync(IsPersonal.CancellableAsyncDelegate, ct),
                PersonalEmailLeftEither)
            .ConfigureAwait(false);
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.MatchesEitherAsync(IsPersonal.CancellableAsyncDelegate, ct),
                NonPersonalEmailLeftEither)
            .ConfigureAwait(false);
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.MatchesEitherAsync(IsPersonal.CancellableAsyncDelegate, ct),
                PersonalPhoneRightEither)
            .ConfigureAwait(false);
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.MatchesEitherAsync(IsPersonal.CancellableAsyncDelegate, ct),
                NonPersonalPhoneRightEither)
            .ConfigureAwait(false);
    }
    #endregion
    #endregion

    #region Helper Predicates
    /// <summary>
    /// Determines whether or not the argument is even.
    /// </summary>
    /// <remarks>
    /// This predicate is used internally to test the relevant <see cref="Either{TLeft, TRight}"/> methods.
    /// </remarks>
    private static readonly FunctionOptions<int, bool> IsEven = new(i => i % 2 == 0);

    /// <summary>
    /// Determines whether or not the argument has an even length.
    /// </summary>
    /// <remarks>
    /// This predicate is used internally to test the relevant <see cref="Either{TLeft, TRight}"/> methods.
    /// </remarks>
    /// <returns></returns>
    private static readonly FunctionOptions<string, bool> IsLengthEven = new(s => s.Length % 2 == 0);

    /// <summary>
    /// Determines whether or not the argument represents personal information.
    /// </summary>
    /// <remarks>
    /// This predicate is used internally to test the relevant <see cref="Either{TLeft, TRight}"/> methods.
    /// </remarks>
    private static readonly FunctionOptions<ContactInformation, bool> IsPersonal = new(ci => ci.IsPersonal);
    #endregion
}
