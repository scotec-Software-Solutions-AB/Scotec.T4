#region

using System.Collections.Generic;
using Scotec.T4.Syntax;

#endregion

namespace Scotec.T4;

internal class ParserResult
{
    public IEnumerable<Part> Parts { get; set; }

    public IDictionary<IncludeDirective, IEnumerable<Part>> IncludedTemplates { get; set; }

    public string TemplateName { get; set; }
}
