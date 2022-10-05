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
    #endregion

    #region Tests
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
        Assert.IsFalse(OddLeftEither.RightMatches(IsLengthEven));
        Assert.IsFalse(OddLengthRightEither.RightMatches(IsLengthEven));
    }
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
    /// <param name="s"></param>
    /// <returns></returns>
    private static readonly FunctionOptions<string, bool> IsLengthEven = new(s => s.Length % 2 == 0);
    #endregion
}
