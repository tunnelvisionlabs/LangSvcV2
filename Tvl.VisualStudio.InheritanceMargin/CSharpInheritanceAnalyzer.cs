namespace Tvl.VisualStudio.InheritanceMargin
{
    using StringBuilder = System.Text.StringBuilder;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using Microsoft.RestrictedUsage.CSharp.Compiler;
    using Microsoft.RestrictedUsage.CSharp.Compiler.IDE;
    using Microsoft.RestrictedUsage.CSharp.Core;
    using Microsoft.RestrictedUsage.CSharp.Extensions;
    using Microsoft.RestrictedUsage.CSharp.Semantics;
    using Microsoft.RestrictedUsage.CSharp.Syntax;
    using Microsoft.VisualStudio.CSharp.Services.Language.Refactoring;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Shell.OutputWindow.Interfaces;

    using ICSharpTextBuffer = Microsoft.VisualStudio.CSharp.Services.Language.Interop.ICSharpTextBuffer;
    using ILangService = Microsoft.VisualStudio.CSharp.Services.Language.Interop.ILangService;
    using IProject = Microsoft.VisualStudio.CSharp.Services.Language.Interop.IProject;
    using Stopwatch = System.Diagnostics.Stopwatch;

    public class CSharpInheritanceAnalyzer : BackgroundParser
    {
        private static readonly ParseErrorEventArgs[] NoErrors = new ParseErrorEventArgs[0];

        private readonly SVsServiceProvider _serviceProvider;

        public CSharpInheritanceAnalyzer(ITextBuffer textBuffer, TaskScheduler taskScheduler, ITextDocumentFactoryService textDocumentFactoryService, IOutputWindowService outputWindowService, SVsServiceProvider serviceProvider)
            : base(textBuffer, taskScheduler, textDocumentFactoryService, outputWindowService)
        {
            Contract.Requires<ArgumentNullException>(serviceProvider != null, "serviceProvider");

            _serviceProvider = serviceProvider;
        }

        public override string Name
        {
            get
            {
                return "C# Inheritance Analyzer";
            }
        }

        [DllImport("CSLangSvc.dll", PreserveSig = false)]
        internal static extern void LangService_GetInstance(out ILangService langService);

        protected override void ReParseImpl()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            ITextSnapshot snapshot = TextBuffer.CurrentSnapshot;

            try
            {
                ITextDocument textDocument;
                if (!TextDocumentFactoryService.TryGetTextDocument(TextBuffer, out textDocument))
                    textDocument = null;

                string fileName = textDocument != null ? textDocument.FilePath : null;
                IDECompilerHost host = new IDECompilerHost();
                IProject project = null;

                ILangService languageService;
                LangService_GetInstance(out languageService);
                if (languageService != null)
                {
                    ICSharpTextBuffer csharpBuffer = languageService.FindTextBuffer(fileName);
                    if (csharpBuffer != null)
                        project = csharpBuffer.Project;
                }

                List<ITagSpan<InheritanceTag>> tags = new List<ITagSpan<InheritanceTag>>();

                if (host != null && project != null && !string.IsNullOrEmpty(fileName))
                {
                    Compilation compilation = host.CreateCompiler(project).GetCompilation();
                    SourceFile sourceFile = compilation.SourceFiles[new FileName(fileName)];
                    ParseTree parseTree = sourceFile.GetParseTree();

                    SpecializedMatchingMemberCollector collector = new SpecializedMatchingMemberCollector(host.Compilers.Select(i => i.GetCompilation()), false);

                    IEnumerable<ParseTreeNode> nodes = parseTree.SelectMethodsPropertiesAndFields();
                    foreach (var node in nodes)
                    {
                        CSharpMember member = null;

                        AccessorDeclarationNode accessorNode = node as AccessorDeclarationNode;
                        if (accessorNode != null)
                        {
                            continue;
                        }

                        FieldDeclarationNode fieldNode = node as FieldDeclarationNode;
                        if (fieldNode != null)
                        {
                            continue;
                        }

                        member = compilation.GetMemberFromMemberDeclaration(node);
                        if (member == null)
                        {
                            MarkDirty(true);
                            return;
                        }

                        if (!SpecializedMatchingMemberCollector.IsSupportedMemberType(member))
                            continue;

                        // methods which this method implements
                        ISet<CSharpMemberIdentifier> implementedMethods = collector.GetImplementedInterfaceMembers(member.SymbolicIdentifier);
                        // methods which this method overrides
                        ISet<CSharpMemberIdentifier> overriddenMethods = collector.GetOverriddenBaseMembers(member.SymbolicIdentifier);
                        // methods which override this method
                        ISet<CSharpMemberIdentifier> overridingMethods = collector.GetOverridersFromDerivedTypes(member.SymbolicIdentifier);
                        // methods which implement this method
                        ISet<CSharpMemberIdentifier> implementingMethods = collector.GetImplementorsForInterfaceMember(member.SymbolicIdentifier);

                        if (implementingMethods.Count == 0 && implementedMethods.Count == 0 && overriddenMethods.Count == 0 && overridingMethods.Count == 0)
                            continue;

                        StringBuilder builder = new StringBuilder();
                        string elementKindDisplayName =
                            member.IsProperty ? "properties" :
                            member.IsEvent ? "events" :
                            "methods";

                        if (implementedMethods.Count > 0)
                        {
                            builder.AppendLine("Implemented " + elementKindDisplayName + ":");
                            foreach (var methodId in implementedMethods)
                                builder.AppendLine("    " + methodId.ToString());
                        }

                        if (overriddenMethods.Count > 0)
                        {
                            builder.AppendLine("Overridden " + elementKindDisplayName + ":");
                            foreach (var methodId in overriddenMethods)
                                builder.AppendLine("    " + methodId.ToString());
                        }

                        if (implementingMethods.Count > 0)
                        {
                            builder.AppendLine("Implementing " + elementKindDisplayName + " in derived types:");
                            foreach (var methodId in implementingMethods)
                                builder.AppendLine("    " + methodId.ToString());
                        }

                        if (overridingMethods.Count > 0)
                        {
                            builder.AppendLine("Overriding " + elementKindDisplayName + " in derived types:");
                            foreach (var methodId in overridingMethods)
                                builder.AppendLine("    " + methodId.ToString());
                        }

                        int nameIndex = node.Token;
                        Token token = parseTree.LexData.Tokens[nameIndex];
                        ITextSnapshotLine line = snapshot.GetLineFromLineNumber(token.StartPosition.Line);
                        SnapshotSpan span = new SnapshotSpan(snapshot, new Span(line.Start + token.StartPosition.Character, token.EndPosition.Character - token.StartPosition.Character));

                        InheritanceGlyph tag;
                        if (implementedMethods.Count > 0)
                        {
                            if (overridingMethods.Count > 0)
                                tag = InheritanceGlyph.ImplementsAndOverridden;
                            else if (implementingMethods.Count > 0)
                                tag = InheritanceGlyph.ImplementsAndHasImplementations;
                            else
                                tag = InheritanceGlyph.Implements;
                        }
                        else if (implementingMethods.Count > 0)
                        {
                            tag = InheritanceGlyph.HasImplementations;
                        }
                        else if (overriddenMethods.Count > 0)
                        {
                            if (overridingMethods.Count > 0)
                                tag = InheritanceGlyph.OverridesAndOverridden;
                            else
                                tag = InheritanceGlyph.Overrides;
                        }
                        else
                        {
                            tag = InheritanceGlyph.Overridden;
                        }

                        tags.Add(new TagSpan<InheritanceTag>(span, new InheritanceTag(tag, builder.ToString().TrimEnd())));
                    }
                }

                InheritanceParseResultEventArgs result = new InheritanceParseResultEventArgs(snapshot, NoErrors, stopwatch.Elapsed, tags);
                OnParseComplete(result);
            }
            catch (InvalidOperationException ex)
            {
                base.MarkDirty(true);
                ex.PreserveStackTrace();
                throw;
            }
        }

        private IEnumerable<CSharpMember> FindHideBySigMethod(CSharpType type, CSharpMember member, bool checkBaseTypes, bool includeInheritedInterfaces)
        {
            List<CSharpMember> results = new List<CSharpMember>();
            FindHideBySigMethod(results, type, member, checkBaseTypes, includeInheritedInterfaces);
            return results;
        }

        private void FindHideBySigMethod(List<CSharpMember> results, CSharpType type, CSharpMember member, bool checkBaseTypes, bool includeInheritedInterfaces)
        {
            foreach (var potential in type.Members)
            {
                if (!member.IsSameSignature(potential))
                    continue;

                results.Add(potential);
            }

            if (checkBaseTypes)
            {
                CSharpType baseType = type.BaseClass;
                if (baseType != null)
                    FindHideBySigMethod(results, baseType, member, checkBaseTypes, includeInheritedInterfaces);
            }

            if (includeInheritedInterfaces)
            {
                IList<CSharpType> interfaces = type.BaseInterfaces;
                if (interfaces != null)
                {
                    foreach (CSharpType interfaceType in interfaces.OfType<CSharpType>())
                        FindHideBySigMethod(results, interfaceType, member, checkBaseTypes, includeInheritedInterfaces);
                }
            }
        }

        private List<TypeDeclarationNode> GetTypeDeclarationNodes(ParseTree parseTree)
        {
            TypeDeclarationNodeSelector collector = new TypeDeclarationNodeSelector();
            collector.Visit(parseTree.RootNode);
            return collector.Nodes;
        }

        private class TypeDeclarationNodeSelector : ParseTreeVisitor
        {
            private readonly List<TypeDeclarationNode> _nodes = new List<TypeDeclarationNode>();

            public List<TypeDeclarationNode> Nodes
            {
                get
                {
                    return _nodes;
                }
            }

            public override bool TraverseInteriorTree(ParseTreeNode node)
            {
                return false;
            }

            public override void VisitTypeDeclarationNode(TypeDeclarationNode node)
            {
                _nodes.Add(node);
                base.VisitTypeDeclarationNode(node);
            }
        }

        private class SpecializedMatchingMemberCollector : MatchingMemberCollector
        {
            public SpecializedMatchingMemberCollector(IEnumerable<Compilation> allProjects, bool searchOverloads)
                : base(allProjects, searchOverloads)
            {
            }

            public static new bool IsSupportedMemberType(CSharpMember member)
            {
                return MatchingMemberCollector.IsSupportedMemberType(member);
            }

            public ISet<CSharpMemberIdentifier> GetImplementedInterfaceMembers(CSharpMemberIdentifier memberId)
            {
                CSharpMember member = ResolveMemberIdentifier(AllProjects, memberId).Item1;
                ISet<CSharpMemberIdentifier> resultList = CreateHashSet<CSharpMemberIdentifier>(new CSharpMemberIdentifierEqualityComparer());
                if (!IsSupportedMemberType(member))
                    return resultList;

                AddImplementedInterfaceMembers(member, resultList);
                return resultList;
            }

            public ISet<CSharpMemberIdentifier> GetImplementorsForInterfaceMember(CSharpMemberIdentifier memberId)
            {
                CSharpMember member = ResolveMemberIdentifier(AllProjects, memberId).Item1;
                ISet<CSharpMemberIdentifier> resultList = CreateHashSet<CSharpMemberIdentifier>(new CSharpMemberIdentifierEqualityComparer());
                if (!IsSupportedMemberType(member))
                    return resultList;

                AddImplementorsForInterfaceMember(member, resultList);
                return resultList;
            }

            public ISet<CSharpMemberIdentifier> GetOverriddenBaseMembers(CSharpMemberIdentifier memberId)
            {
                CSharpMember member = ResolveMemberIdentifier(AllProjects, memberId).Item1;
                ISet<CSharpMemberIdentifier> resultList = CreateHashSet<CSharpMemberIdentifier>(new CSharpMemberIdentifierEqualityComparer());
                if (!IsSupportedMemberType(member))
                    return resultList;

                AddOverridenBaseMembers(member, resultList);
                return resultList;
            }

            public ISet<CSharpMemberIdentifier> GetOverridersFromDerivedTypes(CSharpMemberIdentifier memberId)
            {
                CSharpMember member = ResolveMemberIdentifier(AllProjects, memberId).Item1;
                ISet<CSharpMemberIdentifier> resultList = CreateHashSet<CSharpMemberIdentifier>(new CSharpMemberIdentifierEqualityComparer());
                if (!IsSupportedMemberType(member))
                    return resultList;

                AddVirtualOverridersFromDerivedTypes(member, resultList);
                return resultList;
            }

            private static Tuple<CSharpMember, Compilation> ResolveMemberIdentifier(IEnumerable<Compilation> projectList, CSharpMemberIdentifier memberId)
            {
                return Resolve(projectList, project => project.ResolveMemberIdentifier(memberId), memberId.AssemblyIdentifier);
            }

            private static Tuple<T, Compilation> Resolve<T>(IEnumerable<Compilation> compilations, Func<Compilation, T> resolver, CSharpAssemblyIdentifier definingAssembly)
                where T : class
            {
                Compilation compilation = null;
                T result = default(T);
                foreach (Compilation c in compilations)
                {
                    T t = resolver(c);
                    if (t != null)
                    {
                        result = t;
                        compilation = c;
                    }

                    if (c.MainAssembly.SymbolicIdentifier.Equals(definingAssembly))
                    {
                        return Tuple.Create(t, c);
                    }
                }

                if (compilation != null && result != null)
                {
                    return Tuple.Create(result, compilation);
                }

                return null;
            }

            private class CSharpMemberIdentifierEqualityComparer : IEqualityComparer<CSharpMemberIdentifier>
            {
                public bool Equals(CSharpMemberIdentifier left, CSharpMemberIdentifier right)
                {
                    if (object.ReferenceEquals(left, right))
                        return true;

                    if (object.ReferenceEquals(left, null) || object.ReferenceEquals(right, null))
                        return false;

                    return left.Equals(right);
                }

                public int GetHashCode(CSharpMemberIdentifier identifier)
                {
                    return identifier.GetHashCode();
                }
            }
        }
    }
}
