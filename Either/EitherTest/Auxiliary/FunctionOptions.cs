using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemTest.Core.Utilities.Monads.Auxiliary;

/// <summary>
/// Stores synchronous and asynchronous versions of a function needed for testing of various
/// delegate-parameter-accepting methods.
/// </summary>
internal sealed class FunctionOptions<TArg, TResult>
{
    /// <summary>
    /// The function for which options are being constructed.
    /// </summary>
    private Func<TArg, TResult> Delegate { get; }

    /// <summary>
    /// Constructs a new instance of the <see cref="FunctionOptions{TArg, TResult}"/> class set up to allow calls
    /// to the function passed in.
    /// </summary>
    /// <param name="Function"></param>
    public FunctionOptions(Func<TArg, TResult> Function)
    {
        Delegate = Function;
    }

    /// <summary>
    /// Calls the method synchronously.
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    public TResult Invoke(TArg arg) => Delegate(arg);

    /// <summary>
    /// Calls the method asynchronously.
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    public Task<TResult> InvokeAsync(TArg arg) => Task.FromResult(Delegate(arg));

    /// <summary>
    /// Calls the method asynchronously with cancellation.
    /// </summary>
    /// <remarks>
    /// This will delay the task by <see cref="AsyncCancellableDelay"/> before returning the result.
    /// </remarks>
    /// <param name="arg"></param>
    /// <returns></returns>
    public async Task<TResult> InvokeCancellableAsync(TArg arg, CancellationToken cancellationToken)
    {
        await Task.Delay(AsyncTesting.CancellableDelegateDelay, cancellationToken).ConfigureAwait(false);
        return Delegate(arg);
    }

    public static implicit operator Func<TArg, TResult>(FunctionOptions<TArg, TResult> opts) => opts.Invoke;
    public static implicit operator Func<TArg, Task<TResult>>(FunctionOptions<TArg, TResult> opts)
        => opts.InvokeAsync;
    public static implicit operator Func<TArg, CancellationToken, Task<TResult>>(
        FunctionOptions<TArg, TResult> opts)
        => opts.InvokeCancellableAsync;
}
