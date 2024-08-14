#region

using System.Text.RegularExpressions;

#endregion

namespace Scotec.T4.Syntax;

internal class TemplateDirective : Directive
{
    public TemplateDirective(Match match, MacroResolver macroResolver)
        : base(match, macroResolver)
    {
    }

    public string Language => Attributes.TryGetValue("language", out var language) ? language : "C#";

    public string ClassName => Attributes.TryGetValue("classname", out var className) ? className : string.Empty;

    public string CodeFile => Attributes.TryGetValue("codefile", out var codeFile) ? codeFile : string.Empty;
}
