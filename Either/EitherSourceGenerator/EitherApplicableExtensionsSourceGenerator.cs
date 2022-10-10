﻿using System;
using System.Linq;
using System.Text;

namespace Rem.Core.Utilities.Monads.SourceGeneration;

/// <summary>
/// Generates a static class that contains apply methods for the "either" monad defined in this solution.
/// </summary>
[Generator]
public class EitherApplicableExtensionsSourceGenerator : ISourceGenerator
{
    #region Constants
    private const string Namespace = "Rem.Core.Utilities.Monads";
    private const string TypeName = "EitherApplicableExtensions";
    #endregion

    #region Generation
    /// <inheritdoc/>
    public void Execute(GeneratorExecutionContext context)
    {
        // Build up the source code
        // Start with the class definition and doc comment
        // Need to explicitly enable `nullable` context
        StringBuilder sourceBuilder = new($@"// <auto-generated/>

#nullable enable

using Rem.Core.Attributes;
using System;

namespace {Namespace};

/// <summary>
/// Applicable extension methods for instances of <see cref=""Either{{TLeft, TRight}}"".
/// </summary>
public static class {TypeName}
{{
");

        // Add all methods starting with the no-argument versions and progressing to the 16-argument versions
        sourceBuilder.Append(GetNoArgsMethodsString());
        sourceBuilder.AppendLine();
        sourceBuilder.Append(GetSingleArgMethodsString());
        for (int i = 2; i <= 16; i++)
        {
            sourceBuilder.AppendLine();
            sourceBuilder.Append(GetMultipleArgMethodsString(i));
        }

        // Add the end of the class
        sourceBuilder.Append($@"
}}");

        // Add the generated file to the context
        context.AddSource($"{TypeName}.g.cs", sourceBuilder.ToString());
    }

    /// <inheritdoc/>
    /// <remarks>
    /// This method does nothing, as there is no initialization required for this generator.
    /// </remarks>
    public void Initialize(GeneratorInitializationContext context)
    {
        //
    }
    #endregion

    #region Helpers
    private static string GetNoArgsMethodsString()
        => @"
    /// <summary>
    /// Calls the function on the left side of the current instance.
    /// </summary>
    /// <typeparam name=""TLeftResult"">The return type of the left side.</typeparam>
    /// <typeparam name=""TRight"">The type of the right side.</typeparam>
    /// <param name=""either""></param>
    /// <returns></returns>
    public static Either<TLeftResult, TRight> InvokeLeft<TLeftResult, TRight>(
        [NonDefaultableStruct] this Either<Func<TLeftResult>, TRight> either)
        => either.IsRight ? new(either._right) : new(either._left());

    /// <summary>
    /// Calls the function wrapped in the current instance.
    /// </summary>
    /// <typeparam name=""TLeftResult"">The return type of the left side.</typeparam>
    /// <typeparam name=""TRightResult"">The return type of the right side.</typeparam>
    /// <param name=""either""></param>
    /// <returns></returns>
    public static Either<TLeftResult, TRightResult> InvokeEither<TLeftResult, TRightResult>(
        [NonDefaultableStruct] this Either<Func<TLeftResult>, Func<TRightResult>> either)
        => either.IsRight ? new(either._right()) : new(either._left());

    /// <summary>
    /// Calls the function on the right side of the current instance.
    /// </summary>
    /// <typeparam name=""TLeft"">The type of the left side.</typeparam>
    /// <typeparam name=""TRightResult"">The return type of the right side.</typeparam>
    /// <param name=""either""></param>
    /// <returns></returns>
    [return: NotDefaultIfNotDefault(""either"")]
    public static Either<TLeft, TRightResult> InvokeRight<TLeft, TRightResult>(
        this Either<TLeft, Func<TRightResult>> either)
        => either.IsRight ? new(either._right()) : new(either._left);

    /// <summary>
    /// Calls the function on the left side of the current instance.
    /// </summary>
    /// <typeparam name=""TRight"">The type of the right side.</typeparam>
    /// <param name=""either""></param>
    public static void InvokeLeft<TRight>(
        this Either<
#nullable disable
            Action,
#nullable enable
            TRight> either)
    {
        if (either.IsLeft) either._left?.Invoke();
    }

    /// <summary>
    /// Calls the function wrapped in the current instance.
    /// </summary>
    /// <param name=""either""></param>
    public static void InvokeEither(
#nullable disable
        this Either<Action, Action> either)
#nullable enable
    {
        if (either.IsRight) either._right?.Invoke();
        else either._left?.Invoke();
    }

    /// <summary>
    /// Calls the function on the right side of the current instance.
    /// </summary>
    /// <typeparam name=""TLeft"">The type of the left side.</typeparam>
    /// <param name=""either""></param>
    public static void InvokeRight<TLeft>(
        this Either<
                TLeft,
#nullable disable
                Action
#nullable enable
            > either)
    {
        if (either.IsRight) either._right?.Invoke();
    }
";

    private static string GetSingleArgMethodsString()
        => @"
    /// <summary>
    /// Applies the function on the left side of the current instance to the supplied argument.
    /// </summary>
    /// <typeparam name=""TArg"">The type of the argument of the left side.</typeparam>
    /// <typeparam name=""TLeftResult"">The return type of the left side.</typeparam>
    /// <typeparam name=""TRight"">The type of the right side.</typeparam>
    /// <param name=""either""></param>
    /// <param name=""arg"">The argument to apply the left side to.</param>
    /// <returns></returns>
    public static Either<TLeftResult, TRight> InvokeLeft<TArg, TLeftResult, TRight>(
        [NonDefaultableStruct] this Either<Func<TArg, TLeftResult>, TRight> either, TArg arg)
        => either.IsRight ? new(either._right) : new(either._left(arg));

    /// <summary>
    /// Applies the function on the right side of the current instance to the supplied argument.
    /// </summary>
    /// <typeparam name=""TLeft"">The type of the left side.</typeparam>
    /// <typeparam name=""TArg"">The type of the argument of the right side.</typeparam>
    /// <typeparam name=""TRightResult"">The return type of the right side.</typeparam>
    /// <param name=""either""></param>
    /// <param name=""arg"">The argument to apply the right side to.</param>
    /// <returns></returns>
    [return: NotDefaultIfNotDefault(""either"")]
    public static Either<TLeft, TRightResult> InvokeRight<TLeft, TArg, TRightResult>(
        this Either<TLeft, Func<TArg, TRightResult>> either, TArg arg)
        => either.IsRight ? new(either._right(arg)) : new(either._left);

    /// <summary>
    /// Calls the function on the left side of the current instance on the supplied argument.
    /// </summary>
    /// <typeparam name=""TArg"">The type of the argument of the left side.</typeparam>
    /// <typeparam name=""TRight"">The type of the right side.</typeparam>
    /// <param name=""either""></param>
    /// <param name=""arg"">The argument to call the left side on.</param>
    public static void InvokeLeft<TArg, TRight>(
        this Either<
                Action<TArg
#nullable disable
                    >,
#nullable enable
                TRight> either,
        TArg arg)
    {
        if (either.IsLeft) either._left?.Invoke(arg);
    }

    /// <summary>
    /// Calls the function on the right side of the current instance on the supplied argument.
    /// </summary>
    /// <typeparam name=""TLeft"">The type of the left side.</typeparam>
    /// <typeparam name=""TArg"">The type of the argument of the right side.</typeparam>
    /// <param name=""either""></param>
    /// <param name=""arg"">The argument to call the right side on.</param>
    public static void InvokeRight<TLeft, TArg>(
        this Either<
                TLeft,
                Action<TArg
#nullable disable
                    >
#nullable enable
            > either, TArg arg)
    {
        if (either.IsRight) either._right?.Invoke(arg);
    }
";

    private static string GetMultipleArgMethodsString(int argCount)
    {
        #region Static Helpers
        static string delegateArgTypeName(int number) => $"TArg{number}";

        static string delegateArgName(int number) => $"arg{number}";
        #endregion

        #region Setup
        var argNumberRange = Enumerable.Range(1, argCount);

        var delegateArgTypeList = argNumberRange.Select(delegateArgTypeName);

        var delegateArgNameList = argNumberRange.Select(delegateArgName);

        var delegateArgTypeListStr = string.Join(", ", delegateArgTypeList);

        var delegateArgNameListStr = string.Join(", ", delegateArgNameList);

        string delegateArgTypeDocsStr(EitherSide side)
            => string.Join(
                Environment.NewLine,
                delegateArgTypeList.Zip(
                    argNumberRange,
                    (tStr, i)
                        => "    /// "
                            + TypeArgDocStr(
                                tStr,
                                $"The type of the {i.FormatAsOrdinal()} argument of the"
                                    + $" {side.ToString().ToLower()} side.")));

        string delegateArgDocsStr(EitherSide side)
            => string.Join(
                Environment.NewLine,
                delegateArgNameList.Zip(
                    argNumberRange,
                    (pName, i)
                        => "    /// "
                            + ParamDocStr(
                                pName,
                                $"The {i.FormatAsOrdinal()} argument to apply the"
                                    + $" {side.ToString().ToLower()} side to.")));

        string funcTypeStr(EitherSide side) => $"Func<{delegateArgTypeListStr}, {FuncResultTypeStr(side)}>";

        string funcEitherTypeStr(EitherSide side)
            => side == EitherSide.Left
                ? $"Either<{funcTypeStr(EitherSide.Left)}, TRight>"
                : $"Either<TLeft, {funcTypeStr(EitherSide.Right)}>";

        string actionEitherTypeStr(EitherSide side)
            => side == EitherSide.Left
                ? $@"Either<
                Action<{delegateArgTypeListStr}
#nullable disable
                    >,
#nullable enable
                TRight>"
                : $@"Either<
                TLeft,
                Action<{delegateArgTypeListStr}
#nullable disable
                    >
#nullable enable
                >";

        var funcArgDeclarationsListStr
            = string.Join(", ", delegateArgTypeList.Zip(delegateArgNameList, (type, name) => $"{type} {name}"));

        string funcMethodArgDeclarationsListStr(EitherSide side)
            => $"this {funcEitherTypeStr(side)} either, {funcArgDeclarationsListStr}";

        string actionMethodArgDeclarationsListStr(EitherSide side)
            => $"this {actionEitherTypeStr(side)} either, {funcArgDeclarationsListStr}";
        #endregion

        return $@"
    /// <summary>
    /// Applies the function on the left side of the current instance to the arguments passed in.
    /// </summary>
{delegateArgTypeDocsStr(EitherSide.Left)}
    /// <typeparam name=""TLeftResult"">The return type of the left side.</typeparam>
    /// <typeparam name=""TRight"">The type of the right side.</typeparam>
    /// <param name=""either""></param>
{delegateArgDocsStr(EitherSide.Left)}
    /// <returns></returns>
    public static Either<TLeftResult, TRight> InvokeLeft<{delegateArgTypeListStr}, TLeftResult, TRight>(
        [NonDefaultableStruct] {funcMethodArgDeclarationsListStr(EitherSide.Left)})
        => either.IsRight ? new(either._right) : new(either._left({delegateArgNameListStr}));

    /// <summary>
    /// Applies the function on the right side of the current instance to the arguments passed in.
    /// </summary>
    /// <typeparam name=""TLeft"">The type of the left side.</typeparam>
{delegateArgTypeDocsStr(EitherSide.Right)}
    /// <typeparam name=""TRightResult"">The return type of the right side.</typeparam>
    /// <param name=""either""></param>
{delegateArgDocsStr(EitherSide.Right)}
    /// <returns></returns>
    [return: NotDefaultIfNotDefault(""either"")]
    public static Either<TLeft, TRightResult> InvokeRight<TLeft, {delegateArgTypeListStr}, TRightResult>(
        {funcMethodArgDeclarationsListStr(EitherSide.Right)})
        => either.IsRight ? new(either._right({delegateArgNameListStr})) : new(either._left);

    /// <summary>
    /// Calls the function on the left side of the current instance on the arguments passed in.
    /// </summary>
{delegateArgTypeDocsStr(EitherSide.Left)}
    /// <typeparam name=""TRight"">The type of the right side.</typeparam>
    /// <param name=""either""></param>
{delegateArgDocsStr(EitherSide.Left)}
    public static void InvokeLeft<{delegateArgTypeListStr}, TRight>(
        {actionMethodArgDeclarationsListStr(EitherSide.Left)})
    {{
        if (either.IsLeft) either._left?.Invoke({delegateArgNameListStr});
    }}

    /// <summary>
    /// Calls the function on the right side of the current instance on the arguments passed in.
    /// </summary>
    /// <typeparam name=""TLeft"">The type of the left side.</typeparam>
{delegateArgTypeDocsStr(EitherSide.Right)}
    /// <param name=""either""></param>
{delegateArgDocsStr(EitherSide.Right)}
    public static void InvokeRight<TLeft, {delegateArgTypeListStr}>(
        {actionMethodArgDeclarationsListStr(EitherSide.Right)})
    {{
        if (either.IsRight) either._right?.Invoke({delegateArgNameListStr});
    }}
";
    }

    private static string FuncResultTypeStr(EitherSide side)
        => side == EitherSide.Left ? "TLeftResult" : "TRightResult";

    private static string TypeArgDocStr(string name, string description)
        => $"<typeparam name=\"{name}\">{description}</typeparam>";

    private static string ParamDocStr(string name, string description)
        => $"<param name=\"{name}\">{description}</param>";
    #endregion
}
