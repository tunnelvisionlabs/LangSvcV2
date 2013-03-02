namespace Tvl.VisualStudio.Language.Php.Classification
{
    using Antlr4.Runtime;

    partial class V4PhpClassifierLexer
    {
        private static bool IsTagStart(IIntStream input)
        {
            if (input.La(1) != '<')
                return false;

            int la2 = input.La(2);
            if (la2 < 0)
                return false;

            switch (la2)
            {
            case '?':
                return true;

            case '!':
                {
                    int la3 = input.La(3);
                    if (char.IsLetterOrDigit((char)la3))
                        return true;

                    if (la3 == '-' && input.La(4) == '-')
                        return true;

                    if (la3 == '[' && input.La(4) == 'C' && input.La(5) == 'D' && input.La(6) == 'A' && input.La(7) == 'T' && input.La(8) == 'A' && input.La(9) == '[')
                        return true;
                }

                return false;

            case '/':
                return char.IsLetterOrDigit((char)input.La(3));

            default:
                if (char.IsLetterOrDigit((char)la2))
                    return true;

                break;
            }

            return false;
        }
    }
}
