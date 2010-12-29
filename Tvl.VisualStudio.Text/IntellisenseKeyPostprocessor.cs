namespace Tvl.VisualStudio.Text
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Text;
    using Microsoft.VisualStudio.Text.Editor;
    using System.Windows.Input;

    public class IntellisenseKeyPostprocessor : KeyProcessor
    {
        private readonly ITextBuffer _textBuffer;
        private readonly ICompletionTarget _completionTarget;

        public IntellisenseKeyPostprocessor(ITextBuffer textBuffer, ICompletionTarget completionTarget)
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

        public CompletionInfo CompletionInfo
        {
            get
            {
                return CompletionTarget.CompletionInfo;
            }
        }

        public ICompletionTarget CompletionTarget
        {
            get
            {
                return _completionTarget;
            }
        }

        public override void KeyUp(KeyEventArgs args)
        {
            bool flag = false;
            switch (args.Key)
            {
            case Key.Delete:
                flag = true;
                break;

            case Key.Back:
                bool flag2 = false;
                int position = this.CompletionTarget.TextView.Caret.Position.BufferPosition.Position;
                if (CompletionHelper.IsCompletionPresenterActive(CompletionTarget, true))
                {
                    int length = CompletionInfo.TextSoFar.Length;
                    if (length > 0)
                    {
                        if (length == 1)
                        {
                            ITextSnapshotLine lineFromPosition = CompletionTarget.CompletionSession.TextView.TextSnapshot.GetLineFromPosition(position);
                            if (this.CompletionTarget.CompletionSession.TextView.TextSnapshot.GetText(lineFromPosition.Start.Position, position - lineFromPosition.Start.Position).Trim().Length == 0)
                            {
                                this.CompletionTarget.DismissCompletion();
                            }
                        }
                        flag = true;
                    }
                    else
                    {
                        this.CompletionTarget.DismissCompletion();
                        flag2 = true;
                    }
                }
                else
                {
                    ITextSnapshotLine line2 = this.TextBuffer.CurrentSnapshot.GetLineFromPosition(position);
                    if (position > line2.Start.Position)
                    {
                        flag2 = true;
                    }
                }
                if (flag2)
                {
                    CompletionHelper.DoTriggerCompletion(this.CompletionTarget, CompletionInfoType.ContextInfo, false, IntellisenseInvocationType.BackspaceDeleteOrBackTab);
                }
                break;
            }

            if (flag && CompletionHelper.IsCompletionPresenterActive(CompletionTarget, true) && (CompletionInfo.DropDownFlags & CompletionDropDownFlags.AliasBuilder) != 0)
            {
                CompletionTarget.CompletionSession.Recalculate();
            }
        }

        public override void TextInput(TextCompositionEventArgs args)
        {
            if (args.Text.Length == 1)
                ShowOrUpdateIntellisense(args.Text[0]);
        }

        protected virtual void ShowOrUpdateIntellisense(char invocationChar)
        {
            if (CompletionHelper.IsCompletionPresenterActive(this.CompletionTarget, true))
            {
                if ((this.CompletionInfo.TextSoFar.Length == 1) & char.IsDigit(invocationChar))
                {
                    this.CompletionTarget.DismissCompletion();
                }
                else if (!CompletionHelper.DoCallMatch(this.CompletionTarget))
                {
                    this.CompletionTarget.DismissCompletion();
                }
                else if ((this.CompletionInfo.DropDownFlags & CompletionDropDownFlags.AliasBuilder) != 0)
                {
                    this.CompletionTarget.CompletionSession.Recalculate();
                }
            }
            else
            {
                IntellisenseInvocationType invocationType = IntellisenseInvocationType.Default;
                bool flag = false;
                ITextSnapshot currentSnapshot = this.TextBuffer.CurrentSnapshot;
                SnapshotPoint bufferPosition = this.CompletionTarget.TextView.Caret.Position.BufferPosition;
                if (char.IsLetter(invocationChar) || (invocationChar == '_'))
                {
                    int column = bufferPosition - bufferPosition.GetContainingLine().Start;
                    if (column == 1)
                    {
                        invocationType = IntellisenseInvocationType.IdentifierChar;
                        flag = true;
                    }
                    else if (column > 1)
                    {
                        string text = currentSnapshot.GetText(bufferPosition.Position - 2, 2);
                        if (!char.IsLetter(text[0]) && (text[1] == invocationChar))
                        {
                            invocationType = IntellisenseInvocationType.IdentifierChar;
                            flag = true;
                        }
                    }
                }
                if (flag || (" (){=><,.\"':@+-*/&^%\\?[#".IndexOf(invocationChar) >= 0))
                {
                    switch (invocationChar)
                    {
                    case ' ':
                        invocationType = IntellisenseInvocationType.Space;
                        break;

                    case '#':
                        invocationType = IntellisenseInvocationType.Sharp;
                        break;
                    }
                    CompletionHelper.DoTriggerCompletion(this.CompletionTarget, CompletionInfoType.ContextInfo, false, invocationType);
                }
            }
        }
    }
}
