using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemTest.Core.Utilities.Monads;

/// <summary>
/// Tests the
/// <see cref="Either{TLeft, TRight}.CombineSides{TResult}(Func{TLeft, TResult}, Func{TRight, TResult})"/> method.
/// </summary>
[TestClass]
public class CombineSidesTest
{
    /// <inheritdoc cref="CombineSidesTest"/>
    [TestMethod]
    public void Test()
    {
        static int leftF(int[] arr) => arr.Length;
        static int rightF(string s) => s.Length;

        Assert.AreEqual(4, Either<int[], string>.New(new int[4]).CombineSides(leftF, rightF));
        Assert.AreEqual(3, Either<int[], string>.New("vvv").CombineSides(leftF, rightF));
    }
}
