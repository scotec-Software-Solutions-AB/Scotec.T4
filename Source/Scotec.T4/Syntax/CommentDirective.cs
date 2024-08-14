#region

using System.Text.RegularExpressions;

#endregion

namespace Scotec.T4.Syntax;

internal class CommentDirective : Part
{
    public CommentDirective(Match match)
    {
        Match = match;

        if (match.Groups["comment"].Captures.Count > 0)
        {
            Content = match.Groups["comment"].Captures[0].Value;
        }
    }

    protected Match Match { get; }

    public override int Index => Match.Index;

    public override int Position => Index;

    public override int Length => Match.Length;

    public string Content { get; }
}
