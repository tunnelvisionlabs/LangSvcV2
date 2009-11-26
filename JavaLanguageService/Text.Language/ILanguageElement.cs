namespace JavaLanguageService.Text.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text;

    public interface ILanguageElement
    {
        ITrackingSpan Extent
        {
            get;
        }
    }
}
