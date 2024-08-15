#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Scotec.T4.CodeBuilder;
using Scotec.T4.Compiler;
using Scotec.T4.Syntax;
#if !NETFRAMEWORK
using System.Runtime.Loader;
#endif

#endregion

namespace Scotec.T4;

/// <summary>
///     The Generator class. This class implements the IGenerator interface.
/// </summary>
public class Generator : IGenerator
{
    // Use static list because each generator can be created only once within an application domain.
    private static readonly Dictionary<string, Task<Type>> CompilationTasks = new();

    /// <summary>
    ///     Constructor.
    /// </summary>
    public Generator()
    {
        Settings = new GeneratorSettings();
    }

    private IGeneratorSettings Settings { get; }

    private Type GetGeneratorType(T4Template template, bool noCache)
    {
        Task<Type> task;

        lock (CompilationTasks)
        {
            if (noCache || !CompilationTasks.TryGetValue(template.Id, out task))
            {
                task = new Task<Type>(() => Compile(template));

                if (!noCache)
                {
                    CompilationTasks.Add(template, task);
                }

                task.Start();
            }
        }

        if (!task.IsCompleted)
        {
            task.Wait();
        }

        return task.Result;
    }

    private void GenerateToFile(T4Template template, bool noCache, Encoding encoding, string output, IDictionary<string, object> parameters)
    {
        var path = Path.GetDirectoryName(output);
        if (string.IsNullOrEmpty(path))
        {
            throw new T4Exception("Missing output path.");
        }

        try
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            using var writer = new StreamWriter(output, false, encoding);
            writer.AutoFlush = false;
            Generate(template, noCache, writer, parameters);
            writer.Close();
        }
        catch (T4Exception)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new T4Exception($"Error while generating template '{template}'. See inner exception.", e);
        }
    }

    private void Generate(T4Template template, bool noCache, TextWriter output, IDictionary<string, object> parameters)
    {
        parameters ??= new Dictionary<string, object>();
        var generatorType = GetGeneratorType(template, noCache);

        // Get the constructor for the generator.
        // First parameter is always a string for the endOfLine. This is parameter is implicit and not visible to the user.
        var parameterTypes = new List<Type> { typeof(string), typeof(IDictionary<string, object>) };
        //parameterTypes.AddRange(parameters.Select(parameter => parameter.GetType()));
        //var constructor = generatorType.GetConstructor(parameterTypes.ToArray());

        var parameterList = new List<object> { GetEndOfLine(), parameters }.ToArray();
        //dynamic generator = constructor.Invoke(parameterList);
        //var generator = (T4Generator)constructor.Invoke(parameterList);
        var generator = (T4Generator)Activator.CreateInstance(generatorType, parameterList);

        generator.Generate(output);

        // Flush the stream.
        output.Flush();
    }

    private string GetEndOfLine()
    {
        switch (Settings.EndOfLine)
        {
            case EndOfLine.CR:
                return "\r";
            case EndOfLine.LFCR:
                return "\n\r";
            case EndOfLine.CRLF:
                return "\r\n";
            case EndOfLine.LF:
                return "\n";
            case EndOfLine.NEL:
                return "\u0085";
            case EndOfLine.LS:
                return "\u2028";
            case EndOfLine.PS:
                return "\u2029";
            default:
                return "\r\n";
        }
    }

    private Type Compile(T4Template template)
    {
        // Parse the template
        var parser = new Parser(Settings);
        var parserResult = parser.Parse(template, null);

        // Get the language.
// ReSharper disable PossibleMultipleEnumeration
        var templateDirective = (from p in parserResult.Parts
                                 where p is TemplateDirective
                                 select (TemplateDirective)p).First();
// ReSharper restore PossibleMultipleEnumeration

        // Get a code builder.
        var codeBuilder = GetCodeBuilder(templateDirective.Language);

        // Generate the code for the generator.
// ReSharper disable PossibleMultipleEnumeration
        var code = codeBuilder.Generate(parserResult);

        var codeFileName = (from p in parserResult.Parts
                            where p is TemplateDirective
                            select ((TemplateDirective)p).CodeFile).First();

        var codeFile = string.Empty;
        if (!string.IsNullOrEmpty(codeFileName))
        {
// ReSharper disable AssignNullToNotNullAttribute
            codeFileName = Path.Combine(Path.GetDirectoryName(template), codeFileName);
// ReSharper restore AssignNullToNotNullAttribute
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

    private CodeCompiler GetCompiler(string templateDirectiveLanguage)
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

    private string[] GetReferencedAssemlies(ParserResult parserResult)
    {
        // Get all referenced assemblies from the main template.
        var assemblies = (from p in parserResult.Parts
                          where p is AssemblyDirective
                          select ((AssemblyDirective)p).Name).ToList();

        assemblies = assemblies.Union(Settings.ReferenceAssemblies).ToList();
        // Add all referenced assemblies from the included templates.
        assemblies = assemblies.Union(from i in parserResult.IncludedTemplates.Values
                                      from p in i
                                      where p is AssemblyDirective
                                      select ((AssemblyDirective)p).Name).Distinct().ToList();

        // Include this assembly to the referenced assemblies list.
        assemblies.Insert(0, Assembly.GetExecutingAssembly().Location);

        var referencePaths = GetReferencePaths();
        var assemblyPaths = assemblies.Select(assembly => FindAssembly(assembly, referencePaths)).ToList();

        // Add the System.dll as default. Thus it is not needed in the template file.
        //assemlyPaths.Add( "System.dll" );
        //var libs = DependencyContext.Default.RuntimeLibraries;
        //var libs2 = AppDomain.CurrentDomain.GetAssemblies();
        //assemlyPaths.AddRange( libs2.Where( lib => !lib.IsDynamic ).Select( lib => lib.Location ) );

#if !NETFRAMEWORK
        assemblyPaths.AddRange(((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator));
#else
#endif
        return assemblyPaths.ToArray();
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

//        private CodeDomProvider GetCodeDomProvider( string language, string compilerVersion )
//        {
//            if( !CodeDomProvider.IsDefinedLanguage( language ) )
//                throw new T4Exception( $"Unknown language '{language}'." );

//            // Make a copy of the settings.
//            var options = new Dictionary<string, string>( Settings.Options );

//            // Remove from options if added anywhere.
//            if( options.ContainsKey( "CompilerVersion" ) )
//                options.Remove( "CompilerVersion" );

//            switch( Settings.CompilerVersion )
//            {
//                // If CompilerVersion.Auto or CompilerVersion.Template and compilerVersion is empty
//                // let .NET choose the appropriate compiler version. No CompilerVersion property must be added to the options.
//                case CompilerVersion.Template:
//                    if( !string.IsNullOrEmpty( compilerVersion ) )
//                        options.Add( "CompilerVersion", compilerVersion );
//                    break;
//                case CompilerVersion.V40:
//                    options.Add( "CompilerVersion", "v4.0" );
//                    break;
//            }

//#if !FRAMEWORK35
//            return CodeDomProvider.CreateProvider( language, options );
//#else
//            switch( language )
//            {
//                case "C#":
//                    return new CSharpCodeProvider( options );
//                case "VB":
//                    return new VBCodeProvider( options );
//                default:
//                    throw new T4Exception( string.Format( "Unknown language '{0}'.", language ) );
//            }
//#endif
//        }

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

        foreach (var path in Settings.ReferencePaths)
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

        result.AddRange(Settings.ReferencePaths);

        return result.Distinct();
    }

    private string FindAssembly(string fileName, IEnumerable<string> referencePaths)
    {
        // No filename results in null path:
        if (string.IsNullOrEmpty(fileName))
        {
            return null;
        }

        // If filename contains commas, it is a strong name.
        if (fileName.Contains(','))
        {
            return GetPathFromGAC(fileName);
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

    private string GetPathFromGAC(string strongName)
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

    #region Nested type: GeneratorSettings

    internal class GeneratorSettings : IGeneratorSettings
    {
        public GeneratorSettings()
        {
            ReferencePaths = new List<string>();
            ReferenceAssemblies = new List<string>();
            Options = new Dictionary<string, string>();
            TemplateParameters = new Dictionary<string, string>();
            EndOfLine = EndOfLine.CRLF;
        }

        #region IGeneratorSettings Members

        public IList<string> ReferencePaths { get; }

        public IList<string> ReferenceAssemblies { get; }

        public IDictionary<string, string> Options { get; }

        public EndOfLine EndOfLine { get; set; }

        public IDictionary<string, string> TemplateParameters { get; set; }

        #endregion
    }

    #endregion

    #region IGenerator Members

    IGeneratorSettings IGenerator.Settings => Settings;

    void IGenerator.GenerateToFile(string template, string output, IDictionary<string, object> parameters)
    {
        GenerateToFile(template, false, Encoding.UTF8, output, parameters);
    }

    void IGenerator.GenerateToFile(string template, bool noCache, string output, IDictionary<string, object> parameters)
    {
        GenerateToFile(template, noCache, Encoding.UTF8, output, parameters);
    }

    void IGenerator.GenerateToFile(string template, Encoding encoding, string output, IDictionary<string, object> parameters)
    {
        GenerateToFile(template, false, encoding, output, parameters);
    }

    void IGenerator.GenerateToFile(string template, bool noCache, Encoding encoding, string output, IDictionary<string, object> parameters)
    {
        GenerateToFile(template, noCache, encoding, output, parameters);
    }

    void IGenerator.Generate(string template, TextWriter output, IDictionary<string, object> parameters)
    {
        Generate(template, false, output, parameters);
    }

    void IGenerator.Generate(string template, bool noCache, TextWriter output, IDictionary<string, object> parameters)
    {
        try
        {
            Generate(template, noCache, output, parameters);
        }
        catch (T4Exception)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new T4Exception($"Error while generating template '{template}'. See inner exception.", e);
        }
    }

    Task IGenerator.Compile(IEnumerable<string> templates)
    {
        var task = new Task(() => Parallel.ForEach(templates, template => GetGeneratorType(template, false)));
        task.Start();

        return task;
    }

    Task IGenerator.Compile(IEnumerable<string> templates, bool noCache)
    {
        var task = new Task(() => Parallel.ForEach(templates, template => GetGeneratorType(template, noCache)));
        task.Start();

        return task;
    }

    #endregion
}
