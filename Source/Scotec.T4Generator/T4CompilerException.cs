#region

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

#endregion

namespace Scotec.T4Generator;

/// <summary>
///     The exception that is thrown when the compilation of the generated code fails.
/// </summary>
[Serializable]
public class T4CompilerException : T4Exception
{
    internal T4CompilerException(string message, IList<Diagnostic> errors, string generatedCode, string codeBehind)
        : base(message)
    {
        Errors = errors;
        GeneratedCode = generatedCode;
        CodeBehind = codeBehind;
    }

    /// <summary>
    ///     Gets the list of compiler errors.
    /// </summary>
    public IList<Diagnostic> Errors { get; }

    /// <summary>
    ///     Gets the generated code that could not be compiled.
    /// </summary>
    public string GeneratedCode { get; }

    /// <summary>
    ///     Gets the subsidiary code that has been referenced in the template directive.
    /// </summary>
    public string CodeBehind { get; }
}
