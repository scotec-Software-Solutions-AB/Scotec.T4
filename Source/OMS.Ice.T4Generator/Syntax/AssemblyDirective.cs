#region

using System.Text.RegularExpressions;

#endregion


namespace OMS.Ice.T4Generator.Syntax
{
    internal class AssemblyDirective : Directive
    {
        public AssemblyDirective( Match match, MacroResolver macroResolver )
            : base( match, macroResolver )
        {
        }

        public string Name => Attributes["name"];
    }
}