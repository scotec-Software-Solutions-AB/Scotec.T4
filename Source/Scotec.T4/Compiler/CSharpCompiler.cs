﻿#region

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

#endregion

namespace Scotec.T4.Compiler;

internal class CSharpCompiler : CodeCompiler
{
    internal override Compilation Compile(string className, string generatedCode, string codeBehind, PortableExecutableReference[] references)
    {
        var syntaxTrees = new List<SyntaxTree> { CSharpSyntaxTree.ParseText(generatedCode, new CSharpParseOptions(LanguageVersion.Latest)) };
        if (!string.IsNullOrEmpty(codeBehind))
        {
            syntaxTrees.Add(CSharpSyntaxTree.ParseText(codeBehind));
        }

        return CSharpCompilation.Create($"{className}_{Guid.NewGuid():D}.dll", syntaxTrees, references,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, false, null, null, null, null, OptimizationLevel.Release));
    }
}
