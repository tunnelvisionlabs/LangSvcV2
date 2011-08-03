namespace Tvl.VisualStudio.Language.Antlr3.OptionsPages
{
    using System;
    using System.Windows.Forms;

    public partial class AntlrIntellisenseOptionsControl : UserControl
    {
        public AntlrIntellisenseOptionsControl( AntlrIntellisenseOptions optionsPage )
        {
            InitializeComponent();

            OptionsPage = optionsPage;
            ReloadOptions();
        }

        private AntlrIntellisenseOptions OptionsPage
        {
            get;
            set;
        }

        public void ReloadOptions()
        {
            chkShowCompletionAfterTypedChar.Checked = OptionsPage.ShowCompletionAfterTypedChar;
            chkKeywordsInCompletion.Enabled = !OptionsPage.ShowCompletionAfterTypedChar;
            chkKeywordsInCompletion.Checked = OptionsPage.KeywordsInCompletionLists || OptionsPage.ShowCompletionAfterTypedChar;
            chkCodeSnippetsInCompletion.Enabled = !OptionsPage.ShowCompletionAfterTypedChar;
            chkCodeSnippetsInCompletion.Checked = OptionsPage.CodeSnippetsInCompletionLists || OptionsPage.ShowCompletionAfterTypedChar;

            txtCompletionChars.Text = OptionsPage.CommitCharacters;
            chkCommitOnSpace.Checked = OptionsPage.CommitOnSpace;
            chkNewLineAfterEnter.Checked = OptionsPage.NewLineAfterEnterCompletion;

            chkRecentCompletions.Checked = OptionsPage.RecentCompletions;
        }

        public void ApplyChanges()
        {
            OptionsPage.ShowCompletionAfterTypedChar = chkShowCompletionAfterTypedChar.Checked;
            if ( !OptionsPage.ShowCompletionAfterTypedChar )
            {
                OptionsPage.KeywordsInCompletionLists = chkKeywordsInCompletion.Checked;
                OptionsPage.CodeSnippetsInCompletionLists = chkCodeSnippetsInCompletion.Checked;
            }

            OptionsPage.CommitCharacters = txtCompletionChars.Text;
            OptionsPage.CommitOnSpace = chkCommitOnSpace.Checked;
            OptionsPage.NewLineAfterEnterCompletion = chkNewLineAfterEnter.Checked;

            OptionsPage.RecentCompletions = chkRecentCompletions.Checked;
        }

        private void chkShowCompletionAfterTypedChar_CheckedChanged( object sender, EventArgs e )
        {
            chkKeywordsInCompletion.Enabled = !chkShowCompletionAfterTypedChar.Checked;
            chkCodeSnippetsInCompletion.Enabled = !chkShowCompletionAfterTypedChar.Checked;
        }
    }
}
