namespace Tvl.VisualStudio.InheritanceMargin
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using CanExecuteRoutedEventArgs = System.Windows.Input.CanExecuteRoutedEventArgs;
    using CommandRouter = Tvl.VisualStudio.InheritanceMargin.CommandTranslation.CommandRouter;
    using CommandTargetParameters = Tvl.VisualStudio.InheritanceMargin.CommandTranslation.CommandTargetParameters;
    using ExecutedRoutedEventArgs = System.Windows.Input.ExecutedRoutedEventArgs;
    using FrameworkElement = System.Windows.FrameworkElement;
    using MouseEventArgs = System.Windows.Input.MouseEventArgs;

    public class InheritanceTag : IInheritanceTag
    {
        private readonly InheritanceGlyph _glyph;
        private readonly string _tooltip;
        private FrameworkElement _marginGlyph;

        private readonly List<IInheritanceTarget> _targets;

        public InheritanceTag(InheritanceGlyph glyph, string tooltip, List<IInheritanceTarget> members)
        {
            this._glyph = glyph;
            this._tooltip = tooltip;
            this._targets = members;
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

        public ReadOnlyCollection<IInheritanceTarget> Targets
        {
            get
            {
                return _targets.AsReadOnly();
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
                Targets[index].NavigateTo();
            }
        }

        public void HandleCanExecuteInheritanceTargetsList(object sender, CanExecuteRoutedEventArgs e)
        {
            CommandTargetParameters parameter = e.Parameter as CommandTargetParameters;
            if (parameter != null)
            {
                int index = parameter.Id - InheritanceMarginConstants.cmdidInheritanceTargetsList;
                if (index < Targets.Count)
                {
                    e.CanExecute = true;
                    parameter.Enabled = true;
                    parameter.Visible = true;
                    parameter.Pressed = false;
                    parameter.Text = Targets[index].DisplayName;
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
