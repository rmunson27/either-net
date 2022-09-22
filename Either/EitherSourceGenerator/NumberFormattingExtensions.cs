using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rem.Core.Utilities.Monads.SourceGeneration;

/// <summary>
/// Formatting methods for numbers for internal use in the library.
/// </summary>
internal static class NumberFormattingExtensions
{
    /// <summary>
    /// Formats the current <see cref="int"/> as an ordinal number by adding the appropriate suffix.
    /// </summary>
    /// <remarks>
    /// This method is not meant to be called on negative integers, and performing such a call will result in
    /// undefined behavior.
    /// </remarks>
    /// <param name="i"></param>
    /// <returns></returns>
    public static string FormatAsOrdinal(this int i) => (i % 100) switch
    {
        11 or 12 or 13 => $"{i}th", // Handle exceptions to the general pattern first

        _ => (i % 10) switch // Everything else matches this pattern
        {
            1 => $"{i}st",
            2 => $"{i}nd",
            3 => $"{i}rd",
            _ => $"{i}th",
        },
    };
}
