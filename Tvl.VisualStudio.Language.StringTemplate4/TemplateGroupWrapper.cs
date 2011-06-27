namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Antlr.Runtime;
    using Antlr4.StringTemplate;
    using Antlr4.StringTemplate.Compiler;
    using Antlr4.StringTemplate.Misc;

//#error TODO: handle dictionaries.
    public class TemplateGroupWrapper : TemplateGroup
    {
        private readonly List<TemplateInformation> _templateInformation = new List<TemplateInformation>();

        private readonly Dictionary<CompiledTemplate, TemplateInformation> _compiledTemplateInformation =
            new Dictionary<CompiledTemplate, TemplateInformation>();

        public TemplateGroupWrapper(char delimiterStartChar, char delimiterStopChar)
            : base(delimiterStartChar, delimiterStopChar)
        {
        }

        public override void DefineDictionary(string name, IDictionary<string, object> mapping)
        {
            base.DefineDictionary(name, mapping);
        }

        public override CompiledTemplate DefineRegion(string enclosingTemplateName, IToken regionT, string template, IToken templateToken)
        {
            CompiledTemplate result = base.DefineRegion(enclosingTemplateName, regionT, template, templateToken);
            TemplateInformation info = new TemplateInformation(enclosingTemplateName, regionT, templateToken, result);
            _templateInformation.Add(info);
            _compiledTemplateInformation.Add(result, info);

            return result;
        }

        public override CompiledTemplate DefineTemplate(string templateName, IToken nameT, List<FormalArgument> args, string template, IToken templateToken)
        {
            CompiledTemplate result = base.DefineTemplate(templateName, nameT, args, template, templateToken);
            TemplateInformation info = new TemplateInformation(nameT, templateToken, result);
            _templateInformation.Add(info);
            _compiledTemplateInformation.Add(result, info);

            return result;
        }

        public override CompiledTemplate DefineTemplateAlias(IToken aliasT, IToken targetT)
        {
            CompiledTemplate result = base.DefineTemplateAlias(aliasT, targetT);
            _templateInformation.Add(new TemplateInformation(aliasT, targetT, result));

            return result;
        }

        internal ICollection<TemplateInformation> GetTemplateInformation()
        {
            return _templateInformation;
        }

        internal TemplateInformation GetTemplateInformation(CompiledTemplate template)
        {
            Contract.Requires<ArgumentNullException>(template != null, "template");
            Contract.Requires<ArgumentException>(template.NativeGroup == this);
            Contract.Ensures(Contract.Result<TemplateInformation>() != null);

            return _compiledTemplateInformation[template];
        }

        internal class DictionaryInformation
        {
            //private readonly 
        }

        internal class TemplateInformation
        {
            private readonly string _enclosingTemplateName;
            private readonly IToken _nameToken;
            private readonly IToken _templateToken;
            private readonly CompiledTemplate _template;

            public TemplateInformation(IToken nameToken, IToken templateToken, CompiledTemplate template)
                : this(null, nameToken, templateToken, template)
            {
            }

            public TemplateInformation(string enclosingTemplateName, IToken nameToken, IToken templateToken, CompiledTemplate template)
            {
                this._enclosingTemplateName = enclosingTemplateName;
                this._nameToken = nameToken;
                this._templateToken = templateToken;
                this._template = template;
            }

            public string EnclosingTemplateName
            {
                get
                {
                    return _enclosingTemplateName;
                }
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

            public CompiledTemplate Template
            {
                get
                {
                    return _template;
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
