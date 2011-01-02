namespace Tvl.VisualStudio.Language.Alloy
{
    using System.Collections.Generic;

    partial class AlloyIntellisenseCache
    {
        private sealed class UniqueFileComparer : EqualityComparer<AlloyFileReference>
        {
            private static readonly UniqueFileComparer _default = new UniqueFileComparer();

            public static new UniqueFileComparer Default
            {
                get
                {
                    return _default;
                }
            }

            public override bool Equals(AlloyFileReference x, AlloyFileReference y)
            {
                return x.Equals(y);
            }

            public override int GetHashCode(AlloyFileReference obj)
            {
                return obj.GetHashCode();
            }
        }
    }
}
