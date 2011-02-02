namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Antlr4.StringTemplate.Compiler;
    using Antlr.Runtime;
    using Tvl.VisualStudio.Language.Parsing;
    using Microsoft.VisualStudio.Text;

    public class GroupParserWrapper : GroupParser
    {
        public GroupParserWrapper(ITokenStream input)
            : base(input)
        {
        }

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

        protected virtual void OnParseError(ParseErrorEventArgs e)
        {
            var t = ParseError;
            if (t != null)
                t(this, e);
        }
    }
}
