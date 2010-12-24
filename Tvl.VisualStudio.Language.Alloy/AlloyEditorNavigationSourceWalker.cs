namespace Tvl.VisualStudio.Language.Alloy
{
    using ImageSource = System.Windows.Media.ImageSource;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Antlr.Runtime.Tree;
    using Tvl.VisualStudio.Text.Navigation;
    using Antlr.Runtime;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Language.Intellisense;
    using System.Collections.ObjectModel;

    internal class AlloyEditorNavigationSourceWalker : AlloyBaseWalker
    {
        private readonly List<IEditorNavigationTarget> _targets = new List<IEditorNavigationTarget>();
        private readonly ReadOnlyCollection<IToken> _tokens;
        private readonly AlloyEditorNavigationSourceProvider _provider;
        private readonly ITextSnapshot _snapshot;

        private AlloyEditorNavigationSourceWalker(ITreeNodeStream input, ReadOnlyCollection<IToken> tokens, AlloyEditorNavigationSourceProvider provider, ITextSnapshot snapshot)
            : base(input, snapshot, provider.OutputWindowService)
        {
            _tokens = tokens;
            _provider = provider;
            _snapshot = snapshot;
        }

        private IEditorNavigationTypeRegistryService EditorNavigationTypeRegistryService
        {
            get
            {
                return _provider.EditorNavigationTypeRegistryService;
            }
        }

        public static List<IEditorNavigationTarget> ExtractNavigationTargets(AlloyParser.compilationUnit_return parseResult, ReadOnlyCollection<IToken> tokens, AlloyEditorNavigationSourceProvider provider, ITextSnapshot snapshot)
        {
            BufferedTreeNodeStream input = new BufferedTreeNodeStream(parseResult.Tree);
            AlloyEditorNavigationSourceWalker walker = new AlloyEditorNavigationSourceWalker(input, tokens, provider, snapshot);
            walker.compilationUnit();

            return walker._targets;
        }

        protected override void HandleSignature(CommonTree signature, IList<IToken> qualifiers, IList<CommonTree> names, CommonTree extendsSpec, CommonTree body, CommonTree block)
        {
            if (names == null)
                return;

            foreach (var name in names)
                HandleSignatureOrEnum(signature, name);
        }

        protected override void HandleEnum(CommonTree @enum, CommonTree name, CommonTree body)
        {
            HandleSignatureOrEnum(@enum, name);
        }

        protected override void HandleAssert(CommonTree assert, CommonTree name, CommonTree body)
        {
        }

        protected override void HandleFact(CommonTree fact, CommonTree name, CommonTree body)
        {
        }

        protected override void HandleFunction(CommonTree function, CommonTree name, bool isPrivate, IList<CommonTree> parameters, CommonTree returnSpec, CommonTree body)
        {
            var group = StandardGlyphGroup.GlyphGroupMethod;
            var item = isPrivate ? StandardGlyphItem.GlyphItemPrivate : StandardGlyphItem.GlyphItemPublic;
            var glyph = _provider.GetGlyph(group, item);
            HandleFunctionOrPredicate(function, name, parameters, returnSpec, glyph);
        }

        protected override void HandlePredicate(CommonTree function, CommonTree name, bool isPrivate, IList<CommonTree> parameters, CommonTree body)
        {
            var group = StandardGlyphGroup.GlyphGroupIntrinsic;
            var item = isPrivate ? StandardGlyphItem.GlyphItemPrivate : StandardGlyphItem.GlyphItemPublic;
            var glyph = _provider.GetGlyph(group, item);
            HandleFunctionOrPredicate(function, name, parameters, null, glyph);
        }

        private void HandleSignatureOrEnum(CommonTree tree, CommonTree name)
        {
            if (tree == null || name == null)
                return;

            var navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Types);
            var startToken = _tokens[tree.TokenStartIndex];
            var stopToken = _tokens[tree.TokenStopIndex];
            Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
            SnapshotSpan ruleSpan = new SnapshotSpan(_snapshot, span);
            SnapshotSpan ruleSeek = new SnapshotSpan(_snapshot, new Span(name.Token.StartIndex, 0));
            var group = tree.Type == KW_SIG ? StandardGlyphGroup.GlyphGroupStruct : StandardGlyphGroup.GlyphGroupEnum;
            var item = StandardGlyphItem.GlyphItemPublic;
            var glyph = _provider.GetGlyph(group, item);
            _targets.Add(new EditorNavigationTarget(name.Text, navigationType, ruleSpan, ruleSeek, glyph));
        }

        private void HandleFunctionOrPredicate(CommonTree tree, CommonTree name, IList<CommonTree> parameters, CommonTree returnSpec, ImageSource glyph)
        {
            if (tree == null || name == null)
                return;

            var navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Members);
            var startToken = _tokens[tree.TokenStartIndex];
            var stopToken = _tokens[tree.TokenStopIndex];
            Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
            SnapshotSpan ruleSpan = new SnapshotSpan(_snapshot, span);
            SnapshotSpan ruleSeek = new SnapshotSpan(_snapshot, new Span(name.Token.StartIndex, 0));

            string functionDisplayName = (name.Type == DOT) ? GetNameText((CommonTree)name.Children[1]) : GetNameText(name);
            string thisType = (name.Type == DOT) ? GetNameText((CommonTree)name.Children[0]) : null;
            IEnumerable<string> thisParam = !string.IsNullOrEmpty(thisType) ? Enumerable.Repeat(string.Format("this : {0}", thisType), 1) : Enumerable.Empty<string>();
            IList<string> parameterText = thisParam.Concat(parameters != null ? parameters.SelectMany(GetParameterText) : new string[0]).ToArray();
            string returnText = returnSpec != null ? string.Format(" : {0}", GetNameText(returnSpec)) : string.Empty;
            string displayName = string.Format("{0} {1}[{2}]{3}", tree.Text, functionDisplayName, string.Join(", ", parameterText), returnText);
            _targets.Add(new EditorNavigationTarget(displayName, navigationType, ruleSpan, ruleSeek, glyph));
        }

        private IEnumerable<string> GetParameterText(CommonTree parameter)
        {
            string[] names = GetParameterNames(parameter);
            string type = GetParameterType(parameter);
            return Array.ConvertAll(names, i => string.Format("{0} : {1}", i, type));
        }

        private string[] GetParameterNames(CommonTree parameter)
        {
            if (parameter == null)
                return new string[0];

            int i = 0;
            for (i = 0; i < parameter.ChildCount; i++)
            {
                if (parameter.Children[i].Type == KW_PRIVATE || parameter.Children[i].Type == KW_DISJ)
                    continue;

                break;
            }

            if (i == parameter.ChildCount)
                return new string[0];

            if (parameter.Children[i].Type == COMMA)
            {
                CommonTree nameList = (CommonTree)parameter.Children[i];
                return nameList.Children.Select(j => GetNameText((CommonTree)j)).ToArray();
            }
            else
            {
                return new string[] { GetNameText((CommonTree)parameter.Children[i]) };
            }
        }

        private string GetParameterType(CommonTree parameter)
        {
            CommonTree type = (CommonTree)parameter.Children[parameter.Children.Count - 1];
            return GetString(type.TokenStartIndex, type.TokenStopIndex);
        }

        private string GetNameText(CommonTree name)
        {
            if (name == null)
                return string.Empty;

            return GetString(name.TokenStartIndex, name.TokenStopIndex);
        }

        private string GetString(int startIndex, int stopIndex)
        {
            return string.Join(string.Empty, _tokens.Skip(startIndex).Take(stopIndex - startIndex + 1).Select(i => i.Text).ToArray());
        }
    }
}
