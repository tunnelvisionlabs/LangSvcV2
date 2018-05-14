namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Text.Navigation;

    using IAstRuleReturnScope = Antlr.Runtime.IAstRuleReturnScope;

    internal sealed class AlloyEditorNavigationSource : IEditorNavigationSource
    {
        private List<IEditorNavigationTarget> _navigationTargets;
        private readonly AlloyEditorNavigationSourceProvider _provider;

        public AlloyEditorNavigationSource([NotNull] ITextBuffer textBuffer, [NotNull] AlloyBackgroundParser backgroundParser, [NotNull] AlloyEditorNavigationSourceProvider provider)
        {
            Requires.NotNull(textBuffer, nameof(textBuffer));
            Requires.NotNull(backgroundParser, nameof(backgroundParser));
            Requires.NotNull(provider, nameof(provider));

            this.TextBuffer = textBuffer;
            this.BackgroundParser = backgroundParser;
            this._provider = provider;

            this.BackgroundParser.ParseComplete += HandleBackgroundParseComplete;
            this.BackgroundParser.RequestParse(false);
        }

        public event EventHandler NavigationTargetsChanged;

        private ITextBuffer TextBuffer
        {
            get;
            set;
        }

        private AlloyBackgroundParser BackgroundParser
        {
            get;
            set;
        }

        private IEditorNavigationTypeRegistryService EditorNavigationTypeRegistryService
        {
            get
            {
                return _provider.EditorNavigationTypeRegistryService;
            }
        }

        public IEnumerable<IEditorNavigationType> GetNavigationTypes()
        {
            yield return EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Types);
            yield return EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Members);
        }

        public IEnumerable<IEditorNavigationTarget> GetNavigationTargets()
        {
            if (_navigationTargets == null)
            {
                var previousParseResult = BackgroundParser.PreviousParseResult;
                if (previousParseResult != null)
                    UpdateNavigationTargets(previousParseResult);
            }

            return _navigationTargets ?? Enumerable.Empty<IEditorNavigationTarget>();
        }

        private void OnNavigationTargetsChanged(EventArgs e)
        {
            var t = NavigationTargetsChanged;
            if (t != null)
                t(this, e);
        }

        private void HandleBackgroundParseComplete(object sender, ParseResultEventArgs e)
        {
            AntlrParseResultEventArgs antlrParseResultArgs = e as AntlrParseResultEventArgs;
            if (antlrParseResultArgs == null)
                return;

            UpdateNavigationTargets(antlrParseResultArgs);
        }

        private void UpdateNavigationTargets([NotNull] AntlrParseResultEventArgs antlrParseResultArgs)
        {
            Debug.Assert(antlrParseResultArgs != null);

            List<IEditorNavigationTarget> navigationTargets = null;
            if (antlrParseResultArgs != null)
            {
                IAstRuleReturnScope parseResult = antlrParseResultArgs.Result as IAstRuleReturnScope;
                if (parseResult != null)
                    navigationTargets = AlloyEditorNavigationSourceWalker.ExtractNavigationTargets(parseResult, antlrParseResultArgs.Tokens, _provider, antlrParseResultArgs.Snapshot);
            }

            this._navigationTargets = navigationTargets ?? new List<IEditorNavigationTarget>();
            OnNavigationTargetsChanged(EventArgs.Empty);
        }
    }
}
