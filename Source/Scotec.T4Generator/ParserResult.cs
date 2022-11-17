#region

using System.Collections.Generic;
using Scotec.T4Generator.Syntax;

#endregion


namespace Scotec.T4Generator
{
    internal class ParserResult
    {
        public IEnumerable<Part> Parts { get; set; }

        public IDictionary<IncludeDirective, IEnumerable<Part>> IncludedTemplates { get; set; }

        public string TemplateName { get; set; }
    }
}