#region

using System.Text.RegularExpressions;

#endregion

namespace Scotec.T4.Syntax;

internal class ParameterDirective : Directive
{
    public ParameterDirective(Match match, MacroResolver macroResolver)
        : base(match, macroResolver)
    {
    }

    public string Name => Attributes["name"];

    public string Type => Attributes["type"];
}
