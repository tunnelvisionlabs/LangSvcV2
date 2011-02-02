namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Antlr4.StringTemplate;
    using Antlr4.StringTemplate.Compiler;
    using Antlr.Runtime;
    using Antlr4.StringTemplate.Misc;
    using System.Diagnostics.Contracts;

    public class TemplateGroupWrapper : TemplateGroup
    {
        private readonly Dictionary<CompiledTemplate, TemplateInformation> _templateInformation =
            new Dictionary<CompiledTemplate, TemplateInformation>();

        private CompiledTemplate _lastRegionTemplate;

        public TemplateGroupWrapper(char delimiterStartChar, char delimiterStopChar)
            : base(delimiterStartChar, delimiterStopChar)
        {
        }

        public IEnumerable<CompiledTemplate> Templates
        {
            get
            {
                return templates.Values;
            }
        }

        public override CompiledTemplate DefineRegion(string enclosingTemplateName, IToken regionT, string template)
        {
            CompiledTemplate result = base.DefineRegion(enclosingTemplateName, regionT, template);
            _lastRegionTemplate = result;

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

        public override void DefineTemplateOrRegion(string templateName, string regionSurroundingTemplateName, IToken templateToken, string template, IToken nameToken, List<FormalArgument> args)
        {
            base.DefineTemplateOrRegion(templateName, regionSurroundingTemplateName, templateToken, template, nameToken, args);
            if (regionSurroundingTemplateName != null)
            {
                _templateInformation[_lastRegionTemplate] = new TemplateInformation(nameToken, templateToken);
                _lastRegionTemplate = null;
            }
        }

        internal TemplateInformation GetTemplateInformation(CompiledTemplate template)
        {
            Contract.Requires<ArgumentNullException>(template != null, "template");
            Contract.Requires<ArgumentException>(template.nativeGroup == this);
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
