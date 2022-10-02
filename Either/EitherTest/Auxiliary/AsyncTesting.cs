using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemTest.Core.Utilities.Monads.Auxiliary;

/// <summary>
/// Constants and static functionality for testing of asynchronous methods.
/// </summary>
internal static class AsyncTesting
{
    /// <summary>
    /// The timespan to delay in a cancellable asynchronous delegate.
    /// </summary>
    public static readonly TimeSpan CancellableDelegateDelay = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// The timespan to wait before testing a cancellation of an async operation.
    /// </summary>
    public static readonly TimeSpan CancellationTestWaitPeriod = TimeSpan.FromMilliseconds(200);
}
