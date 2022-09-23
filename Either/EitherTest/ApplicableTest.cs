using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemTest.Core.Utilities.Monads;

/// <summary>
/// Tests applicable extension methods for <see cref="Either{TLeft, TRight}"/> instances.
/// </summary>
[TestClass]
public class ApplicableTest
{
    #region Tests
    /// <summary>
    /// Tests the <see cref="EitherApplicableExtensions.ApplyLeft"/> method and related generic overloads.
    /// </summary>
    [TestMethod]
    public void TestApplyLeft()
    {
        // No arguments
        Assert.That.HasLeft(1, Either<Func<int>, string>.New(GetOne).ApplyLeft());
        Assert.That.HasRight("", Either<Func<int>, string>.New("").ApplyLeft());

        // 1 argument
        Assert.That.HasLeft(16, Either<Func<int, int>, string>.New(Square).ApplyLeft(4));
        Assert.That.HasRight("", Either<Func<int, int>, string>.New("").ApplyLeft(4));

        // 2 arguments (all generic overloads with 2 or more arguments are generated using the same method)
        Assert.That.HasLeft(3, Either<Func<int, int, int>, string>.New(Divide).ApplyLeft(15, 5));
        Assert.That.HasRight("", Either<Func<int, int, int>, string>.New("").ApplyLeft(1, 2));
    }

    /// <summary>
    /// Tests the <see cref="EitherApplicableExtensions.Apply"/> method.
    /// </summary>
    [TestMethod]
    public void TestApply()
    {
        Assert.That.HasLeft(1, Either<Func<int>, Func<int>>.NewLeft(GetOne).Apply());
        Assert.That.HasRight(1, Either<Func<int>, Func<int>>.NewRight(GetOne).Apply());
    }

    /// <summary>
    /// Tests the <see cref="EitherApplicableExtensions.ApplyRight"/> method and related generic overloads.
    /// </summary>
    [TestMethod]
    public void TestApplyRight()
    {
        // No arguments
        Assert.That.HasRight(1, Either<string, Func<int>>.New(GetOne).ApplyRight());
        Assert.That.HasLeft("", Either<string, Func<int>>.New("").ApplyRight());

        // 1 argument
        Assert.That.HasRight(16, Either<string, Func<int, int>>.New(Square).ApplyRight(4));
        Assert.That.HasLeft("", Either<string, Func<int, int>>.New("").ApplyRight(4));

        // 2 arguments (all generic overloads with 2 or more arguments are generated using the same method)
        Assert.That.HasRight(5, Either<string, Func<int, int, int>>.New(Divide).ApplyRight(15, 3));
        Assert.That.HasLeft("", Either<string, Func<int, int, int>>.New("").ApplyRight(1, 2));
    }
    #endregion

    #region Helpers
    /// <summary>
    /// Gets the number 1.
    /// </summary>
    /// <remarks>
    /// This function is used internally to test the applicable methods.
    /// </remarks>
    /// <returns></returns>
    private static int GetOne() => 1;

    /// <summary>
    /// Squares the number passed in.
    /// </summary>
    /// <remarks>
    /// This function is used internally to test the applicable methods.
    /// </remarks>
    /// <param name="i"></param>
    /// <returns></returns>
    private static int Square(int i) => i * i;

    /// <summary>
    /// Divides the first number passed in by the second.
    /// </summary>
    /// <remarks>
    /// This function is used internally to test the applicable methods.
    /// <para/>
    /// A non-symmetric function was used <i>purposefully</i> to ensure the arguments are handled in the correct
    /// order internally.
    /// </remarks>
    /// <param name="dividend"></param>
    /// <param name="divisor"></param>
    /// <returns></returns>
    private static int Divide(int dividend, int divisor) => dividend / divisor;
    #endregion
}
