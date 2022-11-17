#region

using System.Text.RegularExpressions;

#endregion


namespace Scotec.T4Generator.Syntax
{
    internal class ExpressionControlBlock : Part
    {
        public ExpressionControlBlock( Match match )
        {
            Match = match;

            if( match.Groups["content"].Captures.Count > 0 )
            {
                var capture = match.Groups["content"].Captures[0];
                Content = capture.Value;
                Position = capture.Index;
            }
        }

        protected Match Match { get; }

        public override int Index => Match.Index;

        public override int Position { get; }

        public override int Length => Match.Length;

        public string Content { get; }
    }
}