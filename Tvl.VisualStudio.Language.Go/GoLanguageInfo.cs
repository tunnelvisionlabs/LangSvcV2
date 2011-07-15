namespace Tvl.VisualStudio.Language.Go
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Tvl.VisualStudio.Text;

    [Guid(GoConstants.GoLanguageGuidString)]
    internal class GoLanguageInfo : LanguageInfo
    {
        public GoLanguageInfo(SVsServiceProvider serviceProvider)
            : base(serviceProvider, typeof(GoLanguageInfo).GUID)
        {
        }

        public override string LanguageName
        {
            get
            {
                return GoConstants.GoLanguageName;
            }
        }

        public override IEnumerable<string> FileExtensions
        {
            get
            {
                yield return GoConstants.GoFileExtension;
            }
        }
    }
}
