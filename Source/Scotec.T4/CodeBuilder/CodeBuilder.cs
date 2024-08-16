#region

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scotec.T4.Syntax;

#endregion

namespace Scotec.T4.CodeBuilder;

internal abstract class CodeBuilder
{
    private readonly string[] _defaultImports;
    private string _code;
    private IEnumerable<string> _imports;
    private IEnumerable<Parameter> _parameters;

    protected CodeBuilder()
    {
        _defaultImports = new[]
        {
            "System",
            "System.Linq",
            "System.Threading.Tasks",
            "System.Collections.Generic"
        };
    }

    protected IDictionary<IncludeDirective, IEnumerable<Part>> IncludedTemplates { get; set; }

    public string GeneratorType => $"{Namespace}.{ClassName}";

    public string Namespace { get; private set; }

    public string ClassName { get; private set; }

    protected string EscapedClassName => EscapeIdentifier(ClassName);

    private ParserResult ParserResult { get; set; }

    private IEnumerable<Part> Parts => ParserResult.Parts;

    public string Generate(ParserResult parserResult)
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

        if (string.IsNullOrEmpty(fullName))
        {
            ClassName = ParserResult.Template.Name;
            Namespace = "Scotec.T4";
        }
        else
        {
            var index = fullName.LastIndexOf('.');

            ClassName = fullName.Substring(index + 1);
            Namespace = index >= 0 ? fullName.Substring(0, index) : "Scotec.T4";
        }
    }

    private void GetImports()
    {
        // Get all imports from main template.
        var imports = Parts.OfType<ImportDirective>().Select(directive => directive.Namespace);

        // Add all imports from included templates.
        imports = imports.Concat(IncludedTemplates.Values.SelectMany(included => included.OfType<ImportDirective>()
                                                                                         .Select(directive => directive.Namespace)));

        _imports = _defaultImports.Concat(imports).Distinct().ToList();
    }

    private void GetParameters()
    {
        _parameters = GetParameters(Parts);
    }

    protected static IEnumerable<Parameter> GetParameters(IEnumerable<Part> parts)
    {
        return (from p in parts
                where p is ParameterDirective
                let param = (ParameterDirective)p
                select new Parameter { Name = param.Name, Type = param.Type }).ToList();
    }

    private void Generate()
    {
        var imports = CreateImports(_imports);
        var fields = CreateFields(_parameters);
        var parameters = CreateConstructorParameters(_parameters);
        var initializers = CreateFieldInitializers(_parameters);
        var implementation = CreateImplementation(Parts);
        var features = CreateFeatures();
        var includes = CreateIncludeMethods();

        _code = GetCodeTemplate();

        // Replace all CRLF by LF. This is not really necessary, but it keeps the line endings in sync with the generated code.
        _code = _code.Replace("\r\n", "\n");

        _code = _code.Replace("<%namespace%>", Namespace);
        _code = _code.Replace("<%classname%>", EscapedClassName);
        _code = _code.Replace("<%baseclassname%>", "Scotec.T4.T4Generator");
        _code = _code.Replace("<%imports%>", imports);
        _code = _code.Replace("<%fields%>", fields);
        _code = _code.Replace("<%parameters%>", parameters);
        _code = _code.Replace("<%fieldinitializations%>", initializers);
        _code = _code.Replace("<%implementation%>", implementation.ToString());
        _code = _code.Replace("<%features%>", features.ToString());
        _code = _code.Replace("<%includes%>", includes.ToString());
    }

    private StringBuilder CreateImplementation(IEnumerable<Part> parts)
    {
        var allParts = (from p in parts
                        where p is StandardControlBlock or TextBlock or ExpressionControlBlock or IncludeDirective
                        select p).ToList();

        var result = new StringBuilder();

        foreach (var part in allParts)
        {
            if (part is StandardControlBlock block)
            {
                result.Append(BeginLinePragma(part));
                result.Append(block.Content);

                // Always add a line break at the end of the standard control block.
                result.Append("\n");
                result.Append(EndLinePragma());
            }
            else if (part is TextBlock textBlock)
            {
                result.Append(BeginLinePragma(textBlock));
                CreateTextBlock(result, CreateTextLines(textBlock.Content));
                result.Append(EndLinePragma());
            }
            else if (part is ExpressionControlBlock controlBlock)
            {
                result.Append(BeginLinePragma(controlBlock));
                CreateInlineCode(result, controlBlock.Content);
                result.Append(EndLinePragma());
            }
            else if (part is IncludeDirective directive)
            {
                CreateIncludeCode(result, directive);
            }
        }

        return result;
    }

    private void CreateIncludeCode(StringBuilder result, IncludeDirective directive)
    {
        if (directive.Mode == IncludeMode.Method)
        {
            result.Append(BeginLinePragma(directive));
            CreateMethodCall(result, directive);
            result.Append(EndLinePragma());
        }
        else
        {
            result.Append(CreateImplementation(IncludedTemplates[directive]));
        }
    }

    private StringBuilder CreateFeatures()
    {
        var parts = (from p in Parts
                     where p is FeatureControlBlock
                     select (FeatureControlBlock)p).ToList();

        parts.AddRange(from i in IncludedTemplates.Values
                       from p in i
                       where p is FeatureControlBlock
                       select (FeatureControlBlock)p);

        var result = new StringBuilder();

        foreach (var part in parts)
        {
            result.Append(part.Content);
        }

        return result;
    }

    private StringBuilder CreateIncludeMethods()
    {
        var result = new StringBuilder();
        foreach (var include in IncludedTemplates)
        {
            if (include.Key.Mode == IncludeMode.Inline)
            {
                continue;
            }

            var content = GetIncludeTemplate();
            var parameters = CreateParameters(GetParameters(include.Value));
            content = content.Replace("<%templatename%>", include.Key.Name);
            content = content.Replace("<%parameters%>", parameters);
            content = content.Replace("<%include%>", CreateImplementation(include.Value).ToString());
            result.Append(content);
        }

        return result;
    }

    private static IEnumerable<LineInfo> CreateTextLines(string content)
    {
        var result = (from t in content.Split('\n')
                      select new LineInfo { HasEol = true, Text = Unescape(t) }).ToList();

        // The last line must not have a line break.
        result.Last().HasEol = false;

        return result;
    }

    protected abstract string GetCodeTemplate();

    protected abstract string GetIncludeTemplate();

    protected abstract string CreateImports(IEnumerable<string> imports);

    protected abstract string CreateFields(IEnumerable<Parameter> parameters);

    protected abstract string CreateParameters(IEnumerable<Parameter> parameters);

    protected abstract string CreateConstructorParameters(IEnumerable<Parameter> parameters);

    protected abstract string CreateCallParameters(IEnumerable<Parameter> parameters);

    protected abstract string CreateFieldInitializers(IEnumerable<Parameter> parameters);

    protected abstract void CreateTextBlock(StringBuilder result, IEnumerable<LineInfo> content);

    protected abstract void CreateInlineCode(StringBuilder result, string statement);

    protected abstract string CreateMethodCall(StringBuilder result, IncludeDirective directive);

    protected abstract string BeginLinePragma(Part part);

    protected abstract string EndLinePragma();

    protected abstract string EscapeIdentifier(string possibleIdentifier);

    private static string Unescape(string content)
    {
        return content.Replace(@"\<#", "<#").Replace(@"\#>", "#>");
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
