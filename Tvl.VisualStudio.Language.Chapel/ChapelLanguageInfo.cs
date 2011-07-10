namespace Tvl.VisualStudio.Language.Chapel
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Tvl.VisualStudio.Text;

    [Guid(ChapelConstants.ChapelLanguageGuidString)]
    internal class ChapelLanguageInfo : LanguageInfo
    {
        public ChapelLanguageInfo(SVsServiceProvider serviceProvider)
            : base(serviceProvider, typeof(ChapelLanguageInfo).GUID)
        {
        }

        public override string LanguageName
        {
            get
            {
                return ChapelConstants.ChapelLanguageName;
            }
        }

        public override IEnumerable<string> FileExtensions
        {
            get
            {
                yield return ChapelConstants.ChapelFileExtension;
            }
        }
    }
}
