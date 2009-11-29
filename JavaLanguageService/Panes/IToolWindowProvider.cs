namespace JavaLanguageService.Panes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IToolWindowProvider
    {
        IToolWindow CreateWindow();
    }
}
