#if !ROSLYN

namespace Tvl.VisualStudio.InheritanceMargin.CSharp
{
    using System.Collections.Generic;
    using Microsoft.RestrictedUsage.CSharp.Syntax;
    using IEnumerable = System.Collections.IEnumerable;
    using IEnumerator = System.Collections.IEnumerator;

    internal class TypeCollector : ParseTreeVisitor, IEnumerable<ParseTreeNode>
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

        // Types cannot be declared in an accessor, so we stop here
        public override void VisitAccessorDeclarationNode(AccessorDeclarationNode node)
        {
        }

        // Types cannot be declared in a constructor, so we stop here
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
}

#endif
