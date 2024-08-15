#region

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scotec.T4.Syntax;

#endregion

namespace Scotec.T4.CodeBuilder;

internal class CsCodeBuilder : CodeBuilder
{
    protected override string GetCodeTemplate()
    {
        return Templates.CsCodeTemplate;
    }

    protected override string GetIncludeTemplate()
    {
        return Templates.CsIncludeTemplate;
    }

    protected override string CreateImports(IEnumerable<string> imports)
    {
        var result = new StringBuilder();

        foreach (var import in imports)
        {
            result.AppendLine($"using {import};");
        }

        return result.ToString();
    }

    protected override string CreateFields(IEnumerable<Parameter> parameters)
    {
        var result = new StringBuilder();

        foreach (var parameter in parameters)
        {
            result.AppendLine($"\t\tprivate {parameter.Type} {parameter.Name};");
        }

        return result.ToString();
    }

    protected override string CreateConstructorParameters(IEnumerable<Parameter> parameters)
    {
        return ", IDictionary<string, object> parameters";
    }

    protected override string CreateParameters(IEnumerable<Parameter> parameters)
    {
        return string.Join(", ", parameters.Select(parameter => $"{parameter.Type} {parameter.Name}"));
    }

    protected override string CreateCallParameters(IEnumerable<Parameter> parameters)
    {
        return string.Join(",", parameters.Select(parameter => parameter.Name));
    }

    protected override string CreateFieldInitializers(IEnumerable<Parameter> parameters)
    {
        var result = new StringBuilder();

        foreach (var parameter in parameters)
        {
            result.AppendLine($"\t\t\tthis.{parameter.Name} = ({parameter.Type})parameters[\"{parameter.Name}\"];");
        }

        return result.ToString();
    }

    protected override void CreateTextBlock(StringBuilder result, IEnumerable<LineInfo> content)
    {
        foreach (var line in content)
        {
            result.AppendLine($"\t\t\tWrite( \"{Escape(line.Text)}\"{(line.HasEol ? " + EndOfLine" : string.Empty)} );");
        }
    }

    protected override void CreateInlineCode(StringBuilder result, string statement)
    {
        result.AppendLine($"\t\t\tWrite( {statement} );");
    }

    protected override string CreateMethodCall(StringBuilder result, IncludeDirective directive)
    {
        var parameters = GetParameters(IncludedTemplates[directive]);

        result.AppendLine($"\t\t\t{directive.Name}TemplateMethod({CreateCallParameters(parameters)});");

        return result.ToString();
    }

    protected override string BeginLinePragma(Part part)
    {
        return $"#line {part.Line} \"{part.Source}\"\r\n";
    }

    protected override string EndLinePragma()
    {
        return string.Empty;
    }

    private static string Escape(string text)
    {
        return text.Replace("\\", "\\\\").Replace("\"", "\\\"");
    }

    protected override string EscapeIdentifier(string possibleIdentifier)
    {
        return possibleIdentifier.StartsWith("@") ? possibleIdentifier : "@" + possibleIdentifier;
    }
}
