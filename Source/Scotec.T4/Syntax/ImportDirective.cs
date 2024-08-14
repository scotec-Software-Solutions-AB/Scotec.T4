#region

using System.Text.RegularExpressions;

#endregion

namespace Scotec.T4.Syntax;

internal class ImportDirective : Directive
{
    public ImportDirective(Match match, MacroResolver macroResolver)
        : base(match, macroResolver)
    {
    }

    public string Namespace => Attributes["namespace"];
}
