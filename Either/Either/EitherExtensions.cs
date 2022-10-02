using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rem.Core.Utilities.Monads;

/// <summary>
/// General-purpose extension methods for the <see cref="Either{TLeft, TRight}"/> struct.
/// </summary>
public static class EitherExtensions
{
    #region Where
    /// <summary>
    /// Filters the value wrapped in the current <see cref="Either{TLeft, TRight}"/> by the specified predicate.
    /// </summary>
    /// <typeparam name="TLeft"></typeparam>
    /// <typeparam name="TRight"></typeparam>
    /// <typeparam name="TParent"></typeparam>
    /// <param name="either"></param>
    /// <param name="parentPredicate"></param>
    /// <returns></returns>
    public static IEnumerable<TParent> WhereEither<TLeft, TRight, TParent>(
        this Either<TLeft, TRight> either, Func<TParent, bool> parentPredicate)
        where TLeft : TParent
        where TRight : TParent
    {
        if (either.IsRight)
        {
            if (parentPredicate(either._right)) yield return either._right;
        }
        else
        {
            if (parentPredicate(either._left)) yield return either._left;
        }
    }
    #endregion
}
