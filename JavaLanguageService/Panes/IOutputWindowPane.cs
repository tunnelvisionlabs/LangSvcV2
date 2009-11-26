namespace JavaLanguageService.Panes
{
    using System;

    public interface IOutputWindowPane : IDisposable
    {
        string Name
        {
            get;
            set;
        }

        void Activate();
        void Hide();
        void Write(string text);
        void WriteLine(string text);
    }
}
