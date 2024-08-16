#region

using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

#endregion

namespace Scotec.T4;

/// <summary>
///     T4 text template generator interface.
/// </summary>
public interface IGenerator
{
    /// <summary>
    ///     Gets the generator settings.
    /// </summary>
    IGeneratorSettings Settings { get; }

    /// <summary>
    ///     Generates a textual output for the given template into the specifies file.
    /// </summary>
    /// <param name="template"> The T4 text template. </param>
    /// <param name="output"> The target file. </param>
    /// <param name="parameters"> Template parameters. </param>
    void GenerateToFile(T4Template template, string output, IDictionary<string, object> parameters);

    /// <summary>
    ///     Generates a textual output for the given template into the specifies file.
    /// </summary>
    /// <param name="template"> The T4 text template. </param>
    /// <param name="noCache">The generator always creates and loads a new assembly for the given template</param>
    /// <param name="output"> The target file. </param>
    /// <param name="parameters"> Template parameters. </param>
    void GenerateToFile(T4Template template, bool noCache, string output, IDictionary<string, object> parameters);

    /// <summary>
    ///     Generates a textual output for the given template into the specifies file.
    /// </summary>
    /// <param name="template"> The T4 text template. </param>
    /// <param name="encoding"> Encoding used when writing the textual output to the target file. </param>
    /// <param name="output"> The target file. </param>
    /// <param name="parameters"> Template parameters. </param>
    void GenerateToFile(T4Template template, Encoding encoding, string output, IDictionary<string, object> parameters);

    /// <summary>
    ///     Generates a textual output for the given template into the specifies file.
    /// </summary>
    /// <param name="template"> The T4 text template. </param>
    /// <param name="noCache">The generator always creates and loads a new assembly for the given template</param>
    /// <param name="encoding"> Encoding used when writing the textual output to the target file. </param>
    /// <param name="output"> The target file. </param>
    /// <param name="parameters"> Template parameters. </param>
    void GenerateToFile(T4Template template, bool noCache, Encoding encoding, string output, IDictionary<string, object> parameters);

    /// <summary>
    ///     Generates textual output for the given template into a text writer.
    /// </summary>
    /// <param name="template"> The T4 text template. </param>
    /// <param name="output"> The target text writer. </param>
    /// <param name="parameters"> Template parameters. </param>
    void Generate(T4Template template, TextWriter output, IDictionary<string, object> parameters);

    /// <summary>
    ///     Generates textual output for the given template into a text writer.
    /// </summary>
    /// <param name="template"> The T4 text template. </param>
    /// <param name="noCache">The generator always creates and loads a new assembly for the given template</param>
    /// <param name="output"> The target text writer. </param>
    /// <param name="parameters"> Template parameters. </param>
    void Generate(T4Template template, bool noCache, TextWriter output, IDictionary<string, object> parameters);

    /// <summary>
    ///     Asynchronously precompiles a list of templates without generating text. You don't have to wait until this method
    ///     has finished. You can start generating text immediately.
    /// </summary>
    /// <param name="templates"> A list of templates to precompile. </param>
    /// <returns> Return a Task object. This can be used to wait until the compilation has finished. </returns>
    Task Compile(IEnumerable<T4Template> templates);

    /// <summary>
    ///     Asynchronously precompiles a list of templates without generating text. You don't have to wait until this method
    ///     has finished. You can start generating text immediately.
    /// </summary>
    /// <param name="templates"> A list of templates to precompile. </param>
    /// <param name="noCache">The generator always creates and loads a new assembly for the given template</param>
    /// <returns> Return a Task object. This can be used to wait until the compilation has finished. </returns>
    Task Compile(IEnumerable<T4Template> templates, bool noCache);
}
