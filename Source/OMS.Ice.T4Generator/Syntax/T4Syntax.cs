#region

using System.Collections.Generic;
using System.Text.RegularExpressions;

#endregion


namespace OMS.Ice.T4Generator.Syntax
{
    internal enum Expression
    {
        Comment,
        Directive,
        StandardControlBlock,
        FeatureControlBlock,
        ExpressionControlBlock
    }

    internal class T4Syntax
    {
        private const string EndOfLine = @"(\w*\n)?";
        private const string Escape = @"(?<!\\)";

        public T4Syntax()
        {
            Expressions = new Dictionary<Expression, Regex>
            {
                {
                    Expression.Comment, new Regex( @"<#--( |\t)*\n?(?<comment>(?s:.*?))( |\t)*\n?(?<!\\)#>" + EndOfLine,
                                                  RegexOptions.Compiled )
                },
                {
                    Expression.Directive, new Regex(
                                                    Escape + @"<#@\s+(?<type>\w+)(\s+(?<key>\w+)\s*=[""'](?<value>[^""']*)[""'])*\s*" + Escape + "#>"
                                                    + EndOfLine,
                                                    RegexOptions.Compiled )
                },
                {
                    Expression.StandardControlBlock,
                    //<#(?![@=$#%+]|--)\s*\n?(?<content>[^=]((.|\n)*?(<#=(.|\n)*?#>)?)*)#>" + EndOfLine,
                    new Regex( Escape + @"<#(?![@=+-])\s*\n?(?<content>[^=]((.|\n)*?)*)" + Escape + "#>" + EndOfLine,
                              RegexOptions.Compiled )
                },
                {
                    //Expression.FeatureBlock, new Regex( "<#\\+\\s*\\r?\\n?(?<content>((.|\\r|\\n)*?(\\w*@[^\\r\\n]*\\r?\\n)*)*)#>(\\w*\\r?\\n)?",
                    Expression.FeatureControlBlock, new Regex( Escape + @"<#\+(?<content>(\s*.[^#>])*)\s*" + Escape + "#>" + EndOfLine,
                                                              RegexOptions.Compiled )
                },
                {
                    Expression.ExpressionControlBlock, new Regex( Escape + @"<#=\s*(?<content>(.[^#>])*)\s*" + Escape + "#>",
                                                                 RegexOptions.Compiled )
                },
            };
        }

        public IDictionary<Expression, Regex> Expressions { get; }
    }
}