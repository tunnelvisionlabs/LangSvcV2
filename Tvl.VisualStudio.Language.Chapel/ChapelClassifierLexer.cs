namespace Tvl.VisualStudio.Language.Chapel
{
    using System.Diagnostics;
    using Antlr.Runtime;
    using JetBrains.Annotations;
    using Tvl.VisualStudio.Language.Parsing;

    internal class ChapelClassifierLexer : ITokenSourceWithState<ChapelClassifierLexerState>
    {
        private readonly ICharStream _input;
        private readonly ChapelCodeClassifierLexer _languageLexer;
        //private readonly ChapelDocCommentClassifierLexer _commentLexer;

        private ChapelClassifierLexerMode _mode;
        private bool _inComment;

        public ChapelClassifierLexer([NotNull] ICharStream input)
            : this(input, ChapelClassifierLexerState.Initial)
        {
            Debug.Assert(input != null);
        }

        public ChapelClassifierLexer([NotNull] ICharStream input, ChapelClassifierLexerState state)
        {
            Requires.NotNull(input, nameof(input));

            _input = input;
            _languageLexer = new ChapelCodeClassifierLexer(input, this);
            //_commentLexer = new ChapelDocCommentClassifierLexer(input, this);

            State = state;
        }

        public ICharStream CharStream
        {
            get
            {
                return _input;
            }
        }

        public string SourceName
        {
            get
            {
                return "Chapel Classifier";
            }
        }

        public string[] TokenNames
        {
            get
            {
                return _languageLexer.TokenNames;
            }
        }

        internal ChapelClassifierLexerState State
        {
            get
            {
                return new ChapelClassifierLexerState(_mode, _inComment);
            }

            set
            {
                _mode = value.Mode;
                _inComment = value.InComment;
            }
        }

        internal ChapelClassifierLexerMode Mode
        {
            get
            {
                return _mode;
            }

            set
            {
                _mode = value;
            }
        }

        internal bool InComment
        {
            get
            {
                return _inComment;
            }

            set
            {
                _inComment = value;
            }
        }

        public ChapelClassifierLexerState GetCurrentState()
        {
            return State;
        }

        public IToken NextToken()
        {
            IToken token = null;
            do
            {
                int position = _input.Index;
                token = NextTokenCore();
                // ensure progress
                if (position == _input.Index)
                    _input.Consume();
            } while (token == null || token.Type == ChapelCodeClassifierLexer.NEWLINE);

            return token;
        }

        private IToken NextTokenCore()
        {
            IToken token = null;

            switch (Mode)
            {
            //case ChapelClassifierLexerMode.ChapelDocComment:
            //    token = _commentLexer.NextToken();

            //    switch (token.Type)
            //    {
            //    case ChapelDocCommentClassifierLexer.END_COMMENT:
            //        _mode = ChapelClassifierLexerMode.ChapelCode;
            //        token.Type = ChapelDocCommentClassifierLexer.DOC_COMMENT_TEXT;
            //        break;

            //    default:
            //        break;
            //    }

            //    break;

            case ChapelClassifierLexerMode.ChapelCode:
            default:
                token = _languageLexer.NextToken();

                //switch (token.Type)
                //{
                //case ChapelColorizerLexer.DOC_COMMENT_START:
                //    _mode = ChapelClassifierLexerMode.ChapelDocComment;
                //    token.Type = ChapelDocCommentClassifierLexer.DOC_COMMENT_TEXT;
                //    break;

                //default:
                //    break;
                //}

                break;
            }

            return token;
        }
    }
}
