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
    /// Tests the <see cref="Either{TLeft, TRight}.Where(Func{TLeft, bool}, Func{TRight, bool})"/> method.
    /// </summary>
    [TestMethod]
    public void TestWhere_Enumerable()
    {
        Assert.That.SequenceEqual(new[] { 2 }, Either<string, int>.New(2).Where(LengthIsEven, IsEven));
        Assert.IsFalse(Either<string, int>.New(3).Where(LengthIsEven, IsEven).Cast<object>().Any());
        Assert.That.SequenceEqual(new[] { "" }, Either<string, int>.New("").Where(LengthIsEven, IsEven));
        Assert.IsFalse(Either<string, int>.New(" ").Where(LengthIsEven, IsEven).Cast<object>().Any());
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
    #endregion
}
