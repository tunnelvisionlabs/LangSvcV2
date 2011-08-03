namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Alloy.Experimental;
    using Tvl.VisualStudio.Shell.OutputWindow;
    using Tvl.VisualStudio.Text.Navigation;
    using ImageSource = System.Windows.Media.ImageSource;

    internal class AlloyEditorNavigationSourceWalker : AlloyBaseWalker
    {
        private readonly List<IEditorNavigationTarget> _targets = new List<IEditorNavigationTarget>();
        private readonly ReadOnlyCollection<IToken> _tokens;
        private readonly IEditorNavigationTypeRegistryService _editorNavigationTypeRegistryService;
        private readonly IGlyphService _glyphService;

        private string _moduleName = string.Empty;

        private AlloyEditorNavigationSourceWalker(ITreeNodeStream input, ITextSnapshot snapshot, ReadOnlyCollection<IToken> tokens, IEditorNavigationTypeRegistryService editorNavigationTypeRegistryService, IGlyphService glyphService, IOutputWindowService outputWindowService)
            : base(input, snapshot, outputWindowService)
        {
            Contract.Requires<ArgumentNullException>(editorNavigationTypeRegistryService != null, "editorNavigationTypeRegistryService");
            Contract.Requires<ArgumentNullException>(glyphService != null, "glyphService");

            _tokens = tokens;
            _editorNavigationTypeRegistryService = editorNavigationTypeRegistryService;
            _glyphService = glyphService;
        }

        private IEditorNavigationTypeRegistryService EditorNavigationTypeRegistryService
        {
            get
            {
                return _editorNavigationTypeRegistryService;
            }
        }

        public static List<IEditorNavigationTarget> ExtractNavigationTargets(IAstRuleReturnScope parseResult, ReadOnlyCollection<IToken> tokens, AlloyEditorNavigationSourceProvider provider, ITextSnapshot snapshot)
        {
            BufferedTreeNodeStream input = new BufferedTreeNodeStream(parseResult.Tree);
            AlloyEditorNavigationSourceWalker walker = new AlloyEditorNavigationSourceWalker(input, snapshot, tokens, provider.EditorNavigationTypeRegistryService, provider.GlyphService, provider.OutputWindowService);
            walker.compilationUnit();

            return walker._targets;
        }

        public static List<IEditorNavigationTarget> ExtractNavigationTargets(IAstRuleReturnScope parseResult, ReadOnlyCollection<IToken> tokens, AlloyAtnEditorNavigationSourceProvider provider, ITextSnapshot snapshot)
        {
            BufferedTreeNodeStream input = new BufferedTreeNodeStream(parseResult.Tree);
            AlloyEditorNavigationSourceWalker walker = new AlloyEditorNavigationSourceWalker(input, snapshot, tokens, provider.EditorNavigationTypeRegistryService, provider.GlyphService, provider.OutputWindowService);
            walker.compilationUnit();

            return walker._targets;
        }

        protected override void HandleModule(CommonTree moduleName)
        {
            _moduleName = moduleName.Text;
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
            var glyph = _glyphService.GetGlyph(group, item);
            HandleFunctionOrPredicate(function, name, parameters, returnSpec, glyph);
        }

        protected override void HandlePredicate(CommonTree function, CommonTree name, bool isPrivate, IList<CommonTree> parameters, CommonTree body)
        {
            var group = StandardGlyphGroup.GlyphGroupIntrinsic;
            var item = isPrivate ? StandardGlyphItem.GlyphItemPrivate : StandardGlyphItem.GlyphItemPublic;
            var glyph = _glyphService.GetGlyph(group, item);
            HandleFunctionOrPredicate(function, name, parameters, null, glyph);
        }

        private void HandleSignatureOrEnum(CommonTree tree, CommonTree name)
        {
            if (tree == null || name == null)
                return;

            string fullName = name.Text;
            if (!string.IsNullOrEmpty(_moduleName))
                fullName = _moduleName + "." + name.Text;

            var navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Types);
            var startToken = _tokens[tree.TokenStartIndex];
            var stopToken = _tokens[tree.TokenStopIndex];
            Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
            SnapshotSpan ruleSpan = new SnapshotSpan(Snapshot, span);
            SnapshotSpan ruleSeek = new SnapshotSpan(Snapshot, new Span(name.Token.StartIndex, 0));
            var group = tree.Type == KW_SIG ? StandardGlyphGroup.GlyphGroupStruct : StandardGlyphGroup.GlyphGroupEnum;
            var item = StandardGlyphItem.GlyphItemPublic;
            var glyph = _glyphService.GetGlyph(group, item);
            _targets.Add(new EditorNavigationTarget(fullName, navigationType, ruleSpan, ruleSeek, glyph));
        }

        private void HandleFunctionOrPredicate(CommonTree tree, CommonTree name, IList<CommonTree> parameters, CommonTree returnSpec, ImageSource glyph)
        {
            if (tree == null || name == null)
                return;

            var navigationType = EditorNavigationTypeRegistryService.GetEditorNavigationType(PredefinedEditorNavigationTypes.Members);
            var startToken = _tokens[tree.TokenStartIndex];
            var stopToken = _tokens[tree.TokenStopIndex];
            Span span = new Span(startToken.StartIndex, stopToken.StopIndex - startToken.StartIndex + 1);
            SnapshotSpan ruleSpan = new SnapshotSpan(Snapshot, span);
            SnapshotSpan ruleSeek = new SnapshotSpan(Snapshot, new Span(name.Token.StartIndex, 0));

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
