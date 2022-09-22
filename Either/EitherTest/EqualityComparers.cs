using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemTest.Core.Utilities.Monads;

/// <summary>
/// Extension methods and static functionality relating to <see cref="IEqualityComparer{T}"/> instances.
/// </summary>
internal static class EqualityComparers
{
    /// <summary>
    /// Creates a new <see cref="EqualityComparer{T}"/> with the methods passed in serving as its implementations of
    /// <see cref="EqualityComparer{T}.Equals(T?, T?)"/> and <see cref="EqualityComparer{T}.GetHashCode(T)"/>.
    /// </summary>
    /// <typeparam name="T">The type of the equality comparer to create.</typeparam>
    /// <param name="Equals">
    /// The implementation of <see cref="EqualityComparer{T}.Equals(T?, T?)"/> to use, or <see langword="null"/> to
    /// use the default implementation for type <typeparamref name="T"/>.
    /// </param>
    /// <param name="GetHashCode">
    /// The implementation of <see cref="EqualityComparer{T}.GetHashCode(T)"/> to use, or <see langword="null"/> to
    /// use the default implementation for type <typeparamref name="T"/>.
    /// </param>
    /// <returns></returns>
    public static EqualityComparer<T> FromMethods<T>(
        Func<T?, T?, bool>? Equals = null, Func<T, int>? GetHashCode = null)
        => new FuncEqualityComparer<T>(
            Equals ?? EqualityComparer<T>.Default.Equals,
            GetHashCode ?? EqualityComparer<T>.Default.GetHashCode!);

    /// <summary>
    /// An <see cref="EqualityComparer{T}"/> that forwards calls to <see cref="Equals(T?, T?)"/> and
    /// <see cref="GetHashCode(T)"/> through methods passed into the constructor.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private sealed class FuncEqualityComparer<T> : EqualityComparer<T>
    {
        private readonly Func<T?, T?, bool> _equals;
        private readonly Func<T, int> _getHashCode;

        public FuncEqualityComparer(Func<T?, T?, bool> Equals, Func<T, int> GetHashCode)
        {
            _equals = Equals;
            _getHashCode = GetHashCode;
        }

        public override bool Equals(T? x, T? y) => _equals(x, y);

        public override int GetHashCode([DisallowNull] T obj) => _getHashCode(obj);
    }
}
