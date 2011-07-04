namespace Tvl.VisualStudio.Language.Php
{
    using System;
    using System.Diagnostics.Contracts;
    using Antlr.Runtime;

    partial class PhpHtmlTagClassifierLexer
    {
        private readonly PhpClassifierLexer _lexer;

        public PhpHtmlTagClassifierLexer(ICharStream input, PhpClassifierLexer lexer)
            : this(input)
        {
            Contract.Requires<ArgumentNullException>(lexer != null, "lexer");

            _lexer = lexer;
        }
    }
}
