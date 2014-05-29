#if !ROSLYN

namespace Tvl.VisualStudio.InheritanceMargin.CSharp
{
    using System.Collections.Generic;
    using Microsoft.RestrictedUsage.CSharp.Compiler;
    using Microsoft.RestrictedUsage.CSharp.Semantics;
    using Microsoft.RestrictedUsage.CSharp.Utilities;
    using Microsoft.VisualStudio.CSharp.Services.Language.Refactoring;

    internal class SpecializedMatchingMemberCollector : MatchingMemberCollector
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
            CSharpMember member = CSharpInheritanceAnalyzer.ResolveMemberIdentifier(AllProjects, memberId).Item1;
            ISet<CSharpMemberIdentifier> resultList = CreateHashSet<CSharpMemberIdentifier>(new CSharpMemberIdentifierEqualityComparer());
            if (!IsSupportedMemberType(member))
                return resultList;

            AddImplementedInterfaceMembers(member, resultList);
            return resultList;
        }

        public ISet<CSharpMemberIdentifier> GetImplementorsForInterfaceMember(CSharpMemberIdentifier memberId)
        {
            CSharpMember member = CSharpInheritanceAnalyzer.ResolveMemberIdentifier(AllProjects, memberId).Item1;
            ISet<CSharpMemberIdentifier> resultList = CreateHashSet<CSharpMemberIdentifier>(new CSharpMemberIdentifierEqualityComparer());
            if (!IsSupportedMemberType(member))
                return resultList;

            AddImplementorsForInterfaceMember(member, resultList);
            return resultList;
        }

        public ISet<CSharpMemberIdentifier> GetOverriddenBaseMembers(CSharpMemberIdentifier memberId)
        {
            CSharpMember member = CSharpInheritanceAnalyzer.ResolveMemberIdentifier(AllProjects, memberId).Item1;
            ISet<CSharpMemberIdentifier> resultList = CreateHashSet<CSharpMemberIdentifier>(new CSharpMemberIdentifierEqualityComparer());
            if (!IsSupportedMemberType(member))
                return resultList;

            AddOverridenBaseMembers(member, resultList);
            return resultList;
        }

        public ISet<CSharpMemberIdentifier> GetOverridersFromDerivedTypes(CSharpMemberIdentifier memberId)
        {
            CSharpMember member = CSharpInheritanceAnalyzer.ResolveMemberIdentifier(AllProjects, memberId).Item1;
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
    }
}

#endif
