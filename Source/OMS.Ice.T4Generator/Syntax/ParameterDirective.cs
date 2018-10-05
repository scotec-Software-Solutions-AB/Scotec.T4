#region

using System.Text.RegularExpressions;

#endregion


namespace OMS.Ice.T4Generator.Syntax
{
    internal class ParameterDirective : Directive
    {
        public ParameterDirective( Match match, MacroResolver macroResolver )
            : base( match, macroResolver )
        {
        }

        public string Name => Attributes["name"];

        public string Type => Attributes["type"];
    }
}