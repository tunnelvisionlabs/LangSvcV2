namespace Tvl.VisualStudio.Language.Alloy
{
    using Microsoft.VisualStudio.Text.Tagging;

    internal class AlloyIntellisenseTag : ITag
    {
        public AlloyIntellisenseTag(string name, AlloyIntellisenseTagType type)
        {
            Name = name;
            Type = type;
        }

        public string Name
        {
            get;
            private set;
        }

        public AlloyIntellisenseTagType Type
        {
            get;
            private set;
        }
    }
}
