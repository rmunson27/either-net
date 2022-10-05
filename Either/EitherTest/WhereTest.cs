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
    #endregion

    #region Tests
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
        const string DefaultValue = "<DefaultValue>";
        static string defaultValueFactory() => DefaultValue;

        Assert.That.HasLeft(2, Either<int, string>.New(2).WhereLeft(IsEven, defaultValueFactory));
        Assert.That.HasRight(DefaultValue, Either<int, string>.New(3).WhereLeft(IsEven, defaultValueFactory));
        Assert.That.HasRight("s", Either<int, string>.New("s").WhereLeft(IsEven, defaultValueFactory));
    }
    #endregion

    #region Either Side
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.WhereEither(Func{TLeft, bool}, Func{TRight, bool})"/> method.
    /// </summary>
    [TestMethod]
    public void TestWhereEither()
    {
        Assert.That.SequenceEqual(new[] { 2 }, Either<string, int>.New(2).WhereEither(LengthIsEven, IsEven));
        Assert.IsFalse(Either<string, int>.New(3).WhereEither(LengthIsEven, IsEven).Cast<object>().Any());
        Assert.That.SequenceEqual(new[] { "" }, Either<string, int>.New("").WhereEither(LengthIsEven, IsEven));
        Assert.IsFalse(Either<string, int>.New(" ").WhereEither(LengthIsEven, IsEven).Cast<object>().Any());
    }

    /// <summary>
    /// Tests the
    /// <see cref="EitherExtensions.WhereEither{TLeft, TRight, TParent}(Either{TLeft, TRight}, Func{TParent, bool})"/>
    /// method.
    /// </summary>
    [TestMethod]
    public void TestWhereEitherExtension()
    {
        Assert.That.SequenceEqual(new[] { PersonalEmail }, Either<Email, Phone>.New(PersonalEmail).WhereEither(IsPersonal));
        Assert.IsFalse(Either<Email, Phone>.New(NonPersonalEmail).WhereEither(IsPersonal).Any());
        Assert.That.SequenceEqual(new[] { PersonalPhone }, Either<Email, Phone>.New(PersonalPhone).WhereEither(IsPersonal));
        Assert.IsFalse(Either<Email, Phone>.New(NonPersonalPhone).WhereEither(IsPersonal).Any());
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
        const string DefaultValue = "<DefaultValue>";
        static string defaultValueFactory() => DefaultValue;

        Assert.That.HasRight(2, Either<string, int>.New(2).WhereRight(IsEven, defaultValueFactory));
        Assert.That.HasLeft(DefaultValue, Either<string, int>.New(3).WhereRight(IsEven, defaultValueFactory));
        Assert.That.HasLeft("s", Either<string, int>.New("s").WhereRight(IsEven, defaultValueFactory));
    }
    #endregion
    #endregion

    #region Helpers
    #region Methods And Functions
    #region Predicates
    /// <summary>
    /// Determines if the given integer is even.
    /// </summary>
    /// <remarks>
    /// This predicate is used internally to test the methods.
    /// </remarks>
    /// <param name="i"></param>
    /// <returns></returns>
    private static bool IsEven(int i) => i % 2 == 0;

    /// <summary>
    /// Determines if the length of the given string is even.
    /// </summary>
    /// <remarks>
    /// This predicate is used internally to test the methods.
    /// </remarks>
    /// <param name="s"></param>
    /// <returns></returns>
    private static bool LengthIsEven(string s) => s.Length % 2 == 0;

    /// <summary>
    /// A function that determines whether or not the contact info passed in is personal.
    /// </summary>
    /// <remarks>
    /// This predicate is used internally to test the methods.
    /// <para/>
    /// The delegate definition is needed to perform the requisite tests with type inference, so is defined this way
    /// as a convenience.
    /// </remarks>
    private static readonly Func<ContactInformation, bool> IsPersonal = info => info.IsPersonal;
    #endregion

    #region Factories
    /// <summary>
    /// Gets the integer 4.
    /// </summary>
    /// <remarks>
    /// This factory is used internally to test the methods.
    /// </remarks>
    /// <returns>4.</returns>
    private static int Four() => 4;

    /// <summary>
    /// Gets the string "nn".
    /// </summary>
    /// <remarks>
    /// This factory is used internally to test the methods.
    /// </remarks>
    /// <returns>"nn".</returns>
    private static string NN() => "nn";

    /// <summary>
    /// Gets the company email stored in this class.
    /// </summary>
    /// <remarks>
    /// This factory is used internally to test the methods.
    /// </remarks>
    /// <returns></returns>
    private static Email GetCompanyEmail() => CompanyEmail;

    /// <summary>
    /// Gets the company phone stored in this class.
    /// </summary>
    /// <remarks>
    /// This factory is used internally to test the methods.
    /// </remarks>
    /// <returns></returns>
    private static Phone GetCompanyPhone() => CompanyPhone;
    #endregion
    #endregion

    #region Types
    /// <summary>
    /// Represents an email address at which a person can be reached.
    /// </summary>
    /// <param name="Address"></param>
    /// <remarks>
    /// This record is used internally to test the methods.
    /// </remarks>
    private sealed record class Email(string Address, bool IsPersonal) : ContactInformation(IsPersonal);

    /// <summary>
    /// Represents a phone number at which a person can be reached.
    /// </summary>
    /// <param name="Number"></param>
    /// <remarks>
    /// This record is used internally to test the methods.
    /// </remarks>
    private sealed record class Phone(long Number, bool IsPersonal) : ContactInformation(IsPersonal);

    /// <summary>
    /// Represents contact information for a person.
    /// </summary>
    /// <param name="IsPersonal">
    /// Whether or not the contact information is personal.
    /// </param>
    /// <remarks>
    /// This record is used internally to test the methods.
    /// </remarks>
    private abstract record class ContactInformation(bool IsPersonal) { }
    #endregion
    #endregion
}
