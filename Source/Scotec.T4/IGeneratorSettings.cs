#region

using System.Collections.Generic;

#endregion

namespace Scotec.T4;

/// <summary>
///     Generator settings interface.
/// </summary>
public interface IGeneratorSettings
{
    /// <summary>
    ///     Gets a list of reference paths. A reference path will be used to relolve assembly references. You can your
    ///     application specific reference paths to the list.
    /// </summary>
    IList<string> ReferencePaths { get; }

    /// <summary>
    ///     Gets a list of referenced assemblies. Typically, assemblies will be referenced in the assembly directive of a T4
    ///     template. However, you can add additional assemblies to this list.
    /// </summary>
    IList<string> ReferenceAssemblies { get; }

    /// <summary>
    ///     Returns a dictionary with CodeDomProvider specific options.
    /// </summary>
    IDictionary<string, string> Options { get; }

    /// <summary>
    ///     Gets or sets the line break characters for the generated textual output. Default is LineEnding.CrLf.
    /// </summary>
    EndOfLine EndOfLine { get; set; }

    /// <summary>
    ///     Gets or sets a dictionary containing key/value pairs that can be used as variables used in assembly,
    ///     import, include, parameters, and template directives.
    /// </summary>
    IDictionary<string, string> TemplateParameters { get; set; }
}
