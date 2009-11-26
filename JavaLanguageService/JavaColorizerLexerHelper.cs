namespace JavaLanguageService
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Antlr.Runtime;

    partial class JavaColorizerLexer
    {
        public override void Recover(IIntStream input, RecognitionException re)
        {
            base.Recover(input, re);
        }

        public override void DisplayRecognitionError(string[] tokenNames, RecognitionException e)
        {
            base.DisplayRecognitionError(tokenNames, e);
        }
    }
}
