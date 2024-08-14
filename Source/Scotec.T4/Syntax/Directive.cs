#region

using System.Collections.Generic;
using System.Text.RegularExpressions;

#endregion

namespace Scotec.T4Generator.Syntax;

internal abstract class Directive : Part
{
    protected Directive(Match match, MacroResolver macroResolver)
    {
        Match = match;
        MacroResolver = macroResolver;

        Attributes = new Dictionary<string, string>();

        var keys = Match.Groups["key"].Captures;
        var values = Match.Groups["value"].Captures;

        if (keys.Count != values.Count)
        {
            throw new T4Exception("Error in T4 directives: Different number of keys and values.");
        }

        for (var i = 0; i < keys.Count; i++)
        {
            Attributes.Add(keys[i].Value.ToLower(), MacroResolver.Resolve(values[i].Value));
        }
    }

    protected Match Match { get; }

    protected MacroResolver MacroResolver { get; }

    public override int Index => Match.Index;

    public override int Length => Match.Length;

    public override int Position => Match.Index;

    public IDictionary<string, string> Attributes { get; }
}
