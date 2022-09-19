using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Rem.Core.Utilities.Monads;

/// <summary>
/// Internal extension methods for equality comparers.
/// </summary>
internal static class EqualityComparerExtensions
{
    /// <summary>
    /// Gets the current instance, or <see cref="EqualityComparer{T}.Default"/> if the current instance
    /// is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="comparer"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static IEqualityComparer<T> DefaultIfNull<T>(this IEqualityComparer<T>? comparer)
        => comparer ?? EqualityComparer<T>.Default;
}
