#region

using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

#endregion


namespace OMS.Ice.T4Generator
{
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
        void GenerateToFile( string template, string output, params object[] parameters );

        /// <summary>
        ///     Generates a textual output for the given template into the specifies file.
        /// </summary>
        /// <param name="template"> The T4 text template. </param>
        /// <param name="encoding"> Encoding used when writing the textual output to the target file. </param>
        /// <param name="output"> The target file. </param>
        /// <param name="parameters"> Template parameters. </param>
        void GenerateToFile( string template, Encoding encoding, string output, params object[] parameters );


        /// <summary>
        ///     Generates textual output for the given template into a text writer.
        /// </summary>
        /// <param name="template"> The T4 text template. </param>
        /// <param name="output"> The target text writer. </param>
        /// <param name="parameters"> Template parameters. </param>
        void Generate( string template, TextWriter output, params object[] parameters );

#if !FRAMEWORK35
        /// <summary>
        ///     Asynchronously precompiles a list of templates without generating text. You don't have to wait until this method
        ///     has finished. You can start generating text immediately.
        /// </summary>
        /// <param name="templates"> A list of templates to precompile. </param>
        /// <returns> Return a Task object. This can be used to wait until the compilation has finished. </returns>
        Task Compile( IEnumerable<string> templates );
#else
/// <summary>
/// Asynchronously compiles a list of templates without generating text.
/// You don't have to wait until this method has finished. You can start generating text immediately.
/// The compilation of the templates itself is still synchronized. To run the  
/// parallel template compilation, use the .NET 4.0 version of OMS.Ice.
/// </summary>
/// <param name="templates">A list of templates to precompile.</param>
        void Compile(IEnumerable<string> templates);
#endif
        //FRAMEWORK35
    }
}