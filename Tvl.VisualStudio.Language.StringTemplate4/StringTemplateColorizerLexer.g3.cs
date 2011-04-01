namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using Antlr.Runtime;
    using Antlr4.StringTemplate;
    using Antlr4.StringTemplate.Compiler;
    using Antlr4.StringTemplate.Misc;

    partial class StringTemplateColorizerLexer
    {
        private readonly TemplateGroup group = new TemplateGroup('<', '>');

        private void HandleAnonymousTemplate()
        {
            IToken templateToken = new CommonToken(input, ANONYMOUS_TEMPLATE, 0, CharIndex, CharIndex);
            TemplateLexer lexer = new TemplateLexer(group.ErrorManager, input, templateToken, group.delimiterStartChar, group.delimiterStopChar);
            lexer.subtemplateDepth = 1;
            IToken t = lexer.NextToken();
            while (lexer.subtemplateDepth >= 1 || t.Type != TemplateLexer.RCURLY)
            {
                if (t.Type == TemplateLexer.EOF_TYPE)
                {
                    MismatchedTokenException e = new MismatchedTokenException('}', input);
                    string msg = "missing final '}' in {...} anonymous template";
                    group.ErrorManager.GroupLexerError(ErrorType.SYNTAX_ERROR, SourceName, e, msg);
                    break;
                }

                t = lexer.NextToken();
            }
        }
    }
}
