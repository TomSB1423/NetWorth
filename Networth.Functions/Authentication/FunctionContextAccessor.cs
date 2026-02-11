using Microsoft.Azure.Functions.Worker;

namespace Networth.Functions.Authentication;

/// <summary>
///     Default implementation of <see cref="IFunctionContextAccessor"/>.
///     Uses AsyncLocal to store the FunctionContext per async execution flow.
/// </summary>
public class FunctionContextAccessor : IFunctionContextAccessor
{
    private static readonly AsyncLocal<FunctionContextHolder> CurrentContext = new();

    /// <inheritdoc />
    public FunctionContext? FunctionContext
    {
        get => CurrentContext.Value?.Context;
        set
        {
            var holder = CurrentContext.Value;
            if (holder != null)
            {
                // Clear current context trapped in the AsyncLocal, if any.
                holder.Context = null;
            }

            if (value != null)
            {
                // Use an object indirection to hold the FunctionContext in the AsyncLocal,
                // so it can be cleared in all ExecutionContexts when its cleared.
                CurrentContext.Value = new FunctionContextHolder { Context = value };
            }
        }
    }

    private sealed class FunctionContextHolder
    {
        public FunctionContext? Context { get; set; }
    }
}
