namespace Tvl.VisualStudio.InheritanceMargin
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using CanExecuteRoutedEventArgs = System.Windows.Input.CanExecuteRoutedEventArgs;
    using CommandRouter = Tvl.VisualStudio.InheritanceMargin.CommandTranslation.CommandRouter;
    using CommandTargetParameters = Tvl.VisualStudio.InheritanceMargin.CommandTranslation.CommandTargetParameters;
    using CSharpMemberIdentifier = Microsoft.RestrictedUsage.CSharp.Semantics.CSharpMemberIdentifier;
    using CSharpTypeIdentifier = Microsoft.RestrictedUsage.CSharp.Semantics.CSharpTypeIdentifier;
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
        private readonly List<Tuple<string, CSharpTypeIdentifier>> _types;

        public InheritanceTag(InheritanceGlyph glyph, string tooltip, List<Tuple<string, CSharpTypeIdentifier>> types)
        {
            this._glyph = glyph;
            this._tooltip = tooltip;
            this._types = types;
            this._members = new List<CSharpMemberIdentifier>();
        }

        public InheritanceTag(InheritanceGlyph glyph, string tooltip, List<CSharpMemberIdentifier> members)
        {
            this._glyph = glyph;
            this._tooltip = tooltip;
            this._types = new List<Tuple<string, CSharpTypeIdentifier>>();
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

        public ReadOnlyCollection<Tuple<string, CSharpTypeIdentifier>> Types
        {
            get
            {
                return _types.AsReadOnly();
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
                if (index < Types.Count)
                    CSharpInheritanceAnalyzer.NavigateToType(Types[index].Item2);
                else if (index < Types.Count + Members.Count)
                    CSharpInheritanceAnalyzer.NavigateToMember(Members[index - Types.Count]);
            }
        }

        public void HandleCanExecuteInheritanceTargetsList(object sender, CanExecuteRoutedEventArgs e)
        {
            CommandTargetParameters parameter = e.Parameter as CommandTargetParameters;
            if (parameter != null)
            {
                int index = parameter.Id - InheritanceMarginConstants.cmdidInheritanceTargetsList;
                if (index < Types.Count + Members.Count)
                {
                    e.CanExecute = true;
                    parameter.Enabled = true;
                    parameter.Visible = true;
                    parameter.Pressed = false;
                    if (index < Types.Count)
                        parameter.Text = Types[index].Item1;
                    else
                        parameter.Text = Members[index - Types.Count].ToString();
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
