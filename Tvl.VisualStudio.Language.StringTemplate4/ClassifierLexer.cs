namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System;
    using System.Diagnostics.Contracts;
    using Antlr.Runtime;
    using Tvl.VisualStudio.Language.Parsing;

    internal class ClassifierLexer : ITokenSourceWithState<ClassifierLexer.LexerState>
    {
        private readonly ICharStream _input;
        private readonly InsideClassifierLexer _insideLexer;
        private readonly OutsideClassifierLexer _outsideLexer;
        private readonly GroupClassifierLexer _groupLexer;

        private TemplateLexerMode _mode;
        private OutermostTemplate _outermost;
        private int _anonymousTemplateLevel;
        private bool _inComment;

        public ClassifierLexer(ICharStream input)
            : this(input, LexerState.Initial)
        {
            Contract.Requires(input != null);
        }

        public ClassifierLexer(ICharStream input, LexerState state)
        {
            Contract.Requires<ArgumentNullException>(input != null, "input");

            _input = input;
            _insideLexer = new InsideClassifierLexer(input);
            _outsideLexer = new OutsideClassifierLexer(input, this);
            _groupLexer = new GroupClassifierLexer(input);

            var stream = input as StringTemplateClassifier.StringTemplateEscapedCharStream;
            if (stream != null)
                stream.Lexer = this;

            State = state;
        }

        public string SourceName
        {
            get
            {
                return "StringTemplate Colorizer";
            }
        }

        public string[] TokenNames
        {
            get
            {
                return _insideLexer.TokenNames;
            }
        }

        internal LexerState State
        {
            get
            {
                return new LexerState(_mode, _outermost, _anonymousTemplateLevel, _inComment);
            }

            set
            {
                _mode = value.Mode;
                _outermost = value.Outermost;
                _anonymousTemplateLevel = value.AnonymousTemplateLevel;
                _inComment = value.InComment;
            }
        }

        internal TemplateLexerMode Mode
        {
            get
            {
                return _mode;
            }

            private set
            {
                _mode = value;
            }
        }

        internal OutermostTemplate Outermost
        {
            get
            {
                return _outermost;
            }

            private set
            {
                _outermost = value;
            }
        }

        internal int AnonymousTemplateLevel
        {
            get
            {
                return _anonymousTemplateLevel;
            }

            private set
            {
                _anonymousTemplateLevel = value;
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

        public LexerState GetCurrentState()
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
            } while (token == null || token.Type == GroupClassifierLexer.NEWLINE);

            return token;
        }

        private IToken NextTokenCore()
        {
            //// TODO: setting these values is the only complicated portion of this whole lexer... :)
            //OutermostTemplate outermost = OutermostTemplate.None;
            //TemplateLexerState state = TemplateLexerState.Group;
            ////int expressionLevel = 0;
            //int anonymousTemplateLevel = 0;

            // when true, the outermost template's closing token is '>>'. otherwise, '"' is the closing token
            bool insideBigString = Outermost == OutermostTemplate.BigString;
            bool insideBigStringLine = Outermost == OutermostTemplate.BigStringLine;

            IToken token = null;

            switch (Mode)
            {
            case TemplateLexerMode.Template:
                if (AnonymousTemplateLevel > 0 && _input.LA(1) == '}')
                {
                    // no longer inside the template - let the group lexer prepare the closing template token
                    token = _groupLexer.NextToken();
                    AnonymousTemplateLevel--;
                    if (AnonymousTemplateLevel == 0 && Outermost == OutermostTemplate.None)
                        Mode = TemplateLexerMode.Group;
                    else
                        Mode = TemplateLexerMode.Expression;
                }
                else if ((insideBigString && AnonymousTemplateLevel == 0 && _input.LA(1) == '>' && _input.LA(2) == '>')
                    || (insideBigStringLine && AnonymousTemplateLevel == 0 && _input.LA(1) == '%' && _input.LA(2) == '>')
                    || (!insideBigString && !insideBigStringLine && AnonymousTemplateLevel == 0 && _input.LA(1) == '"'))
                {
                    // no longer inside the template - let the group lexer prepare the closing template token
                    token = _groupLexer.NextToken();
                    Mode = TemplateLexerMode.Group;
                    Outermost = OutermostTemplate.None;
                    InComment = false;
                }
                else
                {
                    token = _outsideLexer.NextToken();
                    if (token.Type == OutsideClassifierLexer.LANGLE)
                    {
                        //expressionLevel++;
                        Mode = TemplateLexerMode.Expression;
                    }
                }

                break;

            case TemplateLexerMode.Expression:
                if (_input.LA(1) == '>')
                {
                    // no longer inside the expression - let the template lexer prepare the RANGLE token
                    token = _insideLexer.NextToken();
                    Mode = TemplateLexerMode.Template;
                    //Debug.Assert(expressionLevel > 0);
                    //expressionLevel--;
                }
                else
                {
                    token = _insideLexer.NextToken();
                    if (token.Type == InsideClassifierLexer.LBRACE)
                    {
                        AnonymousTemplateLevel++;
                        Mode = TemplateLexerMode.Template;
                    }
                }
                break;

            case TemplateLexerMode.Group:
            default:
                switch (_input.LA(1))
                {
                case '{':
                    token = _groupLexer.NextToken();
                    AnonymousTemplateLevel++;
                    Mode = TemplateLexerMode.Template;
                    Outermost = OutermostTemplate.None;
                    break;

                case '"':
                    token = _groupLexer.NextToken();
                    Mode = TemplateLexerMode.Template;
                    Outermost = OutermostTemplate.String;
                    break;

                case '<':
                    if (_input.LA(2) == '<' || _input.LA(2) == '%')
                    {
                        token = _groupLexer.NextToken();
                        Mode = TemplateLexerMode.Template;
                        Outermost = token.Type == GroupClassifierLexer.BEGIN_BIGSTRINGLINE ? OutermostTemplate.BigStringLine : OutermostTemplate.BigString;
                        break;
                    }
                    else
                    {
                        goto default;
                    }

                default:
                    token = _groupLexer.NextToken();
                    break;
                }

                break;
            }

            return token;
        }

        internal enum TemplateLexerMode
        {
            Group,
            Template,
            Expression,
        }

        internal enum OutermostTemplate
        {
            None,
            String,
            BigString,
            BigStringLine,
        }

        internal struct LexerState : IEquatable<LexerState>
        {
            internal static readonly LexerState Initial = new LexerState();

            internal readonly TemplateLexerMode Mode;
            internal readonly OutermostTemplate Outermost;
            internal readonly int AnonymousTemplateLevel;
            internal readonly bool InComment;

            public LexerState(TemplateLexerMode mode, OutermostTemplate outermost, int anonymousTemplateLevel, bool inComment)
            {
                Mode = mode;
                Outermost = outermost;
                AnonymousTemplateLevel = anonymousTemplateLevel;
                InComment = inComment;
            }

            public bool Equals(LexerState other)
            {
                return AnonymousTemplateLevel == other.AnonymousTemplateLevel
                    && Mode == other.Mode
                    && Outermost == other.Outermost
                    && InComment == other.InComment;
            }

            public override bool Equals(object obj)
            {
                if (!(obj is LexerState))
                    return false;

                return Equals((LexerState)obj);
            }

            public override int GetHashCode()
            {
                return this.AnonymousTemplateLevel.GetHashCode()
                    ^ this.Mode.GetHashCode()
                    ^ this.Outermost.GetHashCode()
                    ^ this.InComment.GetHashCode();
            }
        }
    }
}
