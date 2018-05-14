namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Antlr4.StringTemplate.Compiler;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.OutputWindow.Interfaces;
    using Stopwatch = System.Diagnostics.Stopwatch;

    public class StringTemplateBackgroundParser : BackgroundParser
    {
        public StringTemplateBackgroundParser([NotNull] ITextBuffer textBuffer, [NotNull] TaskScheduler taskScheduler, [NotNull] ITextDocumentFactoryService textDocumentFactoryService, [NotNull] IOutputWindowService outputWindowService)
            : base(textBuffer, taskScheduler, textDocumentFactoryService, outputWindowService)
        {
            Debug.Assert(textBuffer != null);
            Debug.Assert(taskScheduler != null);
            Debug.Assert(textDocumentFactoryService != null);
            Debug.Assert(outputWindowService != null);
        }

        protected override void ReParseImpl()
        {
            var outputWindow = OutputWindowService.TryGetPane(PredefinedOutputWindowPanes.TvlIntellisense);
            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                var snapshot = TextBuffer.CurrentSnapshot;
                SnapshotCharStream input = new SnapshotCharStream(snapshot, new Span(0, snapshot.Length));
                GroupLexer lexer = new GroupLexer(input);
                CommonTokenStream tokens = new CommonTokenStream(lexer);
                GroupParserWrapper parser = new GroupParserWrapper(tokens);
                List<ParseErrorEventArgs> errors = new List<ParseErrorEventArgs>();
                parser.ParseError += (sender, e) =>
                    {
                        errors.Add(e);

                        string message = e.Message;

                        ITextDocument document;
                        if (TextBuffer.Properties.TryGetProperty(typeof(ITextDocument), out document) && document != null)
                        {
                            string fileName = document.FilePath;
                            var line = snapshot.GetLineFromPosition(e.Span.Start);
                            message = string.Format("{0}({1},{2}): {3}", fileName, line.LineNumber + 1, e.Span.Start - line.Start.Position + 1, message);
                        }

                        if (message.Length > 100)
                            message = message.Substring(0, 100) + " ...";

                        if (outputWindow != null)
                            outputWindow.WriteLine(message);

                        if (errors.Count > 100)
                            throw new OperationCanceledException();
                    };

                TemplateGroupWrapper group = new TemplateGroupWrapper('<', '>');
                parser.group(group, "/");
                TemplateGroupRuleReturnScope returnScope = BuiltAstForGroupTemplates(group);

                // Also parse the input using the V4 lexer/parser for downstream operations that make use of it
                IList<Antlr4.Runtime.IToken> v4tokens;
                TemplateParser.GroupFileContext v4result = ParseWithAntlr4(snapshot, out v4tokens);

                OnParseComplete(new StringTemplateParseResultEventArgs(snapshot, errors, stopwatch.Elapsed, tokens.GetTokens(), returnScope, v4tokens, v4result));
            }
            catch (Exception e) when (!ErrorHandler.IsCriticalException(e))
            {
                try
                {
                    if (outputWindow != null)
                        outputWindow.WriteLine(e.Message);
                }
                catch (Exception ex2) when (!ErrorHandler.IsCriticalException(ex2))
                {
                }
            }
        }

        private TemplateParser.GroupFileContext ParseWithAntlr4(ITextSnapshot snapshot, out IList<Antlr4.Runtime.IToken> tokensList)
        {
            var input = new Parsing4.SnapshotCharStream(snapshot, new Span(0, snapshot.Length));
            var lexer = new TemplateLexer(input);
            var tokens = new Antlr4.Runtime.CommonTokenStream(lexer);
            var parser = new TemplateParser(tokens);
            TemplateParser.GroupFileContext parseResult;

            try
            {
                parser.Interpreter.PredictionMode = Antlr4.Runtime.Atn.PredictionMode.Sll;
                parser.RemoveErrorListeners();
                parser.BuildParseTree = true;
                parser.ErrorHandler = new Antlr4.Runtime.BailErrorStrategy();
                parseResult = parser.groupFile();
            }
            catch (Antlr4.Runtime.Misc.ParseCanceledException ex) when (ex.InnerException is Antlr4.Runtime.RecognitionException)
            {
                // retry with default error handler
                tokens.Reset();
                parser.Interpreter.PredictionMode = Antlr4.Runtime.Atn.PredictionMode.Ll;
                //parser.AddErrorListener(DescriptiveErrorListener.INSTANCE);
                parser.SetInputStream(tokens);
                parser.ErrorHandler = new Antlr4.Runtime.DefaultErrorStrategy();
                parseResult = parser.groupFile();
            }

            tokensList = tokens.GetTokens();
            return parseResult;
        }

        private TemplateGroupRuleReturnScope BuiltAstForGroupTemplates(TemplateGroupWrapper group)
        {
            ITreeAdaptor adaptor = new CommonTreeAdaptor();
            object tree = adaptor.Nil();
            foreach (var template in group.CompiledTemplates)
            {
                adaptor.AddChild(tree, template.Ast);
            }

            return new TemplateGroupRuleReturnScope(group, (CommonTree)tree);
        }

        internal class TemplateGroupRuleReturnScope : IAstRuleReturnScope<CommonTree>, IRuleReturnScope
        {
            private readonly TemplateGroupWrapper _group;
            private readonly CommonTree _tree;

            public TemplateGroupRuleReturnScope([NotNull] TemplateGroupWrapper group, [NotNull] CommonTree tree)
            {
                Requires.NotNull(group, nameof(group));
                Requires.NotNull(tree, nameof(tree));

                _group = group;
                _tree = tree;
            }

            public TemplateGroupWrapper Group
            {
                get
                {
                    return _group;
                }
            }

            public CommonTree Tree
            {
                get
                {
                    return _tree;
                }
            }

            object IRuleReturnScope.Start
            {
                get
                {
                    return null;
                }
            }

            object IRuleReturnScope.Stop
            {
                get
                {
                    return null;
                }
            }

            object IAstRuleReturnScope.Tree
            {
                get
                {
                    return _tree;
                }
            }
        }
    }
}
