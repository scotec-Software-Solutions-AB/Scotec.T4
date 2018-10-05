#region

using System.Collections.Generic;
using System.Linq;
using System.Text;
using OMS.Ice.T4Generator.Syntax;

#endregion


namespace OMS.Ice.T4Generator.CodeBuilder
{
    internal abstract class CodeBuilder
    {
        private string _code;
        private IEnumerable<string> _imports;
        private IEnumerable<Parameter> _parameters;

        protected IDictionary<IncludeDirective, IEnumerable<Part>> IncludedTemplates { get; set; }

        public string GeneratorType => $"{Namespace}.{ClassName}";

        public string Namespace { get; private set; }


        public string ClassName { get; private set; }

        protected string EscapedClassName => EscapeIdentifier( ClassName );


        private ParserResult ParserResult { get; set; }

        private IEnumerable<Part> Parts => ParserResult.Parts;

        public string Generate( ParserResult parserResult )
        {
            ParserResult = parserResult;
            IncludedTemplates = parserResult.IncludedTemplates;

            Analyse();
            Generate();

            return _code;
        }

        private void Analyse()
        {
            GetClassName();
            GetImports();
            GetParameters();
        }


        private void GetClassName()
        {
            var fullName = (from p in Parts
                where p is TemplateDirective
                select ((TemplateDirective)p).ClassName).First();

            if( string.IsNullOrEmpty( fullName ) )
            {
                ClassName = ParserResult.TemplateName;
                Namespace = "OMS.Ice.T4Generator";
            }
            else
            {
                var index = fullName.LastIndexOf( '.' );

                ClassName = fullName.Substring( index + 1 );
                Namespace = (index >= 0) ? fullName.Substring( 0, index ) : "OMS.Ice.T4Generator";
            }
        }


        private void GetImports()
        {
            // Get all imports from main template.
            var imports = (from p in Parts
                where p is ImportDirective
                select ((ImportDirective)p).Namespace).ToList();


            // Add all imports from included templates.
            imports = imports.Union( from i in IncludedTemplates.Values
                                    from p in i
                                    where p is ImportDirective
                                    select ((ImportDirective)p).Namespace ).ToList();

            _imports = imports;
        }

        private void GetParameters()
        {
            _parameters = GetParameters( Parts );
        }

        protected static IEnumerable<Parameter> GetParameters( IEnumerable<Part> parts )
        {
            return (from p in parts
                where p is ParameterDirective
                let param = (ParameterDirective)p
                select new Parameter {Name = param.Name, Type = param.Type}).ToList();
        }

        private void Generate()
        {
            var imports = CreateImports( _imports );
            var fields = CreateFields( _parameters );
            var parameters = CreateParameters( _parameters );
            var initializers = CreateFieldInitializers( _parameters );
            var implementation = CreateImplementation( Parts );
            var features = CreateFeatures();
            var includes = CreateIncludeMethods();


            _code = GetCodeTemplate();

            // Replace all CRLF by LF. This is not really necessary but it keeps the line endings in sync with the generated code.
            _code = _code.Replace( "\r\n", "\n" );

            _code = _code.Replace( "<%namespace%>", Namespace );
            _code = _code.Replace( "<%classname%>", EscapedClassName );
            _code = _code.Replace( "<%baseclassname%>", "OMS.Ice.T4Generator.T4Generator" );
            _code = _code.Replace( "<%imports%>", imports.ToString() );
            _code = _code.Replace( "<%fields%>", fields.ToString() );
            _code = _code.Replace( "<%parameters%>", parameters.ToString() );
            _code = _code.Replace( "<%fieldinitializations%>", initializers.ToString() );
            _code = _code.Replace( "<%implementation%>", implementation.ToString() );
            _code = _code.Replace( "<%features%>", features.ToString() );
            _code = _code.Replace( "<%includes%>", includes.ToString() );
        }

        private StringBuilder CreateImplementation( IEnumerable<Part> parts )
        {
            var allParts = (from p in parts
                where p is StandardControlBlock || p is TextBlock || p is ExpressionControlBlock || p is IncludeDirective
                select p).ToList();

            var result = new StringBuilder();

            foreach( var part in allParts )
            {
                if( part is StandardControlBlock block )
                {
                    result.Append( BeginLinePragma( part ) );
                    result.Append( block.Content );

                    // Always add a line break at the end of the standard control block.
                    result.Append( "\n" );
                    result.Append( EndLinePragma() );
                }
                else if( part is TextBlock )
                {
                    result.Append( BeginLinePragma( part ) );
                    CreateTextBlock( result, CreateTextLines( ((TextBlock)part).Content ) );
                    result.Append( EndLinePragma() );
                }
                else if( part is ExpressionControlBlock )
                {
                    result.Append( BeginLinePragma( part ) );
                    CreateInlineCode( result, ((ExpressionControlBlock)part).Content );
                    result.Append( EndLinePragma() );
                }
                else if( part is IncludeDirective )
                {
                    CreateIncludeCode( result, (IncludeDirective)part );
                }
            }

            return result;
        }

        private void CreateIncludeCode( StringBuilder result, IncludeDirective directive )
        {
            if( directive.Mode == IncludeMode.Method )
            {
                result.Append( BeginLinePragma( directive ) );
                CreateMethodCall( result, directive );
                result.Append( EndLinePragma() );
            }
            else
            {
                result.Append( CreateImplementation( IncludedTemplates[directive] ) );
            }
        }

        private StringBuilder CreateFeatures()
        {
            var parts = (from p in Parts
                where p is FeatureControlBlock
                select (FeatureControlBlock)p).ToList();

            parts.AddRange( from i in IncludedTemplates.Values
                           from p in i
                           where p is FeatureControlBlock
                           select (FeatureControlBlock)p );

            var result = new StringBuilder();

            foreach( var part in parts )
                result.Append( part.Content );

            return result;
        }

        private StringBuilder CreateIncludeMethods()
        {
            var result = new StringBuilder();
            foreach( var include in IncludedTemplates )
            {
                if( include.Key.Mode == IncludeMode.Inline )
                    continue;

                var content = GetIncludeTemplate();
                var parameters = CreateParameters( GetParameters( include.Value ) );
                content = content.Replace( "<%templatename%>", include.Key.Name );
                content = content.Replace( "<%parameters%>", parameters.ToString() );
                content = content.Replace( "<%include%>", CreateImplementation( include.Value ).ToString() );
                result.Append( content );
            }


            return result;
        }


        private static IEnumerable<LineInfo> CreateTextLines( string content )
        {
            var result = (from t in content.Split( '\n' )
                select new LineInfo {HasEol = true, Text = Unescape( t )}).ToList();

            // The last line must not have a line break.
            result.Last().HasEol = false;

            return result;
        }

        protected abstract string GetCodeTemplate();

        protected abstract string GetIncludeTemplate();

        protected abstract StringBuilder CreateImports( IEnumerable<string> imports );

        protected abstract StringBuilder CreateFields( IEnumerable<Parameter> parameters );

        protected abstract StringBuilder CreateParameters( IEnumerable<Parameter> parameters );

        protected abstract StringBuilder CreateCallParameters( IEnumerable<Parameter> parameters );

        protected abstract StringBuilder CreateFieldInitializers( IEnumerable<Parameter> parameters );

        protected abstract void CreateTextBlock( StringBuilder result, IEnumerable<LineInfo> content );

        protected abstract void CreateInlineCode( StringBuilder result, string statement );

        protected abstract StringBuilder CreateMethodCall( StringBuilder result, IncludeDirective directive );

        protected abstract string BeginLinePragma( Part part );

        protected abstract string EndLinePragma();

        protected abstract string EscapeIdentifier( string possibleIdentifier );

        private static string Unescape( string content )
        {
            return content.Replace( @"\<#", "<#" ).Replace( @"\#>", "#>" );
        }


        #region Nested type: LineInfo

        protected class LineInfo
        {
            public string Text { get; set; }
            public bool HasEol { get; set; }
        }

        #endregion


        #region Nested type: Parameter

        protected struct Parameter
        {
            public string Name;
            public string Type;
        }

        #endregion
    }
}