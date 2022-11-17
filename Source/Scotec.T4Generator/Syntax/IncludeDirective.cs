#region

using System;
using System.IO;
using System.Text.RegularExpressions;

#endregion


namespace Scotec.T4Generator.Syntax
{
    internal class IncludeDirective : Directive
    {
        public IncludeDirective( Match match, MacroResolver macroResolver )
            : base( match, macroResolver )
        {
        }

        public string File => Attributes["file"];

        public string Name => Path.GetFileNameWithoutExtension( File );

        public IncludeMode Mode
        {
            get
            {
                if( !Attributes.ContainsKey( "mode" ) )
                    return IncludeMode.Inline;

                return Enum.TryParse( Attributes["mode"], true, out IncludeMode mode ) ? mode : IncludeMode.Inline;
            }
        }

        public override bool Equals( object obj )
        {
            return obj is IncludeDirective toCompare && Name.Equals( toCompare.Name ) && Mode.Equals( toCompare.Mode );
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}