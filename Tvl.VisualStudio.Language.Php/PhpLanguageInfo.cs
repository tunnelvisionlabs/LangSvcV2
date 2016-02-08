namespace Tvl.VisualStudio.Language.Php
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Tvl.VisualStudio.Text;

    [Guid(PhpConstants.PhpLanguageGuidString)]
    internal class PhpLanguageInfo : LanguageInfo
    {
        public PhpLanguageInfo(SVsServiceProvider serviceProvider)
            : base(serviceProvider, typeof(PhpLanguageInfo).GUID)
        {
        }

        public override string LanguageName
        {
            get
            {
                return PhpConstants.PhpLanguageName;
            }
        }

        public override IEnumerable<string> FileExtensions
        {
            get
            {
                yield return PhpConstants.PhpFileExtension;
                yield return PhpConstants.Php5FileExtension;
            }
        }
    }
}
