namespace Tvl.VisualStudio.Language.Php.Projection
{
    using System;
    using Antlr4.Runtime;
    using Tvl.VisualStudio.Language.Parsing4;

    partial class PhpContentTypeLexer : ITokenSourceWithState<PhpContentTypeLexerState>
    {
        private string _heredocIdentifier;

        public string HeredocIdentifier
        {
            get
            {
                return _heredocIdentifier;
            }

            set
            {
                _heredocIdentifier = value;
            }
        }

        public ICharStream CharStream
        {
            get
            {
                return (ICharStream)InputStream;
            }
        }

        public override IToken NextToken()
        {
            IToken token = base.NextToken();
            while (token.Type == NEWLINE)
                token = base.NextToken();

            return token;
        }

        public override int PopMode()
        {
            if (_mode == PhpDoc)
                _heredocIdentifier = null;

            return base.PopMode();
        }

        public override void PushMode(int m)
        {
            if (_mode == PhpDoc)
                _heredocIdentifier = Text.Substring(3).Trim('\'');

            base.PushMode(m);
        }

        private bool CheckHeredocEnd(int la1, string text)
        {
            // identifier
            //  - or -
            // identifier;
            bool semi = text[text.Length - 1] == ';';
            string identifier = semi ? text.Substring(0, text.Length - 1) : text;
            return string.Equals(identifier, HeredocIdentifier, StringComparison.Ordinal);
        }

        public PhpContentTypeLexerState GetCurrentState()
        {
            return new PhpContentTypeLexerState(this);
        }
    }
}
