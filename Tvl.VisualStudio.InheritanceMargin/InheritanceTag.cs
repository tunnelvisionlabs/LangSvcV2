namespace Tvl.VisualStudio.InheritanceMargin
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using CanExecuteRoutedEventArgs = System.Windows.Input.CanExecuteRoutedEventArgs;
    using CommandRouter = Tvl.VisualStudio.InheritanceMargin.CommandTranslation.CommandRouter;
    using CommandTargetParameters = Tvl.VisualStudio.InheritanceMargin.CommandTranslation.CommandTargetParameters;
    using CSharpMemberIdentifier = Microsoft.RestrictedUsage.CSharp.Semantics.CSharpMemberIdentifier;
    using ExecutedRoutedEventArgs = System.Windows.Input.ExecutedRoutedEventArgs;
    using FrameworkElement = System.Windows.FrameworkElement;
    using IGlyphTag = Microsoft.VisualStudio.Text.Editor.IGlyphTag;
    using MouseEventArgs = System.Windows.Input.MouseEventArgs;

    public class InheritanceTag : IGlyphTag
    {
        private readonly InheritanceGlyph _glyph;
        private readonly string _tooltip;
        private FrameworkElement _marginGlyph;

        private readonly List<CSharpMemberIdentifier> _members;

        public InheritanceTag(InheritanceGlyph glyph, string tooltip, List<CSharpMemberIdentifier> members)
        {
            this._glyph = glyph;
            this._tooltip = tooltip;
            this._members = members;
        }

        public InheritanceGlyph Glyph
        {
            get
            {
                return _glyph;
            }
        }

        public string ToolTip
        {
            get
            {
                return _tooltip;
            }
        }

        public FrameworkElement MarginGlyph
        {
            get
            {
                return _marginGlyph;
            }

            internal set
            {
                _marginGlyph = value;
            }
        }

        public ReadOnlyCollection<CSharpMemberIdentifier> Members
        {
            get
            {
                return _members.AsReadOnly();
            }
        }

        public void ShowContextMenu(MouseEventArgs e)
        {
            CommandRouter.DisplayContextMenu(InheritanceMarginConstants.guidInheritanceMarginCommandSet, InheritanceMarginConstants.menuInheritanceTargets, _marginGlyph);
        }

        public void HandleExecutedInheritanceTargetsList(object sender, ExecutedRoutedEventArgs e)
        {
            CommandTargetParameters parameter = e.Parameter as CommandTargetParameters;
            if (parameter != null)
            {
                int index = parameter.Id - InheritanceMarginConstants.cmdidInheritanceTargetsList;
                if (index < Members.Count)
                    CSharpInheritanceAnalyzer.NavigateToMember(Members[index]);
            }
        }

        public void HandleCanExecuteInheritanceTargetsList(object sender, CanExecuteRoutedEventArgs e)
        {
            CommandTargetParameters parameter = e.Parameter as CommandTargetParameters;
            if (parameter != null)
            {
                int index = parameter.Id - InheritanceMarginConstants.cmdidInheritanceTargetsList;
                if (index < Members.Count)
                {
                    e.CanExecute = true;
                    parameter.Enabled = true;
                    parameter.Visible = true;
                    parameter.Pressed = false;
                    parameter.Text = Members[index].ToString();
                }
                else
                {
                    e.CanExecute = false;
                    parameter.Enabled = false;
                    parameter.Visible = false;
                }
            }
        }
    }
}
