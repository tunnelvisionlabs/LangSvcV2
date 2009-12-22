namespace Tvl.VisualStudio.Text.Navigation.Implementation
{
    using System;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text.Editor;
    using Microsoft.VisualStudio.Text.Tagging;
    using Tvl.VisualStudio.Text.Tagging;
    using System.Collections.Generic;

    internal class EditorNavigationMargin : IWpfTextViewMargin
    {
        private readonly IWpfTextView _wpfTextView;
        private readonly IEnumerable<IEditorNavigationSource> _sources;
        private readonly IEditorNavigationTypeRegistryService _editorNavigationTypeRegistryService;

        private readonly UniformGrid _container;
        private Tuple<IEditorNavigationType, ComboBox>[] _navigationControls;

        public EditorNavigationMargin(IWpfTextView wpfTextView, IEnumerable<IEditorNavigationSource> sources, IEditorNavigationTypeRegistryService editorNavigationTypeRegistryService)
        {
            this._wpfTextView = wpfTextView;
            this._sources = sources;
            this._editorNavigationTypeRegistryService = editorNavigationTypeRegistryService;

            _navigationControls =
                this._sources
                .SelectMany(source => source.GetNavigationTypes())
                .Distinct()
                //.OrderBy(...)
                .Select(type => Tuple.Create(type, default(ComboBox)))
                .ToArray();

            this._container = new UniformGrid()
            {
                Columns = _navigationControls.Length,
                Rows = 1
            };

            _navigationControls = Array.ConvertAll(_navigationControls, pair => Tuple.Create(pair.Item1, new ComboBox()
                {
                    IsEditable = true,
                    IsReadOnly = true,
                    ToolTip = new ToolTip()
                    {
                        Content = pair.Item1.EditorNavigationType
                    }
                }));

            foreach (var controlPair in _navigationControls)
                this._container.Children.Add(controlPair.Item2);

#if false
            var items = Enumerable.Range(1, 3).Select(i =>
            {
                var panel = new StackPanel()
                {
                    Orientation = Orientation.Horizontal
                };

                panel.Children.Add(new Image()
                {
                    Source = _glyphService.GetGlyph(StandardGlyphGroup.GlyphGroupField, StandardGlyphItem.GlyphItemPublic),
                    VerticalAlignment = VerticalAlignment.Center
                });
                panel.Children.Add(new TextBlock(new Run(string.Join(string.Empty, Enumerable.Range(0, 10).Select(j => "Item" + i))))
                {
                    VerticalAlignment = VerticalAlignment.Center
                });
                return panel;
            });

            foreach (var item in items)
            {
                _membersControl.Items.Add(item);
            }
#endif

            if (this._navigationControls.Length == 0)
            {
                this._container.Visibility = Visibility.Collapsed;
            }
        }

        public bool Disposed
        {
            get;
            private set;
        }

        public bool IsDisposing
        {
            get;
            private set;
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
            if (marginName == "Editor Navigation Margin")
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
            if (IsDisposing)
                throw new InvalidOperationException();

            try
            {
                IsDisposing = true;
                Dispose(true);
            }
            finally
            {
                IsDisposing = false;
            }

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!Disposed)
            {
                Disposed = true;
            }
        }
    }
}
