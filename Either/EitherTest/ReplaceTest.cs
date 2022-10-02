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
        Assert.That.HasLeft(2.0f, Either<int, string>.New(4).ReplaceLeftLazy(Get2.Invoke));
        Assert.That.HasRight("", Either<int, string>.New("").ReplaceLeftLazy(Get2.Invoke));
    }
    #endregion

    #region Both Sides
    #region Eager
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.ReplaceEither{TNewLeft, TNewRight}(TNewLeft, TNewRight)"/> method.
    /// </summary>
    [TestMethod]
    public void TestReplaceEither()
    {
        Assert.That.HasLeft(2.0, Either<int, string>.New(4).ReplaceEither(2.0, DateTime.Now));
        Assert.That.HasRight(Millennium3, Either<int, string>.New("").ReplaceEither(2.0, Millennium3));
    }
    #endregion

    #region Lazy
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.ReplaceEitherLazy{TNewLeft, TNewRight}(Func{TNewLeft}, TNewRight)"/>
    /// method.
    /// </summary>
    [TestMethod]
    public void TestReplaceEitherLazy_LazyLeftOnly()
    {
        Assert.That.HasLeft(2.0f, Either<int, string>.New(4).ReplaceEitherLazy(Get2.Invoke, DateTime.Now));
        Assert.That.HasRight(Millennium3, Either<int, string>.New("").ReplaceEitherLazy(Get2.Invoke, Millennium3));
    }

    /// <summary>
    /// Tests the
    /// <see cref="Either{TLeft, TRight}.ReplaceEitherLazy{TNewLeft, TNewRight}(Func{TNewLeft}, Func{TNewRight})"/> method.
    /// </summary>
    [TestMethod]
    public void TestReplaceEitherLazy()
    {
        Assert.That.HasLeft(2.0f, Either<int, string>.New(4).ReplaceEitherLazy(Get2.Invoke, GetMillennium3.Invoke));
        Assert.That.HasRight(
            Millennium3,
            Either<int, string>.New("rr").ReplaceEitherLazy(Get2.Invoke, GetMillennium3.Invoke));
    }

    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.ReplaceEitherLazy{TNewLeft, TNewRight}(TNewLeft, Func{TNewRight})"/>
    /// method.
    /// </summary>
    [TestMethod]
    public void TestReplaceEitherLazy_LazyRightOnly()
    {
        Assert.That.HasLeft(2.0, Either<int, string>.New(4).ReplaceEitherLazy(2.0, GetMillennium3.Invoke));
        Assert.That.HasRight(Millennium3, Either<int, string>.New("").ReplaceEitherLazy(2.0, GetMillennium3.Invoke));
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
        Assert.That.HasLeft(4, Either<int, string>.New(4).ReplaceRightLazy(GetMillennium3.Invoke));
        Assert.That.HasRight(Millennium3, Either<int, string>.New("").ReplaceRightLazy(GetMillennium3.Invoke));
    }
    #endregion
    #endregion

    #region Helper Factories
    /// <summary>
    /// A factory method that gets the number 2.
    /// </summary>
    private static readonly FunctionOptions<float> Get2 = new(() => 2);

    /// <summary>
    /// A factory method that gets the first instant of the third millennium.
    /// </summary>
    private static readonly FunctionOptions<DateTime> GetMillennium3 = new(() => Millennium3);

    /// <summary>
    /// The first instant of the third millennium.
    /// </summary>
    private static readonly DateTime Millennium3 = DateTime.Parse("January 1, 2000");
    #endregion
}
