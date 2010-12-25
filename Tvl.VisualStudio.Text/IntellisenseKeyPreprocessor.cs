namespace Tvl.VisualStudio.Text
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text;
    using System.Windows.Input;
    using Microsoft.VisualStudio.Language.Intellisense;

    public class IntellisenseKeyPreprocessor : KeyProcessor
    {
        private readonly ITextBuffer _textBuffer;
        private readonly ICompletionTarget _completionTarget;

        public IntellisenseKeyPreprocessor(ITextBuffer textBuffer, ICompletionTarget completionTarget)
        {
            _textBuffer = textBuffer;
            _completionTarget = completionTarget;
        }

        public ITextBuffer TextBuffer
        {
            get
            {
                return _textBuffer;
            }
        }

        public ICompletionTarget CompletionTarget
        {
            get
            {
                return _completionTarget;
            }
        }

        public CompletionInfo CompletionInfo
        {
            get
            {
                var completionTarget = CompletionTarget;
                if (completionTarget == null)
                    return null;

                return completionTarget.CompletionInfo;
            }
        }

        public override void PreviewKeyDown(KeyEventArgs args)
        {
            var completionTarget = CompletionTarget;
            if (completionTarget == null)
                return;

            if (CompletionHelper.IsCompletionPresenterActive(completionTarget, true))
            {
                switch (args.Key)
                {
                case Key.Back:
                    {
                        ITextSnapshot currentSnapshot = TextBuffer.CurrentSnapshot;
                        SnapshotSpan span = CompletionInfo.ApplicableTo.GetSpan(currentSnapshot);
                        if (span.Length > 0)
                        {
                            CompletionInfo.ApplicableTo = currentSnapshot.CreateTrackingSpan((int)span.Start, span.Length - 1, SpanTrackingMode.EdgeInclusive, TrackingFidelityMode.Forward);
                        }
                        return;
                    }
                case Key.Tab:
                    completionTarget.CommitCompletion();
                    args.Handled = true;
                    return;

                case Key.LineFeed:
                case Key.Clear:
                case Key.Pause:
                case Key.Capital:
                case Key.KanaMode:
                case Key.JunjaMode:
                case Key.FinalMode:
                case Key.HanjaMode:
                case Key.ImeConvert:
                case Key.ImeNonConvert:
                case Key.ImeAccept:
                case Key.ImeModeChange:
                case Key.Prior:
                case Key.Next:
                case Key.End:
                    return;

                case Key.Return:
                    {
                        completionTarget.CommitCompletion();
#if false
                        Microsoft.VisualBasic.Editor.ExpressionEditor.ExpressionEditor val = null;
                        if (completionTarget.TextView.Properties.TryGetTypedProperty<Microsoft.VisualBasic.Editor.ExpressionEditor.ExpressionEditor>(ref val))
                        {
                            args.Handled = true;
                        }
#endif
                        return;
                    }
                case Key.Escape:
                    completionTarget.DismissCompletion();
                    args.Handled = true;
                    return;

                case Key.Space:
                    completionTarget.CommitCompletion();
                    return;

                case Key.Home:
                    completionTarget.DismissCompletion();
                    return;
                }
            }
            else if ((args.KeyboardDevice.Modifiers & ModifierKeys.Control) > ModifierKeys.None)
            {
                bool flag = false;
                if (completionTarget.TextView != null)
                {
#if true
                    flag = true;
#else
                    flag = completionTarget.TextView.AllowDefaultKeyProcessing();
#endif
                }
                if (flag)
                {
                    switch (args.Key)
                    {
                    case Key.Space:
                        if ((args.KeyboardDevice.Modifiers & ModifierKeys.Shift) > ModifierKeys.None)
                        {
                            // ctrl+shift+space = signature help
                            CompletionHelper.DoTriggerCompletion(this.CompletionTarget, CompletionInfoType.ContextInfo, true, IntellisenseInvocationType.Default);
                        }
                        else
                        {
                            CompletionHelper.DoTriggerCompletion(this.CompletionTarget, CompletionInfoType.GlobalInfo, false, IntellisenseInvocationType.Default);
                        }
                        args.Handled = true;
                        return;

                    case Key.J:
                        CompletionHelper.DoTriggerCompletion(this.CompletionTarget, CompletionInfoType.GlobalInfo, false, IntellisenseInvocationType.ShowMemberList);
                        args.Handled = true;
                        return;
                    }
                }
            }
        }

        public override void KeyDown(KeyEventArgs args)
        {
            ICompletionTarget completionTarget = CompletionTarget;
            if (completionTarget == null)
                return;

            if (!args.Handled)
            {
                IIntellisenseCommandTarget target = completionTarget.IntellisenseSessionStack as IIntellisenseCommandTarget;
                if (target != null)
                {
                    IntellisenseKeyboardCommand? command = TranslateKeyboardCommand(args);
                    if (command.HasValue)
                        args.Handled = target.ExecuteKeyboardCommand(command.Value);
                }
            }
        }

        public override void TextInput(TextCompositionEventArgs args)
        {
            ICompletionTarget completionTarget = CompletionTarget;
            if (completionTarget == null)
                return;

            if (CompletionHelper.IsCompletionPresenterActive(completionTarget, true))
            {
                if (!string.IsNullOrEmpty(args.Text) && IsCommitChar(args.Text[0]))
                {
                    if (completionTarget.CompletionSession.SelectedCompletionSet.SelectionStatus.Completion == null)
                    {
                        completionTarget.DismissCompletion();
                    }
                    else
                    {
                        completionTarget.CompletionSession.Properties.AddProperty("CommitChar", args.Text[0]);
                        completionTarget.CommitCompletion();
                    }
                }
            }
            else if (!string.IsNullOrEmpty(args.Text))
            {
                ITextSnapshot snapshot = completionTarget.TextView.TextBuffer.CurrentSnapshot;
                int position = completionTarget.TextView.Caret.Position.BufferPosition.Position;
                ITextSnapshotLine line = snapshot.GetLineFromPosition(position);
                char c = args.Text[0];
                if (string.IsNullOrWhiteSpace(line.GetText()) && (IsIdentifierChar(c) || c == ' '))
                {
                    CompletionHelper.DoTriggerCompletion(CompletionTarget, CompletionInfoType.ContextInfo, false, IntellisenseInvocationType.Default);
                }
            }
        }

        protected virtual bool IsCommitChar(char c)
        {
            return char.IsWhiteSpace(c) || ("!^()=<>\\:;.,+-*/{}\" '&%@?".IndexOf(new string(c, 1), StringComparison.Ordinal) > 0);
        }

        protected virtual bool IsIdentifierChar(char c)
        {
            return char.IsLetterOrDigit(c) || c == '_';
        }

        protected virtual IntellisenseKeyboardCommand? TranslateKeyboardCommand(KeyEventArgs args)
        {
            switch (args.Key)
            {
            case Key.Up:
                return IntellisenseKeyboardCommand.Up;

            case Key.Down:
                return IntellisenseKeyboardCommand.Down;

            case Key.Prior:
                return IntellisenseKeyboardCommand.PageUp;

            case Key.Next:
                return IntellisenseKeyboardCommand.PageDown;

            case Key.Home:
                return IntellisenseKeyboardCommand.Home;

            case Key.End:
                return IntellisenseKeyboardCommand.End;

            case Key.Escape:
                return IntellisenseKeyboardCommand.Escape;

            case Key.ImeConvert:
            case Key.ImeNonConvert:
            case Key.ImeAccept:
            case Key.ImeModeChange:
            case Key.Space:
            case Key.Left:
            case Key.Right:
                return null;

            default:
                return null;
            }
        }
    }
}
