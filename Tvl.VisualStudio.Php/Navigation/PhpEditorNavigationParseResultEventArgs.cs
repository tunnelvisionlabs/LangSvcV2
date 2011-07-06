namespace Tvl.VisualStudio.Language.Php.Navigation
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;

    internal class PhpEditorNavigationParseResultEventArgs : AntlrParseResultEventArgs
    {
        private readonly ReadOnlyCollection<CommonTree> _navigationTrees;

        public PhpEditorNavigationParseResultEventArgs(ITextSnapshot snapshot, IList<ParseErrorEventArgs> errors, TimeSpan elapsedTime, IList<IToken> tokens, IRuleReturnScope result, ReadOnlyCollection<CommonTree> navigationTrees)
            : base(snapshot, errors, elapsedTime, tokens, result)
        {
            Contract.Requires<ArgumentNullException>(navigationTrees != null, "navigationTrees");

            this._navigationTrees = navigationTrees;
        }

        public ReadOnlyCollection<CommonTree> NavigationTrees
        {
            get
            {
                return _navigationTrees;
            }
        }
    }
}
