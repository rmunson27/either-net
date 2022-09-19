namespace RemTest.Core.Utilities.Monads;

/// <summary>
/// Tests of value-getters from the <see cref="Either{TLeft, TRight}"/> struct.
/// </summary>
[TestClass]
public class GetValuesTest
{
    /// <summary>
    /// Tests the <see cref="Either{TLeft, TRight}.TryGetLeft(out TLeft)"/>,
    /// <see cref="Either{TLeft, TRight}.TryGetRight(out TRight)"/> and
    /// <see cref="Either{TLeft, TRight}.TryGetRight(out TLeft, out TRight)"/> methods.
    /// </summary>
    [TestMethod]
    public void TestTryGet()
    {
        const int leftEitherValue = 4;
        const string rightEitherValue = "AString";
        Either<int, string> leftEither = new(leftEitherValue);
        Either<int, string> rightEither = new(rightEitherValue);

        int i;
        string? s;

        Assert.IsTrue(leftEither.TryGetLeft(out i));
        Assert.IsFalse(leftEither.TryGetRight(out s));
        Assert.AreEqual(leftEitherValue, i);

        Assert.IsFalse(rightEither.TryGetLeft(out i));
        Assert.IsTrue(rightEither.TryGetRight(out s));
        Assert.AreEqual(rightEitherValue, s);

        resetReadValues();

        Assert.IsFalse(leftEither.TryGetRight(out i, out s));
        Assert.AreEqual(leftEitherValue, i);

        resetReadValues();
        Assert.IsTrue(rightEither.TryGetRight(out i, out s));
        Assert.AreEqual(rightEitherValue, s);

        // Reset the read values so that we are sure they are set again on next call
        void resetReadValues()
        {
            i = 0;
            s = "";
        }
    }
}