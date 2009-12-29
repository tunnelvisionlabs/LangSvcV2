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
    using System.Windows.Input;
    using Tvl.Events;

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

            if (this._navigationControls.Length == 0)
            {
                this._container =
                    new UniformGrid()
                    {
                        Visibility = Visibility.Collapsed
                    };

                return;
            }

            this._container = new UniformGrid()
            {
                Columns = _navigationControls.Length,
                Rows = 1
            };

            _navigationControls = Array.ConvertAll(_navigationControls,
                pair =>
                {
                    ComboBox comboBox =
                        new EditorNavigationComboBox()
                        {
                            Cursor = Cursors.Arrow,
                            ToolTip = new ToolTip()
                            {
                                Content = pair.Item1.Type
                            }
                        };

                    comboBox.SelectionChanged += OnSelectionChanged;
                    return Tuple.Create(pair.Item1, comboBox);
                });

            foreach (var controlPair in _navigationControls)
            {
                this._container.Children.Add(controlPair.Item2);
            }

            foreach (var source in this._sources)
            {
                source.NavigationTargetsChanged += WeakEvents.AsWeak(OnNavigationTargetsChanged, eh => source.NavigationTargetsChanged -= eh);
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

        public bool Updating
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

        private void UpdateNavigationTargets(IEditorNavigationSource source)
        {
            lock (this)
            {
                if (Updating)
                    return;
            }

            try
            {
                Updating = true;
                var targets = source.GetNavigationTargets().ToArray();
                Action action = () => UpdateNavigationTargets(targets);
                VisualElement.Dispatcher.Invoke(action);
            }
            finally
            {
                Updating = false;
            }
        }

        private void UpdateNavigationTargets(IEnumerable<IEditorNavigationTarget> targets)
        {
            foreach (var group in targets.GroupBy(target => target.EditorNavigationType))
            {
                var navigationControl = this._navigationControls.FirstOrDefault(control => control.Item1 == group.Key);
                if (navigationControl == null)
                    continue;

                var combo = navigationControl.Item2;
                combo.Items.Clear();
                foreach (var item in group.OrderBy(i => i.Name, StringComparer.CurrentCultureIgnoreCase))
                    combo.Items.Add(item);

                if (combo.Items.Count > 0)
                    combo.SelectedIndex = 0;

                combo.IsEnabled = combo.Items.Count > 0;
            }

            foreach (var control in this._navigationControls)
            {
                control.Item2.IsEnabled = control.Item2.HasItems;
            }
        }

        private void OnNavigationTargetsChanged(object sender, EventArgs e)
        {
            IEditorNavigationSource source = (IEditorNavigationSource)sender;
            UpdateNavigationTargets(source);
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Updating)
                return;

            if (e.AddedItems.Count > 0)
            {
                IEditorNavigationTarget target = e.AddedItems[0] as IEditorNavigationTarget;
                if (target != null)
                {
                    var seek = target.Seek.Snapshot == null ? target.Span : target.Seek;
                    _wpfTextView.Caret.MoveTo(seek.Start);
                    _wpfTextView.Selection.Select(seek, false);
                    _wpfTextView.ViewScroller.EnsureSpanVisible(target.Seek);
                    Keyboard.Focus(_wpfTextView.VisualElement);
                }
            }
        }
    }
}
