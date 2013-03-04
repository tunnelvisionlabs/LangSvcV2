namespace Tvl.VisualStudio.Language.Php
{
    internal class TemplateRegion
    {
        public readonly string Text;
        public readonly TemplateTokenKind Kind;
        public readonly PhpBlock Block;
        public readonly int Start;

        public TemplateRegion(string text, TemplateTokenKind kind, PhpBlock block, int start)
        {
            Text = text;
            Kind = kind;
            Start = start;
            Block = block;
        }
    }
}
