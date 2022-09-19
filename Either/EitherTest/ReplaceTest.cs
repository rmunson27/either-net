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
        static double newValueFactory() => 2.0;
        Assert.That.HasLeft(2.0, Either<int, string>.New(4).ReplaceLeftLazy(newValueFactory));
        Assert.That.HasRight("", Either<int, string>.New("").ReplaceLeftLazy(newValueFactory));
    }
    #endregion

    #region Right
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.ReplaceRight{TNewRight}(TNewRight)"/> method.
    /// </summary>
    [TestMethod]
    public void TestReplaceRight()
    {
        Assert.That.HasRight(2.0, Either<string, int>.New(4).ReplaceRight(2.0));
        Assert.That.HasLeft("", Either<string, int>.New("").ReplaceRight(2.0));
    }

    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.ReplaceRightLazy{TNewRight}(Func{TNewRight})"/> method.
    /// </summary>
    [TestMethod]
    public void TestReplaceRightLazy()
    {
        static double newValueFactory() => 2.0;
        Assert.That.HasRight(2.0, Either<string, int>.New(4).ReplaceRightLazy(newValueFactory));
        Assert.That.HasLeft("", Either<string, int>.New("").ReplaceRightLazy(newValueFactory));
    }
    #endregion
}
