namespace JavaLanguageService.Text
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using JavaLanguageService.Text.Language;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text.Editor;

    internal class DropdownBarMargin : IWpfTextViewMargin
    {
        private readonly IWpfTextView _wpfTextView;
        private readonly ILanguageElementManager _manager;
        private readonly IGlyphService _glyphService;
        private bool _disposed;

        private readonly UniformGrid _container;
        private readonly ComboBox _typesControl;
        private readonly ComboBox _membersControl;

        public DropdownBarMargin(IWpfTextView wpfTextView, ILanguageElementManager manager, IGlyphService glyphService)
        {
            this._wpfTextView = wpfTextView;
            this._manager = manager;
            this._glyphService = glyphService;

            this._container = new UniformGrid()
                {
                    Columns = 2,
                    Rows = 1
                };

            this._typesControl = new ComboBox()
                {
                    ToolTip = new ToolTip()
                    {
                        Content = "Types"
                    }
                };

            this._membersControl = new ComboBox()
                {
                    ToolTip = new ToolTip()
                    {
                        Content = "Members"
                    }
                };

            this._container.Children.Add(_typesControl);
            this._container.Children.Add(_membersControl);

            var items = Enumerable.Range(1, 3).Select(i =>
            {
                var panel = new StackPanel()
                {
                    Orientation = Orientation.Horizontal
                };

                panel.Children.Add(new Image()
                {
                    Source = _glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupField, StandardGlyphItem.GlyphItemPublic)
                });
                panel.Children.Add(new TextBlock(new Run("Item" + i)));
                return panel;
            });

            foreach (var item in items)
            {
                _membersControl.Items.Add(item);
            }
        }

        public FrameworkElement VisualElement
        {
            get
            {
                return _container;
            }
        }

        public bool Enabled
        {
            get
            {
                return false;
            }
        }

        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            if (marginName == "Dropdown Bar Margin")
                return this;

            return null;
        }

        public double MarginSize
        {
            get
            {
                return VisualElement.ActualHeight;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }
    }
}
