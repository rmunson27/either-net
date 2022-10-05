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
/// <typeparam name="TResult"></typeparam>
internal sealed class FunctionOptions<TResult>
{
    /// <summary>
    /// Gets the synchronous option.
    /// </summary>
    public Func<TResult> Delegate => Invoke;

    /// <summary>
    /// Gets the asynchronous option.
    /// </summary>
    public Func<Task<TResult>> AsyncDelegate => InvokeAsync;

    /// <summary>
    /// Gets the cancellable asynchronous option.
    /// </summary>
    public Func<CancellationToken, Task<TResult>> CancellableAsyncDelegate => InvokeCancellableAsync;

    /// <summary>
    /// Gets the function for which options are being constructed.
    /// </summary>
    private Func<TResult> Function { get; }

    /// <summary>
    /// Constructs a new instance of the <see cref="FunctionOptions{TArg, TResult}"/> class set up to allow calls
    /// to the function passed in.
    /// </summary>
    /// <param name="Function"></param>
    public FunctionOptions(Func<TResult> Function)
    {
        this.Function = Function;
    }

    /// <summary>
    /// Calls the method synchronously.
    /// </summary>
    /// <returns></returns>
    public TResult Invoke() => Function();

    /// <summary>
    /// Calls the method asynchronously.
    /// </summary>
    /// <returns></returns>
    public Task<TResult> InvokeAsync() => Task.FromResult(Function());

    /// <summary>
    /// Calls the method asynchronously with cancellation.
    /// </summary>
    /// <remarks>
    /// This will delay the task by <see cref="AsyncCancellableDelay"/> before returning the result.
    /// </remarks>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<TResult> InvokeCancellableAsync(CancellationToken cancellationToken)
    {
        await Task.Delay(AsyncTesting.CancellableDelegateDelay, cancellationToken).ConfigureAwait(false);
        return Function();
    }

    public static implicit operator Func<TResult>(FunctionOptions<TResult> opts) => opts.Invoke;
    public static implicit operator Func<Task<TResult>>(FunctionOptions<TResult> opts) => opts.InvokeAsync;
    public static implicit operator Func<CancellationToken, Task<TResult>>(FunctionOptions<TResult> opts)
        => opts.InvokeCancellableAsync;
}

/// <summary>
/// Stores synchronous and asynchronous versions of a function needed for testing of various
/// delegate-parameter-accepting methods.
/// </summary>
/// <typeparam name="TArg"></typeparam>
/// <typeparam name="TResult"></typeparam>
internal sealed class FunctionOptions<TArg, TResult>
{
    /// <summary>
    /// Gets the synchronous option.
    /// </summary>
    public Func<TArg, TResult> Delegate => Invoke;

    /// <summary>
    /// Gets the asynchronous option.
    /// </summary>
    public Func<TArg, Task<TResult>> AsyncDelegate => InvokeAsync;

    /// <summary>
    /// Gets the cancellable asynchronous option.
    /// </summary>
    public Func<TArg, CancellationToken, Task<TResult>> CancellableAsyncDelegate => InvokeCancellableAsync;

    /// <summary>
    /// Gets the function for which options are being constructed.
    /// </summary>
    private Func<TArg, TResult> Function { get; }

    /// <summary>
    /// Constructs a new instance of the <see cref="FunctionOptions{TArg, TResult}"/> class set up to allow calls
    /// to the function passed in.
    /// </summary>
    /// <param name="Function"></param>
    public FunctionOptions(Func<TArg, TResult> Function)
    {
        this.Function = Function;
    }

    /// <summary>
    /// Calls the method synchronously.
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    public TResult Invoke(TArg arg) => Function(arg);

    /// <summary>
    /// Calls the method asynchronously.
    /// </summary>
    /// <param name="arg"></param>
    /// <returns></returns>
    public Task<TResult> InvokeAsync(TArg arg) => Task.FromResult(Function(arg));

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
        return Function(arg);
    }

    public static implicit operator Func<TArg, TResult>(FunctionOptions<TArg, TResult> opts) => opts.Invoke;
    public static implicit operator Func<TArg, Task<TResult>>(FunctionOptions<TArg, TResult> opts)
        => opts.InvokeAsync;
    public static implicit operator Func<TArg, CancellationToken, Task<TResult>>(
        FunctionOptions<TArg, TResult> opts)
        => opts.InvokeCancellableAsync;
}
