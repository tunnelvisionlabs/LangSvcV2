namespace JavaLanguageService.Text.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text;

    internal class LanguageElement : ILanguageElement
    {
        public ITrackingSpan Extent
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
