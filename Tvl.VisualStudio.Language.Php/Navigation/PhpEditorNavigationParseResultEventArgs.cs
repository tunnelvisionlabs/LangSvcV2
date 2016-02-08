namespace Tvl.VisualStudio.Language.Php.Navigation
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using Antlr4.Runtime;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing4;

    using ParseErrorEventArgs = Tvl.VisualStudio.Language.Parsing.ParseErrorEventArgs;

    internal class PhpEditorNavigationParseResultEventArgs : AntlrParseResultEventArgs
    {
        private readonly ReadOnlyCollection<ParserRuleContext> _navigationTrees;

        public PhpEditorNavigationParseResultEventArgs(ITextSnapshot snapshot, IList<ParseErrorEventArgs> errors, TimeSpan elapsedTime, IList<IToken> tokens, ParserRuleContext result, ReadOnlyCollection<ParserRuleContext> navigationTrees)
            : base(snapshot, errors, elapsedTime, tokens, result)
        {
            Contract.Requires<ArgumentNullException>(navigationTrees != null, "navigationTrees");

            this._navigationTrees = navigationTrees;
        }

        public ReadOnlyCollection<ParserRuleContext> NavigationTrees
        {
            get
            {
                return _navigationTrees;
            }
        }
    }
}
