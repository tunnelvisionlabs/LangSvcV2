namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using Antlr.Runtime;
    using Tvl.VisualStudio.Language.Parsing;
    using System.Collections.Generic;
    using System.Diagnostics;
    using JetBrains.Annotations;

    internal class AntlrClassifierLexer : ITokenSourceWithState<AntlrClassifierLexerState>
    {
        private readonly ICharStream _input;
        private readonly AntlrGrammarClassifierLexer _grammarLexer;
        private readonly AntlrActionClassifierLexer _actionLexer;

        private AntlrClassifierLexerMode _mode;
        private int _actionLevel;
        private bool _inComment;
        private bool _inOptions;
        private bool _inTokens;

        public AntlrClassifierLexer([NotNull] ICharStream input)
            : this(input, AntlrClassifierLexerState.Initial)
        {
            Debug.Assert(input != null);
        }

        public AntlrClassifierLexer([NotNull] ICharStream input, AntlrClassifierLexerState state)
        {
            Requires.NotNull(input, nameof(input));

            _input = input;
            _grammarLexer = new AntlrGrammarClassifierLexer(input, this);
            _actionLexer = new AntlrActionClassifierLexer(input, this);

            SetCurrentState(state);
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
                return "ANTLR Classifier";
            }
        }

        public string[] TokenNames
        {
            get
            {
                return _actionLexer.TokenNames;
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

        internal bool InDoubleAngleStringLiteral
        {
            get
            {
                return Mode == AntlrClassifierLexerMode.GrammarDoubleAngleStringLiteral;
            }

            set
            {
                Mode = value ? AntlrClassifierLexerMode.GrammarDoubleAngleStringLiteral : AntlrClassifierLexerMode.Grammar;
            }
        }

        internal bool InCharLiteral
        {
            get
            {
                return Mode == AntlrClassifierLexerMode.ActionCharLiteral
                    || Mode == AntlrClassifierLexerMode.ArgActionCharLiteral;
            }

            set
            {
                if (InCharLiteral == value)
                    return;

                switch (Mode)
                {
                case AntlrClassifierLexerMode.Action:
                    Mode = AntlrClassifierLexerMode.ActionCharLiteral;
                    break;

                case AntlrClassifierLexerMode.ActionCharLiteral:
                    Mode = AntlrClassifierLexerMode.Action;
                    break;

                case AntlrClassifierLexerMode.ArgAction:
                    Mode = AntlrClassifierLexerMode.ArgActionCharLiteral;
                    break;

                case AntlrClassifierLexerMode.ArgActionCharLiteral:
                    Mode = AntlrClassifierLexerMode.ArgAction;
                    break;

                case AntlrClassifierLexerMode.Grammar:
                case AntlrClassifierLexerMode.GrammarDoubleAngleStringLiteral:
                case AntlrClassifierLexerMode.ActionStringLiteral:
                case AntlrClassifierLexerMode.ArgActionStringLiteral:
                default:
                    throw new InvalidOperationException();
                }
            }
        }

        internal bool InStringLiteral
        {
            get
            {
                return Mode == AntlrClassifierLexerMode.ActionStringLiteral
                    || Mode == AntlrClassifierLexerMode.ArgActionStringLiteral;
            }

            set
            {
                if (InStringLiteral == value)
                    return;

                switch (Mode)
                {
                case AntlrClassifierLexerMode.Action:
                    Mode = AntlrClassifierLexerMode.ActionStringLiteral;
                    break;

                case AntlrClassifierLexerMode.ActionStringLiteral:
                    Mode = AntlrClassifierLexerMode.Action;
                    break;

                case AntlrClassifierLexerMode.ArgAction:
                    Mode = AntlrClassifierLexerMode.ArgActionStringLiteral;
                    break;

                case AntlrClassifierLexerMode.ArgActionStringLiteral:
                    Mode = AntlrClassifierLexerMode.ArgAction;
                    break;

                case AntlrClassifierLexerMode.Grammar:
                case AntlrClassifierLexerMode.GrammarDoubleAngleStringLiteral:
                case AntlrClassifierLexerMode.ActionCharLiteral:
                case AntlrClassifierLexerMode.ArgActionCharLiteral:
                default:
                    throw new InvalidOperationException();
                }
            }
        }

        internal bool InOptions
        {
            get
            {
                return _inOptions;
            }

            set
            {
                _inOptions = value;
            }
        }

        internal bool InTokens
        {
            get
            {
                return _inTokens;
            }

            set
            {
                _inTokens = value;
            }
        }

        public AntlrClassifierLexerState GetCurrentState()
        {
            return new AntlrClassifierLexerState(_mode, _actionLevel, _inComment, _inOptions, _inTokens);
        }

        public void SetCurrentState(AntlrClassifierLexerState state)
        {
            _mode = state.Mode;
            _actionLevel = state.ActionLevel;
            _inComment = state.InComment;
            _inOptions = state.InOptions;
            _inTokens = state.InTokens;
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
                if (ActionLevel == 1 && (InOptions || InTokens) && _input.LA(1) != '}')
                    goto case AntlrClassifierLexerMode.Grammar;

                switch (_input.LA(1))
                {
                case '{':
                    token = _grammarLexer.NextToken();
                    ActionLevel++;
                    token.Type = AntlrGrammarClassifierLexer.ACTION;
                    break;

                case '}':
                    token = _grammarLexer.NextToken();
                    ActionLevel--;
                    token.Type = AntlrGrammarClassifierLexer.ACTION;
                    if (ActionLevel == 0)
                    {
                        Mode = AntlrClassifierLexerMode.Grammar;
                        if (InOptions || InTokens)
                        {
                            token.Type = AntlrGrammarClassifierLexer.RCURLY;
                            InOptions = false;
                            InTokens = false;
                        }
                    }

                    break;

                default:
                    token = _actionLexer.NextToken();
                    break;
                }

                break;

            case AntlrClassifierLexerMode.ActionCharLiteral:
            case AntlrClassifierLexerMode.ActionStringLiteral:
            case AntlrClassifierLexerMode.ArgActionCharLiteral:
            case AntlrClassifierLexerMode.ArgActionStringLiteral:
                token = _actionLexer.NextToken();
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

            case AntlrClassifierLexerMode.GrammarDoubleAngleStringLiteral:
                token = _grammarLexer.NextToken();
                break;

            case AntlrClassifierLexerMode.Grammar:
            default:
                token = _grammarLexer.NextToken();

                switch (token.Type)
                {
                case AntlrGrammarClassifierLexer.LCURLY:
                    ActionLevel++;
                    Mode = AntlrClassifierLexerMode.Action;
                    if ((!InOptions && !InTokens) || ActionLevel != 1)
                        token.Type = AntlrGrammarClassifierLexer.ACTION;
                    break;

                case AntlrGrammarClassifierLexer.LBRACK:
                    Mode = AntlrClassifierLexerMode.ArgAction;
                    break;

                case AntlrGrammarClassifierLexer.IDENTIFIER:
                    switch (token.Text)
                    {
                    case "options":
                        InOptions = true;
                        break;

                    case "tokens":
                        InTokens = true;
                        break;

                    default:
                        if (InOptions)
                            token.Type = AntlrGrammarClassifierLexer.OptionValue;

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

                                case AntlrGrammarClassifierLexer.ASSIGN:
                                    if (InOptions)
                                    {
                                        if (IsValidOption(token.Text))
                                            token.Type = AntlrGrammarClassifierLexer.ValidGrammarOption;
                                        else
                                            token.Type = AntlrGrammarClassifierLexer.InvalidGrammarOption;
                                    }
                                    else if (InTokens)
                                    {
                                    }
                                    else
                                    {
                                        token.Type = AntlrGrammarClassifierLexer.LABEL;
                                    }
                                    break;

                                case AntlrGrammarClassifierLexer.PLUS_ASSIGN:
                                    token.Type = AntlrGrammarClassifierLexer.LABEL;
                                    break;

                                default:
                                    break;
                                }

                                break;
                            }
                        }
                        finally
                        {
                            _input.Rewind(marker);
                            SetCurrentState(currentState);
                        }

                        break;
                    }
                    break;

                default:
                    break;
                }

                break;
            }

            return token;
        }

        private static readonly HashSet<string> ValidOptions = new HashSet<string>
            {
                "language",
                "tokenVocab",
                "TokenLabelType",
                "superClass",
                "filter",
                "k",
                "backtrack",
                "memoize",
                "output",
                "rewrite",
                "ASTLabelType",
                "greedy",
            };

        private static bool IsValidOption(string option)
        {
            if (string.IsNullOrEmpty(option))
                return false;

            return ValidOptions.Contains(option);
        }
    }
}
