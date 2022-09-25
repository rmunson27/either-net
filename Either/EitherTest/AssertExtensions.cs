using System;
using System.Collections;
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
    #region Either
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
    #endregion

    #region IEnumerable
    /// <summary>
    /// Asserts that the two <see cref="IEnumerable"/> instances are equal.
    /// </summary>
    /// <param name="_"></param>
    /// <param name="expected"></param>
    /// <param name="actual"></param>
    /// <param name="message"></param>
    public static void SequenceEqual(this Assert _, IEnumerable expected, IEnumerable actual, string message = "")
        => _.SequenceEqual(expected.Cast<object>(), actual.Cast<object>(), message);

    /// <summary>
    /// Asserts that the two <see cref="IEnumerable{T}"/> instances are equal.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_"></param>
    /// <param name="expected"></param>
    /// <param name="actual"></param>
    /// <param name="message"></param>
    public static void SequenceEqual<T>(
        this Assert _, IEnumerable<T> expected, IEnumerable<T> actual, string message = "")
    {
        // Make sure the counts are equal so that we can zip without cutting one of the sequences off.
        Assert.AreEqual(expected.Count(), actual.Count(), message);

        foreach (var ((exp, act), index) in expected.Zip(actual, (e, a) => (e, a)).Select((pair, i) => (pair, i)))
        {
            Assert.AreEqual(
                exp, act,
                $"Mismatch between elements at index {index} (expected: {exp}, actual: {act})"
                    + (string.IsNullOrEmpty(message) ? string.Empty : $": {message}"));
        }
    }

    /// <summary>
    /// Asserts that the two <see cref="IEnumerable"/> instances are not equal.
    /// </summary>
    /// <param name="_"></param>
    /// <param name="notExpected"></param>
    /// <param name="actual"></param>
    /// <param name="message"></param>
    public static void SequenceNotEqual(
        this Assert _, IEnumerable notExpected, IEnumerable actual, string message = "")
        => _.SequenceNotEqual(notExpected.Cast<object>(), actual.Cast<object>(), message);

    /// <summary>
    /// Asserts that the two <see cref="IEnumerable{T}"/> instances are not equal.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_"></param>
    /// <param name="notExpected"></param>
    /// <param name="actual"></param>
    /// <param name="message"></param>
    public static void SequenceNotEqual<T>(
        this Assert _, IEnumerable<T> notExpected, IEnumerable<T> actual, string message = "")
    {
        if (notExpected.SequenceEqual(actual)) Assert.Fail(message);
    }
    #endregion
}
