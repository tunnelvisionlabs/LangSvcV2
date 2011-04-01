namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Antlr.Runtime;
    using Antlr4.StringTemplate;
    using Antlr4.StringTemplate.Compiler;
    using Antlr4.StringTemplate.Misc;

    public class TemplateGroupWrapper : TemplateGroup
    {
        private readonly Dictionary<CompiledTemplate, TemplateInformation> _templateInformation =
            new Dictionary<CompiledTemplate, TemplateInformation>();

        public TemplateGroupWrapper(char delimiterStartChar, char delimiterStopChar)
            : base(delimiterStartChar, delimiterStopChar)
        {
            Debug = true;
        }

        public override CompiledTemplate DefineRegion(string enclosingTemplateName, IToken regionT, string template, IToken templateToken)
        {
            CompiledTemplate result = base.DefineRegion(enclosingTemplateName, regionT, template, templateToken);
            _templateInformation[result] = new TemplateInformation(regionT, templateToken);

            return result;
        }

        public override CompiledTemplate DefineTemplate(string templateName, IToken nameT, List<FormalArgument> args, string template, IToken templateToken)
        {
            CompiledTemplate result = base.DefineTemplate(templateName, nameT, args, template, templateToken);
            _templateInformation[result] = new TemplateInformation(nameT, templateToken);

            return result;
        }

        public override CompiledTemplate DefineTemplateAlias(IToken aliasT, IToken targetT)
        {
            return base.DefineTemplateAlias(aliasT, targetT);
        }

        internal TemplateInformation GetTemplateInformation(CompiledTemplate template)
        {
            Contract.Requires<ArgumentNullException>(template != null, "template");
            Contract.Requires<ArgumentException>(template.NativeGroup == this);
            Contract.Ensures(Contract.Result<TemplateInformation>() != null);

            return _templateInformation[template];
        }

        internal class TemplateInformation
        {
            private readonly IToken _nameToken;
            private readonly IToken _templateToken;

            public TemplateInformation(IToken nameToken, IToken templateToken)
            {
                this._nameToken = nameToken;
                this._templateToken = templateToken;
            }

            public IToken NameToken
            {
                get
                {
                    return _nameToken;
                }
            }

            public IToken TemplateToken
            {
                get
                {
                    return _templateToken;
                }
            }

            public Interval GroupInterval
            {
                get
                {
                    return Interval.FromBounds(TemplateToken.StartIndex, TemplateToken.StopIndex + 1);
                }
            }
        }
    }
}
