#region

using Microsoft.CodeAnalysis;

#endregion


namespace OMS.Ice.T4Generator.Compiler
{
    internal abstract class CodeCompiler
    {
        internal abstract Compilation Compile( string className, string generatedCode, string codeBehind, PortableExecutableReference[] references );
    }
}