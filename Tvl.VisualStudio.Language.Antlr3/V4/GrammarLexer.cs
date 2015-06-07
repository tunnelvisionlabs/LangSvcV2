namespace Tvl.VisualStudio.Language.AntlrV4
{
    using Antlr4.Runtime;
    using Antlr4.Runtime.Misc;

    public class GrammarLexer : AbstractGrammarLexer
    {
        private int _ruleType;

        public GrammarLexer(ICharStream input)
            : base(input)
        {
        }

        private bool InLexerRule
        {
            get
            {
                return _ruleType == TOKEN_REF;
            }
        }

        protected override void HandleBeginArgAction()
        {
            if (InLexerRule)
            {
                PushMode(LexerCharSet);
                More();
            }
            else
            {
                PushMode(ArgAction);
            }
        }

        public override IToken Emit()
        {
            if (_type == ID)
            {
                string firstChar = _input.GetText(Interval.Of(_tokenStartCharIndex, _tokenStartCharIndex));
                if (char.IsUpper(firstChar[0]))
                    _type = TOKEN_REF;
                else
                    _type = RULE_REF;

                if (_ruleType == TokenConstants.InvalidType)
                    _ruleType = _type;
            }
            else if (_type == SEMI)
            {
                _ruleType = TokenConstants.InvalidType;
            }

            return base.Emit();
        }
    }
}
