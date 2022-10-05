using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemTest.Core.Utilities.Monads;

/// <summary>
/// Tests the
/// <see cref="Either{TLeft, TRight}.ContainsEither(TLeft, TRight, IEqualityComparer{TLeft}?, IEqualityComparer{TRight}?)"/>
/// method and side-specific analogs.
/// </summary>
[TestClass]
public class ContainsTest
{
    /// <summary>
    /// Determines equality based on the equality of the parity of the arguments.
    /// </summary>
    private static readonly EqualityComparer<int> IntParityComparer
        = EqualityComparers.FromMethods<int>(
            Equals: (i1, i2) => Math.Abs(i1 % 2) == Math.Abs(i2 % 2),
            GetHashCode: i => Math.Abs(i % 2));

    /// <summary>
    /// Determines equality based on the equality of the lengths of the arguments.
    /// </summary>
    /// <remarks>
    /// This <see cref="EqualityComparer{T}"/> is used internally to test the relevant
    /// <see cref="Either{TLeft, TRight}"/> methods.
    /// </remarks>
    private static readonly EqualityComparer<string> StringLengthComparer
        = EqualityComparers.FromMethods<string>(
            Equals: (s1, s2) => s1?.Length == s2?.Length,
            GetHashCode: s => s.Length);

    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.ContainsLeft(TLeft, IEqualityComparer{TLeft}?)"/> method.
    /// </summary>
    [TestMethod]
    public void TestContainsLeft()
    {
        // Default equality comparer
        Assert.IsTrue(Either<int, string>.New(3).ContainsLeft(3));
        Assert.IsFalse(Either<int, string>.New(3).ContainsLeft(4));
        Assert.IsFalse(Either<int, string>.New("").ContainsLeft(3));

        // Specified equality comparer
        Assert.IsTrue(Either<int, string>.New(3).ContainsLeft(5, IntParityComparer));
        Assert.IsFalse(Either<int, string>.New(3).ContainsLeft(4, IntParityComparer));
        Assert.IsFalse(Either<int, string>.New("").ContainsLeft(3, IntParityComparer));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ContainsEither(TLeft, TRight, IEqualityComparer{TLeft}?, IEqualityComparer{TRight}?)"/>
    /// method.
    /// </summary>
    [TestMethod]
    public void TestContainsEither()
    {
        // Default equality comparers
        Assert.IsTrue(Either<int, string>.New(3).ContainsEither(3, ""));
        Assert.IsTrue(Either<int, string>.New("").ContainsEither(3, ""));
        Assert.IsFalse(Either<int, string>.New(4).ContainsEither(3, ""));
        Assert.IsFalse(Either<int, string>.New("S").ContainsEither(3, ""));

        // Speciied equality comparers
        Assert.IsTrue(Either<int, string>.New(5).ContainsEither(3, "sss", IntParityComparer, StringLengthComparer));
        Assert.IsTrue(Either<int, string>.New("444").ContainsEither(3, "sss", IntParityComparer, StringLengthComparer));
        Assert.IsFalse(Either<int, string>.New(4).ContainsEither(3, "sss", IntParityComparer, StringLengthComparer));
        Assert.IsFalse(Either<int, string>.New("ss").ContainsEither(3, "sss", IntParityComparer, StringLengthComparer));
    }

    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.ContainsRight(TRight, IEqualityComparer{TRight}?)"/> method.
    /// </summary>
    [TestMethod]
    public void TestContainsRight()
    {
        // Default equality comparer
        Assert.IsTrue(Either<string, int>.New(3).ContainsRight(3));
        Assert.IsFalse(Either<string, int>.New(3).ContainsRight(4));
        Assert.IsFalse(Either<string, int>.New("").ContainsRight(3));

        // Specified equality comparer
        Assert.IsTrue(Either<string, int>.New(3).ContainsRight(5, IntParityComparer));
        Assert.IsFalse(Either<string, int>.New(3).ContainsRight(4, IntParityComparer));
        Assert.IsFalse(Either<string, int>.New("").ContainsRight(3, IntParityComparer));
    }
}
