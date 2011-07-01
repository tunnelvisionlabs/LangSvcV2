namespace JavaLanguageService
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Text.Navigation;

    using ImageSource = System.Windows.Media.ImageSource;
    using Antlr.Runtime.Tree;

    internal sealed class JavaEditorNavigationSource : IEditorNavigationSource
    {
        private List<IEditorNavigationTarget> _navigationTargets;
        private readonly JavaEditorNavigationSourceProvider _provider;

        public JavaEditorNavigationSource(ITextBuffer textBuffer, IBackgroundParser backgroundParser, JavaEditorNavigationSourceProvider provider)
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

        private void HandleBackgroundParseComplete(object sender, ParseResultEventArgs e)
        {
            AntlrParseResultEventArgs antlrParseResultArgs = e as AntlrParseResultEventArgs;
            if (antlrParseResultArgs == null)
                return;

            UpdateTags(antlrParseResultArgs);
        }

        private void UpdateTags(AntlrParseResultEventArgs antlrParseResultArgs)
        {
            List<IEditorNavigationTarget> navigationTargets = new List<IEditorNavigationTarget>();

            IAstRuleReturnScope resultArgs = antlrParseResultArgs.Result as IAstRuleReturnScope;
            var result = resultArgs != null ? resultArgs.Tree as CommonTree : null;
            if (result != null)
            {
                ITextSnapshot snapshot = antlrParseResultArgs.Snapshot;

                string package = string.Empty;

                for (CommonTreeNodeStream treeNodeStream = new CommonTreeNodeStream(result);
                    treeNodeStream.LA(1) != CharStreamConstants.EndOfFile;
                    treeNodeStream.Consume())
                {
                    switch (treeNodeStream.LA(1))
                    {
                    case Java2Lexer.PACKAGE:
                        {
                            CommonTree child = treeNodeStream.LT(1) as CommonTree;
                            if (child != null && child.ChildCount > 0)
                            {
                                package = GetQualifiedIdentifier(child.GetChild(0));
                            }
                        }

                        break;

                    case Java2Lexer.METHOD_IDENTIFIER:
                        {
                            CommonTree child = treeNodeStream.LT(1) as CommonTree;
                            if (child != null)
                            {
                                string name = child.Token.Text;
                                //IEnumerable<string> args = template.FormalArguments != null ? template.FormalArguments.Select(i => i.Name) : Enumerable.Empty<string>();
                                var args = Enumerable.Empty<string>();
                                string sig = string.Format("{0}({1})", name, string.Join(", ", args));
                                IEditorNavigationType navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Members);
                                var startToken = antlrParseResultArgs.Tokens[child.TokenStartIndex];
                                var stopToken = antlrParseResultArgs.Tokens[child.TokenStopIndex];
                                SnapshotSpan span = new SnapshotSpan(snapshot, new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1));
                                SnapshotSpan seek = new SnapshotSpan(snapshot, new Span(child.Token.StartIndex, 0));
                                StandardGlyphGroup glyphGroup = StandardGlyphGroup.GlyphGroupJSharpMethod;
                                StandardGlyphItem glyphItem = GetGlyphItemFromChildModifier(child);
                                ImageSource glyph = _provider.GetGlyph(glyphGroup, glyphItem);
                                NavigationTargetStyle style = NavigationTargetStyle.None;
                                navigationTargets.Add(new EditorNavigationTarget(sig, navigationType, span, seek, glyph, style));
                            }
                        }

                        break;

                    case Java2Lexer.ENUM_TYPE_IDENTIFIER:
                    case Java2Lexer.INTERFACE_TYPE_IDENTIFIER:
                    case Java2Lexer.CLASS_TYPE_IDENTIFIER:
                        {
                            CommonTree child = treeNodeStream.LT(1) as CommonTree;
                            if (child != null)
                            {
                                string name = child.Token.Text;
                                for (ITree parent = child.Parent; parent != null; parent = parent.Parent)
                                {
                                    switch (parent.Type)
                                    {
                                    case Java2Lexer.ENUM_TYPE_IDENTIFIER:
                                    case Java2Lexer.INTERFACE_TYPE_IDENTIFIER:
                                    case Java2Lexer.CLASS_TYPE_IDENTIFIER:
                                        name = parent.Text + "." + name;
                                        continue;

                                    default:
                                        continue;
                                    }
                                }

                                if (!string.IsNullOrEmpty(package))
                                {
                                    name = package + "." + name;
                                }

                                IEditorNavigationType navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Types);
                                var startToken = antlrParseResultArgs.Tokens[child.TokenStartIndex];
                                var stopToken = antlrParseResultArgs.Tokens[child.TokenStopIndex];
                                SnapshotSpan span = new SnapshotSpan(snapshot, new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1));
                                SnapshotSpan seek = new SnapshotSpan(snapshot, new Span(child.Token.StartIndex, 0));

                                StandardGlyphGroup glyphGroup;
                                switch (child.Type)
                                {
                                case Java2Lexer.ENUM_TYPE_IDENTIFIER:
                                    glyphGroup = StandardGlyphGroup.GlyphGroupEnum;
                                    break;

                                case Java2Lexer.INTERFACE_TYPE_IDENTIFIER:
                                    glyphGroup = StandardGlyphGroup.GlyphGroupJSharpInterface;
                                    break;

                                case Java2Lexer.CLASS_TYPE_IDENTIFIER:
                                default:
                                    glyphGroup = StandardGlyphGroup.GlyphGroupJSharpClass;
                                    break;
                                }

                                StandardGlyphItem glyphItem = GetGlyphItemFromChildModifier(child);
                                ImageSource glyph = _provider.GetGlyph(glyphGroup, glyphItem);
                                NavigationTargetStyle style = NavigationTargetStyle.None;
                                navigationTargets.Add(new EditorNavigationTarget(name, navigationType, span, seek, glyph, style));
                            }
                        }

                        break;

                    default:
                        continue;
                    }
                }
            }

#if false
            if (antlrParseResultArgs != null)
            {
                var result = antlrParseResultArgs.Result as IAstRuleReturnScope;
                if (result != null)
                {
                    foreach (var templateInfo in result.Group.GetTemplateInformation())
                    {
                        Antlr4.Java.Compiler.CompiledTemplate template = templateInfo.Template;

                        if (template.IsAnonSubtemplate)
                            continue;

                        bool isRegion = !string.IsNullOrEmpty(templateInfo.EnclosingTemplateName);
                        if (isRegion)
                        {
                            string sig = string.Format("{0}.{1}()", templateInfo.EnclosingTemplateName, templateInfo.NameToken.Text);
                            //string sig = string.Format("{0}({1})", name, string.Join(", ", args));
                            IEditorNavigationType navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(JavaEditorNavigationTypes.Templates);
                            Interval sourceInterval = templateInfo.GroupInterval;
                            SnapshotSpan span = new SnapshotSpan(e.Snapshot, new Span(sourceInterval.Start, sourceInterval.Length));
                            SnapshotSpan seek = new SnapshotSpan(e.Snapshot, new Span(sourceInterval.Start, 0));
                            ImageSource glyph = _provider.GetGlyph(StandardGlyphGroup.GlyphGroupNamespace, StandardGlyphItem.GlyphItemPublic);
                            NavigationTargetStyle style = NavigationTargetStyle.None;
                            navigationTargets.Add(new EditorNavigationTarget(sig, navigationType, span, seek, glyph, style));
                        }
                        else
                        {
                            // always pull the name from the templateInfo because the template itself could be an aliased template
                            string name = templateInfo.NameToken.Text;
                            IEnumerable<string> args = template.FormalArguments != null ? template.FormalArguments.Select(i => i.Name) : Enumerable.Empty<string>();
                            string sig = string.Format("{0}({1})", name, string.Join(", ", args));
                            IEditorNavigationType navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(JavaEditorNavigationTypes.Templates);
                            Interval sourceInterval = templateInfo.GroupInterval;
                            SnapshotSpan span = new SnapshotSpan(e.Snapshot, new Span(sourceInterval.Start, sourceInterval.Length));
                            SnapshotSpan seek = new SnapshotSpan(e.Snapshot, new Span(sourceInterval.Start, 0));
                            bool isAlias = false;
                            StandardGlyphGroup glyphGroup = isAlias ? StandardGlyphGroup.GlyphGroupTypedef : StandardGlyphGroup.GlyphGroupTemplate;
                            ImageSource glyph = _provider.GetGlyph(StandardGlyphGroup.GlyphGroupTemplate, StandardGlyphItem.GlyphItemPublic);
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
#endif

            this._navigationTargets = navigationTargets;
            OnNavigationTargetsChanged(EventArgs.Empty);
        }

        private static string GetQualifiedIdentifier(ITree tree)
        {
            if (tree.Type == Java2Lexer.IDENTIFIER || tree.ChildCount != 2)
                return tree.Text;

            return GetQualifiedIdentifier(tree.GetChild(0)) + "." + tree.GetChild(1).Text;
        }

        private static StandardGlyphItem GetGlyphItemFromChildModifier(CommonTree tree)
        {
            bool isPublic = tree.Children.Any(i => i.Type == Java2Lexer.PUBLIC);
            bool isProtected = !isPublic && tree.Children.Any(i => i.Type == Java2Lexer.PROTECTED);
            bool isPrivate = !isPublic && !isProtected && tree.Children.Any(i => i.Type == Java2Lexer.PRIVATE);
            bool isModule = !isPublic && !isProtected && !isPrivate;
            StandardGlyphItem glyphItem =
                isPublic ? StandardGlyphItem.GlyphItemPublic :
                isProtected ? StandardGlyphItem.GlyphItemProtected :
                isPrivate ? StandardGlyphItem.GlyphItemPrivate :
                StandardGlyphItem.GlyphItemInternal;

            return glyphItem;
        }
    }
}
