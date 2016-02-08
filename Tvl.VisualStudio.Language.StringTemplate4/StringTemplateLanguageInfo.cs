namespace Tvl.VisualStudio.Language.StringTemplate4
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Tvl.VisualStudio.Text;

    [Guid(StringTemplateConstants.StringTemplateLanguageGuidString)]
    internal class StringTemplateLanguageInfo : LanguageInfo
    {
        public StringTemplateLanguageInfo(SVsServiceProvider serviceProvider)
            : base(serviceProvider, typeof(StringTemplateLanguageInfo).GUID)
        {
        }

        public override string LanguageName
        {
            get
            {
                return StringTemplateConstants.StringTemplateLanguageName;
            }
        }

        public override IEnumerable<string> FileExtensions
        {
            get
            {
                yield return StringTemplateConstants.StringTemplateGroupFileExtension;
                yield return StringTemplateConstants.StringTemplateGroup4FileExtension;
                yield return StringTemplateConstants.StringTemplateTemplateFileExtension;
                yield return StringTemplateConstants.StringTemplateTemplate4FileExtension;
            }
        }
    }
}
