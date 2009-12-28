namespace JavaLanguageService.AntlrLanguage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Tvl.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Text.Navigation;
    using System.Diagnostics.Contracts;
    using System.Windows.Media;

    internal sealed class AntlrEditorNavigationSource : IEditorNavigationSource
    {
        private IEditorNavigationType _parserRuleNavigationType;
        private IEditorNavigationType _lexerRuleNavigationType;
        private ImageSource _lexerRuleGlyph;
        private ImageSource _parserRuleGlyph;
        private List<IEditorNavigationTarget> _navigationTargets;

        public event EventHandler NavigationTargetsChanged;

        public AntlrEditorNavigationSource(ITextBuffer textBuffer, IBackgroundParser backgroundParser, IEditorNavigationTypeRegistryService editorNavigationTypeRegistryService)
        {
            if (textBuffer == null)
                throw new ArgumentNullException("textBuffer");
            if (backgroundParser == null)
                throw new ArgumentNullException("backgroundParser");
            if (editorNavigationTypeRegistryService == null)
                throw new ArgumentNullException("editorNavigationTypeRegistryService");
            Contract.EndContractBlock();

            this.TextBuffer = textBuffer;
            this.BackgroundParser = backgroundParser;
            this.EditorNavigationTypeRegistryService = editorNavigationTypeRegistryService;

            this._parserRuleNavigationType = this.EditorNavigationTypeRegistryService.GetEditorNavigationType(AntlrEditorNavigationTypeNames.ParserRule);
            this._lexerRuleNavigationType = this.EditorNavigationTypeRegistryService.GetEditorNavigationType(AntlrEditorNavigationTypeNames.LexerRule);
            this._navigationTargets = new List<IEditorNavigationTarget>();

            this.BackgroundParser.ParseComplete += OnBackgroundParseComplete;
        }

        private ITextBuffer TextBuffer
        {
            get;
            set;
        }

        private IBackgroundParser BackgroundParser
        {
            get;
            set;
        }

        private IEditorNavigationTypeRegistryService EditorNavigationTypeRegistryService
        {
            get;
            set;
        }

        public IEnumerable<IEditorNavigationType> GetNavigationTypes()
        {
            yield return _parserRuleNavigationType;
            yield return _lexerRuleNavigationType;
        }

        public IEnumerable<IEditorNavigationTarget> GetNavigationTargets()
        {
            return _navigationTargets;
        }

        private void OnNavigationTargetsChanged(EventArgs e)
        {
            var t = NavigationTargetsChanged;
            if (t != null)
                t(this, e);
        }

        private void OnBackgroundParseComplete(object sender, ParseResultEventArgs e)
        {
            OnNavigationTargetsChanged(EventArgs.Empty);
        }
    }
}
