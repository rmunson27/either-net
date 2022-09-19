using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemTest.Core.Utilities.Monads;

/// <summary>
/// Extensions for the <see cref="Assert"/> class.
/// </summary>
internal static class AssertExtensions
{
    /// <summary>
    /// Asserts that the <see cref="Either{TLeft, TRight}"/> passed in has the expected value on the left.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <param name="_"></param>
    /// <param name="expectedValue"></param>
    /// <param name="actualEither"></param>
    /// <param name="message"></param>
    public static void HasLeft<TLeft, TRight>(
        this Assert _,
        TLeft expectedValue, Either<TLeft, TRight> actualEither, string message = "")
    {
        Assert.IsTrue(actualEither.TryGetLeft(out var actualValue), message);
        Assert.AreEqual(expectedValue, actualValue, message);
    }

    /// <summary>
    /// Asserts that the <see cref="Either{TLeft, TRight}"/> passed in has the expected value on the right.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <param name="_"></param>
    /// <param name="expectedValue"></param>
    /// <param name="actualEither"></param>
    /// <param name="message"></param>
    public static void HasRight<TLeft, TRight>(
        this Assert _,
        TRight expectedValue, Either<TLeft, TRight> actualEither, string message = "")
    {
        Assert.IsTrue(actualEither.TryGetRight(out var actualValue), message);
        Assert.AreEqual(expectedValue, actualValue, message);
    }
}
