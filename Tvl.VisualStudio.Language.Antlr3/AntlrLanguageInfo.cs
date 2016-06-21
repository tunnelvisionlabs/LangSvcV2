namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Tvl.VisualStudio.Text;

    [Guid(AntlrConstants.AntlrLanguageGuidString)]
    internal class AntlrLanguageInfo : LanguageInfo
    {
        public AntlrLanguageInfo(SVsServiceProvider serviceProvider)
            : base(serviceProvider, typeof(AntlrLanguageInfo).GUID)
        {
        }

        public override string LanguageName
        {
            get
            {
                return AntlrConstants.AntlrLanguageName;
            }
        }

        public override IEnumerable<string> FileExtensions
        {
            get
            {
                yield return AntlrConstants.AntlrFileExtension;
                yield return AntlrConstants.AntlrFileExtension2;
            }
        }
    }
}
