namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Antlr4.StringTemplate.Misc;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Text.Navigation;
    using ImageSource = System.Windows.Media.ImageSource;

    internal sealed class StringTemplateEditorNavigationSource : IEditorNavigationSource
    {
        private List<IEditorNavigationTarget> _navigationTargets;
        private readonly StringTemplateEditorNavigationSourceProvider _provider;

        public StringTemplateEditorNavigationSource(ITextBuffer textBuffer, IBackgroundParser backgroundParser, StringTemplateEditorNavigationSourceProvider provider)
        {
            Contract.Requires<ArgumentNullException>(textBuffer != null, "textBuffer");
            Contract.Requires<ArgumentNullException>(backgroundParser != null, "backgroundParser");
            Contract.Requires<ArgumentNullException>(provider != null, "provider");

            this.TextBuffer = textBuffer;
            this.BackgroundParser = backgroundParser;
            this._provider = provider;

            this._navigationTargets = new List<IEditorNavigationTarget>();

            this.BackgroundParser.ParseComplete += OnBackgroundParseComplete;
        }

        public event EventHandler NavigationTargetsChanged;

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
            List<IEditorNavigationTarget> navigationTargets = new List<IEditorNavigationTarget>();
            AntlrParseResultEventArgs antlrParseResultArgs = e as AntlrParseResultEventArgs;
            if (antlrParseResultArgs != null)
            {
                var result = antlrParseResultArgs.Result as StringTemplateBackgroundParser.TemplateGroupRuleReturnScope;
                if (result != null)
                {
                    foreach (var template in result.Group.Templates)
                    {
                        if (template.isAnonSubtemplate)
                            continue;

                        string name = template.name.Substring(template.name.LastIndexOf('/') + 1);
                        IEnumerable<string> args = template.formalArguments != null ? template.formalArguments.Select(i => i.Name) : Enumerable.Empty<string>();
                        string sig = string.Format("{0}({1})", name, string.Join(", ", args));
                        IEditorNavigationType navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Members);
                        Interval sourceInterval = result.Group.GetTemplateInformation(template).GroupInterval;
                        SnapshotSpan span = new SnapshotSpan(e.Snapshot, new Span(sourceInterval.Start, sourceInterval.Length));
                        SnapshotSpan seek = new SnapshotSpan(e.Snapshot, new Span(sourceInterval.Start, 0));
                        ImageSource glyph = _provider.GetGlyph(StandardGlyphGroup.GlyphGroupTemplate, StandardGlyphItem.GlyphItemPublic);
                        NavigationTargetStyle style = NavigationTargetStyle.None;
                        navigationTargets.Add(new EditorNavigationTarget(sig, navigationType, span, seek, glyph, style));
                    }
                }
            }

            this._navigationTargets = navigationTargets;
            OnNavigationTargetsChanged(EventArgs.Empty);
        }
    }
}
