using System.Collections.Generic;

namespace Scotec.T4;

/// <summary>
///     T4 genberator settings.
/// </summary>
public class T4Options
{
    /// <summary>
    ///     Constructor
    /// </summary>
    public T4Options()
    {
        ReferencePaths = new List<string>();
        ReferenceAssemblies = new List<string>();
        SearchPaths = new List<string>();
        Options = new Dictionary<string, string>();
        TemplateParameters = new Dictionary<string, string>();
        EndOfLine = EndOfLine.CRLF;
    }

    #region IGeneratorSettings Members

    /// <summary>
    ///     Gets or sets a list of reference paths. A reference path will be used to resolve assembly references.
    /// </summary>
    public IList<string> ReferencePaths { get; set; }

    /// <summary>
    ///     Returns a list of referenced assemblies. Typically, assemblies will be referenced in the assembly directive of a T4
    ///     template.
    /// </summary>
    public IList<string> ReferenceAssemblies { get; set; }

    /// <summary>
    ///     Returns a list of search paths in which templates are searched for.
    /// </summary>
    public IList<string> SearchPaths { get; set; }

    /// <summary>
    ///     Returns a dictionary with CodeDomProvider specific options.
    /// </summary>
    public IDictionary<string, string> Options { get; }

    /// <summary>
    ///     Gets or sets the line break characters for the generated textual output. Default is LineEnding.CrLf.
    /// </summary>
    public EndOfLine EndOfLine { get; set; }

    /// <summary>
    ///     Gets or sets a dictionary containing key/value pairs that can be used as variables used in assembly,
    ///     import, include, parameters, and template directives.
    /// </summary>
    public IDictionary<string, string> TemplateParameters { get; set; }

    #endregion
}
