using Rem.Core.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Rem.Core.Utilities.Monads;

/// <summary>
/// Internal helper methods for determining if instances of type <typeparamref name="T"/> are the default.
/// </summary>
/// <remarks>
/// This class will indicate that values of reference types are the default if they are <see langword="null"/>, that
/// values of value types implementing <see cref="IDefaultableStruct"/> or the <see cref="ImmutableArray{T}"/> struct
/// are the default if the <see cref="IDefaultableStruct.IsDefault"/> property returns <see langword="true"/>, and
/// that values of all other value types are not the default.
/// </remarks>
/// <typeparam name="T"></typeparam>
internal static class Defaults<T>
{
    private static readonly DefaultabilityHelper<T> _helper = DefaultabilityHelper<T>.Create();

    /// <summary>
    /// Determines if the supplied instance of <typeparamref name="T"/> is the default, as according to the rules laid
    /// out by this class's documentation.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsDefault(T value) => _helper.IsDefault(value);
}

internal sealed class ImmutableArrayDefaultabilityHelper<T> : DefaultabilityHelper<ImmutableArray<T>>
{
    /// <inheritdoc/>
    public override bool IsDefault(ImmutableArray<T> value) => value.IsDefault;
}

internal sealed class DefaultableValueDefaultabilityHelper<T> : DefaultabilityHelper<T>
    where T : struct, IDefaultableStruct
{
    /// <inheritdoc/>
    public override bool IsDefault(T value) => value.IsDefault;
}

internal sealed class NullableDefaultabilityHelper<T> : DefaultabilityHelper<T>
{
    /// <summary>
    /// Ensures that this class cannot be constructed outside of this assembly (as it should never be constructed
    /// except when <typeparamref name="T"/> is a nullable type).
    /// </summary>
    internal NullableDefaultabilityHelper() { }

    /// <inheritdoc/>
    public override bool IsDefault(T value) => value is null;
}

internal sealed class NeverDefaultDefaultabilityHelper<T> : DefaultabilityHelper<T>
{
    /// <summary>
    /// Ensures that this class cannot be constructed outside of this assembly (as it should never be constructed
    /// except when <typeparamref name="T"/> is a defaultable or defaultability-oblivious value type).
    /// </summary>
    internal NeverDefaultDefaultabilityHelper() { }

    /// <inheritdoc/>
    public override bool IsDefault(T value) => false;
}

internal abstract class DefaultabilityHelper<T>
{
    /// <summary>
    /// Ensures that this class cannot be extended outside of this assembly.
    /// </summary>
    private protected DefaultabilityHelper() { }

    /// <summary>
    /// Determines whether or not the value passed in is the default of its non-defaultable type, or returns
    /// <see langword="false"/> if <typeparamref name="T"/> is defaultable.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public abstract bool IsDefault(T value);

    public static DefaultabilityHelper<T> Create()
    {
        var type = typeof(T);

        if (type.IsClass || type.IsInterface) return new NullableDefaultabilityHelper<T>();
        else
        {
            var genericTypeDef = type.IsGenericType ? type.GetGenericTypeDefinition() : null;

            if (genericTypeDef == typeof(ImmutableArray<>))
            {

                // Is an immutable array type - need to create an instance using reflection
                var genericTypeParam = type.GetGenericArguments()[0];
                return Unsafe.As<DefaultabilityHelper<T>>(
                    Activator.CreateInstance(
                        typeof(ImmutableArrayDefaultabilityHelper<>).MakeGenericType(genericTypeParam)));
            }
            else if (genericTypeDef == typeof(Nullable<>)) return new NullableDefaultabilityHelper<T>();
            else
            {
                if (type.GetInterfaces().Contains(typeof(IDefaultableStruct)))
                {
                    // Is defaultable - need to create an instance using reflection
                    return Unsafe.As<DefaultabilityHelper<T>>(
                            Activator.CreateInstance(
                                typeof(DefaultableValueDefaultabilityHelper<>).MakeGenericType(typeof(T))));
                }
                else return new NeverDefaultDefaultabilityHelper<T>();
            }
        }
    }
}

