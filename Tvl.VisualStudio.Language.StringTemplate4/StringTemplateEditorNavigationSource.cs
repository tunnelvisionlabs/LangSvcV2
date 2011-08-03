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

            this.BackgroundParser.ParseComplete += HandleBackgroundParseComplete;
            this.BackgroundParser.RequestParse(false);
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
            yield return EditorNavigationTypeRegistryService.GetEditorNavigationType(StringTemplateEditorNavigationTypes.Templates);
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

        private void HandleBackgroundParseComplete(object sender, ParseResultEventArgs e)
        {
            List<IEditorNavigationTarget> navigationTargets = new List<IEditorNavigationTarget>();
            AntlrParseResultEventArgs antlrParseResultArgs = e as AntlrParseResultEventArgs;
            if (antlrParseResultArgs != null)
            {
                var result = antlrParseResultArgs.Result as StringTemplateBackgroundParser.TemplateGroupRuleReturnScope;
                if (result != null)
                {
                    foreach (var templateInfo in result.Group.GetTemplateInformation())
                    {
                        Antlr4.StringTemplate.Compiler.CompiledTemplate template = templateInfo.Template;

                        if (template.IsAnonSubtemplate)
                            continue;

                        bool isRegion = !string.IsNullOrEmpty(templateInfo.EnclosingTemplateName);
                        if (isRegion)
                        {
                            string sig = string.Format("{0}.{1}()", templateInfo.EnclosingTemplateName, templateInfo.NameToken.Text);
                            //string sig = string.Format("{0}({1})", name, string.Join(", ", args));
                            IEditorNavigationType navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(StringTemplateEditorNavigationTypes.Templates);
                            Interval sourceInterval = templateInfo.GroupInterval;
                            SnapshotSpan span = new SnapshotSpan(e.Snapshot, new Span(sourceInterval.Start, sourceInterval.Length));
                            SnapshotSpan seek = new SnapshotSpan(e.Snapshot, new Span(sourceInterval.Start, 0));
                            ImageSource glyph = _provider.GlyphService.GetGlyph(StandardGlyphGroup.GlyphGroupNamespace, StandardGlyphItem.GlyphItemPublic);
                            NavigationTargetStyle style = NavigationTargetStyle.None;
                            navigationTargets.Add(new EditorNavigationTarget(sig, navigationType, span, seek, glyph, style));
                        }
                        else
                        {
                            // always pull the name from the templateInfo because the template itself could be an aliased template
                            string name = templateInfo.NameToken.Text;
                            IEnumerable<string> args = template.FormalArguments != null ? template.FormalArguments.Select(i => i.Name) : Enumerable.Empty<string>();
                            string sig = string.Format("{0}({1})", name, string.Join(", ", args));
                            IEditorNavigationType navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(StringTemplateEditorNavigationTypes.Templates);
                            Interval sourceInterval = templateInfo.GroupInterval;
                            SnapshotSpan span = new SnapshotSpan(e.Snapshot, new Span(sourceInterval.Start, sourceInterval.Length));
                            SnapshotSpan seek = new SnapshotSpan(e.Snapshot, new Span(sourceInterval.Start, 0));
                            bool isAlias = false;
                            StandardGlyphGroup glyphGroup = isAlias ? StandardGlyphGroup.GlyphGroupTypedef : StandardGlyphGroup.GlyphGroupTemplate;
                            ImageSource glyph = _provider.GlyphService.GetGlyph(StandardGlyphGroup.GlyphGroupTemplate, StandardGlyphItem.GlyphItemPublic);
                            NavigationTargetStyle style = NavigationTargetStyle.None;
                            navigationTargets.Add(new EditorNavigationTarget(sig, navigationType, span, seek, glyph, style));
                        }
                    }

                    //foreach (var dictionaryInfo in result.Group.GetDictionaryInformation())
                    //{
                    //    string name = dictionaryInfo.Name;
                    //    IEditorNavigationType navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Members);
                    //    Interval sourceInterval = dictionaryInfo.GroupInterval;
                    //    SnapshotSpan span = new SnapshotSpan(e.Snapshot, new Span(sourceInterval.Start, sourceInterval.Length));
                    //    SnapshotSpan seek = new SnapshotSpan(e.Snapshot, new Span(sourceInterval.Start, 0));
                    //    ImageSource glyph = _provider.GetGlyph(StandardGlyphGroup.GlyphGroupModule, StandardGlyphItem.GlyphItemPublic);
                    //    NavigationTargetStyle style = NavigationTargetStyle.None;
                    //    navigationTargets.Add(new EditorNavigationTarget(sig, navigationType, span, seek, glyph, style));
                    //}
                }
            }

            this._navigationTargets = navigationTargets;
            OnNavigationTargetsChanged(EventArgs.Empty);
        }
    }
}
