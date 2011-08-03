namespace Tvl.VisualStudio.Language.Go
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Threading;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Text.Navigation;
    using IDispatcherGlyphService = Tvl.VisualStudio.Language.Intellisense.IDispatcherGlyphService;

    internal sealed class GoEditorNavigationSource : IEditorNavigationSource
    {
        private List<IEditorNavigationTarget> _navigationTargets;
        private readonly GoEditorNavigationSourceProvider _provider;

        public event EventHandler NavigationTargetsChanged;

        public GoEditorNavigationSource(ITextBuffer textBuffer, IBackgroundParser backgroundParser, GoEditorNavigationSourceProvider provider)
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

        private IDispatcherGlyphService GlyphService
        {
            get
            {
                return _provider.GlyphService;
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
            List<IEditorNavigationTarget> navigationTargets = new List<IEditorNavigationTarget>();
            if (antlrParseResultArgs != null)
            {
                //// add the Global Scope type
                //{
                //    var name = "Global Scope";
                //    var navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Types);
                //    var span = new SnapshotSpan(e.Snapshot, new Span(0, e.Snapshot.Length));
                //    var seek = new SnapshotSpan(e.Snapshot, new Span(0, 0));
                //    var glyph = GetGlyph(StandardGlyphGroup.GlyphGroupNamespace, StandardGlyphItem.GlyphItemPublic);
                //    var target = new EditorNavigationTarget(name, navigationType, span, seek, glyph);
                //    navigationTargets.Add(target);
                //}

                IAstRuleReturnScope resultArgs = antlrParseResultArgs.Result as IAstRuleReturnScope;
                var result = resultArgs != null ? resultArgs.Tree as CommonTree : null;
                string packageName = string.Empty;

                if (result != null && result.Children != null)
                {
                    foreach (CommonTree child in result.Children)
                    {
                        if (child == null || string.IsNullOrEmpty(child.Text))
                            continue;

                        switch (child.Type)
                        {
                        case GoLexer.KW_PACKAGE:
                            {
                                packageName = ((CommonTree)child.Children[0]).Token.Text;
                                if (string.IsNullOrWhiteSpace(packageName))
                                    continue;

                                var navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Types);
                                var startToken = antlrParseResultArgs.Tokens[child.TokenStartIndex];
                                var stopToken = antlrParseResultArgs.Tokens[child.TokenStopIndex];
                                //Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                                //SnapshotSpan ruleSpan = new SnapshotSpan(e.Snapshot, span);
                                // applies to the whole file
                                var span = new SnapshotSpan(e.Snapshot, new Span(0, e.Snapshot.Length));
                                SnapshotSpan ruleSeek = new SnapshotSpan(e.Snapshot, new Span(((CommonTree)child.Children[0]).Token.StartIndex, 0));
                                var glyph = _provider.GlyphService.GetGlyph(StandardGlyphGroup.GlyphGroupModule, StandardGlyphItem.GlyphItemPublic);
                                navigationTargets.Add(new EditorNavigationTarget(packageName, navigationType, span, ruleSeek, glyph));
                            }
                            break;

                        case GoLexer.KW_TYPE:
                            // each child tree is a typeSpec, the root of which is an identifier that names the type
                            foreach (CommonTree typeSpec in child.Children)
                            {
                                var typeName = typeSpec.Token.Text;
                                if (string.IsNullOrWhiteSpace(typeName))
                                    continue;

                                for (ITree parent = typeSpec.Parent; parent != null; parent = parent.Parent)
                                {
                                    if (parent.Type == GoParser.TYPE_IDENTIFIER)
                                        typeName = parent.Text + "." + typeName;
                                }

                                if (!string.IsNullOrWhiteSpace(packageName))
                                    typeName = packageName + "." + typeName;

                                var navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Types);
                                var startToken = antlrParseResultArgs.Tokens[typeSpec.TokenStartIndex];
                                var stopToken = antlrParseResultArgs.Tokens[typeSpec.TokenStopIndex];
                                Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                                SnapshotSpan ruleSpan = new SnapshotSpan(e.Snapshot, span);
                                SnapshotSpan ruleSeek = new SnapshotSpan(e.Snapshot, new Span(typeSpec.Token.StartIndex, 0));
                                var glyph = _provider.GlyphService.GetGlyph(GetGlyphGroupForType(typeSpec), char.IsUpper(typeName[0]) ? StandardGlyphItem.GlyphItemPublic : StandardGlyphItem.GlyphItemPrivate);
                                navigationTargets.Add(new EditorNavigationTarget(typeName, navigationType, ruleSpan, ruleSeek, glyph));

                                if (typeSpec.ChildCount > 0 && typeSpec.Children[0].Type == GoLexer.KW_STRUCT && typeSpec.Children[0].ChildCount > 0)
                                {
                                    foreach (CommonTree fieldSpec in ((CommonTree)typeSpec.Children[0]).Children)
                                    {
                                        if (fieldSpec.Type != GoParser.FIELD_DECLARATION)
                                            continue;

                                        foreach (CommonTree fieldNameIdentifier in ((CommonTree)fieldSpec.GetFirstChildWithType(GoParser.AST_VARS)).Children)
                                        {
                                            string fieldName = fieldNameIdentifier.Text;
                                            navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Members);
                                            startToken = antlrParseResultArgs.Tokens[fieldNameIdentifier.TokenStartIndex];
                                            stopToken = antlrParseResultArgs.Tokens[fieldSpec.TokenStopIndex];
                                            span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                                            ruleSpan = new SnapshotSpan(e.Snapshot, span);
                                            ruleSeek = new SnapshotSpan(e.Snapshot, new Span(fieldNameIdentifier.Token.StartIndex, 0));
                                            glyph = _provider.GlyphService.GetGlyph(StandardGlyphGroup.GlyphGroupField, char.IsUpper(fieldName[0]) ? StandardGlyphItem.GlyphItemPublic : StandardGlyphItem.GlyphItemPrivate);
                                            navigationTargets.Add(new EditorNavigationTarget(fieldName, navigationType, ruleSpan, ruleSeek, glyph));
                                        }
                                    }
                                }
                            }

                            break;

                        case GoLexer.KW_CONST:
                        case GoLexer.KW_VAR:
                            foreach (CommonTree spec in child.Children)
                            {
                                CommonTree decl = (CommonTree)spec.Children[0];
                                foreach (CommonTree nameToken in decl.Children)
                                {
                                    var name = nameToken.Token.Text;
                                    if (string.IsNullOrWhiteSpace(name))
                                        continue;

                                    var navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Members);
                                    var startToken = antlrParseResultArgs.Tokens[nameToken.TokenStartIndex];
                                    var stopToken = antlrParseResultArgs.Tokens[nameToken.TokenStopIndex];
                                    Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                                    SnapshotSpan ruleSpan = new SnapshotSpan(e.Snapshot, span);
                                    SnapshotSpan ruleSeek = new SnapshotSpan(e.Snapshot, new Span(nameToken.Token.StartIndex, 0));
                                    var group = (child.Type == GoLexer.KW_CONST) ? StandardGlyphGroup.GlyphGroupConstant : StandardGlyphGroup.GlyphGroupVariable;
                                    var item = char.IsUpper(name[0]) ? StandardGlyphItem.GlyphItemPublic : StandardGlyphItem.GlyphItemPrivate;
                                    var glyph = _provider.GlyphService.GetGlyph(group, item);
                                    navigationTargets.Add(new EditorNavigationTarget(name, navigationType, ruleSpan, ruleSeek, glyph));
                                }
                            }
                            break;

                        case GoLexer.KW_FUNC:
                            {
                                // the first child is either a receiver (method) or an identifier with the name of the function
                                var token = ((CommonTree)child.Children[0]).Token;
                                if (token.Type == GoLexer.METHOD_RECEIVER)
                                    token = ((CommonTree)child.Children[1]).Token;

                                var functionName = token.Text;
                                if (string.IsNullOrWhiteSpace(functionName))
                                    continue;

                                ITree receiver = child.GetFirstChildWithType(GoParser.METHOD_RECEIVER);
                                if (receiver != null)
                                {
                                    string receiverName;
                                    if (receiver.ChildCount >= 2)
                                        receiverName = receiver.GetChild(receiver.ChildCount - 2).Text;
                                    else
                                        receiverName = "?";

                                    functionName = receiverName + "." + functionName;
                                }

                                IEnumerable<string> args = ProcessFunctionParameters(child);
                                string sig = string.Format("{0}({1})", functionName, string.Join(", ", args));
                                var navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Members);
                                var startToken = antlrParseResultArgs.Tokens[child.TokenStartIndex];
                                var stopToken = antlrParseResultArgs.Tokens[child.TokenStopIndex];
                                Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
                                SnapshotSpan ruleSpan = new SnapshotSpan(e.Snapshot, span);
                                SnapshotSpan ruleSeek = new SnapshotSpan(e.Snapshot, new Span(child.Token.StartIndex, 0));
                                var glyph = _provider.GlyphService.GetGlyph(StandardGlyphGroup.GlyphGroupMethod, char.IsUpper(functionName[0]) ? StandardGlyphItem.GlyphItemPublic : StandardGlyphItem.GlyphItemPrivate);
                                navigationTargets.Add(new EditorNavigationTarget(sig, navigationType, ruleSpan, ruleSeek, glyph));
                            }

                            break;

                        default:
                            continue;
                        }
                    }
                }
            }

            this._navigationTargets = navigationTargets;
            OnNavigationTargetsChanged(EventArgs.Empty);
        }

        private IEnumerable<string> ProcessFunctionParameters(CommonTree child)
        {
            CommonTree parameterTree = child.GetFirstChildWithType(GoLexer.LPAREN) as CommonTree;
            if (parameterTree == null)
                yield break;

            foreach (CommonTree parameter in parameterTree.Children)
            {
                if (parameter.Type == GoLexer.RPAREN)
                    continue;

                if (parameter.ChildCount == 0)
                {
                    yield return parameter.Text;
                }
                else if (parameter.ChildCount == 1)
                {
                    yield return string.Format("{0} {1}", parameter.Text, GetTypeString((CommonTree)parameter.GetChild(0)));
                }
                else
                {
                    yield return "<unknown parameter>";
                }
            }
        }

        private string GetTypeString(ITree parameterType)
        {
            if (parameterType != null && parameterType.Type == GoLexer.ELLIP && parameterType.ChildCount == 1)
                return "..." + GetTypeString(parameterType.GetChild(0));

            return GoTypeFormatter.FormatType(parameterType);
        }

        private StandardGlyphGroup GetGlyphGroupForType(CommonTree typeSpec)
        {
            if (typeSpec.ChildCount > 0)
            {
                switch (typeSpec.Children[0].Type)
                {
                case GoLexer.KW_STRUCT:
                    return StandardGlyphGroup.GlyphGroupStruct;

                case GoLexer.KW_INTERFACE:
                    return StandardGlyphGroup.GlyphGroupInterface;

                case GoLexer.KW_FUNC:
                    return StandardGlyphGroup.GlyphGroupMethod;

                default:
                    return StandardGlyphGroup.GlyphGroupType;
                }
            }

            return StandardGlyphGroup.GlyphGroupType;
        }
    }
}
