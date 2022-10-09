using Rem.Core.Utilities.Monads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemTest.Core.Utilities.Monads;

/// <summary>
/// Tests the <see cref="Either{TLeft, TRight}.WhereRight(Func{TRight, bool})"/> and
/// <see cref="Either{TLeft, TRight}.WhereLeft(Func{TLeft, bool})"/> methods and related overloads.
/// </summary>
[TestClass]
public class WhereTest
{
    #region Constants
    /// <summary>
    /// An email constant to use to test the methods.
    /// </summary>
    private static readonly Email PersonalEmail = new("r@s.t", true),
                                  NonPersonalEmail = new("u@v.w", false),
                                  CompanyEmail = new("x@y.z", false);

    /// <summary>
    /// A phone constant to use to test the methods.
    /// </summary>
    private static readonly Phone PersonalPhone = new(3515550193, true),
                                  NonPersonalPhone = new(1075550166, false),
                                  CompanyPhone = new(1043550154, false);

    /// <summary>
    /// A default string value to use to test the methods.
    /// </summary>
    private const string DefaultString = "<Default Value>";
    #endregion

    #region Tests
    #region Synchronous
    #region Left
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.WhereLeft(Func{TLeft, bool})"/> method.
    /// </summary>
    [TestMethod]
    public void TestWhereLeft_Enumerable()
    {
        Assert.IsTrue(new[] { 2 }.SequenceEqual(Either<int, string>.New(2).WhereLeft(IsEven)));
        Assert.IsTrue(Array.Empty<int>().SequenceEqual(Either<int, string>.New(3).WhereLeft(IsEven)));
        Assert.IsTrue(Array.Empty<int>().SequenceEqual(Either<int, string>.New("").WhereLeft(IsEven)));
    }

    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.WhereLeft(Func{TLeft, bool}, TRight)"/> method.
    /// </summary>
    [TestMethod]
    public void TestWhereLeft_Either_Eager()
    {
        Assert.That.HasLeft(2, Either<int, string>.New(2).WhereLeft(IsEven, ""));
        Assert.That.HasRight("", Either<int, string>.New(3).WhereLeft(IsEven, ""));
        Assert.That.HasRight("s", Either<int, string>.New("s").WhereLeft(IsEven, ""));
    }

    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.WhereLeftLazy(Func{TLeft, bool}, Func{TRight})"/> method.
    /// </summary>
    [TestMethod]
    public void TestWhereLeft_Either_Lazy()
    {
        Assert.That.HasLeft(2, Either<int, string>.New(2).WhereLeftLazy(IsEven, GetDefaultString));
        Assert.That.HasRight(DefaultString, Either<int, string>.New(3).WhereLeftLazy(IsEven, GetDefaultString));
        Assert.That.HasRight("s", Either<int, string>.New("s").WhereLeftLazy(IsEven, GetDefaultString));
    }
    #endregion

    #region Either Side
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.WhereEither(Func{TLeft, bool}, Func{TRight, bool})"/> method.
    /// </summary>
    [TestMethod]
    public void TestWhereEither()
    {
        Assert.That.IsSingleton(2, Either<string, int>.New(2).WhereEither(LengthIsEven, IsEven));
        Assert.That.IsEmpty(Either<string, int>.New(3).WhereEither(LengthIsEven, IsEven));
        Assert.That.IsSingleton("", Either<string, int>.New("").WhereEither(LengthIsEven, IsEven));
        Assert.That.IsEmpty(Either<string, int>.New(" ").WhereEither(LengthIsEven, IsEven));
    }

    /// <summary>
    /// Tests the
    /// <see cref="EitherExtensions.WhereEither{TLeft, TRight, TParent}(Either{TLeft, TRight}, Func{TParent, bool})"/>
    /// method.
    /// </summary>
    [TestMethod]
    public void TestWhereEitherExtension()
    {
        Assert.That.IsSingleton(
            PersonalEmail, Either<Email, Phone>.New(PersonalEmail).WhereEither(IsPersonal.Delegate));
        Assert.That.IsEmpty(Either<Email, Phone>.New(NonPersonalEmail).WhereEither(IsPersonal.Delegate));
        Assert.That.IsSingleton(
            PersonalPhone, Either<Email, Phone>.New(PersonalPhone).WhereEither(IsPersonal.Delegate));
        Assert.That.IsEmpty(Either<Email, Phone>.New(NonPersonalPhone).WhereEither(IsPersonal.Delegate));
    }
    #endregion

    #region Right
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.WhereRight(Func{TRight, bool})"/> method.
    /// </summary>
    [TestMethod]
    public void TestWhereRight_Enumerable()
    {
        Assert.IsTrue(new[] { 2 }.SequenceEqual(Either<string, int>.New(2).WhereRight(IsEven)));
        Assert.IsTrue(Array.Empty<int>().SequenceEqual(Either<string, int>.New(3).WhereRight(IsEven)));
        Assert.IsTrue(Array.Empty<int>().SequenceEqual(Either<string, int>.New("").WhereRight(IsEven)));
    }

    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.WhereRight(Func{TRight, bool}, TLeft)"/> method.
    /// </summary>
    [TestMethod]
    public void TestWhereRight_Either_Eager()
    {
        Assert.That.HasRight(2, Either<string, int>.New(2).WhereRight(IsEven, ""));
        Assert.That.HasLeft("", Either<string, int>.New(3).WhereRight(IsEven, ""));
        Assert.That.HasLeft("s", Either<string, int>.New("s").WhereRight(IsEven, ""));
    }

    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.WhereRightLazy(Func{TRight, bool}, Func{TLeft})"/> method.
    /// </summary>
    [TestMethod]
    public void TestWhereRight_Either_Lazy()
    {
        Assert.That.HasRight(2, Either<string, int>.New(2).WhereRightLazy(IsEven, GetDefaultString));
        Assert.That.HasLeft(DefaultString, Either<string, int>.New(3).WhereRightLazy(IsEven, GetDefaultString));
        Assert.That.HasLeft("s", Either<string, int>.New("s").WhereRightLazy(IsEven, GetDefaultString));
    }
    #endregion
    #endregion

    #region Asynchronous
    #region Left
    #region Eager
    #region Enumerable (No Default Value)
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.WhereLeftAsync(Func{TLeft, Task{bool}})"/> method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereLeftAsync_Enumerable_NonCancellable()
    {
        Assert.That.IsSingleton(
            2, await Either<int, string>.New(2).WhereLeftAsync(IsEven.InvokeAsync).ConfigureAwait(false));
        Assert.That.IsEmpty(await Either<int, string>.New(3).WhereLeftAsync(IsEven.InvokeAsync).ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<int, string>.New("").WhereLeftAsync(IsEven.InvokeAsync).ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereLeftAsync(Func{TLeft, CancellationToken, Task{bool}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereLeftAsync_Enumerable_Cancellable_NoCancellation()
    {
        Assert.That.IsSingleton(
            2, await Either<int, string>.New(2).WhereLeftAsync(IsEven.InvokeCancellableAsync).ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<int, string>.New(3).WhereLeftAsync(IsEven.InvokeCancellableAsync).ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<int, string>.New("").WhereLeftAsync(IsEven.InvokeCancellableAsync).ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereLeftAsync(Func{TLeft, CancellationToken, Task{bool}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereLeftAsync_Enumerable_Cancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereLeftAsync(IsEven.InvokeCancellableAsync, ct),
                Either<int, string>.New(2))
            .ConfigureAwait(false);

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereLeftAsync(IsEven.InvokeCancellableAsync, ct),
                Either<int, string>.New(3))
            .ConfigureAwait(false);

        Assert.That.IsEmpty(
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.WhereLeftAsync(IsEven.InvokeCancellableAsync, ct),
                    Either<int, string>.New(""))
                .ConfigureAwait(false));
    }
    #endregion

    #region Either (Default Value)
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereLeftAsync(Func{TLeft, Task{bool}}, TRight)"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereLeftAsync_NonCancellable()
    {
        Assert.That.HasLeft(
            2,
            await Either<int, string>.New(2).WhereLeftAsync(IsEven.InvokeAsync, DefaultString)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            DefaultString,
            await Either<int, string>.New(3).WhereLeftAsync(IsEven.InvokeAsync, DefaultString)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            "s",
            await Either<int, string>.New("s").WhereLeftAsync(IsEven.InvokeAsync, DefaultString)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereLeftAsync(Func{TLeft, CancellationToken, Task{bool}}, TRight, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereLeftAsync_Cancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            2,
            await Either<int, string>.New(2)
                    .WhereLeftAsync(IsEven.InvokeCancellableAsync, DefaultString)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            DefaultString,
            await Either<int, string>.New(3)
                    .WhereLeftAsync(IsEven.InvokeCancellableAsync, DefaultString)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            "s",
            await Either<int, string>.New("s")
                    .WhereLeftAsync(IsEven.InvokeCancellableAsync, DefaultString)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereLeftAsync(Func{TLeft, CancellationToken, Task{bool}}, TRight, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereLeftAsync_Cancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereLeftAsync(IsEven.InvokeCancellableAsync, DefaultString, ct),
                Either<int, string>.New(2))
            .ConfigureAwait(false);

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereLeftAsync(IsEven.InvokeCancellableAsync, DefaultString, ct),
                Either<int, string>.New(3))
            .ConfigureAwait(false);

        Assert.That.HasRight(
            "s",
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.WhereLeftAsync(IsEven.InvokeCancellableAsync, DefaultString, ct),
                    Either<int, string>.New("s"))
                .ConfigureAwait(false));
    }
    #endregion
    #endregion

    #region Lazy
    #region Only Predicate Async
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereLeftLazyAsync(Func{TLeft, Task{bool}}, Func{TRight})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereLeftLazyAsync_OnlyPredicateAsync_NonCancellable()
    {
        Assert.That.HasLeft(
            2,
            await Either<int, string>.New(2).WhereLeftLazyAsync(IsEven.InvokeAsync, GetDefaultString.Invoke)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            DefaultString,
            await Either<int, string>.New(3).WhereLeftLazyAsync(IsEven.InvokeAsync, GetDefaultString.Invoke)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            "s",
            await Either<int, string>.New("s").WhereLeftLazyAsync(IsEven.InvokeAsync, GetDefaultString.Invoke)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereLeftLazyAsync(Func{TLeft, CancellationToken, Task{bool}}, Func{TRight}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereLeftLazyAsync_OnlyPredicateAsync_Cancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            2,
            await Either<int, string>.New(2)
                    .WhereLeftLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.Invoke)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            DefaultString,
            await Either<int, string>.New(3)
                    .WhereLeftLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.Invoke)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            "s",
            await Either<int, string>.New("s")
                    .WhereLeftLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.Invoke)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereLeftLazyAsync(Func{TLeft, CancellationToken, Task{bool}}, Func{TRight}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereLeftLazyAsync_OnlyPredicateAsync_Cancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereLeftLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.Invoke, ct),
                Either<int, string>.New(2))
            .ConfigureAwait(false);

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereLeftLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.Invoke, ct),
                Either<int, string>.New(3))
            .ConfigureAwait(false);

        Assert.That.HasRight(
            "s",
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.WhereLeftLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.Invoke, ct),
                    Either<int, string>.New("s"))
                .ConfigureAwait(false));
    }
    #endregion

    #region Only Factory Async
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereLeftLazyAsync(Func{TLeft, bool}, Func{Task{TRight}})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereLeftLazyAsync_OnlyFactoryAsync_NonCancellable()
    {
        Assert.That.HasLeft(
            2,
            await Either<int, string>.New(2).WhereLeftLazyAsync(IsEven.Invoke, GetDefaultString.InvokeAsync));

        Assert.That.HasRight(
            DefaultString,
            await Either<int, string>.New(3).WhereLeftLazyAsync(IsEven.Invoke, GetDefaultString.InvokeAsync));

        Assert.That.HasRight(
            "s",
            await Either<int, string>.New("s").WhereLeftLazyAsync(IsEven.Invoke, GetDefaultString.InvokeAsync));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereLeftLazyAsync(Func{TLeft, bool}, Func{CancellationToken, Task{TRight}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereLeftLazyAsync_OnlyFactoryAsync_Cancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            2,
            await Either<int, string>.New(2)
                    .WhereLeftLazyAsync(IsEven.Invoke, GetDefaultString.InvokeCancellableAsync)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            DefaultString,
            await Either<int, string>.New(3)
                    .WhereLeftLazyAsync(IsEven.Invoke, GetDefaultString.InvokeCancellableAsync)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            "s",
            await Either<int, string>.New("s")
                    .WhereLeftLazyAsync(IsEven.Invoke, GetDefaultString.InvokeCancellableAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereLeftLazyAsync(Func{TLeft, bool}, Func{CancellationToken, Task{TRight}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereLeftLazyAsync_OnlyFactoryAsync_Cancellable_Cancellation()
    {
        Assert.That.HasLeft(
            2,
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.WhereLeftLazyAsync(IsEven.Invoke, GetDefaultString.InvokeCancellableAsync, ct),
                    Either<int, string>.New(2))
                .ConfigureAwait(false));

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereLeftLazyAsync(IsEven.Invoke, GetDefaultString.InvokeCancellableAsync, ct),
                Either<int, string>.New(3))
            .ConfigureAwait(false);

        Assert.That.HasRight(
            "s",
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.WhereLeftLazyAsync(IsEven.Invoke, GetDefaultString.InvokeCancellableAsync, ct),
                    Either<int, string>.New("s"))
                .ConfigureAwait(false));
    }
    #endregion

    #region Both Async
    #region Non-Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereLeftLazyAsync(Func{TLeft, Task{bool}}, Func{Task{TRight}})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereLeftLazyAsync_BothAsync_NonCancellable()
    {
        Assert.That.HasLeft(
            2,
            await Either<int, string>.New(2).WhereLeftLazyAsync(IsEven.InvokeAsync, GetDefaultString.InvokeAsync));

        Assert.That.HasRight(
            DefaultString,
            await Either<int, string>.New(3).WhereLeftLazyAsync(IsEven.InvokeAsync, GetDefaultString.InvokeAsync));

        Assert.That.HasRight(
            "s",
            await Either<int, string>.New("s").WhereLeftLazyAsync(IsEven.InvokeAsync, GetDefaultString.InvokeAsync));
    }
    #endregion

    #region Only Predicate Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereLeftLazyAsync(Func{TLeft, CancellationToken, Task{bool}}, Func{Task{TRight}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereLeftLazyAsync_BothAsync_OnlyPredicateCancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            2,
            await Either<int, string>.New(2)
                    .WhereLeftLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.InvokeAsync)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            DefaultString,
            await Either<int, string>.New(3)
                    .WhereLeftLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.InvokeAsync)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            "s",
            await Either<int, string>.New("s")
                    .WhereLeftLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.InvokeAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereLeftLazyAsync(Func{TLeft, CancellationToken, Task{bool}}, Func{Task{TRight}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereLeftLazyAsync_BothAsync_OnlyPredicateCancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereLeftLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.InvokeAsync, ct),
                Either<int, string>.New(2))
            .ConfigureAwait(false);

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereLeftLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.InvokeAsync, ct),
                Either<int, string>.New(3))
            .ConfigureAwait(false);

        Assert.That.HasRight(
            "s",
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.WhereLeftLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.InvokeAsync, ct),
                    Either<int, string>.New("s"))
                .ConfigureAwait(false));
    }
    #endregion

    #region Only Factory Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereLeftLazyAsync(Func{TLeft, Task{bool}}, Func{CancellationToken, Task{TRight}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereLeftLazyAsync_BothAsync_OnlyFactoryCancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            2,
            await Either<int, string>.New(2)
                    .WhereLeftLazyAsync(IsEven.InvokeAsync, GetDefaultString.InvokeCancellableAsync)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            DefaultString,
            await Either<int, string>.New(3)
                    .WhereLeftLazyAsync(IsEven.InvokeAsync, GetDefaultString.InvokeCancellableAsync)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            "s",
            await Either<int, string>.New("s")
                    .WhereLeftLazyAsync(IsEven.InvokeAsync, GetDefaultString.InvokeCancellableAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereLeftLazyAsync(Func{TLeft, Task{bool}}, Func{CancellationToken, Task{TRight}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereLeftLazyAsync_BothAsync_OnlyFactoryCancellable_Cancellation()
    {
        Assert.That.HasLeft(
            2,
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.WhereLeftLazyAsync(IsEven.InvokeAsync, GetDefaultString.InvokeCancellableAsync, ct),
                    Either<int, string>.New(2))
                .ConfigureAwait(false));

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereLeftLazyAsync(IsEven.InvokeAsync, GetDefaultString.InvokeCancellableAsync, ct),
                Either<int, string>.New(3))
            .ConfigureAwait(false);

        Assert.That.HasRight(
            "s",
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.WhereLeftLazyAsync(IsEven.InvokeAsync, GetDefaultString.InvokeCancellableAsync, ct),
                    Either<int, string>.New("s"))
                .ConfigureAwait(false));
    }
    #endregion

    #region Both Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereLeftLazyAsync(Func{TLeft, Task{bool}}, Func{CancellationToken, Task{TRight}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereLeftLazyAsync_BothAsyncCancellable_NoCancellation()
    {
        Assert.That.HasLeft(
            2,
            await Either<int, string>.New(2)
                    .WhereLeftLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.InvokeCancellableAsync)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            DefaultString,
            await Either<int, string>.New(3)
                    .WhereLeftLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.InvokeCancellableAsync)
                .ConfigureAwait(false));

        Assert.That.HasRight(
            "s",
            await Either<int, string>.New("s")
                    .WhereLeftLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.InvokeCancellableAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereLeftLazyAsync(Func{TLeft, Task{bool}}, Func{CancellationToken, Task{TRight}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereLeftLazyAsync_BothAsyncCancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereLeftLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.InvokeCancellableAsync, ct),
                Either<int, string>.New(2))
            .ConfigureAwait(false);

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereLeftLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.InvokeCancellableAsync, ct),
                Either<int, string>.New(3))
            .ConfigureAwait(false);

        Assert.That.HasRight(
            "s",
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.WhereLeftLazyAsync(
                        IsEven.InvokeCancellableAsync, GetDefaultString.InvokeCancellableAsync, ct),
                    Either<int, string>.New("s"))
                .ConfigureAwait(false));
    }
    #endregion
    #endregion
    #endregion
    #endregion

    #region Either Side
    #region Instance
    #region Left Async Only
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereEitherAsync(Func{TLeft, Task{bool}}, Func{TRight, bool})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereEitherAsync_LeftAsyncOnly_NonCancellable()
    {
        Assert.That.IsSingleton(
            2,
            await Either<int, string>.New(2).WhereEitherAsync(IsEven.InvokeAsync, LengthIsEven.Invoke)
                .ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<int, string>.New(3).WhereEitherAsync(IsEven.InvokeAsync, LengthIsEven.Invoke)
                .ConfigureAwait(false));

        Assert.That.IsSingleton(
            "",
            await Either<int, string>.New("").WhereEitherAsync(IsEven.InvokeAsync, LengthIsEven.Invoke)
                .ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<int, string>.New(".").WhereEitherAsync(IsEven.InvokeAsync, LengthIsEven.Invoke)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereEitherAsync(Func{TLeft, CancellationToken, Task{bool}}, Func{TRight, bool}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereEitherAsync_LeftAsyncOnly_Cancellable_NoCancellation()
    {
        Assert.That.IsSingleton(
            2,
            await Either<int, string>.New(2).WhereEitherAsync(IsEven.InvokeCancellableAsync, LengthIsEven.Invoke)
                .ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<int, string>.New(3).WhereEitherAsync(IsEven.InvokeCancellableAsync, LengthIsEven.Invoke)
                .ConfigureAwait(false));

        Assert.That.IsSingleton(
            "",
            await Either<int, string>.New("").WhereEitherAsync(IsEven.InvokeCancellableAsync, LengthIsEven.Invoke)
                .ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<int, string>.New(".").WhereEitherAsync(IsEven.InvokeCancellableAsync, LengthIsEven.Invoke)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereEitherAsync(Func{TLeft, CancellationToken, Task{bool}}, Func{TRight, bool}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereEitherAsync_LeftAsyncOnly_Cancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereEitherAsync(IsEven.InvokeCancellableAsync, LengthIsEven.Invoke, ct),
                Either<int, string>.New(2))
            .ConfigureAwait(false);
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereEitherAsync(IsEven.InvokeCancellableAsync, LengthIsEven.Invoke, ct),
                Either<int, string>.New(3))
            .ConfigureAwait(false);

        Assert.That.IsSingleton(
            "",
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.WhereEitherAsync(IsEven.InvokeCancellableAsync, LengthIsEven.Invoke, ct),
                    Either<int, string>.New(""))
                .ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.WhereEitherAsync(IsEven.InvokeCancellableAsync, LengthIsEven.Invoke, ct),
                    Either<int, string>.New("."))
                .ConfigureAwait(false));
    }
    #endregion

    #region Right Async Only
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereEitherAsync(Func{TLeft, bool}, Func{TRight, Task{bool}})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereEitherAsync_RightAsyncOnly_NonCancellable()
    {
        Assert.That.IsSingleton(
            2,
            await Either<int, string>.New(2).WhereEitherAsync(IsEven.Invoke, LengthIsEven.InvokeAsync)
                .ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<int, string>.New(3).WhereEitherAsync(IsEven.Invoke, LengthIsEven.InvokeAsync)
                .ConfigureAwait(false));

        Assert.That.IsSingleton(
            "",
            await Either<int, string>.New("").WhereEitherAsync(IsEven.Invoke, LengthIsEven.InvokeAsync)
                .ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<int, string>.New(".").WhereEitherAsync(IsEven.Invoke, LengthIsEven.InvokeAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereEitherAsync(Func{TLeft, bool}, Func{TRight, CancellationToken, Task{bool}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereEitherAsync_RightAsyncOnly_Cancellable_NoCancellation()
    {
        Assert.That.IsSingleton(
            2,
            await Either<int, string>.New(2).WhereEitherAsync(IsEven.Invoke, LengthIsEven.InvokeCancellableAsync)
                .ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<int, string>.New(3).WhereEitherAsync(IsEven.Invoke, LengthIsEven.InvokeCancellableAsync)
                .ConfigureAwait(false));

        Assert.That.IsSingleton(
            "",
            await Either<int, string>.New("").WhereEitherAsync(IsEven.Invoke, LengthIsEven.InvokeCancellableAsync)
                .ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<int, string>.New(".").WhereEitherAsync(IsEven.Invoke, LengthIsEven.InvokeCancellableAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereEitherAsync(Func{TLeft, bool}, Func{TRight, CancellationToken, Task{bool}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereEitherAsync_RightAsyncOnly_Cancellable_Cancellation()
    {
        Assert.That.IsSingleton(
            2,
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.WhereEitherAsync(IsEven.Invoke, LengthIsEven.InvokeCancellableAsync, ct),
                    Either<int, string>.New(2))
                .ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.WhereEitherAsync(IsEven.Invoke, LengthIsEven.InvokeCancellableAsync, ct),
                    Either<int, string>.New(3))
                .ConfigureAwait(false));

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereEitherAsync(IsEven.Invoke, LengthIsEven.InvokeCancellableAsync, ct),
                Either<int, string>.New(""))
            .ConfigureAwait(false);
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereEitherAsync(IsEven.Invoke, LengthIsEven.InvokeCancellableAsync, ct),
                Either<int, string>.New("."))
            .ConfigureAwait(false);
    }
    #endregion

    #region Both Async
    #region Non-Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereEitherAsync(Func{TLeft, Task{bool}}, Func{TRight, Task{bool}})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereEitherAsync_BothAsync_NonCancellable()
    {
        Assert.That.IsSingleton(
            2,
            await Either<int, string>.New(2).WhereEitherAsync(IsEven.Invoke, LengthIsEven.InvokeAsync)
                .ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<int, string>.New(3).WhereEitherAsync(IsEven.Invoke, LengthIsEven.InvokeAsync)
                .ConfigureAwait(false));

        Assert.That.IsSingleton(
            "",
            await Either<int, string>.New("").WhereEitherAsync(IsEven.Invoke, LengthIsEven.InvokeAsync)
                .ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<int, string>.New(".").WhereEitherAsync(IsEven.Invoke, LengthIsEven.InvokeAsync)
                .ConfigureAwait(false));
    }
    #endregion

    #region Only Left Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereEitherAsync(Func{TLeft, CancellationToken, Task{bool}}, Func{TRight, Task{bool}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereEitherAsync_BothAsync_OnlyLeftCancellable_NoCancellation()
    {
        Assert.That.IsSingleton(
            2,
            await Either<int, string>.New(2).WhereEitherAsync(IsEven.InvokeCancellableAsync, LengthIsEven.InvokeAsync)
                .ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<int, string>.New(3).WhereEitherAsync(IsEven.InvokeCancellableAsync, LengthIsEven.InvokeAsync)
                .ConfigureAwait(false));

        Assert.That.IsSingleton(
            "",
            await Either<int, string>.New("").WhereEitherAsync(IsEven.InvokeCancellableAsync, LengthIsEven.InvokeAsync)
                .ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<int, string>.New(".")
                    .WhereEitherAsync(IsEven.InvokeCancellableAsync, LengthIsEven.InvokeAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereEitherAsync(Func{TLeft, CancellationToken, Task{bool}}, Func{TRight, Task{bool}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are used properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereEitherAsync_BothAsync_OnlyLeftCancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereEitherAsync(IsEven.InvokeCancellableAsync, LengthIsEven.InvokeAsync, ct),
                Either<int, string>.New(2))
            .ConfigureAwait(false);
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereEitherAsync(IsEven.InvokeCancellableAsync, LengthIsEven.InvokeAsync, ct),
                Either<int, string>.New(3))
            .ConfigureAwait(false);

        Assert.That.IsSingleton(
            "",
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.WhereEitherAsync(IsEven.InvokeCancellableAsync, LengthIsEven.InvokeAsync, ct),
                    Either<int, string>.New(""))
                .ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.WhereEitherAsync(IsEven.InvokeCancellableAsync, LengthIsEven.InvokeAsync, ct),
                    Either<int, string>.New("."))
                .ConfigureAwait(false));
    }
    #endregion

    #region Only Right Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereEitherAsync(Func{TLeft, Task{bool}}, Func{TRight, CancellationToken, Task{bool}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereEitherAsync_BothAsync_OnlyRightCancellable_NoCancellation()
    {
        Assert.That.IsSingleton(
            2,
            await Either<int, string>.New(2).WhereEitherAsync(IsEven.InvokeAsync, LengthIsEven.InvokeCancellableAsync)
                .ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<int, string>.New(3).WhereEitherAsync(IsEven.InvokeAsync, LengthIsEven.InvokeCancellableAsync)
                .ConfigureAwait(false));

        Assert.That.IsSingleton(
            "",
            await Either<int, string>.New("").WhereEitherAsync(IsEven.InvokeAsync, LengthIsEven.InvokeCancellableAsync)
                .ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<int, string>.New(".").WhereEitherAsync(IsEven.InvokeAsync, LengthIsEven.InvokeCancellableAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereEitherAsync(Func{TLeft, bool}, Func{TRight, CancellationToken, Task{bool}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereEitherAsync_BothAsync_OnlyRightCancellable_Cancellation()
    {
        Assert.That.IsSingleton(
            2,
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.WhereEitherAsync(IsEven.InvokeAsync, LengthIsEven.InvokeCancellableAsync, ct),
                    Either<int, string>.New(2))
                .ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.WhereEitherAsync(IsEven.InvokeAsync, LengthIsEven.InvokeCancellableAsync, ct),
                    Either<int, string>.New(3))
                .ConfigureAwait(false));

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereEitherAsync(IsEven.InvokeAsync, LengthIsEven.InvokeCancellableAsync, ct),
                Either<int, string>.New(""))
            .ConfigureAwait(false);
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereEitherAsync(IsEven.InvokeAsync, LengthIsEven.InvokeCancellableAsync, ct),
                Either<int, string>.New("."))
            .ConfigureAwait(false);
    }
    #endregion

    #region Both Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereEitherAsync(Func{TLeft, CancellationToken, Task{bool}}, Func{TRight, CancellationToken, Task{bool}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereEitherAsync_BothAsyncCancellable_NoCancellation()
    {
        Assert.That.IsSingleton(
            2,
            await Either<int, string>.New(2)
                    .WhereEitherAsync(IsEven.InvokeCancellableAsync, LengthIsEven.InvokeCancellableAsync)
                .ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<int, string>.New(3)
                    .WhereEitherAsync(IsEven.InvokeCancellableAsync, LengthIsEven.InvokeCancellableAsync)
                .ConfigureAwait(false));

        Assert.That.IsSingleton(
            "",
            await Either<int, string>.New("")
                    .WhereEitherAsync(IsEven.InvokeCancellableAsync, LengthIsEven.InvokeCancellableAsync)
                .ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<int, string>.New(".")
                    .WhereEitherAsync(IsEven.InvokeCancellableAsync, LengthIsEven.InvokeCancellableAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereEitherAsync(Func{TLeft, bool}, Func{TRight, CancellationToken, Task{bool}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereEitherAsync_BothAsyncCancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereEitherAsync(IsEven.InvokeCancellableAsync, LengthIsEven.InvokeCancellableAsync, ct),
                Either<int, string>.New(2))
            .ConfigureAwait(false);
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereEitherAsync(IsEven.InvokeCancellableAsync, LengthIsEven.InvokeCancellableAsync, ct),
                Either<int, string>.New(3))
            .ConfigureAwait(false);

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereEitherAsync(IsEven.InvokeCancellableAsync, LengthIsEven.InvokeCancellableAsync, ct),
                Either<int, string>.New(""))
            .ConfigureAwait(false);
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereEitherAsync(IsEven.InvokeCancellableAsync, LengthIsEven.InvokeCancellableAsync, ct),
                Either<int, string>.New("."))
            .ConfigureAwait(false);
    }
    #endregion
    #endregion
    #endregion

    #region Extension
    /// <summary>
    /// Tests the
    /// <see cref="EitherExtensions.WhereEitherAsync{TLeft, TRight, TParent}(Either{TLeft, TRight}, Func{TParent, Task{bool}})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereEitherExtensionAsync_NonCancellable()
    {
        Assert.That.IsSingleton(
            PersonalEmail,
            await Either<Email, Phone>.New(PersonalEmail).WhereEitherAsync(IsPersonal.AsyncDelegate)
                .ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<Email, Phone>.New(NonPersonalEmail).WhereEitherAsync(IsPersonal.AsyncDelegate)
                .ConfigureAwait(false));

        Assert.That.IsSingleton(
            PersonalPhone,
            await Either<Email, Phone>.New(PersonalPhone).WhereEitherAsync(IsPersonal.AsyncDelegate)
                .ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<Email, Phone>.New(NonPersonalPhone).WhereEitherAsync(IsPersonal.AsyncDelegate)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="EitherExtensions.WhereEitherAsync{TLeft, TRight, TParent}(Either{TLeft, TRight}, Func{TParent, CancellationToken, Task{bool}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereEitherExtensionAsync_Cancellable_NoCancellation()
    {
        Assert.That.IsSingleton(
            PersonalEmail,
            await Either<Email, Phone>.New(PersonalEmail).WhereEitherAsync(IsPersonal.CancellableAsyncDelegate)
                .ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<Email, Phone>.New(NonPersonalEmail).WhereEitherAsync(IsPersonal.CancellableAsyncDelegate)
                .ConfigureAwait(false));

        Assert.That.IsSingleton(
            PersonalPhone,
            await Either<Email, Phone>.New(PersonalPhone).WhereEitherAsync(IsPersonal.CancellableAsyncDelegate)
                .ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<Email, Phone>.New(NonPersonalPhone).WhereEitherAsync(IsPersonal.CancellableAsyncDelegate)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="EitherExtensions.WhereEitherAsync{TLeft, TRight, TParent}(Either{TLeft, TRight}, Func{TParent, CancellationToken, Task{bool}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereEitherExtensionAsync_Cancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereEitherAsync(IsPersonal.CancellableAsyncDelegate, ct),
                Either<Email, Phone>.New(PersonalEmail))
            .ConfigureAwait(false);
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereEitherAsync(IsPersonal.CancellableAsyncDelegate, ct),
                Either<Email, Phone>.New(PersonalPhone))
            .ConfigureAwait(false);
    }
    #endregion
    #endregion

    #region Right
    #region Eager
    #region Enumerable (No Default Value)
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.WhereRightAsync(Func{TRight, Task{bool}})"/> method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereRightAsync_Enumerable_NonCancellable()
    {
        Assert.That.IsEmpty(await Either<int, string>.New(2).WhereRightAsync(LengthIsEven.InvokeAsync).ConfigureAwait(false));
        Assert.That.IsSingleton(
            "", await Either<int, string>.New("").WhereRightAsync(LengthIsEven.InvokeAsync).ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<int, string>.New(".").WhereRightAsync(LengthIsEven.InvokeAsync).ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TRight, TRight}.WhereRightAsync(Func{TRight, CancellationToken, Task{bool}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereRightAsync_Enumerable_Cancellable_NoCancellation()
    {
        Assert.That.IsEmpty(await Either<int, string>.New(2).WhereRightAsync(LengthIsEven.InvokeAsync).ConfigureAwait(false));
        Assert.That.IsSingleton(
            "", await Either<int, string>.New("").WhereRightAsync(LengthIsEven.InvokeAsync).ConfigureAwait(false));
        Assert.That.IsEmpty(
            await Either<int, string>.New(".").WhereRightAsync(LengthIsEven.InvokeAsync).ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TRight, TRight}.WhereRightAsync(Func{TRight, CancellationToken, Task{bool}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereRightAsync_Enumerable_Cancellable_Cancellation()
    {
        Assert.That.IsEmpty(
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.WhereRightAsync(LengthIsEven.InvokeCancellableAsync, ct),
                    Either<int, string>.New(2))
                .ConfigureAwait(false));

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereRightAsync(LengthIsEven.InvokeCancellableAsync, ct),
                Either<int, string>.New(""))
            .ConfigureAwait(false);

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereRightAsync(LengthIsEven.InvokeCancellableAsync, ct),
                Either<int, string>.New("."))
            .ConfigureAwait(false);
    }
    #endregion

    #region Either (Default Value)
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereRightAsync(Func{TRight, Task{bool}}, TLeft)"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereRightAsync_NonCancellable()
    {
        Assert.That.HasRight(
            2,
            await Either<string, int>.New(2).WhereRightAsync(IsEven.InvokeAsync, DefaultString)
                .ConfigureAwait(false));

        Assert.That.HasLeft(
            DefaultString,
            await Either<string, int>.New(3).WhereRightAsync(IsEven.InvokeAsync, DefaultString)
                .ConfigureAwait(false));

        Assert.That.HasLeft(
            "s",
            await Either<string, int>.New("s").WhereRightAsync(IsEven.InvokeAsync, DefaultString)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereRightAsync(Func{TRight, CancellationToken, Task{bool}}, TLeft, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereRightAsync_Cancellable_NoCancellation()
    {
        Assert.That.HasRight(
            2,
            await Either<string, int>.New(2)
                    .WhereRightAsync(IsEven.InvokeCancellableAsync, DefaultString)
                .ConfigureAwait(false));

        Assert.That.HasLeft(
            DefaultString,
            await Either<string, int>.New(3)
                    .WhereRightAsync(IsEven.InvokeCancellableAsync, DefaultString)
                .ConfigureAwait(false));

        Assert.That.HasLeft(
            "s",
            await Either<string, int>.New("s")
                    .WhereRightAsync(IsEven.InvokeCancellableAsync, DefaultString)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereRightAsync(Func{TRight, CancellationToken, Task{bool}}, TLeft, CancellationToken)"/>
    /// method, cancelling it to ensure cancellation tokens are used properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereRightAsync_Cancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereRightAsync(IsEven.InvokeCancellableAsync, DefaultString, ct),
                Either<string, int>.New(2))
            .ConfigureAwait(false);

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereRightAsync(IsEven.InvokeCancellableAsync, DefaultString, ct),
                Either<string, int>.New(3))
            .ConfigureAwait(false);

        Assert.That.HasLeft(
            "s",
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.WhereRightAsync(IsEven.InvokeCancellableAsync, DefaultString, ct),
                    Either<string, int>.New("s"))
                .ConfigureAwait(false));
    }
    #endregion
    #endregion

    #region Lazy
    #region Only Predicate Async
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereRightLazyAsync(Func{TRight, Task{bool}}, Func{TLeft})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereRightLazyAsync_OnlyPredicateAsync_NonCancellable()
    {
        Assert.That.HasRight(
            2,
            await Either<string, int>.New(2).WhereRightLazyAsync(IsEven.InvokeAsync, GetDefaultString.Invoke)
                .ConfigureAwait(false));

        Assert.That.HasLeft(
            DefaultString,
            await Either<string, int>.New(3).WhereRightLazyAsync(IsEven.InvokeAsync, GetDefaultString.Invoke)
                .ConfigureAwait(false));

        Assert.That.HasLeft(
            "s",
            await Either<string, int>.New("s").WhereRightLazyAsync(IsEven.InvokeAsync, GetDefaultString.Invoke)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereRightLazyAsync(Func{TRight, CancellationToken, Task{bool}}, Func{TLeft}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereRightLazyAsync_OnlyPredicateAsync_Cancellable_NoCancellation()
    {
        Assert.That.HasRight(
            2,
            await Either<string, int>.New(2)
                    .WhereRightLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.Invoke)
                .ConfigureAwait(false));

        Assert.That.HasLeft(
            DefaultString,
            await Either<string, int>.New(3)
                    .WhereRightLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.Invoke)
                .ConfigureAwait(false));

        Assert.That.HasLeft(
            "s",
            await Either<string, int>.New("s")
                    .WhereRightLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.Invoke)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereRightLazyAsync(Func{TRight, CancellationToken, Task{bool}}, Func{TLeft}, CancellationToken)"/>
    /// method, cancelling it to ensure cancellation tokens are used properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereRightLazyAsync_OnlyPredicateAsync_Cancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereRightLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.Invoke, ct),
                Either<string, int>.New(2))
            .ConfigureAwait(false);

        Assert.That.HasLeft(
            DefaultString,
            await Either<string, int>.New(3)
                    .WhereRightLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.Invoke)
                .ConfigureAwait(false));

        Assert.That.HasLeft(
            "s",
            await Either<string, int>.New("s")
                    .WhereRightLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.Invoke)
                .ConfigureAwait(false));
    }
    #endregion

    #region Only Factory Async
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereRightLazyAsync(Func{TRight, bool}, Func{Task{TLeft}})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereRightLazyAsync_OnlyFactoryAsync_NonCancellable()
    {
        Assert.That.HasRight(
            2,
            await Either<string, int>.New(2).WhereRightLazyAsync(IsEven.Invoke, GetDefaultString.InvokeAsync)
                .ConfigureAwait(false));

        Assert.That.HasLeft(
            DefaultString,
            await Either<string, int>.New(3).WhereRightLazyAsync(IsEven.Invoke, GetDefaultString.InvokeAsync)
                .ConfigureAwait(false));

        Assert.That.HasLeft(
            "s",
            await Either<string, int>.New("s").WhereRightLazyAsync(IsEven.Invoke, GetDefaultString.InvokeAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereRightLazyAsync(Func{TRight, bool}, Func{CancellationToken, Task{TLeft}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereRightLazyAsync_OnlyFactoryAsync_Cancellable_NoCancellation()
    {
        Assert.That.HasRight(
            2,
            await Either<string, int>.New(2)
                    .WhereRightLazyAsync(IsEven.Invoke, GetDefaultString.InvokeCancellableAsync)
                .ConfigureAwait(false));

        Assert.That.HasLeft(
            DefaultString,
            await Either<string, int>.New(3)
                    .WhereRightLazyAsync(IsEven.Invoke, GetDefaultString.InvokeCancellableAsync)
                .ConfigureAwait(false));

        Assert.That.HasLeft(
            "s",
            await Either<string, int>.New("s")
                    .WhereRightLazyAsync(IsEven.Invoke, GetDefaultString.InvokeCancellableAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereRightLazyAsync(Func{TRight, bool}, Func{CancellationToken, Task{TLeft}}, CancellationToken)"/>
    /// method, cancelling it to ensure cancellation tokens are used properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereRightLazyAsync_OnlyFactoryAsync_Cancellable_Cancellation()
    {
        Assert.That.HasRight(
            2,
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.WhereRightLazyAsync(IsEven.Invoke, GetDefaultString.InvokeCancellableAsync, ct),
                    Either<string, int>.New(2))
                .ConfigureAwait(false));

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereRightLazyAsync(IsEven.Invoke, GetDefaultString.InvokeCancellableAsync, ct),
                Either<string, int>.New(3))
            .ConfigureAwait(false);

        Assert.That.HasLeft(
            "s",
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.WhereRightLazyAsync(IsEven.Invoke, GetDefaultString.InvokeCancellableAsync, ct),
                    Either<string, int>.New("s"))
                .ConfigureAwait(false));
    }
    #endregion

    #region Both Async
    #region Non-Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereRightLazyAsync(Func{TRight, Task{bool}}, Func{Task{TLeft}})"/>
    /// method.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereRightLazyAsync_BothAsync_NonCancellable()
    {
        Assert.That.HasRight(
            2,
            await Either<string, int>.New(2).WhereRightLazyAsync(IsEven.InvokeAsync, GetDefaultString.InvokeAsync)
                .ConfigureAwait(false));

        Assert.That.HasLeft(
            DefaultString,
            await Either<string, int>.New(3).WhereRightLazyAsync(IsEven.InvokeAsync, GetDefaultString.InvokeAsync)
                .ConfigureAwait(false));

        Assert.That.HasLeft(
            "s",
            await Either<string, int>.New("s").WhereRightLazyAsync(IsEven.InvokeAsync, GetDefaultString.InvokeAsync)
                .ConfigureAwait(false));
    }
    #endregion

    #region Only Predicate Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereRightLazyAsync(Func{TRight, CancellationToken, Task{bool}}, Func{Task{TLeft}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereRightLazyAsync_BothAsync_OnlyPredicateCancellable_NoCancellation()
    {
        Assert.That.HasRight(
            2,
            await Either<string, int>.New(2)
                    .WhereRightLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.InvokeAsync)
                .ConfigureAwait(false));

        Assert.That.HasLeft(
            DefaultString,
            await Either<string, int>.New(3)
                    .WhereRightLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.InvokeAsync)
                .ConfigureAwait(false));

        Assert.That.HasLeft(
            "s",
            await Either<string, int>.New("s")
                    .WhereRightLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.InvokeAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereRightLazyAsync(Func{TRight, CancellationToken, Task{bool}}, Func{Task{TLeft}}, CancellationToken)"/>
    /// method, cancelling it to ensure that cancellation tokens are handled properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereRightLazyAsync_BothAsync_OnlyPredicateCancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereRightLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.InvokeAsync, ct),
                Either<string, int>.New(2))
            .ConfigureAwait(false);

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereRightLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.InvokeAsync, ct),
                Either<string, int>.New(3))
            .ConfigureAwait(false);

        Assert.That.HasLeft(
            "s",
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.WhereRightLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.InvokeAsync, ct),
                    Either<string, int>.New("s"))
                .ConfigureAwait(false));
    }
    #endregion

    #region Only Factory Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereRightLazyAsync(Func{TRight, Task{bool}}, Func{CancellationToken, Task{TLeft}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereRightLazyAsync_BothAsync_OnlyFactoryCancellable_NoCancellation()
    {
        Assert.That.HasRight(
            2,
            await Either<string, int>.New(2)
                    .WhereRightLazyAsync(IsEven.InvokeAsync, GetDefaultString.InvokeCancellableAsync)
                .ConfigureAwait(false));

        Assert.That.HasLeft(
            DefaultString,
            await Either<string, int>.New(3)
                    .WhereRightLazyAsync(IsEven.InvokeAsync, GetDefaultString.InvokeCancellableAsync)
                .ConfigureAwait(false));

        Assert.That.HasLeft(
            "s",
            await Either<string, int>.New("s")
                    .WhereRightLazyAsync(IsEven.InvokeAsync, GetDefaultString.InvokeCancellableAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereRightLazyAsync(Func{TRight, Task{bool}}, Func{CancellationToken, Task{TLeft}}, CancellationToken)"/>
    /// method, cancelling it to ensure cancellation tokens are used properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereRightLazyAsync_BothAsync_OnlyFactoryCancellable_Cancellation()
    {
        Assert.That.HasRight(
            2,
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.WhereRightLazyAsync(IsEven.InvokeAsync, GetDefaultString.InvokeCancellableAsync, ct),
                    Either<string, int>.New(2))
                .ConfigureAwait(false));

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereRightLazyAsync(IsEven.InvokeAsync, GetDefaultString.InvokeCancellableAsync, ct),
                Either<string, int>.New(3))
            .ConfigureAwait(false);

        Assert.That.HasLeft(
            "s",
            await Assert.That.IsNotCanceledAsync(
                    (e, ct) => e.WhereRightLazyAsync(IsEven.InvokeAsync, GetDefaultString.InvokeCancellableAsync, ct),
                    Either<string, int>.New("s"))
                .ConfigureAwait(false));
    }
    #endregion

    #region Both Cancellable
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereRightLazyAsync(Func{TRight, CancellationToken, Task{bool}}, Func{CancellationToken, Task{TLeft}}, CancellationToken)"/>
    /// method without cancelling it.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereRightLazyAsync_BothAsyncCancellable_NoCancellation()
    {
        Assert.That.HasRight(
            2,
            await Either<string, int>.New(2)
                    .WhereRightLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.InvokeCancellableAsync)
                .ConfigureAwait(false));

        Assert.That.HasLeft(
            DefaultString,
            await Either<string, int>.New(3)
                    .WhereRightLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.InvokeCancellableAsync)
                .ConfigureAwait(false));

        Assert.That.HasLeft(
            "s",
            await Either<string, int>.New("s")
                    .WhereRightLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.InvokeCancellableAsync)
                .ConfigureAwait(false));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.WhereRightLazyAsync(Func{TRight, CancellationToken, Task{bool}}, Func{CancellationToken, Task{TLeft}}, CancellationToken)"/>
    /// method, cancelling it to ensure cancellation tokens are used properly.
    /// </summary>
    /// <returns></returns>
    [TestMethod]
    public async Task TestWhereRightLazyAsync_BothAsyncCancellable_Cancellation()
    {
        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereRightLazyAsync(
                                IsEven.InvokeCancellableAsync, GetDefaultString.InvokeCancellableAsync, ct),
                Either<string, int>.New(2))
            .ConfigureAwait(false);

        await Assert.That.IsCanceledAsync(
                (e, ct) => e.WhereRightLazyAsync(
                                IsEven.InvokeCancellableAsync, GetDefaultString.InvokeCancellableAsync, ct),
                Either<string, int>.New(3))
            .ConfigureAwait(false);

        Assert.That.HasLeft(
            "s",
            await Either<string, int>.New("s")
                    .WhereRightLazyAsync(IsEven.InvokeCancellableAsync, GetDefaultString.InvokeCancellableAsync)
                .ConfigureAwait(false));
    }
    #endregion
    #endregion
    #endregion
    #endregion
    #endregion
    #endregion

    #region Helpers
    #region Predicates
    /// <summary>
    /// Determines if the given integer is even.
    /// </summary>
    /// <remarks>
    /// This predicate is used internally to test the methods.
    /// </remarks>
    /// <returns></returns>
    private static readonly FunctionOptions<int, bool> IsEven = new(i => i % 2 == 0);

    /// <summary>
    /// Determines if the length of the given string is even.
    /// </summary>
    /// <remarks>
    /// This predicate is used internally to test the methods.
    /// </remarks>
    /// <returns></returns>
    private static readonly FunctionOptions<string, bool> LengthIsEven = new(s => s.Length % 2 == 0);

    /// <summary>
    /// A function that determines whether or not the contact info passed in is personal.
    /// </summary>
    /// <remarks>
    /// This predicate is used internally to test the methods.
    /// <para/>
    /// The delegate definition is needed to perform the requisite tests with type inference, so is defined this way
    /// as a convenience.
    /// </remarks>
    private static readonly FunctionOptions<ContactInformation, bool> IsPersonal = new(ci => ci.IsPersonal);
    #endregion

    #region Factories
    /// <summary>
    /// A factory that gets <see cref="DefaultString"/>.
    /// </summary>
    /// <remarks>
    /// This factory is used internally to test the methods.
    /// </remarks>
    private static readonly FunctionOptions<string> GetDefaultString = new(() => DefaultString);
    #endregion
    #endregion
}
