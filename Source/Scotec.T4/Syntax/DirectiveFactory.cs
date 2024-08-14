#region

using System.Text.RegularExpressions;

#endregion

namespace Scotec.T4.Syntax;

internal static class DirectiveFactory
{
    public static Directive CreateDirective(Match match, MacroResolver macroResolver)
    {
        return match.Groups["type"].Value switch
        {
            "template" => new TemplateDirective(match, macroResolver),
            "parameter" => new ParameterDirective(match, macroResolver),
            "assembly" => new AssemblyDirective(match, macroResolver),
            "import" => new ImportDirective(match, macroResolver),
            "include" => new IncludeDirective(match, macroResolver),
            _ => throw new T4Exception($"Unknow directive '{match.Groups["type"].Value}'.")
        };
    }
}
