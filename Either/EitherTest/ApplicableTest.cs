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
    #region Apply
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

    #region Invoke
    /// <summary>
    /// Tests the <see cref="EitherApplicableExtensions.InvokeLeft"/> method and related generic overloads.
    /// </summary>
    [TestMethod]
    public void TestInvokeLeft()
    {
        var testState = new ActionRunner();

        // Should do nothing if the either is right (but not throw an exception)
        Either<Action, string>.New("").InvokeLeft();
        Either<Action<int>, string>.New("").InvokeLeft(2);
        Either<Action<int, int>, string>.New("").InvokeLeft(2, 3);

        // Should do nothing if the delegate is null (but not throw an exception)
        Either<Action?, string>.NewLeft(null).InvokeLeft();
        Either<Action<int>?, string>.NewLeft(null).InvokeLeft(1);
        Either<Action<int, int>?, string>.NewLeft(null).InvokeLeft(1, 2);

        // Should invoke the delegate
        Either<Action, string>.New(testState.IncrementI).InvokeLeft();
        Assert.AreEqual(1, testState.I);
        Either<Action<int>, string>.New(testState.AddToI).InvokeLeft(2);
        Assert.AreEqual(3, testState.I);
        Either<Action<int, int>, string>.New(testState.AddQuotientToI).InvokeLeft(15, 3);
        Assert.AreEqual(8, testState.I);
    }

    /// <summary>
    /// Tests the <see cref="EitherApplicableExtensions.Invoke"/> method.
    /// </summary>
    [TestMethod]
    public void TestInvoke()
    {
        var testState = new ActionRunner();

        // Should do nothing if the delegate is null (but not throw an exception)
        Either<Action?, Action?>.NewLeft(null).Invoke();
        Either<Action?, Action?>.NewRight(null).Invoke();

        // Should invoke the delegate
        Either<Action, Action>.NewLeft(testState.IncrementI).Invoke();
        Assert.AreEqual(1, testState.I);
        Either<Action, Action>.NewRight(testState.IncrementI).Invoke();
        Assert.AreEqual(2, testState.I);
    }

    /// <summary>
    /// Tests the <see cref="EitherApplicableExtensions.InvokeRight"/> method and related generic overloads.
    /// </summary>
    [TestMethod]
    public void TestInvokeRight()
    {
        var testState = new ActionRunner();

        // Should do nothing if the either is right (but not throw an exception)
        Either<string, Action>.New("").InvokeRight();
        Either<string, Action<int>>.New("").InvokeRight(2);
        Either<string, Action<int, int>>.New("").InvokeRight(2, 3);

        // Should do nothing if the delegate is null (but not throw an exception)
        Either<string, Action?>.NewRight(null).InvokeRight();
        Either<string, Action<int>?>.NewRight(null).InvokeRight(1);
        Either<string, Action<int, int>?>.NewRight(null).InvokeRight(1, 2);

        // Should invoke the delegate
        Either<string, Action>.New(testState.IncrementI).InvokeRight();
        Assert.AreEqual(1, testState.I);
        Either<string, Action<int>>.New(testState.AddToI).InvokeRight(2);
        Assert.AreEqual(3, testState.I);
        Either<string, Action<int, int>>.New(testState.AddQuotientToI).InvokeRight(15, 3);
        Assert.AreEqual(8, testState.I);
    }
    #endregion
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

    /// <summary>
    /// Stores an integer and a series of actions that can be used to modify it.
    /// </summary>
    /// <remarks>
    /// This class is used internally to test the applicable methods.
    /// </remarks>
    private sealed class ActionRunner
    {
        /// <summary>
        /// An integer stored by the <see cref="ActionRunner"/> class.
        /// </summary>
        public int I { get; private set; }

        /// <summary>
        /// Resets the state of this object to the default state (with <see cref="I"/> set to 0).
        /// </summary>
        public void Reset() { I = 0; }

        /// <summary>
        /// Increments <see cref="I"/>.
        /// </summary>
        public void IncrementI() => I++;

        /// <summary>
        /// Adds the specified integer to <see cref="I"/>.
        /// </summary>
        /// <param name="i"></param>
        public void AddToI(int i) => I += i;

        /// <summary>
        /// Adds the quotient of the specified integers to <see cref="I"/>.
        /// </summary>
        /// <remarks>
        /// A non-symmetric function was used <i>purposefully</i> to ensure the arguments are handled in the correct
        /// order internally.
        /// </remarks>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        public void AddQuotientToI(int dividend, int divisor) => I += dividend / divisor;
    }
    #endregion
}
