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
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.LeftMatches(Func{TLeft, bool})"/> method.
    /// </summary>
    [TestMethod]
    public void TestLeftMatches()
    {
        Assert.IsTrue(Either<int, string>.New(4).LeftMatches(IsEven));
        Assert.IsFalse(Either<int, string>.New(3).LeftMatches(IsEven));
        Assert.IsFalse(Either<int, string>.New("").LeftMatches(IsEven));
    }

    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.EitherMatches(Func{TLeft, bool}, Func{TRight, bool})"/> method.
    /// </summary>
    [TestMethod]
    public void TestEitherMatches()
    {
        Assert.IsTrue(Either<int, string>.New(4).EitherMatches(IsEven, IsLengthEven));
        Assert.IsTrue(Either<int, string>.New("").EitherMatches(IsEven, IsLengthEven));
        Assert.IsFalse(Either<int, string>.New(3).EitherMatches(IsEven, IsLengthEven));
        Assert.IsFalse(Either<int, string>.New(".").EitherMatches(IsEven, IsLengthEven));
    }

    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.RightMatches(Func{TRight, bool})"/> method.
    /// </summary>
    [TestMethod]
    public void TestRightMatches()
    {
        Assert.IsTrue(Either<string, int>.New(4).RightMatches(IsEven));
        Assert.IsFalse(Either<string, int>.New(3).RightMatches(IsEven));
        Assert.IsFalse(Either<string, int>.New("").RightMatches(IsEven));
    }

    /// <summary>
    /// Determines whether or not the argument is even.
    /// </summary>
    /// <remarks>
    /// This predicate is used internally to test the relevant <see cref="Either{TLeft, TRight}"/> methods.
    /// </remarks>
    /// <param name="i">The integer to determine the parity of.</param>
    /// <returns>Whether or not the argument is even.</returns>
    private static bool IsEven(int i) => i % 2 == 0;

    /// <summary>
    /// Determines whether or not the argument has an even length.
    /// </summary>
    /// <remarks>
    /// This predicate is used internally to test the relevant <see cref="Either{TLeft, TRight}"/> methods.
    /// </remarks>
    /// <param name="s"></param>
    /// <returns></returns>
    private static bool IsLengthEven(string s) => s.Length % 2 == 0;
}
