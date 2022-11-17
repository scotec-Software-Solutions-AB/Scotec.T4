#region

using System.Text.RegularExpressions;

#endregion


namespace Scotec.T4Generator.Syntax
{
    internal static class DirectiveFactory
    {
        public static Directive CreateDirective( Match match, MacroResolver macroResolver )
        {
            switch( match.Groups["type"].Value )
            {
                case "template":
                    return new TemplateDirective( match, macroResolver );
                case "parameter":
                    return new ParameterDirective( match, macroResolver );
                case "assembly":
                    return new AssemblyDirective( match, macroResolver );
                case "import":
                    return new ImportDirective( match, macroResolver );
                case "include":
                    return new IncludeDirective( match, macroResolver );
                default:
                    throw new T4Exception( $"Unknow directive '{match.Groups["type"].Value}'." );
            }
        }
    }
}