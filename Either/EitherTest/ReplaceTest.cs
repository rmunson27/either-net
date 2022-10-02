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
    #region Tests
    #region Left
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
        Assert.That.HasLeft(2.0, Either<int, string>.New(4).ReplaceLeftLazy(Get2));
        Assert.That.HasRight("", Either<int, string>.New("").ReplaceLeftLazy(Get2));
    }
    #endregion

    #region Both Sides
    #region Eager
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.Replace{TNewLeft, TNewRight}(TNewLeft, TNewRight)"/> method.
    /// </summary>
    [TestMethod]
    public void TestReplace()
    {
        Assert.That.HasLeft(2.0, Either<int, string>.New(4).Replace(2.0, DateTime.Now));
        Assert.That.HasRight(NewMillennium, Either<int, string>.New("").Replace(2.0, NewMillennium));
    }
    #endregion

    #region Lazy
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.ReplaceLazyLeft{TNewLeft, TNewRight}(Func{TNewLeft}, TNewRight)"/>
    /// method.
    /// </summary>
    [TestMethod]
    public void TestReplaceLazyLeft()
    {
        Assert.That.HasLeft(2.0, Either<int, string>.New(4).ReplaceLazyLeft(Get2, DateTime.Now));
        Assert.That.HasRight(NewMillennium, Either<int, string>.New("").ReplaceLazyLeft(Get2, NewMillennium));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceLazy{TNewLeft, TNewRight}(Func{TNewLeft}, Func{TNewRight})"/> method.
    /// </summary>
    [TestMethod]
    public void TestReplaceLazy()
    {
        Assert.That.HasLeft(2.0, Either<int, string>.New(4).ReplaceLazy(Get2, GetNewMillennium));
        Assert.That.HasRight(NewMillennium, Either<int, string>.New("rr").ReplaceLazy(Get2, GetNewMillennium));
    }

    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.ReplaceLazyRight{TNewLeft, TNewRight}(TNewLeft, Func{TNewRight})"/>
    /// method.
    /// </summary>
    [TestMethod]
    public void TestReplaceLazyRight()
    {
        Assert.That.HasLeft(2.0, Either<int, string>.New(4).ReplaceLazyRight(2.0, GetNewMillennium));
        Assert.That.HasRight(NewMillennium, Either<int, string>.New("").ReplaceLazyRight(2.0, GetNewMillennium));
    }
    #endregion
    #endregion

    #region Right
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
        Assert.That.HasLeft(4, Either<int, string>.New(4).ReplaceRightLazy(GetNewMillennium));
        Assert.That.HasRight(NewMillennium, Either<int, string>.New("").ReplaceRightLazy(GetNewMillennium));
    }
    #endregion
    #endregion

    #region Helper Factories
    /// <summary>
    /// A factory method that gets the number 2.
    /// </summary>
    /// <returns></returns>
    private static double Get2() => 2;

    /// <summary>
    /// A factory method that gets the first second of the new millennium.
    /// </summary>
    /// <returns></returns>
    private static DateTime GetNewMillennium() => NewMillennium;

    /// <summary>
    /// The first second of the new millennium.
    /// </summary>
    private static readonly DateTime NewMillennium = DateTime.Parse("January 1, 2000");
    #endregion
}
