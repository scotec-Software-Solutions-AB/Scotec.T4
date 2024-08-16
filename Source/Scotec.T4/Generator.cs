#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

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
    private readonly Dictionary<string, Task<Type>> _compilationTasks = new();

    /// <summary>
    ///     Constructor.
    /// </summary>
    public Generator()
    {
        Options = new T4Options();
    }

    /// <summary>
    ///     Constructor.
    /// </summary>
    public Generator(T4Options options)
    {
        Options = options;
    }

    public TextGenerator Build(T4Template template)
    {
        var generatorType = GetGeneratorType(template);

        return new TextGenerator(generatorType, template.Name, GetEndOfLine());
    }

    public T4Options Options { get; }

    public void GenerateToFile(T4Template template, string outputFile, IDictionary<string, object> parameters)
    {
        GenerateToFile(template, outputFile, Encoding.UTF8, parameters);
    }

    public void Generate(T4Template template, TextWriter output, IDictionary<string, object> parameters)
    {
        var textGenerator = Build(template);
        textGenerator.Generate(output, parameters).GetAwaiter().GetResult();
    }

    public Task Compile(IEnumerable<T4Template> templates)
    {
        var task = new Task(() => Parallel.ForEach(templates, template => GetGeneratorType(template)));
        task.Start();

        return task;
    }

    public void GenerateToFile(T4Template template, string outputFile, Encoding encoding, IDictionary<string, object> parameters)
    {
        var path = Path.GetDirectoryName(outputFile);
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

            using var writer = new StreamWriter(outputFile, false, encoding);
            writer.AutoFlush = false;
            Generate(template, writer, parameters);
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

    private Type GetGeneratorType(T4Template template)
    {
        Task<Type> task;

        lock (_compilationTasks)
        {
            if (!_compilationTasks.TryGetValue(template.Name, out task))
            {
                task = new Task<Type>(() =>
                {
                    var parser = new Parser(Options);
                    var parserResult = parser.Parse(template);

                    return new T4Compiler(Options).Compile(parserResult);
                });
                _compilationTasks.Add(template.Name, task);
                task.Start();
            }
        }

        if (!task.IsCompleted)
        {
            task.Wait();
        }

        return task.Result;
    }

    private string GetEndOfLine()
    {
        switch (Options.EndOfLine)
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
}
