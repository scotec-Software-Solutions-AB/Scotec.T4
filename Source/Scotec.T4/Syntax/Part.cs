namespace Scotec.T4Generator.Syntax;

internal abstract class Part
{
    public abstract int Index { get; }

    public abstract int Position { get; }

    public abstract int Length { get; }

    public int Line { get; set; }

    public string Source { get; set; }
}
