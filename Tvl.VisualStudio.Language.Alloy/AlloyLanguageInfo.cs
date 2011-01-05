namespace Tvl.VisualStudio.Language.Alloy
{
    using System;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Tvl.VisualStudio.Text;

    [Guid(AlloyConstants.AlloyLanguageGuidString)]
    internal class AlloyLanguageInfo : LanguageInfo
    {
        public AlloyLanguageInfo(SVsServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        public override LanguagePreferences LanguagePreferences
        {
            get
            {
                return ComponentModel.GetService<AlloyLanguagePackage>().LanguagePreferences;
            }
        }

        public override string LanguageName
        {
            get
            {
                return AlloyConstants.AlloyLanguageName;
            }
        }

        public override int GetFileExtensions(out string pbstrExtensions)
        {
            pbstrExtensions = AlloyConstants.AlloyFileExtension;
            return VSConstants.S_OK;
        }
    }
}
