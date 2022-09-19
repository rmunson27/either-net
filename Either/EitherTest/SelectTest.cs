using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemTest.Core.Utilities.Monads;

/// <summary>
/// Tests of the
/// <see cref="Either{TLeft, TRight}.Select{TLeftResult, TRightResult}(Func{TLeft, TLeftResult}, Func{TRight, TRightResult})"/>
/// and
/// <see cref="Either{TLeft, TRight}.SelectMany{TLeftResult, TRightResult}(Func{TLeft, Either{TLeftResult, TRightResult}}, Func{TRight, Either{TLeftResult, TRightResult}})"/>
/// methods and related overloads.
/// </summary>
[TestClass]
public class SelectTest
{
    #region Select
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.SelectLeft{TLeftResult}(Func{TLeft, TLeftResult})"/> method.
    /// </summary>
    [TestMethod]
    public void TestSelectLeft()
    {
        Assert.That.HasLeft(4, Either<object?, string>.NewLeft(null).SelectLeft(o => o is null ? 4 : 5));
        Assert.That.HasRight("", Either<object?, string>.New("").SelectLeft(o => o is null ? 4 : 5));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.Select{TLeftResult, TRightResult}(Func{TLeft, TLeftResult}, Func{TRight, TRightResult})"/>
    /// method.
    /// </summary>
    [TestMethod]
    public void TestSelect()
    {
        static string intToStr(int i) => i.ToString();
        static int strToInt(string s) => int.Parse(s);

        Assert.That.HasLeft("4", Either<int, string>.New(4).Select(intToStr, strToInt));
        Assert.That.HasRight(4, Either<int, string>.New("4").Select(intToStr, strToInt));
    }

    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.SelectRight{TRightResult}(Func{TRight, TRightResult})"/> method.
    /// </summary>
    [TestMethod]
    public void TestSelectRight()
    {
        Assert.That.HasRight(4, Either<object?, string>.New("").SelectRight(s => string.IsNullOrEmpty(s) ? 4 : 5));
        Assert.That.HasLeft(null, Either<object?, string>.NewLeft(null).SelectRight(s => string.IsNullOrEmpty(s) ? 4 : 5));
    }
    #endregion

    #region SelectMany
    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectManyLeft{TLeftResult}(Func{TLeft, Either{TLeftResult, TRight}})"/>
    /// method.
    /// </summary>
    [TestMethod]
    public void TestSelectManyLeft()
    {
        static Either<ulong, string?> leftF(int i)
        {
            if (i % 2 == 0) return i > 0 ? (ulong)i : i.ToString();
            else return null;
        }

        // Function is called
        Assert.That.HasLeft(2uL, Either<int, string?>.New(2).SelectManyLeft(leftF));
        Assert.That.HasRight("-2", Either<int, string?>.New(-2).SelectManyLeft(leftF));
        Assert.That.HasRight(null, Either<int, string?>.New(3).SelectManyLeft(leftF));

        // Function is not called
        Assert.That.HasRight("sss", Either<int, string?>.New("sss").SelectManyLeft(leftF));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectMany{TLeftResult, TRightResult}(Func{TLeft, Either{TLeftResult, TRightResult}}, Func{TRight, Either{TLeftResult, TRightResult}})"/>
    /// method.
    /// </summary>
    [TestMethod]
    public void TestSelectMany()
    {
        static Either<ulong, float> leftF(int i)
        {
            if (i % 2 == 0) return i > 0 ? Either<ulong, float>.NewLeft((ulong)i) : float.NegativeInfinity;
            else return float.NaN;
        }

        static Either<ulong, float> rightF(string? s)
        {
            if (string.IsNullOrEmpty(s)) return float.PositiveInfinity;
            else return (ulong)s.Length;
        }

        // Left function is called
        Assert.That.HasLeft(2uL, Either<int, string?>.New(2).SelectMany(leftF, rightF));
        Assert.That.HasRight(float.NegativeInfinity, Either<int, string?>.New(-2).SelectMany(leftF, rightF));
        Assert.That.HasRight(float.NaN, Either<int, string?>.New(3).SelectMany(leftF, rightF));

        // Right function is called
        Assert.That.HasRight(float.PositiveInfinity, Either<int, string?>.New("").SelectMany(leftF, rightF));
        Assert.That.HasLeft(3uL, Either<int, string?>.New("rrr").SelectMany(leftF, rightF));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.SelectManyRight{TRightResult}(Func{TRight, Either{TLeft, TRightResult}})"/>
    /// method.
    /// </summary>
    [TestMethod]
    public void TestSelectManyRight()
    {
        static Either<string?, ulong> rightF(int i)
        {
            if (i % 2 == 0) return i > 0 ? (ulong)i : i.ToString();
            else return null;
        }

        // Function is called
        Assert.That.HasRight(2uL, Either<string?, int>.New(2).SelectManyRight(rightF));
        Assert.That.HasLeft("-2", Either<string?, int>.New(-2).SelectManyRight(rightF));
        Assert.That.HasLeft(null, Either<string?, int>.New(3).SelectManyRight(rightF));

        // Function is not called
        Assert.That.HasLeft("sss", Either<string?, int>.New("sss").SelectManyRight(rightF));
    }
    #endregion
}
