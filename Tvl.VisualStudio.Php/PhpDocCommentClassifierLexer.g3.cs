namespace Tvl.VisualStudio.Language.Php
{
    using System;
    using Antlr.Runtime;
    using System.Diagnostics.Contracts;

    partial class PhpDocCommentClassifierLexer
    {
        private const string DocCommentStartSymbols = "$@&~<>#%\"\\";

        private readonly PhpClassifierLexer _lexer;

        public PhpDocCommentClassifierLexer(ICharStream input, PhpClassifierLexer lexer)
            : this(input)
        {
            Contract.Requires<ArgumentNullException>(lexer != null, "lexer");

            _lexer = lexer;
        }

        private static bool IsDocCommentStartCharacter(int c)
        {
            if (char.IsLetter((char)c))
                return true;

            return DocCommentStartSymbols.IndexOf((char)c) >= 0;
        }
    }
}
