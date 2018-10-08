#region

using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMS.Ice.T4Generator.Properties;
using OMS.Ice.T4Generator.Syntax;

#endregion


namespace OMS.Ice.T4Generator.CodeBuilder
{
    internal class VBCodeBuilder : CodeBuilder
    {
        protected override string GetCodeTemplate()
        {
            return Resources.VBCodeTemplate;
        }

        protected override string GetIncludeTemplate()
        {
            return Resources.VBIncludeTemplate;
        }

        protected override string CreateImports( IEnumerable<string> imports )
        {
            var result = new StringBuilder();

            foreach( var import in imports )
                result.AppendLine( $"Imports {import}" );

            return result.ToString();
        }

        protected override string CreateFields( IEnumerable<Parameter> parameters )
        {
            var result = new StringBuilder();

            foreach( var parameter in parameters )
                result.AppendLine( $"\t\tPrivate {parameter.Name} as {parameter.Type}" );

            return result.ToString();
        }

        protected override string CreateParameters( IEnumerable<Parameter> parameters )
        {
            return string.Join(",", parameters.Select(parameter => $"ByVal {parameter.Name} As {parameter.Type}"));
        }

        protected override string CreateCallParameters( IEnumerable<Parameter> parameters )
        {
            return string.Join(",", parameters.Select(parameter => parameter.Name));
        }

        protected override string CreateFieldInitializers( IEnumerable<Parameter> parameters )
        {
            var result = new StringBuilder();

            foreach( var parameter in parameters )
                result.AppendLine( string.Format( "\t\t\tMe.{0} = {0}", parameter.Name ) );

            return result.ToString();
        }

        protected override void CreateTextBlock( StringBuilder result, IEnumerable<LineInfo> content )
        {
            foreach( var line in content )
                result.AppendLine( $"\t\t\tWrite( \"{Escape(line.Text)}\"{(line.HasEol ? " + EndOfLine" : string.Empty)} );");
        }

        protected override void CreateInlineCode( StringBuilder result, string statement )
        {
            result.AppendLine( $"\t\t\tWrite( {statement} )" );
        }

        protected override string CreateMethodCall( StringBuilder result, IncludeDirective directive )
        {
            var parameters = GetParameters( IncludedTemplates[directive] );

            result.AppendLine( $"\t\t\t{directive.Name}TemplateMethod({CreateCallParameters( parameters )})" );

            return result.ToString();
        }

        protected override string BeginLinePragma( Part part )
        {
            return $"#ExternalSource(\"{part.Source}\", {part.Line})\r\n";
        }

        protected override string EndLinePragma()
        {
            return "#End ExternalSource\r\n";
        }


        private static string Escape( string text )
        {
            return text.Replace( "\"", "\"\"" );
        }

        protected override string EscapeIdentifier( string possibleIdentifier )
        {
            return possibleIdentifier.StartsWith( "[" ) ? possibleIdentifier : "[" + possibleIdentifier + "]";
        }
    }
}