namespace Tvl.VisualStudio.Language.Parsing4
{
    using System;
    using Antlr4.Runtime;

    public class SnapshotTokenFactory : ITokenFactory
    {
        private readonly Tuple<ITokenSource, ICharStream> effectiveSource;

        public SnapshotTokenFactory(ITokenSource effectiveSource)
        {
            this.effectiveSource = Tuple.Create(effectiveSource, effectiveSource.InputStream);
        }

        public SnapshotTokenFactory(Tuple<ITokenSource, ICharStream> effectiveSource)
        {
            this.effectiveSource = effectiveSource;
        }

        public IToken Create(Tuple<ITokenSource, ICharStream> source, int type, string text, int channel, int start, int stop, int line, int charPositionInLine)
        {
            if (effectiveSource != null)
            {
                source = effectiveSource;
            }

            SnapshotToken t = new SnapshotToken(source, type, channel, start, stop);
            t.Line = line;
            t.Column = charPositionInLine;
            if (text != null)
            {
                t.Text = text;
            }
            return t;
        }

        public IToken Create(int type, string text)
        {
            return new SnapshotToken(type, text);
        }
    }
}
