namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Atn;
    using Antlr4.Runtime.Misc;
    using Tvl.VisualStudio.Language.Parsing4;

    public class GrammarHighlighterLexer : AbstractGrammarHighlighterLexer, ITokenSourceWithState<V4GrammarClassifierLexerState>
    {
        private bool _inOptions;
        private bool _inTokens;
        private int _ruleType;

        public GrammarHighlighterLexer(ICharStream input)
            : base(input)
        {
            _interp = new GrammarHighlighterATNSimulator(this, _ATN);
        }

        public bool IsInOptions
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

        public bool IsInTokens
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

        public int RuleType
        {
            get
            {
                return _ruleType;
            }

            set
            {
                if (value != TOKEN_REF && value != RULE_REF && value != TokenConstants.InvalidType)
                    throw new ArgumentException();

                _ruleType = value;
            }
        }

        protected bool IsInLexerRule
        {
            get
            {
                return _ruleType == TOKEN_REF;
            }
        }

        protected override int GetMultilineCommentType()
        {
            return _modeStack[_modeStack.Count - 1] == DefaultMode ? ML_COMMENT : Action_ML_COMMENT;
        }

        protected override void HandleBeginArgAction()
        {
            if (IsInLexerRule)
                PushMode(LexerCharSet);
            else
                PushMode(ArgAction);
        }

        public override IToken NextToken()
        {
            IToken result = base.NextToken();
            while (result != null && result.Type == NEWLINE)
                result = base.NextToken();

            return result;
        }

        public override IToken Emit()
        {
            switch (_type)
            {
            case TOKENS:
                HandleAcceptPositionForKeyword("tokens");
                IsInTokens = true;
                break;

            case OPTIONS:
                HandleAcceptPositionForKeyword("options");
                IsInOptions = true;
                break;

            case LABEL:
                HandleAcceptPositionForIdentifier();
                if (IsInOptions)
                    _type = ValidGrammarOption;
                else if (IsInTokens)
                    _type = IDENTIFIER;

                break;

            case RCURLY:
                IsInTokens = false;
                IsInOptions = false;
                break;

            case SEMI:
                RuleType = TokenConstants.InvalidType;
                break;

            case IDENTIFIER:
                if (_ruleType == TokenConstants.InvalidType)
                {
                    string firstChar = _input.GetText(Interval.Of(_tokenStartCharIndex, _tokenStartCharIndex));
                    if (char.IsUpper(firstChar, 0))
                        _ruleType = TOKEN_REF;
                    else
                        _ruleType = RULE_REF;
                }

                break;

            default:
                break;
            }

            return base.Emit();
        }

        protected new GrammarHighlighterATNSimulator Interpreter
        {
            get
            {
                return (GrammarHighlighterATNSimulator)base.Interpreter;
            }
        }

        private bool HandleAcceptPositionForIdentifier()
        {
            String tokenText = Text;
            int identifierLength = 0;
            while (identifierLength < tokenText.Length && IsIdentifierChar(tokenText[identifierLength]))
            {
                identifierLength++;
            }

            if (InputStream.Index > _tokenStartCharIndex + identifierLength)
            {
                int offset = identifierLength - 1;
                Interpreter.ResetAcceptPosition(CharStream, _tokenStartCharIndex + offset, _tokenStartLine, _tokenStartCharPositionInLine + offset);
                return true;
            }

            return false;
        }

        private bool HandleAcceptPositionForKeyword(String keyword)
        {
            if (InputStream.Index > _tokenStartCharIndex + keyword.Length)
            {
                int offset = keyword.Length - 1;
                Interpreter.ResetAcceptPosition(CharStream, _tokenStartCharIndex + offset, _tokenStartLine, _tokenStartCharPositionInLine + offset);
                return true;
            }

            return false;
        }

        private static bool IsIdentifierChar(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_';
        }

        protected class GrammarHighlighterATNSimulator : LexerATNSimulator
        {
            public GrammarHighlighterATNSimulator(Lexer recog, ATN atn)
                : base(recog, atn)
            {
            }

            public void ResetAcceptPosition(ICharStream input, int index, int line, int charPositionInLine)
            {
                input.Seek(index);
                this.line = line;
                this.charPositionInLine = charPositionInLine;
                Consume(input);
            }
        }

        public ICharStream CharStream
        {
            get
            {
                return (ICharStream)InputStream;
            }
        }

        public V4GrammarClassifierLexerState GetCurrentState()
        {
            return new V4GrammarClassifierLexerState(this);
        }
    }
}
