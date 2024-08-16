#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Scotec.T4.Syntax;
#if !NETFRAMEWORK
using System.Runtime.Loader;
#endif

#endregion

namespace Scotec.T4;

internal class Parser
{
    private static readonly Regex LineEndings = new("(\r\n|\n\r|\n|\r|\u0085|\u000C|\u2028|\u2029)", RegexOptions.Compiled);
    private static readonly Regex Identifier = new(@"[^\p{Ll}\p{Lu}\p{Lt}\p{Lo}\p{Nd}\p{Nl}\p{Mn}\p{Mc}\p{Cf}\p{Pc}\p{Lm}]", RegexOptions.Compiled);

    public Parser(T4Options settings)
    {
        Settings = settings;
    }

    private T4Template Template { get; set; }
    public int[] Lines { get; private set; }    
    private T4Options Settings { get; }

    public ParserResult Parse(T4Template template)
    {
        var searchPaths = new[] { AppContext.BaseDirectory }.Concat(Settings.SearchPaths).ToList();
        if (!string.IsNullOrWhiteSpace(template.File))
        {
            var fullPath = Helper.FindFile(template.File, searchPaths);
            searchPaths.Insert(0, Path.GetDirectoryName(fullPath));
        }

        var includedTemplates = new Dictionary<IncludeDirective, IEnumerable<Part>>();
        var parts = Parse(template, searchPaths, includedTemplates);

        return new ParserResult
        {
            Template = template,
            IncludedTemplates = includedTemplates,
            SearchPaths = searchPaths.ToList(),
            Parts = parts
        };
    }

    private void ReadTemplate(T4Template template, IList<string> searchPaths, out string content)
    {
        Func<string> readContent;
        if (string.IsNullOrWhiteSpace(template.File))
        {
            readContent = () => template.Template;
        }
        else
        {
            readContent = () =>
            {
                using var stream = File.OpenText(Helper.FindFile(template.File, searchPaths));
                return LineEndings.Replace(stream.ReadToEnd(), "\n");
            };
        }

        // Read the content and convert all types of line endings to LF
        // It is not possible to print a backslash '\' before the opening tag '<=' because the regex interprets
        // a backslash as an escape character. Escaping the escape character would result in a very complex regex.
        // Therefore, replace the escaped backslash by an expression block containing a backslash as string.
        // '\\<# ... #>! results in '<#= "\\" #><# ... #>'
        content = LineEndings.Replace(readContent(), "\n").Replace(@"\\<#", @"<#= @""\"" #><#");
    }

    private IEnumerable<Part> Parse(T4Template template, IList<string> searchPaths,
                                    IDictionary<IncludeDirective, IEnumerable<Part>> includedTemplates)
    {
        Template = template;
        ReadTemplate(template, searchPaths, out var content);

        Lines = GetLines(content);

        var parts = new List<Part>();
        ReadExpressions(parts, content);

        ReadTextBlocks(parts, content);

        ReadIncludes(parts, includedTemplates, searchPaths);

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
        return new TextBlock(position, content) { Line = GetLineFromPosition(position), Source = Template.File };
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
                Expression.Directive => DirectiveFactory.CreateDirective(match, new MacroResolver(Settings.TemplateParameters)),
                Expression.Comment => new CommentDirective(match),
                Expression.StandardControlBlock => new StandardControlBlock(match),
                Expression.ExpressionControlBlock => new ExpressionControlBlock(match),
                Expression.FeatureControlBlock => new FeatureControlBlock(match),
                _ => null
            };

            if (part != null)
            {
                part.Line = GetLineFromPosition(part.Position);
                part.Source = Template.File;
                parts.Add(part);
            }
        }
    }

    private void ReadIncludes(IEnumerable<Part> parts, IDictionary<IncludeDirective, IEnumerable<Part>> includedTemplates, IList<string> searchPaths)
    {
        var includes = (from p in parts
                        where p is IncludeDirective
                        select (IncludeDirective)p).ToList();

        foreach (var include in includes)
        {
            var parser = new Parser(Settings);

            if (includedTemplates.ContainsKey(include))
            {
                continue;
            }

            // Add the key but do not assign a value. We need the key as a break condition in subsequent calls to ReadIncludes().
            includedTemplates.Add(include, null);

            var includedFile = Helper.FindFile(include.File, searchPaths);
            var includedParts = parser.Parse(T4Template.FromFile(includedFile), searchPaths, includedTemplates).ToArray();
            includedTemplates[include] = includedParts;

            ReadIncludes(includedParts, includedTemplates, searchPaths);
        }
    }

    private int GetLineFromPosition(int position)
    {
        return Array.IndexOf(Lines, (from l in Lines
                                     where l <= position
                                     select l).Last()) + 1;
    }
}
