﻿#region

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;

#endregion


namespace OMS.Ice.T4Generator.Compiler
{
    internal class VisualBasicCompiler : CodeCompiler
    {
        internal override Compilation Compile( string className, string generatedCode, string codeBehind, PortableExecutableReference[] references )
        {
            var syntaxTrees = new List<SyntaxTree> {VisualBasicSyntaxTree.ParseText( generatedCode )};
            if( !string.IsNullOrEmpty( codeBehind ) )
                syntaxTrees.Add( CSharpSyntaxTree.ParseText( codeBehind ) );

            return VisualBasicCompilation.Create( $"{className}_{Guid.NewGuid():D}.dll", syntaxTrees, references,
                                                 new VisualBasicCompilationOptions( OutputKind.DynamicallyLinkedLibrary ) );
        }
    }
}