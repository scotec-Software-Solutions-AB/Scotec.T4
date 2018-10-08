#region

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

#endregion


namespace OMS.Ice.T4Generator.Compiler
{
    internal class CSharpCompiler : CodeCompiler
    {
        internal override Compilation Compile( string className, string generatedCode, string codeBehind, PortableExecutableReference[] references )
        {
            var syntaxTrees = new List<SyntaxTree> {CSharpSyntaxTree.ParseText( generatedCode )};
            if( !string.IsNullOrEmpty( codeBehind ) )
                syntaxTrees.Add( CSharpSyntaxTree.ParseText( codeBehind ) );

            return CSharpCompilation.Create( $"{className}_{Guid.NewGuid():D}.dll", syntaxTrees, references,
                                            new CSharpCompilationOptions( OutputKind.DynamicallyLinkedLibrary ) );
        }
    }
}