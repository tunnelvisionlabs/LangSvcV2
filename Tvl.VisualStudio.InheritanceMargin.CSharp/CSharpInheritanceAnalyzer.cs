namespace Tvl.VisualStudio.InheritanceMargin.CSharp
{
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
    using Microsoft.RestrictedUsage.CSharp.Utilities;
    using Microsoft.VisualStudio.CSharp.Services.Language.Refactoring;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl;
    using Tvl.VisualStudio.Language.Parsing;
    using Tvl.VisualStudio.Shell.OutputWindow.Interfaces;

    using CSRPOSDATA = Microsoft.VisualStudio.CSharp.Services.Language.Interop.CSRPOSDATA;
    using ICSharpTextBuffer = Microsoft.VisualStudio.CSharp.Services.Language.Interop.ICSharpTextBuffer;
    using IEnumerable = System.Collections.IEnumerable;
    using IEnumerator = System.Collections.IEnumerator;
    using ILangService = Microsoft.VisualStudio.CSharp.Services.Language.Interop.ILangService;
    using IProject = Microsoft.VisualStudio.CSharp.Services.Language.Interop.IProject;
    using Stopwatch = System.Diagnostics.Stopwatch;
    using StringBuilder = System.Text.StringBuilder;

    public class CSharpInheritanceAnalyzer : BackgroundParser
    {
        private static readonly ParseErrorEventArgs[] NoErrors = new ParseErrorEventArgs[0];

        private readonly SVsServiceProvider _serviceProvider;
        private readonly IInheritanceTagFactory _tagFactory;

        public CSharpInheritanceAnalyzer(ITextBuffer textBuffer, TaskScheduler taskScheduler, ITextDocumentFactoryService textDocumentFactoryService, IOutputWindowService outputWindowService, SVsServiceProvider serviceProvider, IInheritanceTagFactory tagFactory)
            : base(textBuffer, taskScheduler, textDocumentFactoryService, outputWindowService)
        {
            Contract.Requires<ArgumentNullException>(serviceProvider != null, "serviceProvider");

            _serviceProvider = serviceProvider;
            _tagFactory = tagFactory;
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

        public static Tuple<CSharpMember, Compilation> ResolveMemberIdentifier(IEnumerable<Compilation> projectList, CSharpMemberIdentifier memberId)
        {
            return Resolve(projectList, project => project.ResolveMemberIdentifier(memberId), memberId.AssemblyIdentifier);
        }

        public static Tuple<CSharpType, Compilation> ResolveTypeIdentifier(IEnumerable<Compilation> projectList, CSharpTypeIdentifier typeId)
        {
            return Resolve(projectList, project => project.ResolveTypeIdentifier(typeId), typeId.AssemblyIdentifier);
        }

        public static Tuple<T, Compilation> Resolve<T>(IEnumerable<Compilation> compilations, Func<Compilation, T> resolver, CSharpAssemblyIdentifier definingAssembly)
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

        public static void NavigateToType(CSharpTypeIdentifier typeIdentifier)
        {
            bool result = false;
            try
            {
                CSharpType currentType = ResolveType(typeIdentifier);
                if (currentType == null)
                    return;

                var sourceLocations = currentType.SourceLocations;
                if (sourceLocations == null || sourceLocations.Count == 0)
                    return;

                var location = sourceLocations[0];
                if (location.FileName == null || !location.Position.IsValid)
                    return;

                CSRPOSDATA position = new CSRPOSDATA()
                {
                    LineIndex = location.Position.Line,
                    ColumnIndex = location.Position.Character
                };

                ILangService languageService;
                CSharpInheritanceAnalyzer.LangService_GetInstance(out languageService);
                if (languageService == null)
                    return;

                IProject project = null;
                languageService.OpenSourceFile(project, location.FileName.Value, position);
            }
            catch (ApplicationException)
            {
                //_callHierarchy.LanguageService.DisplayErrorMessage(exception.Message);
                return;
            }
            catch (InvalidOperationException)
            {
                //this._callHierarchy.LanguageService.DisplayErrorMessage(exception2.Message);
                return;
            }

            if (!result)
            {
                //NativeMessageId.Create(this._callHierarchy.LanguageService, jrc_StringResource_identifiers.IDS_HIDDEN_DEFINITION, new object[0]).DisplayError(this._callHierarchy.LanguageService);
            }
        }

        public static void NavigateToMember(CSharpMemberIdentifier memberIdentifier)
        {
            bool result = false;
            try
            {
                CSharpMember currentMember = ResolveMember(memberIdentifier);
                if (currentMember == null)
                    return;

                var sourceLocations = currentMember.SourceLocations;
                if (sourceLocations == null || sourceLocations.Count == 0)
                    return;

                var location = sourceLocations[0];
                if (location.FileName == null || !location.Position.IsValid)
                    return;

                CSRPOSDATA position = new CSRPOSDATA()
                {
                    LineIndex = location.Position.Line,
                    ColumnIndex = location.Position.Character
                };

                ILangService languageService;
                CSharpInheritanceAnalyzer.LangService_GetInstance(out languageService);
                if (languageService == null)
                    return;

                IProject project = null;
                languageService.OpenSourceFile(project, location.FileName.Value, position);
            }
            catch (ApplicationException)
            {
                //_callHierarchy.LanguageService.DisplayErrorMessage(exception.Message);
                return;
            }
            catch (InvalidOperationException)
            {
                //this._callHierarchy.LanguageService.DisplayErrorMessage(exception2.Message);
                return;
            }

            if (!result)
            {
                //NativeMessageId.Create(this._callHierarchy.LanguageService, jrc_StringResource_identifiers.IDS_HIDDEN_DEFINITION, new object[0]).DisplayError(this._callHierarchy.LanguageService);
            }
        }

        private static CSharpType ResolveType(CSharpTypeIdentifier memberIdentifier)
        {
            IDECompilerHost host = new IDECompilerHost();
            var currentCompilations = host.Compilers.Select(i => i.GetCompilation()).ToList();
            var resolved = ResolveTypeIdentifier(currentCompilations, memberIdentifier);
            if (resolved != null)
                return resolved.Item1;

            return null;
        }

        private static CSharpMember ResolveMember(CSharpMemberIdentifier memberIdentifier)
        {
            IDECompilerHost host = new IDECompilerHost();
            var currentCompilations = host.Compilers.Select(i => i.GetCompilation()).ToList();
            var resolved = ResolveMemberIdentifier(currentCompilations, memberIdentifier);
            if (resolved != null)
                return resolved.Item1;

            return null;
        }

        protected override void ReParseImpl()
        {
            Stopwatch stopwatch = Stopwatch.StartNew();

            ITextSnapshot snapshot = TextBuffer.CurrentSnapshot;

            try
            {
                ITextDocument textDocument = TextDocument;
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

                List<ITagSpan<IInheritanceTag>> tags = new List<ITagSpan<IInheritanceTag>>();

                if (host != null && project != null && !string.IsNullOrEmpty(fileName))
                {
                    Compilation compilation = host.CreateCompiler(project).GetCompilation();
                    SourceFile sourceFile;
                    if (!compilation.SourceFiles.TryGetValue(new FileName(fileName), out sourceFile))
                    {
                        InheritanceParseResultEventArgs errorResult = new InheritanceParseResultEventArgs(snapshot, NoErrors, stopwatch.Elapsed, tags);
                        OnParseComplete(errorResult);
                        return;
                    }

                    ParseTree parseTree = sourceFile.GetParseTree();

                    SpecializedMatchingMemberCollector collector = new SpecializedMatchingMemberCollector(host.Compilers.Select(i => i.GetCompilation()), false);

                    IEnumerable<ParseTreeNode> nodes = SelectTypes(parseTree);
                    foreach (var node in nodes)
                    {
                        CSharpType type = null;

                        type = compilation.GetTypeFromTypeDeclaration(node);
                        if (type == null)
                        {
                            MarkDirty(true);
                            return;
                        }

                        if (type.IsSealed)
                            continue;

                        // types which implement or derive from this type
                        ISet<CSharpType> derivedClasses = collector.GetDerivedTypes(type.SymbolicIdentifier);

                        if (derivedClasses.Count == 0)
                            continue;

                        StringBuilder builder = new StringBuilder();
                        string elementKindDisplayName =
                            "types";

                        builder.AppendLine("Derived " + elementKindDisplayName + ":");
                        foreach (var derived in derivedClasses)
                            builder.AppendLine("    " + derived.GetFullTypeName());

                        int nameIndex = node.Token;
                        Token token = parseTree.LexData.Tokens[nameIndex];
                        ITextSnapshotLine line = snapshot.GetLineFromLineNumber(token.StartPosition.Line);
                        SnapshotSpan span = new SnapshotSpan(snapshot, new Span(line.Start + token.StartPosition.Character, token.EndPosition.Character - token.StartPosition.Character));

                        InheritanceGlyph tag = type.IsInterface ? InheritanceGlyph.HasImplementations : InheritanceGlyph.Overridden;

                        var targets = derivedClasses.Select(i => new TypeTarget(i.GetFullTypeName(), i.SymbolicIdentifier));
                        tags.Add(new TagSpan<IInheritanceTag>(span, _tagFactory.CreateTag(tag, builder.ToString().TrimEnd(), targets)));
                    }

                    nodes = parseTree.SelectMethodsPropertiesAndFields();
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

                        List<CSharpMemberIdentifier> members = new List<CSharpMemberIdentifier>();
                        members.AddRange(implementedMethods);
                        members.AddRange(overriddenMethods);
                        members.AddRange(implementingMethods);
                        members.AddRange(overridingMethods);

                        var targets = members.Select(i => new MemberTarget(i));
                        tags.Add(new TagSpan<IInheritanceTag>(span, _tagFactory.CreateTag(tag, builder.ToString().TrimEnd(), targets)));
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

        private IEnumerable<ParseTreeNode> SelectTypes(ParseTree parseTree)
        {
            if (parseTree == null)
                return Enumerable.Empty<ParseTreeNode>();

            return new TypeCollector(parseTree.RootNode);
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

            public ISet<CSharpType> GetDerivedTypes(CSharpTypeIdentifier typeId)
            {
                ISet<CSharpType> result = new HashSet<CSharpType>();
                foreach (Compilation compilation in AllProjects)
                {
                    CSharpType baseType = compilation.ResolveTypeIdentifier(typeId);
                    if (baseType == null)
                        continue;

                    BaseTypeCollector collector = baseType.IsInterface ? GetBaseInterfaceCollector() : GetBaseClassCollector();
                    foreach (CSharpType type in compilation.MainAssembly.Types)
                    {
                        // an interface can't be derived from a class
                        if (type.IsInterface && !baseType.IsInterface)
                            continue;

                        var baseTypes = collector.GetBaseTypes(type);
                        if (baseTypes.Contains(baseType))
                            result.Add(type);
                    }
                }

                return result;
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

        public class TypeCollector : ParseTreeVisitor, IEnumerable<ParseTreeNode>
        {
            private readonly List<ParseTreeNode> nodes = new List<ParseTreeNode>();

            public TypeCollector(ParseTreeNode node)
            {
                this.Visit(node);
            }

            public IEnumerator<ParseTreeNode> GetEnumerator()
            {
                return this.nodes.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            public override bool TraverseInteriorTree(ParseTreeNode node)
            {
                return false;
            }

            public override void VisitAccessorDeclarationNode(AccessorDeclarationNode node)
            {
            }

            public override void VisitConstructorDeclarationNode(ConstructorDeclarationNode node)
            {
            }

            public override void VisitDelegateDeclarationNode(DelegateDeclarationNode node)
            {
            }

            public override void VisitEnumMemberDeclarationNode(EnumMemberDeclarationNode node)
            {
            }

            public override void VisitFieldDeclarationNode(FieldDeclarationNode node)
            {
            }

            public override void VisitMemberDeclarationNode(MemberDeclarationNode node)
            {
            }

            public override void VisitMethodDeclarationNode(MethodDeclarationNode node)
            {
            }

            public override void VisitNamespaceDeclarationNode(NamespaceDeclarationNode node)
            {
                this.VisitList<ParseTreeNode>(node.NamespaceMemberDeclarations);
            }

            public override void VisitNestedTypeDeclarationNode(NestedTypeDeclarationNode node)
            {
                this.Visit(node.Type);
            }

            public override void VisitOperatorDeclarationNode(OperatorDeclarationNode node)
            {
            }

            public override void VisitParseTreeNode(ParseTreeNode node)
            {
            }

            public override void VisitPropertyDeclarationNode(PropertyDeclarationNode node)
            {
            }

            public override void VisitTypeDeclarationNode(TypeDeclarationNode node)
            {
                this.nodes.Add(node);

                for (MemberDeclarationNode node2 = node.MemberDeclarations; node2 != null; node2 = node2.Next)
                {
                    this.Visit(node2);
                }
            }
        }

        private sealed class TypeTarget : IInheritanceTarget
        {
            private readonly string _displayName;
            private readonly CSharpTypeIdentifier _typeIdentifier;

            public TypeTarget(string displayName, CSharpTypeIdentifier typeIdentifier)
            {
                _displayName = displayName;
                _typeIdentifier = typeIdentifier;
            }

            public string DisplayName
            {
                get
                {
                    return _displayName;
                }
            }

            public void NavigateTo()
            {
                NavigateToType(_typeIdentifier);
            }
        }

        private sealed class MemberTarget : IInheritanceTarget
        {
            private readonly CSharpMemberIdentifier _memberIdentifier;

            public MemberTarget(CSharpMemberIdentifier memberIdentifier)
            {
                _memberIdentifier = memberIdentifier;
            }

            public string DisplayName
            {
                get
                {
                    return _memberIdentifier.ToString();
                }
            }

            public void NavigateTo()
            {
                NavigateToMember(_memberIdentifier);
            }

        }
    }
}
