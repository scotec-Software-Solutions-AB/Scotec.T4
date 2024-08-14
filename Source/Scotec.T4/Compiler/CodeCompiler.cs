#region

using Microsoft.CodeAnalysis;

#endregion

namespace Scotec.T4.Compiler;

internal abstract class CodeCompiler
{
    internal abstract Compilation Compile(string className, string generatedCode, string codeBehind, PortableExecutableReference[] references);
}
