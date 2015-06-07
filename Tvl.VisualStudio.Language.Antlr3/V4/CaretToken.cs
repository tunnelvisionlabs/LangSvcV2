namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using Antlr4.Runtime;

    internal class CaretToken : CommonToken, ICaretToken
    {
        public const int CaretTokenType = -2;

        private readonly IToken _originalToken;

        public CaretToken(Tuple<ITokenSource, ICharStream> source, int channel, int start, int stop)
            : base(source, CaretTokenType, channel, start, stop)
        {
        }

        public CaretToken(IToken oldToken)
            : base(oldToken)
        {
            this.channel = TokenConstants.DefaultChannel;
            this._originalToken = oldToken;
            this.type = CaretTokenType;
        }

        public IToken OriginalToken
        {
            get
            {
                return _originalToken;
            }
        }

        public int OriginalType
        {
            get
            {
                return _originalToken.Type;
            }
        }
    }
}
