#region

using System.Collections.Generic;
using OMS.Ice.T4Generator.Syntax;

#endregion


namespace OMS.Ice.T4Generator
{
    internal class ParserResult
    {
        public IEnumerable<Part> Parts { get; set; }

        public IDictionary<IncludeDirective, IEnumerable<Part>> IncludedTemplates { get; set; }

        public string TemplateName { get; set; }
    }
}