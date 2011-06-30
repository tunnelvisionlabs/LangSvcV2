namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using System.Diagnostics.Contracts;
    using Antlr.Runtime;
    using Tvl.VisualStudio.Language.Parsing;

    internal class AntlrClassifierLexer : ITokenSourceWithState<AntlrClassifierLexerState>
    {
        private readonly ICharStream _input;
        private readonly AntlrGrammarClassifierLexer _grammarLexer;
        private readonly AntlrActionClassifierLexer _actionLexer;

        private AntlrClassifierLexerMode _mode;
        private int _actionLevel;
        private bool _inComment;

        public AntlrClassifierLexer(ICharStream input)
            : this(input, AntlrClassifierLexerState.Initial)
        {
            Contract.Requires(input != null);
        }

        public AntlrClassifierLexer(ICharStream input, AntlrClassifierLexerState state)
        {
            Contract.Requires<ArgumentNullException>(input != null, "input");

            _input = input;
            _grammarLexer = new AntlrGrammarClassifierLexer(input, this);
            _actionLexer = new AntlrActionClassifierLexer(input, this);

            State = state;
        }

        public string SourceName
        {
            get
            {
                return "StringTemplate Classifier";
            }
        }

        public string[] TokenNames
        {
            get
            {
                return _actionLexer.TokenNames;
            }
        }

        internal AntlrClassifierLexerState State
        {
            get
            {
                return new AntlrClassifierLexerState(_mode, _actionLevel, _inComment);
            }

            set
            {
                _mode = value.Mode;
                _actionLevel = value.ActionLevel;
                _inComment = value.InComment;
            }
        }

        internal int ActionLevel
        {
            get
            {
                return _actionLevel;
            }

            set
            {
                _actionLevel = value;
            }
        }

        internal AntlrClassifierLexerMode Mode
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

        public AntlrClassifierLexerState GetCurrentState()
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
            } while (token == null || token.Type == AntlrGrammarClassifierLexer.NEWLINE);

            return token;
        }

        private IToken NextTokenCore()
        {
            IToken token = null;

            switch (Mode)
            {
            case AntlrClassifierLexerMode.Action:
                switch (_input.LA(1))
                {
                case '{':
                    token = _grammarLexer.NextToken();
                    ActionLevel++;
                    break;

                case '}':
                    token = _grammarLexer.NextToken();
                    ActionLevel--;
                    if (ActionLevel == 0)
                        Mode = AntlrClassifierLexerMode.Grammar;

                    break;

                default:
                    token = _actionLexer.NextToken();
                    break;
                }

                break;

            case AntlrClassifierLexerMode.ArgAction:
                if (_input.LA(1) == ']')
                {
                    token = _grammarLexer.NextToken();
                    Mode = AntlrClassifierLexerMode.Grammar;
                }
                else
                {
                    token = _actionLexer.NextToken();
                }

                break;

            case AntlrClassifierLexerMode.Grammar:
            default:
                token = _grammarLexer.NextToken();

                switch (token.Type)
                {
                case AntlrGrammarClassifierLexer.LCURLY:
                    ActionLevel++;
                    Mode = AntlrClassifierLexerMode.Action;
                    break;

                case AntlrGrammarClassifierLexer.LBRACK:
                    Mode = AntlrClassifierLexerMode.ArgAction;
                    break;

                case AntlrGrammarClassifierLexer.IDENTIFIER:
                    AntlrClassifierLexerState currentState = GetCurrentState();
                    int marker = _input.Mark();
                    try
                    {
                        while (true)
                        {
                            IToken nextToken = NextToken();
                            switch (nextToken.Type)
                            {
                            case AntlrGrammarClassifierLexer.NEWLINE:
                            case AntlrGrammarClassifierLexer.WS:
                            case AntlrGrammarClassifierLexer.COMMENT:
                            case AntlrGrammarClassifierLexer.DOC_COMMENT:
                            case AntlrGrammarClassifierLexer.ML_COMMENT:
                            case AntlrGrammarClassifierLexer.SL_COMMENT:
                                continue;

                            default:
                                break;
                            }

                            if (nextToken.Type == AntlrGrammarClassifierLexer.ASSIGN)
                                token.Type = AntlrGrammarClassifierLexer.LABEL;

                            break;
                        }
                    }
                    finally
                    {
                        _input.Rewind(marker);
                        State = currentState;
                    }

                    break;

                default:
                    break;
                }

                break;
            }

            return token;
        }
    }
}
