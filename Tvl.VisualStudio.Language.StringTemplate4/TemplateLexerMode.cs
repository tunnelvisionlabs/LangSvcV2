namespace Tvl.VisualStudio.Language.StringTemplate4
{
    internal enum TemplateLexerMode
    {
        Group,
        DelimitersOpenSpec,
        DelimitersCloseSpec,
        Template,
        Expression,
        AnonymousTemplateParameters,
    }
}
