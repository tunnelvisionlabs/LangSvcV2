namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System;
    using System.Diagnostics.Contracts;
    using Antlr.Runtime;
    using Tvl.VisualStudio.Language.Parsing;

    internal class ClassifierLexer : ITokenSourceWithState<ClassifierLexerState>
    {
        private readonly ICharStream _input;
        private readonly InsideClassifierLexer _insideLexer;
        private readonly OutsideClassifierLexer _outsideLexer;
        private readonly GroupClassifierLexer _groupLexer;

        private TemplateLexerMode _mode;
        private OutermostTemplate _outermost;
        private int _anonymousTemplateLevel;
        private bool _inComment;
        private char _openDelimiter = '<';
        private char _closeDelimiter = '>';

        public ClassifierLexer(ICharStream input)
            : this(input, ClassifierLexerState.Initial)
        {
            Contract.Requires(input != null);
        }

        public ClassifierLexer(ICharStream input, ClassifierLexerState state)
        {
            Contract.Requires<ArgumentNullException>(input != null, "input");

            _input = input;
            _insideLexer = new InsideClassifierLexer(input, this);
            _outsideLexer = new OutsideClassifierLexer(input, this);
            _groupLexer = new GroupClassifierLexer(input, this);

            var stream = input as StringTemplateClassifier.StringTemplateEscapedCharStream;
            if (stream != null)
                stream.Lexer = this;

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

        internal ClassifierLexerState State
        {
            get
            {
                return new ClassifierLexerState(_mode, _outermost, _anonymousTemplateLevel, _inComment, _openDelimiter, _closeDelimiter);
            }

            set
            {
                _mode = value.Mode;
                _outermost = value.Outermost;
                _anonymousTemplateLevel = value.AnonymousTemplateLevel;
                _inComment = value.InComment;
                _openDelimiter = value.OpenDelimiter;
                _closeDelimiter = value.CloseDelimiter;
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

        internal char OpenDelimiter
        {
            get
            {
                return _openDelimiter;
            }
        }

        internal char CloseDelimiter
        {
            get
            {
                return _closeDelimiter;
            }
        }

        public ClassifierLexerState GetCurrentState()
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
            case TemplateLexerMode.AnonymousTemplateParameters:
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
                    if (token.Type == OutsideClassifierLexer.LDELIM)
                    {
                        //expressionLevel++;
                        Mode = TemplateLexerMode.Expression;
                    }
                    else if (Mode == TemplateLexerMode.AnonymousTemplateParameters)
                    {
                        switch (token.Type)
                        {
                        case OutsideClassifierLexer.ID:
                            token.Type = OutsideClassifierLexer.PARAMETER_DEFINITION;
                            break;

                        case OutsideClassifierLexer.PIPE:
                            Mode = TemplateLexerMode.Template;
                            break;

                        default:
                            break;
                        }
                    }
                    else if (Mode == TemplateLexerMode.Template)
                    {
                        switch (token.Type)
                        {
                        case OutsideClassifierLexer.ID:
                        case OutsideClassifierLexer.PIPE:
                        case OutsideClassifierLexer.COMMA:
                        case OutsideClassifierLexer.WS:
                            token.Type = OutsideClassifierLexer.TEXT;
                            break;

                        default:
                            break;
                        }
                    }
                }

                break;

            case TemplateLexerMode.Expression:
                if (_input.LA(1) == CloseDelimiter)
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
                        Mode = CheckAnonymousTemplateForParameters();
                    }
                }
                break;

            case TemplateLexerMode.Group:
            case TemplateLexerMode.DelimitersOpenSpec:
            case TemplateLexerMode.DelimitersCloseSpec:
            default:
                switch (_input.LA(1))
                {
                case '{':
                    token = _groupLexer.NextToken();
                    AnonymousTemplateLevel++;
                    Mode = CheckAnonymousTemplateForParameters();
                    Outermost = OutermostTemplate.None;
                    break;

                case '"':
                    token = _groupLexer.NextToken();
                    if (Mode == TemplateLexerMode.Group)
                    {
                        Mode = TemplateLexerMode.Template;
                        Outermost = OutermostTemplate.String;
                    }
                    else if (Mode == TemplateLexerMode.DelimitersOpenSpec)
                    {
                        if (token.Text.Length > 2)
                            _openDelimiter = token.Text[1];

                        Mode = TemplateLexerMode.DelimitersCloseSpec;
                    }
                    else if (Mode == TemplateLexerMode.DelimitersCloseSpec)
                    {
                        if (token.Text.Length > 2)
                            _closeDelimiter = token.Text[1];

                        Mode = TemplateLexerMode.Group;
                    }

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

                    switch (token.Type)
                    {
                    case GroupClassifierLexer.ID:
                        if (token.Text == "delimiters")
                            Mode = TemplateLexerMode.DelimitersOpenSpec;

                        break;

                    case GroupClassifierLexer.LEGACY_DELIMITERS:
                        Mode = TemplateLexerMode.DelimitersOpenSpec;
                        break;

                    default:
                        break;
                    }

                    break;
                }

                break;
            }

            return token;
        }

        private TemplateLexerMode CheckAnonymousTemplateForParameters()
        {
            int position = _input.Mark();
            ClassifierLexerState currentState = State;

            try
            {
                Mode = TemplateLexerMode.AnonymousTemplateParameters;
                bool previousWasArg = false;
                while (true)
                {
                    IToken token = NextToken();
                    switch (token.Type)
                    {
                    case OutsideClassifierLexer.COMMA:
                        if (!previousWasArg)
                            return TemplateLexerMode.Template;

                        previousWasArg = false;
                        continue;

                    case OutsideClassifierLexer.PARAMETER_DEFINITION:
                    case OutsideClassifierLexer.ID:
                        if (previousWasArg)
                            return TemplateLexerMode.Template;

                        previousWasArg = true;
                        continue;

                    case OutsideClassifierLexer.PIPE:
                        if (previousWasArg)
                            return TemplateLexerMode.AnonymousTemplateParameters;

                        return TemplateLexerMode.Template;

                    case OutsideClassifierLexer.WS:
                    case OutsideClassifierLexer.COMMENT:
                    case OutsideClassifierLexer.NEWLINE:
                        continue;

                    default:
                        return TemplateLexerMode.Template;
                    }
                }
            }
            finally
            {
                _input.Rewind(position);
                State = currentState;
            }
        }
    }
}
