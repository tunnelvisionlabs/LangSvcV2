namespace Tvl.VisualStudio.Language.Java
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Tvl.VisualStudio.Text;

    [Guid(Constants.JavaLanguageGuidString)]
    internal class JavaLanguageInfo : LanguageInfo
    {
        public JavaLanguageInfo(SVsServiceProvider serviceProvider)
            : base(serviceProvider, typeof(JavaLanguageInfo).GUID)
        {
        }

        public override string LanguageName
        {
            get
            {
                return Constants.JavaLanguageName;
            }
        }

        public override IEnumerable<string> FileExtensions
        {
            get
            {
                yield return Constants.JavaFileExtension;
            }
        }
    }
}
