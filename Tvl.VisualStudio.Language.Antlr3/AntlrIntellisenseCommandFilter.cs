namespace Tvl.VisualStudio.Language.Antlr3
{
    using System;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Classification;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Operations;
    using Microsoft.VisualStudio.TextManager.Interop;
    using Tvl.VisualStudio.Language.Intellisense;
    using Marshal = System.Runtime.InteropServices.Marshal;

    internal class AntlrIntellisenseCommandFilter : IntellisenseCommandFilter
    {
        private readonly IClassifier _classifier;
        private readonly ITextStructureNavigator _textStructureNavigator;
        private ITextView _textView;

        public AntlrIntellisenseCommandFilter(IVsTextView textViewAdapter, AntlrIntellisenseController controller)
            : base(textViewAdapter, controller)
        {
            var textView = Controller.Provider.EditorAdaptersFactoryService.GetWpfTextView(textViewAdapter);
            var textBuffer = textView.TextBuffer;
            _classifier = Controller.Provider.ClassifierAggregatorService.GetClassifier(textView.TextBuffer);
            _textStructureNavigator = Controller.Provider.TextStructureNavigatorSelectorService.GetTextStructureNavigator(textBuffer);
        }

        internal new AntlrIntellisenseController Controller
        {
            get
            {
                return (AntlrIntellisenseController)base.Controller;
            }
        }

        private IClassifier Classifier
        {
            get
            {
                return _classifier;
            }
        }

        private ITextStructureNavigator TextStructureNavigator
        {
            get
            {
                return _textStructureNavigator;
            }
        }

        private ITextView TextView
        {
            get
            {
                if (_textView == null)
                    _textView = Controller.Provider.EditorAdaptersFactoryService.GetWpfTextView(TextViewAdapter);

                return _textView;
            }
        }

        protected override void HandlePostExec(ref Guid commandGroup, uint commandId, uint executionOptions, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (commandGroup == VsMenus.guidStandardCommandSet2K)
            {
                switch ((VSConstants.VSStd2KCmdID)commandId)
                {
                case VSConstants.VSStd2KCmdID.TYPECHAR:
                    //case VSConstants.VSStd2KCmdID.BACKSPACE:
                    //case VSConstants.VSStd2KCmdID.TAB:
                    //case VSConstants.VSStd2KCmdID.BACKTAB:
                    //case VSConstants.VSStd2KCmdID.DELETE:
                    char typedChar = Convert.ToChar(Marshal.GetObjectForNativeVariant(pvaIn));
                    switch (typedChar)
                    {
                    /* currently only implemented for $ references */
                    //case '@':
                    //case ':':
                    case '$':
                    //case '.':
                        SnapshotPoint currentPosition = TextView.Caret.Position.BufferPosition;
                        TextExtent wordExtent = TextStructureNavigator.GetExtentOfWord(currentPosition);
                        SnapshotSpan wordSpan = wordExtent.Span;
                        if (wordExtent.Span.Start >= currentPosition)
                        {
                            wordExtent = TextStructureNavigator.GetExtentOfWord(currentPosition - 1);
                            wordSpan = wordExtent.Span;
                        }

                        if (wordSpan.End == currentPosition && wordSpan.Length <= 2)
                        {
                            string wordText = wordSpan.GetText();
                            switch (wordText)
                            {
                            case "@":
                            case "$":
                                {
                                    ITrackingPoint triggerPoint = currentPosition.Snapshot.CreateTrackingPoint(currentPosition, PointTrackingMode.Positive);
                                    base.Controller.TriggerCompletion(triggerPoint, CompletionInfoType.AutoListMemberInfo, IntellisenseInvocationType.IdentifierChar);
                                }
                                break;

                            case ".":
                            case "::":
                                {
                                    ITrackingPoint triggerPoint = currentPosition.Snapshot.CreateTrackingPoint(currentPosition, PointTrackingMode.Positive);
                                    base.Controller.TriggerCompletion(triggerPoint, CompletionInfoType.AutoListMemberInfo, IntellisenseInvocationType.ShowMemberList);
                                }
                                break;

                            default:
                                break;
                            }
                        }

                        break;

                    default:
                        break;
                    }
                    break;

                default:
                    break;
                }
            }

            base.HandlePostExec(ref commandGroup, commandId, executionOptions, pvaIn, pvaOut);
        }
    }
}
