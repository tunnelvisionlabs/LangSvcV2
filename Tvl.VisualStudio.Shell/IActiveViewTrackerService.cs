namespace Tvl.VisualStudio.Shell
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text.Editor;

    public interface IActiveViewTrackerService
    {
        event EventHandler<ViewChangedEventArgs> ViewChanged;
        //event EventHandler<ViewChangedEventArgs> ViewWithMouseChanged;

        ITextView ActiveView
        {
            get;
        }

        //ITextView ViewWithMouse
        //{
        //    get;
        //}
    }
}
