#region

using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

#endregion

namespace Scotec.T4;

/// <summary>
///     Base class for the generated classes. This class should be used by the generator only.
/// </summary>
public abstract class T4Generator
{
    /// <summary>
    ///     Constructor
    /// </summary>
    /// <param name="endOfLine">The line ending used for the generated text.</param>
    protected T4Generator(string endOfLine)
    {
        EndOfLine = endOfLine;
    }

    /// <summary>
    ///     The line ending used for the generated text.
    /// </summary>
    public string EndOfLine { get; }

    /// <summary>
    ///     The TextWriter can be used in the template code to write textual data directly into the output.
    /// </summary>
    protected TextWriter Output { get; private set; }

    /// <summary>
    ///     Generates the textual output. This method will be called by the template generator.
    /// </summary>
    /// <param name="output"> The target stream. </param>
    public void Generate(TextWriter output)
    {
        GenerateAsync(output).GetAwaiter().GetResult();
    }

    /// <summary>
    ///     Generates the textual output. This method will be called by the template generator.
    /// </summary>
    /// <param name="output"> The target stream. </param>
    public async Task GenerateAsync(TextWriter output)
    {
        Output = output;
        await GenerateAsync();
        await output.FlushAsync();

    }

    /// <summary>
    ///     Used by the T4 text template generator.
    /// </summary>
    protected abstract Task GenerateAsync();

    /// <summary>
    ///     Used by the T4 text template generator. Can be also called in the code part of the template.
    /// </summary>
    [DebuggerStepThrough]
    protected async Task WriteAsync(string text)
    {
        if (text != null)
        {
            await Output.WriteAsync(text);
        }
    }

    /// <summary>
    ///     Used by the T4 text template generator. Can be also called in the code part of the template.
    /// </summary>
    [DebuggerStepThrough]
    protected async Task WriteAsync(object value)
    {
        if (value != null)
        {
            await WriteAsync(value.ToString());
        }
    }
}
