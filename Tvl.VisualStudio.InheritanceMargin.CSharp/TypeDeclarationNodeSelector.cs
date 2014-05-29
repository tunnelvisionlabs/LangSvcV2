#if !ROSLYN

namespace Tvl.VisualStudio.InheritanceMargin.CSharp
{
    using System.Collections.Generic;
    using Microsoft.RestrictedUsage.CSharp.Syntax;

    internal class TypeDeclarationNodeSelector : ParseTreeVisitor
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
}

#endif
