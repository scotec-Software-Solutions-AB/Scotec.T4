using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Scotec.T4.CodeBuilder;
using Scotec.T4.Compiler;
using Scotec.T4.Syntax;
#if !NETFRAMEWORK
using System.Runtime.Loader;
#endif

namespace Scotec.T4;

internal class T4Compiler
{
    private readonly T4Options _settings;

    public T4Compiler(T4Options settings)
    {
        _settings = settings;
    }

    public Type Compile(ParserResult parserResult)
    {
        // Get the language.
        var templateDirective = (from p in parserResult.Parts
                                 where p is TemplateDirective
                                 select (TemplateDirective)p).First();

        // Get a code builder.
        var codeBuilder = GetCodeBuilder(templateDirective.Language);

        // Generate the code for the generator.
        var code = codeBuilder.Generate(parserResult);

        var codeFileName = (from p in parserResult.Parts
                            where p is TemplateDirective
                            select ((TemplateDirective)p).CodeFile).First();

        var codeFile = string.Empty;
        if (!string.IsNullOrEmpty(codeFileName))
        {
            codeFileName = Helper.FindFile(codeFileName, parserResult.SearchPaths);
            codeFile = File.OpenText(codeFileName).ReadToEnd();
        }

        var assemlyPaths = GetReferencedAssemlies(parserResult);
        var references = assemlyPaths.Select(path => MetadataReference.CreateFromFile(path)).ToArray();
        var compiler = GetCompiler(templateDirective.Language);
        var compilation = compiler.Compile(codeBuilder.ClassName, code, codeFile, references);

        using var stream = new MemoryStream();
        var result = compilation.Emit(stream);
        if (!result.Success)
        {
            var failures = result.Diagnostics.Where(diagnostic => diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error);
            foreach (var diagnostic in failures)
            {
                Console.Error.WriteLine("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
            }

            throw new T4CompilerException("Compiler error.", failures.ToList(), code, codeFile);
        }

        stream.Seek(0, SeekOrigin.Begin);

#if !NETFRAMEWORK
        var currentLoadContext = AssemblyLoadContext.GetLoadContext(GetType().Assembly);
        var assembly = currentLoadContext.LoadFromStream(stream);
#else
        var data = stream.ToArray();
        var assembly = Assembly.Load(data);
#endif

        return assembly.GetType(codeBuilder.GeneratorType);
    }

    private static CodeBuilder.CodeBuilder GetCodeBuilder(string language)
    {
        CodeBuilder.CodeBuilder codeBuilder;
        switch (language)
        {
            case "C#":
                codeBuilder = new CsCodeBuilder();
                break;
            case "VB":
                codeBuilder = new VbCodeBuilder();
                break;
            default:
                codeBuilder = new CsCodeBuilder();
                break;
        }

        return codeBuilder;
    }

    private string[] GetReferencedAssemlies(ParserResult parserResult)
    {
        // Include this assembly to the referenced assemblies list.
        var assemblies = (IEnumerable<string>)new [] {Assembly.GetExecutingAssembly().Location};


        // Get all referenced assemblies from the main template.
        assemblies = assemblies.Concat(from p in parserResult.Parts
                          where p is AssemblyDirective
                          select ((AssemblyDirective)p).Name);

        assemblies = assemblies.Concat(_settings.ReferenceAssemblies);
        // Add all referenced assemblies from the included templates.
        assemblies = assemblies.Concat(from i in parserResult.IncludedTemplates.Values
                                       from p in i
                                       where p is AssemblyDirective
                                       select ((AssemblyDirective)p).Name)
                               .Distinct();

        var referencePaths = GetReferencePaths();
        var assemblyPaths = assemblies.Select(assembly => FindAssembly(assembly, referencePaths)).ToList();

#if !NETFRAMEWORK
        assemblyPaths.AddRange(((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator));
#else
        // Add the System.dll as default. Thus it is not needed in the template file.
        assemblyPaths.Add( "System.dll" );
        var libs = AppDomain.CurrentDomain.GetAssemblies();
        assemblyPaths.AddRange( libs.Where( lib => !lib.IsDynamic ).Select( lib => lib.Location ) );
#endif
        return assemblyPaths.ToArray();
    }

    private IEnumerable<string> GetReferencePaths()
    {
        var result = new List<string>();

        var domain = AppDomain.CurrentDomain;
        var baseDirectory = domain.BaseDirectory;
        result.Add(baseDirectory);

        if (!string.IsNullOrEmpty(domain.RelativeSearchPath))
        {
            result.AddRange(from d in domain.RelativeSearchPath.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries)
                            select Path.Combine(baseDirectory, d));
        }

        foreach (var path in _settings.ReferencePaths)
        {
            if (path.EndsWith("*"))
            {
                result.AddRange(Directory.GetDirectories(path.Trim('*'), "*", SearchOption.AllDirectories));
            }
            else
            {
                result.Add(path);
            }
        }

        result.AddRange(_settings.ReferencePaths);

        return result.Distinct();
    }

    private static string FindAssembly(string fileName, IEnumerable<string> referencePaths)
    {
        // No filename results in null path:
        if (string.IsNullOrEmpty(fileName))
        {
            return null;
        }

        // If filename contains commas, it is a strong name.
        if (fileName.Contains(','))
        {
            return GetPathFromGac(fileName);
        }

        // If absolute path, just return it.
        if (Path.IsPathRooted(fileName))
        {
            return fileName;
        }

        // Search for the file in each reference directory.
        var path = referencePaths.Select(dir => Path.Combine(dir, fileName)).FirstOrDefault(File.Exists);

        if (string.IsNullOrEmpty(path))
        {
            return fileName;
        }

        return path;
    }

    private static string GetPathFromGac(string strongName)
    {
        try
        {
            var assembly = Assembly.Load(strongName);

            return assembly.Location;
        }
        catch (Exception e)
        {
            throw new T4Exception($"Could not load strong named assembly: '{strongName}'", e);
        }
    }

    private static CodeCompiler GetCompiler(string templateDirectiveLanguage)
    {
        switch (templateDirectiveLanguage)
        {
            case "CS":
            case "C#":
                return new CSharpCompiler();
            case "VB":
                return new VisualBasicCompiler();
            default:
                throw new T4Exception($"Unknown language; {templateDirectiveLanguage} ");
        }
    }
}
