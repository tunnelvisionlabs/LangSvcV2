namespace JavaLanguageService.ShellServices
{
    using System;
    using Microsoft.VisualStudio.Text.Editor;

    public class ViewChangedEventArgs : EventArgs
    {
        public ViewChangedEventArgs(ITextView oldView, ITextView newView)
        {
            this.OldView = oldView;
            this.NewView = newView;
        }

        public ITextView OldView
        {
            get;
            private set;
        }

        public ITextView NewView
        {
            get;
            private set;
        }
    }
}
