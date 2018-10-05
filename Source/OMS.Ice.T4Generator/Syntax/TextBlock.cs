namespace OMS.Ice.T4Generator.Syntax
{
    internal class TextBlock : Part
    {
        private readonly int _position;

        public TextBlock( int position, string content )
        {
            _position = position;
            Content = content;
        }

        public override int Index => _position;

        public override int Position => _position;

        public string Content { get; }

        public override int Length => Content.Length;
    }
}