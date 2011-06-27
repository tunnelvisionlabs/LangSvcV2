namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using Antlr.Runtime;
    using Antlr.Runtime.Tree;
    using Antlr4.StringTemplate.Compiler;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Text;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Shell.OutputWindow;

    public class StringTemplateBackgroundParser : BackgroundParser
    {
        public StringTemplateBackgroundParser(ITextBuffer textBuffer, IOutputWindowService outputWindowService)
            : base(textBuffer)
        {
            this.OutputWindowService = outputWindowService;
        }

        public IOutputWindowService OutputWindowService
        {
            get;
            private set;
        }

        protected override void ReParseImpl()
        {
            var outputWindow = OutputWindowService.TryGetPane(StringTemplateConstants.AntlrIntellisenseOutputWindow);
            try
            {
                var snapshot = TextBuffer.CurrentSnapshot;
                SnapshotCharStream input = new SnapshotCharStream(snapshot);
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
                    };

                TemplateGroupWrapper group = new TemplateGroupWrapper('<', '>');
                parser.group(group, "/");
                TemplateGroupRuleReturnScope returnScope = BuiltAstForGroupTemplates(group);
                OnParseComplete(new AntlrParseResultEventArgs(snapshot, errors, tokens.GetTokens(), returnScope));
            }
            catch (Exception e)
            {
                if (ErrorHandler.IsCriticalException(e))
                    throw;

                try
                {
                    if (outputWindow != null)
                        outputWindow.WriteLine(e.Message);
                }
                catch (Exception ex2)
                {
                    if (ErrorHandler.IsCriticalException(ex2))
                        throw;
                }
            }
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

            public TemplateGroupRuleReturnScope(TemplateGroupWrapper group, CommonTree tree)
            {
                Contract.Requires<ArgumentNullException>(group != null, "group");
                Contract.Requires<ArgumentNullException>(tree != null, "tree");

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
