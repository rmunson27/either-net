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
    /// Tests the <see cref="Either{TLeft, TRight}.WhereLeft(Func{TLeft, bool}, Func{TRight})"/> method.
    /// </summary>
    [TestMethod]
    public void TestWhereLeft_Either_Lazy()
    {
        Assert.That.HasLeft(2, Either<int, string>.New(2).WhereLeft(IsEven, GetDefaultString));
        Assert.That.HasRight(DefaultString, Either<int, string>.New(3).WhereLeft(IsEven, GetDefaultString));
        Assert.That.HasRight("s", Either<int, string>.New("s").WhereLeft(IsEven, GetDefaultString));
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
    /// Tests the <see cref="Either{TLeft, TRight}.WhereRight(Func{TRight, bool}, Func{TLeft})"/> method.
    /// </summary>
    [TestMethod]
    public void TestWhereRight_Either_Lazy()
    {
        Assert.That.HasRight(2, Either<string, int>.New(2).WhereRight(IsEven, GetDefaultString));
        Assert.That.HasLeft(DefaultString, Either<string, int>.New(3).WhereRight(IsEven, GetDefaultString));
        Assert.That.HasLeft("s", Either<string, int>.New("s").WhereRight(IsEven, GetDefaultString));
    }
    #endregion
    #endregion

    #region Asynchronous
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
