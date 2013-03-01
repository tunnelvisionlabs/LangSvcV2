namespace Tvl.VisualStudio.Language.AntlrV4
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Shell;
    using Tvl.VisualStudio.Text;

    [Guid(Antlr4Constants.Antlr4LanguageGuidString)]
    internal class Antlr4LanguageInfo : LanguageInfo
    {
        public Antlr4LanguageInfo(SVsServiceProvider serviceProvider)
            : base(serviceProvider, typeof(Antlr4LanguageInfo).GUID)
        {
        }

        public override string LanguageName
        {
            get
            {
                return Antlr4Constants.AntlrLanguageName;
            }
        }

        public override IEnumerable<string> FileExtensions
        {
            get
            {
                yield return Antlr4Constants.AntlrFileExtension;
            }
        }
    }
}
