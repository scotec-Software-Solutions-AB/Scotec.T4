#region

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.VisualBasic;
using LanguageVersion = Microsoft.CodeAnalysis.VisualBasic.LanguageVersion;

#endregion

namespace Scotec.T4.Compiler;

internal class VisualBasicCompiler : CodeCompiler
{
    internal override Compilation Compile(string className, string generatedCode, string codeBehind, PortableExecutableReference[] references)
    {
        var syntaxTrees = new List<SyntaxTree> { VisualBasicSyntaxTree.ParseText(generatedCode, new VisualBasicParseOptions(LanguageVersion.Latest)) };
        if (!string.IsNullOrEmpty(codeBehind))
        {
            syntaxTrees.Add(CSharpSyntaxTree.ParseText(codeBehind));
        }

        return VisualBasicCompilation.Create($"{className}_{Guid.NewGuid():D}.dll", syntaxTrees, references,
            new VisualBasicCompilationOptions(OutputKind.DynamicallyLinkedLibrary, false, null, null, null, null, null, OptionStrict.Off, true, true, false,
                null, false, OptimizationLevel.Release));
    }
}
