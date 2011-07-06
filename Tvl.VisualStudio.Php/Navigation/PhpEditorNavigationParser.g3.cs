namespace Tvl.VisualStudio.Language.Php.Navigation
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;

    partial class PhpEditorNavigationParser
    {
        private readonly List<CommonTree> _navigationTrees = new List<CommonTree>();

        public event EventHandler<ParseErrorEventArgs> ParseError;

        internal ReadOnlyCollection<CommonTree> NavigationTrees
        {
            get
            {
                return _navigationTrees.AsReadOnly();
            }
        }

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
