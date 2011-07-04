namespace Tvl.VisualStudio.Language.Php
{
    using System;
    using System.Diagnostics.Contracts;
    using Antlr.Runtime;

    partial class PhpHtmlTextClassifierLexer
    {
        private readonly PhpClassifierLexer _lexer;

        public PhpHtmlTextClassifierLexer(ICharStream input, PhpClassifierLexer lexer)
            : this(input)
        {
            Contract.Requires<ArgumentNullException>(lexer != null, "lexer");

            _lexer = lexer;
        }

        private static bool IsTagStart(IIntStream input)
        {
            if (input.LA(1) != '<')
                return false;

            int la2 = input.LA(2);
            if (la2 < 0)
                return false;

            switch (la2)
            {
            case '?':
                return true;

            case '!':
                {
                    int la3 = input.LA(3);
                    if (char.IsLetterOrDigit((char)la3))
                        return true;

                    if (la3 == '-' && input.LA(4) == '-')
                        return true;

                    if (la3 == '[' && input.LA(4) == 'C' && input.LA(5) == 'D' && input.LA(6) == 'A' && input.LA(7) == 'T' && input.LA(8) == 'A' && input.LA(9) == '[')
                        return true;
                }

                return false;

            case '/':
                return char.IsLetterOrDigit((char)input.LA(3));

            default:
                if (char.IsLetterOrDigit((char)la2))
                    return true;

                break;
            }

            return false;
        }
    }
}
