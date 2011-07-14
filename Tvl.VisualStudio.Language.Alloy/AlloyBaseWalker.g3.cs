namespace Tvl.VisualStudio.Language.Alloy
{
    using System.Collections.Generic;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Shell.OutputWindow;

    abstract partial class AlloyBaseWalker
    {
        private readonly ITextSnapshot _snapshot;
        private readonly IOutputWindowService _outputWindowService;

        public AlloyBaseWalker(ITreeNodeStream input, ITextSnapshot snapshot, IOutputWindowService outputWindowService)
            : base(input)
        {
            _snapshot = snapshot;
            _outputWindowService = outputWindowService;
        }

        protected IOutputWindowService OutputWindowService
        {
            get
            {
                return _outputWindowService;
            }
        }

        protected ITextBuffer TextBuffer
        {
            get
            {
                return _snapshot.TextBuffer;
            }
        }

        protected ITextSnapshot Snapshot
        {
            get
            {
                return _snapshot;
            }
        }

        public override void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
        {
            var outputWindow = OutputWindowService.TryGetPane(PredefinedOutputWindowPanes.TvlIntellisense);
            if (outputWindow != null)
            {
                string header = GetErrorHeader(e);
                string message = GetErrorMessage(e, tokenNames);
                Span span = new Span();
                if (e.Token != null)
                    span = Span.FromBounds(e.Token.StartIndex, e.Token.StopIndex + 1);

                if (message.Length > 100)
                    message = message.Substring(0, 100) + " ...";

                ITextDocument document;
                if (TextBuffer.Properties.TryGetProperty(typeof(ITextDocument), out document) && document != null)
                {
                    string fileName = document.FilePath;
                    var line = Snapshot.GetLineFromPosition(span.Start);
                    message = string.Format("{0}({1},{2}): {3}: {4}", fileName, line.LineNumber + 1, span.Start - line.Start.Position + 1, GetType().Name, message);
                }

                outputWindow.WriteLine(message);
            }

            base.DisplayRecognitionError(tokenNames, e);
        }

        protected virtual void HandleModule(CommonTree moduleName)
        {
        }

        protected virtual void EnterSignature(CommonTree signature, IList<IToken> qualifiers, IList<CommonTree> names, CommonTree extendsSpec)
        {
        }

        protected virtual void HandleSignature(CommonTree signature, IList<IToken> qualifiers, IList<CommonTree> names, CommonTree extendsSpec, CommonTree body, CommonTree block)
        {
        }

        protected virtual void EnterEnum(CommonTree @enum, CommonTree name)
        {
        }

        protected virtual void HandleEnum(CommonTree @enum, CommonTree name, CommonTree body)
        {
        }

        protected virtual void EnterFact(CommonTree fact, CommonTree name)
        {
        }

        protected virtual void HandleFact(CommonTree fact, CommonTree name, CommonTree body)
        {
        }

        protected virtual void EnterAssert(CommonTree assert, CommonTree name)
        {
        }

        protected virtual void HandleAssert(CommonTree assert, CommonTree name, CommonTree body)
        {
        }

        protected virtual void EnterFunction(CommonTree function, CommonTree name, bool isPrivate)
        {
        }

        protected virtual void HandleFunction(CommonTree function, CommonTree name, bool isPrivate, IList<CommonTree> parameters, CommonTree returnSpec, CommonTree body)
        {
        }

        protected virtual void EnterPredicate(CommonTree function, CommonTree name, bool isPrivate)
        {
        }

        protected virtual void HandlePredicate(CommonTree function, CommonTree name, bool isPrivate, IList<CommonTree> parameters, CommonTree body)
        {
        }

        private IToken GetToken(CommonTree tree)
        {
            if (tree == null)
                return null;

            return tree.Token;
        }
    }
}
