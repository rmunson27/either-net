using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Rem.Core.Utilities.Either;

/// <summary>
/// An exception thrown when an attempt is made to access an <see cref="Either{TLeft, TRight}"/> that is not wrapping
/// a value of the expected side.
/// </summary>
public class EitherException : InvalidOperationException
{
    /// <summary>
    /// Constructs a new instance of the <see cref="EitherException"/> class.
    /// </summary>
    public EitherException() { }

    /// <summary>
    /// Constructs a new instance of the <see cref="EitherException"/> class with the supplied error message.
    /// </summary>
    /// <param name="message"></param>
    public EitherException(string message) : base(message) { }

    /// <summary>
    /// Constructs a new instance of the <see cref="EitherException"/> class with the supplied error message and
    /// inner exception.
    /// </summary>
    /// <param name="message"></param>
    /// <param name="innerException"></param>
    public EitherException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Constructs a new instance of the <see cref="EitherException"/> class from the serialization data passed
    /// in (serialization constructor).
    /// </summary>
    /// <param name="info"></param>
    /// <param name="context"></param>
    protected EitherException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}
