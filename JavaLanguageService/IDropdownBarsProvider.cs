using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace JavaLanguageService
{
    public interface IDropdownBarsProvider
    {
        IDropdownBars GetDropdownBars(ITextView textView);
    }
}
