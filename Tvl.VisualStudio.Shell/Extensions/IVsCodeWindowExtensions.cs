namespace Tvl.VisualStudio.Shell
{
    using System;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.TextManager.Interop;

    public static class IVsCodeWindowExtensions
    {
        public static IVsTextLines GetBuffer([NotNull] this IVsCodeWindow codeWindow)
        {
            Requires.NotNull(codeWindow, nameof(codeWindow));

            IVsTextLines buffer;
            ErrorHandler.ThrowOnFailure(codeWindow.GetBuffer(out buffer));
            return buffer;
        }

        public static string GetEditorCaption([NotNull] this IVsCodeWindow codeWindow, READONLYSTATUS status)
        {
            Requires.NotNull(codeWindow, nameof(codeWindow));

            string caption;
            if (ErrorHandler.Failed(codeWindow.GetEditorCaption(status, out caption)))
                return null;

            return caption;
        }

        public static IVsTextView GetLastActiveView([NotNull] this IVsCodeWindow codeWindow)
        {
            Requires.NotNull(codeWindow, nameof(codeWindow));

            IVsTextView view;
            if (ErrorHandler.Failed(codeWindow.GetLastActiveView(out view)))
                return null;

            return view;
        }

        public static IVsTextView GetPrimaryView([NotNull] this IVsCodeWindow codeWindow)
        {
            Requires.NotNull(codeWindow, nameof(codeWindow));

            IVsTextView view;
            if (ErrorHandler.Failed(codeWindow.GetPrimaryView(out view)))
                return null;

            return view;
        }

        public static IVsTextView GetSecondaryView([NotNull] this IVsCodeWindow codeWindow)
        {
            Requires.NotNull(codeWindow, nameof(codeWindow));

            IVsTextView view;
            if (ErrorHandler.Failed(codeWindow.GetSecondaryView(out view)))
                return null;

            return view;
        }

        public static Guid GetViewClassID([NotNull] this IVsCodeWindow codeWindow)
        {
            Requires.NotNull(codeWindow, nameof(codeWindow));

            Guid classID;
            ErrorHandler.ThrowOnFailure(codeWindow.GetViewClassID(out classID));
            return classID;
        }

        public static bool IsReadOnly([NotNull] this IVsCodeWindow codeWindow)
        {
            Requires.NotNull(codeWindow, nameof(codeWindow));

            IVsCodeWindowEx codeWindowEx = codeWindow as IVsCodeWindowEx;
            if (codeWindowEx == null)
                throw new NotSupportedException();

            int result = codeWindowEx.IsReadOnly();
            ErrorHandler.ThrowOnFailure(result);
            return result == VSConstants.S_OK;
        }
    }
}
