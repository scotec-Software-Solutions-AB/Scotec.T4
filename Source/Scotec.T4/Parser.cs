#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Scotec.T4.Syntax;

#endregion

namespace Scotec.T4;

internal class Parser
{
    private static readonly Regex LineEndings = new("(\r\n|\n\r|\n|\r|\u0085|\u000C|\u2028|\u2029)", RegexOptions.Compiled);
    private static readonly Regex Identifier = new(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]", RegexOptions.Compiled);

    public Parser(IGeneratorSettings settings)
    {
        Settings = settings;
    }

    private string TemplateFile { get; set; }
    public int[] Lines { get; private set; }
    private string TemplatePath { get; set; }
    private IGeneratorSettings Settings { get; }

    public ParserResult Parse(T4Template template, string templatePath)
    {
        var result = new ParserResult { IncludedTemplates = new Dictionary<IncludeDirective, IEnumerable<Part>>() };
        result.Parts = Parse(template, templatePath, result.IncludedTemplates);
        result.TemplateName = MakeIdentifier(Path.GetFileNameWithoutExtension(TemplateFile));

        return result;
    }

    private static string MakeIdentifier(string possibleIdentifier)
    {
        return Identifier.Replace(possibleIdentifier, "_");
    }

    private IEnumerable<Part> Parse(string templateFile, string templatePath,
                                    IDictionary<IncludeDirective, IEnumerable<Part>> includedTemplates)
    {
        TemplatePath = string.IsNullOrEmpty(templatePath) ? Path.GetDirectoryName(templateFile) : templatePath;
        TemplateFile = MakePathAbsolute(templateFile);

        var stream = File.OpenText(TemplateFile);

        // Read the content and convert all types of line endings to LF
        var content = LineEndings.Replace(stream.ReadToEnd(), "\n");

        // It is not possible to print a backslash '\' before the opening tag '<=' because the regex interprets
        // a backslash as an escape character. Escaping the escape character would result in a very complex regex.
        // Therefore, replace the escaped backslash by an expression block containing a backslash as string.
        // '\\<# ... #>! results in '<#= "\\" #><# ... #>'
        content = content.Replace(@"\\<#", @"<#= @""\"" #><#");

        stream.Close();

        Lines = GetLines(content);

        var parts = new List<Part>();
        ReadExpressions(parts, content);

        ReadTextBlocks(parts, content);

        ReadIncludes(parts, includedTemplates);

        // Sort all parts 
        parts = (from p in parts
                 orderby p.Index
                 select p).ToList();

        return parts;
    }

    private static int[] GetLines(string content)
    {
        return (from Match match in new Regex(@".*(\n|.$)").Matches(content)
                select match.Index).ToArray();
    }

    private void ReadTextBlocks(ICollection<Part> parts, string content)
    {
        var sorted = (from p in parts
                      orderby p.Index
                      select p).ToList();

        var readIndex = 0;

        foreach (var part in sorted)
        {
            if (readIndex < part.Index)
            {
                parts.Add(CreateTextBlock(readIndex, content.Substring(readIndex, part.Index - readIndex)));
            }

            readIndex = part.Index + part.Length;
        }

        if (readIndex < content.Length)
        {
            parts.Add(CreateTextBlock(readIndex, content.Substring(readIndex)));
        }
    }

    private TextBlock CreateTextBlock(int position, string content)
    {
        return new TextBlock(position, content) { Line = GetLineFromPosition(position), Source = TemplateFile };
    }

    private void ReadExpressions(List<Part> parts, string content)
    {
        var syntax = new T4Syntax();

        foreach (var expression in syntax.Expressions)
        {
            Parse(parts, expression.Key, expression.Value, content);
        }
    }

    private void Parse(ICollection<Part> parts, Expression type, Regex syntax, string content)
    {
        foreach (Match match in syntax.Matches(content))
        {
            Part part = type switch
            {
                Expression.Directive => DirectiveFactory.CreateDirective(match, new MacroResolver(Settings)),
                Expression.Comment => new CommentDirective(match),
                Expression.StandardControlBlock => new StandardControlBlock(match),
                Expression.ExpressionControlBlock => new ExpressionControlBlock(match),
                Expression.FeatureControlBlock => new FeatureControlBlock(match),
                _ => null
            };

            if (part != null)
            {
                part.Line = GetLineFromPosition(part.Position);
                part.Source = TemplateFile;
                parts.Add(part);
            }
        }
    }

    private void ReadIncludes(IEnumerable<Part> parts, IDictionary<IncludeDirective, IEnumerable<Part>> includedTemplates)
    {
        var includes = (from p in parts
                        where p is IncludeDirective
                        select (IncludeDirective)p).ToList();

        foreach (var include in includes)
        {
            var parser = new Parser(Settings);
            //var fullPath = MakePathAbsolute( include.File );
            //var templateName = Path.GetFileNameWithoutExtension( fullPath );
            //if( templateName == null )
            //    throw new T4Exception( "Invalid template file name." );

            //if((from i in includedTemplates.Keys where i.Name == templateName select i).Any())
            if (includedTemplates.ContainsKey(include))
            {
                continue;
            }

            // Add the key but do not assign a value. We need the key as a break condition in subsequent calls to ReadIncludes().
            includedTemplates.Add(include, null);
            var includedParts = parser.Parse(include.File, TemplatePath, includedTemplates).ToArray();
            includedTemplates[include] = includedParts;

            ReadIncludes(includedParts, includedTemplates);
        }
    }

    private int GetLineFromPosition(int position)
    {
        return Array.IndexOf(Lines, (from l in Lines
                                     where l <= position
                                     select l).Last()) + 1;
    }

    private string MakePathAbsolute(string path)
    {
        if (Path.IsPathRooted(path))
        {
            return path;
        }

        var newPath = new Uri(Path.Combine(TemplatePath, path));

        return newPath.LocalPath;
    }
}
