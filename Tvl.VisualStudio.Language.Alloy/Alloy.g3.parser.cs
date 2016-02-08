namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;

    partial class AlloyParser
    {
        public event EventHandler<ParseErrorEventArgs> ParseError;

        public override void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
        {
            string header = GetErrorHeader(e);
            string message = GetErrorMessage(e, tokenNames);
            Span span = new Span();
            if (e.Token != null)
                span = Span.FromBounds(e.Token.StartIndex, e.Token.StopIndex + 1);

            ParseErrorEventArgs args = new ParseErrorEventArgs(message, span);
            OnParseError(args);

            base.DisplayRecognitionError(tokenNames, e);
        }

        //public override void EmitErrorMessage(string msg)
        //{
        //    OnParseError(new ParseErrorEventArgs(msg));
        //    base.EmitErrorMessage(msg);
        //}

        protected virtual void OnParseError(ParseErrorEventArgs e)
        {
            var t = ParseError;
            if (t != null)
                t(this, e);
        }
    }
}
