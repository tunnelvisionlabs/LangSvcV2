namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Tvl.VisualStudio.Text;

    [Guid(AlloyConstants.AlloyLanguageGuidString)]
    internal class AlloyLanguageInfo : LanguageInfo
    {
        public AlloyLanguageInfo(SVsServiceProvider serviceProvider)
            : base(serviceProvider, typeof(AlloyLanguageInfo).GUID)
        {
        }

        public override string LanguageName
        {
            get
            {
                return AlloyConstants.AlloyLanguageName;
            }
        }

        public override IEnumerable<string> FileExtensions
        {
            get
            {
                yield return AlloyConstants.AlloyFileExtension;
            }
        }
    }
}
