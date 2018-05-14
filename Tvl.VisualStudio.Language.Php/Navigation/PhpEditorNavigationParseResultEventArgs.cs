namespace Tvl.VisualStudio.Language.Php.Navigation
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using Antlr4.Runtime;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing4;

    using ParseErrorEventArgs = Tvl.VisualStudio.Language.Parsing.ParseErrorEventArgs;

    internal class PhpEditorNavigationParseResultEventArgs : AntlrParseResultEventArgs
    {
        private readonly ReadOnlyCollection<ParserRuleContext> _navigationTrees;

        public PhpEditorNavigationParseResultEventArgs(ITextSnapshot snapshot, IList<ParseErrorEventArgs> errors, TimeSpan elapsedTime, IList<IToken> tokens, ParserRuleContext result, [NotNull] ReadOnlyCollection<ParserRuleContext> navigationTrees)
            : base(snapshot, errors, elapsedTime, tokens, result)
        {
            Requires.NotNull(navigationTrees, nameof(navigationTrees));

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
