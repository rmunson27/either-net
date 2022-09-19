using Rem.Core.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemTest.Core.Utilities.Monads;

/// <summary>
/// Tests the defaultability of <see cref="Either{TLeft, TRight}"/> instances.
/// </summary>
[TestClass]
public class DefaultabilityTest
{
    /// <summary>
    /// Tests the defaultability of <see cref="Either{TLeft, TRight}"/> instances with reference types on the left.
    /// </summary>
    [TestMethod]
    public void TestReference()
    {
        // Defaults of values with class on the left should be marked as default
        Assert.IsTrue(default(Either<string, int>).IsDefault);
        Assert.IsTrue(default(Either<string?, int>).IsDefault);
        Assert.IsTrue(Either<string?, int>.New(null).IsDefault);

        // Values with non-default class on the left should not be marked as default
        Assert.IsFalse(Either.TRight<int>.Left("").IsDefault);
        Assert.IsFalse(Either<string, int>.New("").IsDefault);

        // Values with value on the right should never be marked as default
        Assert.IsFalse(Either<string, int>.New(4).IsDefault);
        Assert.IsFalse(Either<string?, int>.New(4).IsDefault);
        Assert.IsFalse(Either<string?, object?>.NewRight(null).IsDefault);
    }

    /// <summary>
    /// Tests the defaultability of <see cref="Either{TLeft, TRight}"/> instances with defaultable value types on
    /// the left.
    /// </summary>
    [TestMethod]
    public void TestDefaultableValue()
    {
        // Defaults of values with defaultable value on the left should be marked as default
        Assert.IsTrue(default(Either<DefaultableStruct, int>).IsDefault);

        // Values with non-default defaultable values on the left should not be marked as default
        Assert.IsFalse(Either<DefaultableStruct, int>.NewLeft(new(new())).IsDefault);

        // Values with value on the right should never be marked as default
        Assert.IsFalse(Either<DefaultableStruct, int>.New(4).IsDefault);
        Assert.IsFalse(Either<DefaultableStruct, object?>.New(null).IsDefault);
    }

    /// <summary>
    /// Tests the defaultability of <see cref="Either{TLeft, TRight}"/> instances with non-nullable, non-defaultable
    /// value types on the left.
    /// </summary>
    [TestMethod]
    public void TestNonDefaultableValue()
    {
        // Values with non-defaultable values on the left should never be marked as default
        Assert.IsFalse(default(Either<int, object>).IsDefault);
        Assert.IsFalse(default(Either<int, byte>).IsDefault);
        Assert.IsFalse(Either<int, string>.New("").IsDefault);
        Assert.IsFalse(Either<int, string>.New(4).IsDefault);
    }

    /// <summary>
    /// Tests the defaultability of <see cref="Either{TLeft, TRight}"/> instances with <see cref="ImmutableArray{T}"/>
    /// types on the left.
    /// </summary>
    [TestMethod]
    public void TestImmutableArray()
    {
        // Default values with immutable arrays on the left should be marked as default
        Assert.IsTrue(default(Either<ImmutableArray<int>, object>).IsDefault);
        Assert.IsTrue(Either<ImmutableArray<int>, int>.NewLeft(default).IsDefault);

        // Values with non-default immutable arrays on the left should not be marked as default
        Assert.IsFalse(Either.TRight<int>.Left(ImmutableArray<int>.Empty).IsDefault);
    }

    /// <summary>
    /// Tests the defaultability of <see cref="Either{TLeft, TRight}"/> instances with nullable value types on
    /// the left.
    /// </summary>
    [TestMethod]
    public void TestNullableValue()
    {
        // Default values with nullable values on the left should be marked as default
        Assert.IsTrue(default(Either<int?, string>).IsDefault);
        Assert.IsTrue(Either<int?, string>.NewLeft(null).IsDefault);

        // Values with non-null nullable values on the left should not be marked as default
        Assert.IsFalse(Either<int?, string>.New(3).IsDefault);
    }

    private readonly struct DefaultableStruct : IDefaultableStruct
    {
        public bool IsDefault => o is null;
        readonly object o;

        public DefaultableStruct(object o) { this.o = o; }
    }
}
