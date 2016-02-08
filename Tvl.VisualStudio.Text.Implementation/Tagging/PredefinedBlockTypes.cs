namespace Tvl.VisualStudio.Text.Tagging.Implementation
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Utilities;

    internal static class PredefinedBlockTypes
    {
        [Export]
        [Name(PredefinedBlockTypeNames.Root)]
        public static BlockTypeDefinition RootBlockType
        {
            get;
            private set;
        }

        [Export]
        [Name(PredefinedBlockTypeNames.Loop)]
        public static BlockTypeDefinition LoopBlockType
        {
            get;
            private set;
        }

        [Export]
        [Name(PredefinedBlockTypeNames.Conditional)]
        public static BlockTypeDefinition ConditionalBlockType
        {
            get;
            private set;
        }

        [Export]
        [Name(PredefinedBlockTypeNames.Method)]
        public static BlockTypeDefinition MethodBlockType
        {
            get;
            private set;
        }

        [Export]
        [Name(PredefinedBlockTypeNames.Class)]
        public static BlockTypeDefinition ClassBlockType
        {
            get;
            private set;
        }

        [Export]
        [Name(PredefinedBlockTypeNames.Namespace)]
        public static BlockTypeDefinition NamespaceBlockType
        {
            get;
            private set;
        }
    }
}
