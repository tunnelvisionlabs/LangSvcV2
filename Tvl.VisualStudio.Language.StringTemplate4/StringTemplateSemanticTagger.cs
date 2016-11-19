namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System;
    using System.Collections.Generic;
    using Antlr4.Runtime;
    using Antlr4.Runtime.Tree;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Tagging;
    using Parsing4;
    using IBackgroundParser = Parsing.IBackgroundParser;
    using ParseResultEventArgs = Parsing.ParseResultEventArgs;

    internal class StringTemplateSemanticTagger : SimpleTagger<IClassificationTag>
    {
        private readonly ITextBuffer _buffer;
        private readonly IBackgroundParser _backgroundParser;
        private readonly IClassificationTypeRegistryService _classificationTypeRegistryService;

        private readonly IClassificationType _templateDefinitionClassificationType;
        private readonly IClassificationType _templateReferenceClassificationType;
        private readonly IClassificationType _regionDefinitionClassificationType;
        private readonly IClassificationType _regionReferenceClassificationType;
        private readonly IClassificationType _dictionaryDefinitionClassificationType;
        private readonly IClassificationType _dictionaryReferenceClassificationType;
        private readonly IClassificationType _parameterDefinitionClassificationType;
        private readonly IClassificationType _parameterReferenceClassificationType;
        private readonly IClassificationType _attributeReferenceClassificationType;
        private readonly IClassificationType _expressionOptionClassificationType;

        public StringTemplateSemanticTagger(ITextBuffer buffer, IBackgroundParser backgroundParser, IClassificationTypeRegistryService classificationTypeRegistryService)
            : base(buffer)
        {
            _buffer = buffer;
            _backgroundParser = backgroundParser;
            _classificationTypeRegistryService = classificationTypeRegistryService;

            _templateDefinitionClassificationType = classificationTypeRegistryService.GetClassificationType(StringTemplateClassificationTypeNames.TemplateDefinition);
            _templateReferenceClassificationType = classificationTypeRegistryService.GetClassificationType(StringTemplateClassificationTypeNames.TemplateReference);
            _regionDefinitionClassificationType = classificationTypeRegistryService.GetClassificationType(StringTemplateClassificationTypeNames.RegionDefinition);
            _regionReferenceClassificationType = classificationTypeRegistryService.GetClassificationType(StringTemplateClassificationTypeNames.RegionReference);
            _dictionaryDefinitionClassificationType = classificationTypeRegistryService.GetClassificationType(StringTemplateClassificationTypeNames.DictionaryDefinition);
            _dictionaryReferenceClassificationType = classificationTypeRegistryService.GetClassificationType(StringTemplateClassificationTypeNames.DictionaryReference);
            _parameterDefinitionClassificationType = classificationTypeRegistryService.GetClassificationType(StringTemplateClassificationTypeNames.ParameterDefinition);
            _parameterReferenceClassificationType = classificationTypeRegistryService.GetClassificationType(StringTemplateClassificationTypeNames.ParameterReference);
            _attributeReferenceClassificationType = classificationTypeRegistryService.GetClassificationType(StringTemplateClassificationTypeNames.AttributeReference);
            _expressionOptionClassificationType = classificationTypeRegistryService.GetClassificationType(StringTemplateClassificationTypeNames.ExpressionOption);

            _backgroundParser.ParseComplete += HandleParseComplete;
        }

        private void HandleParseComplete(object sender, ParseResultEventArgs e)
        {
            var antlrArgs = e as StringTemplateParseResultEventArgs;
            if (antlrArgs == null)
                return;

            var intermediateContainer = new List<KeyValuePair<ITrackingSpan, IClassificationTag>>();
            var sourceSnapshot = e.Snapshot;
            var currentSnapshot = _buffer.CurrentSnapshot;
            var listener = new SemanticAnalyzerListener();

            ParseTreeWalker.Default.Walk(listener, antlrArgs.Result4);

            AddTags(intermediateContainer, sourceSnapshot, currentSnapshot, listener.TemplateDefinitions, _templateDefinitionClassificationType);
            AddTags(intermediateContainer, sourceSnapshot, currentSnapshot, listener.TemplateReferences, _templateReferenceClassificationType);
            AddTags(intermediateContainer, sourceSnapshot, currentSnapshot, listener.RegionDefinitions, _regionDefinitionClassificationType);
            AddTags(intermediateContainer, sourceSnapshot, currentSnapshot, listener.RegionReferences, _regionReferenceClassificationType);
            AddTags(intermediateContainer, sourceSnapshot, currentSnapshot, listener.DictionaryDefinitions, _dictionaryDefinitionClassificationType);
            AddTags(intermediateContainer, sourceSnapshot, currentSnapshot, listener.DictionaryReferences, _dictionaryReferenceClassificationType);
            AddTags(intermediateContainer, sourceSnapshot, currentSnapshot, listener.ParameterDefinitions, _parameterDefinitionClassificationType);
            AddTags(intermediateContainer, sourceSnapshot, currentSnapshot, listener.ParameterReferences, _parameterReferenceClassificationType);
            AddTags(intermediateContainer, sourceSnapshot, currentSnapshot, listener.AttributeReferences, _attributeReferenceClassificationType);
            AddTags(intermediateContainer, sourceSnapshot, currentSnapshot, listener.Options, _expressionOptionClassificationType);

            intermediateContainer.Sort(
                (a, b) =>
                {
                    int diff = a.Key.GetStartPoint(currentSnapshot) - b.Key.GetStartPoint(currentSnapshot);
                    if (diff != 0)
                    {
                        return diff;
                    }

                    return a.Key.GetEndPoint(currentSnapshot) - b.Key.GetEndPoint(currentSnapshot);
                });

            using (Update())
            {
                RemoveTagSpans(_ => true);
                foreach (var pair in intermediateContainer)
                {
                    CreateTagSpan(pair.Key, pair.Value);
                }
            }
        }

        private void AddTags(List<KeyValuePair<ITrackingSpan, IClassificationTag>> intermediateContainer, ITextSnapshot sourceSnapshot, ITextSnapshot currentSnapshot, List<IToken> tokens, IClassificationType classificationType)
        {
            IClassificationTag tag = new ClassificationTag(classificationType);
            foreach (IToken token in tokens)
            {
                int startIndex = token.StartIndex;
                int stopIndex = token.StopIndex;
                if (startIndex < 0 || stopIndex < 0 || (startIndex > stopIndex + 1))
                {
                    continue;
                }

                ITrackingSpan trackingRegion = sourceSnapshot.CreateTrackingSpan(Span.FromBounds(startIndex, stopIndex + 1), SpanTrackingMode.EdgePositive);
                intermediateContainer.Add(new KeyValuePair<ITrackingSpan, IClassificationTag>(trackingRegion, tag));
            }
        }

        public class SemanticAnalyzerListener : TemplateParserBaseListener
        {
            private readonly List<int> memberContext = new List<int>();
            private readonly List<ISet<string>> parameters = new List<ISet<string>>();

            private readonly List<IToken> templateDefinitions = new List<IToken>();
            private readonly List<IToken> templateReferences = new List<IToken>();
            private readonly List<IToken> regionDefinitions = new List<IToken>();
            private readonly List<IToken> regionReferences = new List<IToken>();
            private readonly List<IToken> dictionaryDefinitions = new List<IToken>();
            private readonly List<IToken> dictionaryReferences = new List<IToken>();
            private readonly List<IToken> parameterDefinitions = new List<IToken>();
            private readonly List<IToken> parameterReferences = new List<IToken>();
            private readonly List<IToken> attributeReferences = new List<IToken>();
            private readonly List<IToken> options = new List<IToken>();

            public List<IToken> TemplateDefinitions =>
                templateDefinitions;

            public List<IToken> TemplateReferences =>
                templateReferences;

            public List<IToken> RegionDefinitions =>
                regionDefinitions;

            public List<IToken> RegionReferences =>
                regionReferences;

            public List<IToken> DictionaryDefinitions =>
                dictionaryDefinitions;

            public List<IToken> DictionaryReferences =>
                dictionaryReferences;

            public List<IToken> ParameterDefinitions =>
                parameterDefinitions;

            public List<IToken> ParameterReferences =>
                parameterReferences;

            public List<IToken> AttributeReferences =>
                attributeReferences;

            public List<IToken> Options =>
                options;

            [RuleDependency(typeof(TemplateParser), TemplateParser.RULE_dictDef, 0, Dependents.Parents)]
            public override void EnterDictDef(TemplateParser.DictDefContext ctx)
            {
                if (ctx.name != null)
                {
                    dictionaryDefinitions.Add(ctx.name);
                }
            }

            [RuleDependency(typeof(TemplateParser), TemplateParser.RULE_templateDef, 3, Dependents.Parents)]
            public override void EnterTemplateDef(TemplateParser.TemplateDefContext ctx)
            {
                parameters.Add(new HashSet<String>());

                if (ctx.name != null)
                {
                    if (ctx.enclosing != null)
                    {
                        regionDefinitions.Add(ctx.name);
                        templateReferences.Add(ctx.enclosing);
                    }
                    else
                    {
                        templateDefinitions.Add(ctx.name);
                    }
                }

                if (ctx.alias != null)
                {
                    templateDefinitions.Add(ctx.alias);
                }

                if (ctx.target != null)
                {
                    templateReferences.Add(ctx.target);
                }
            }

            [RuleDependency(typeof(TemplateParser), TemplateParser.RULE_templateDef, 3, Dependents.Parents)]
            public override void ExitTemplateDef(TemplateParser.TemplateDefContext ctx)
            {
                parameters.RemoveAt(parameters.Count - 1);
            }

            [RuleDependency(typeof(TemplateParser), TemplateParser.RULE_formalArg, 1, Dependents.Parents)]
            public override void EnterFormalArg(TemplateParser.FormalArgContext ctx)
            {
                if (ctx.name != null)
                {
                    parameterDefinitions.Add(ctx.name);

                    ISet<string> currentParameters = parameters[parameters.Count - 1];
                    currentParameters.Add(ctx.name.Text);
                }
            }

            [RuleDependency(typeof(TemplateParser), TemplateParser.RULE_anonymousTemplate, 2, Dependents.Parents)]
            public override void EnterAnonymousTemplate(TemplateParser.AnonymousTemplateContext ctx)
            {
                parameters.Add(new HashSet<string>());
            }

            [RuleDependency(typeof(TemplateParser), TemplateParser.RULE_anonymousTemplate, 2, Dependents.Parents)]
            public override void ExitAnonymousTemplate(TemplateParser.AnonymousTemplateContext ctx)
            {
                parameters.RemoveAt(parameters.Count - 1);
            }

            [RuleDependency(typeof(TemplateParser), TemplateParser.RULE_anonymousTemplateParameters, 0, Dependents.Parents)]
            public override void EnterAnonymousTemplateParameters(TemplateParser.AnonymousTemplateParametersContext ctx)
            {
                if (ctx._names != null)
                {
                    parameterDefinitions.AddRange(ctx._names);

                    ISet<string> currentParameters = parameters[parameters.Count - 1];
                    foreach (IToken token in ctx._names)
                    {
                        currentParameters.Add(token.Text);
                    }
                }
            }

            [RuleDependency(typeof(TemplateParser), TemplateParser.RULE_option, 0, Dependents.Parents)]
            public override void EnterOption(TemplateParser.OptionContext ctx)
            {
                if (ctx.name != null)
                {
                    options.Add(ctx.name);
                }
            }

            [RuleDependency(typeof(TemplateParser), TemplateParser.RULE_includeExpr, 0, Dependents.Parents)]
            public override void EnterIncludeExpr(TemplateParser.IncludeExprContext ctx)
            {
                if (ctx.templateName != null)
                {
                    if (ctx.AT() != null)
                    {
                        regionReferences.Add(ctx.templateName);
                    }
                    else
                    {
                        templateReferences.Add(ctx.templateName);
                    }
                }

                if (ctx.regionName != null)
                {
                    regionReferences.Add(ctx.regionName);
                }
            }

            [RuleDependency(typeof(TemplateParser), TemplateParser.RULE_primary, 1, Dependents.Parents)]
            public override void EnterPrimary(TemplateParser.PrimaryContext ctx)
            {
                ITerminalNode id = ctx.ID();
                if (id != null)
                {
                    ISet<String> currentParameters = parameters.Count == 0 ? null : parameters[parameters.Count - 1];
                    if (currentParameters != null && currentParameters.Contains(id.Symbol.Text))
                    {
                        parameterReferences.Add(id.Symbol);
                    }
                    else
                    {
                        attributeReferences.Add(id.Symbol);
                    }
                }
            }

            //[RuleDependency(typeof(TemplateParser), TemplateParser.RULE_group, 0)]
            public override void ExitGroup(TemplateParser.GroupContext ctx)
            {
            }
        }
    }
}
