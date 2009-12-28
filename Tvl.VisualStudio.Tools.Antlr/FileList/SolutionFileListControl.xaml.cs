namespace Tvl.VisualStudio.Tools.FileList
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;
    using Tvl.VisualStudio.Shell.Extensions;
    using Path = System.IO.Path;
    using Microsoft.VisualStudio.Shell;
    using File = System.IO.File;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

    public partial class SolutionFileListControl : UserControl
    {
        private readonly SolutionFileListProvider _provider;
        private readonly List<ProjectItemInfo> _items;

        internal SolutionFileListControl(SolutionFileListProvider provider)
        {
            if (provider == null)
                throw new ArgumentNullException("provider");
            Contract.EndContractBlock();

            this._provider = provider;
            this._items = new List<ProjectItemInfo>();
            InitializeComponent();
        }

        public bool CaseSensitive
        {
            get
            {
                return (chkCaseSensitive.IsChecked ?? false);
            }
        }

        public string Filter
        {
            get
            {
                string text = txtQuery.Text;
                if (text.StartsWith("/"))
                    text = text.Substring(1);

                return text;
            }
        }

        public bool UseRegex
        {
            get
            {
                return (chkRegex.IsChecked ?? false) || txtQuery.Text.StartsWith("/");
            }
        }

        private void UpdateFiles()
        {
            try
            {
                this._items.Clear();

                IVsSolution solution = (IVsSolution)_provider.ServiceProvider.GetService<SVsSolution>();
                if (solution == null)
                    return;

                List<ProjectItemInfo> items = new List<ProjectItemInfo>();
                foreach (IVsHierarchy projectHierarchy in GetProjectHierarchies(solution))
                {
                    EnvDTE.Project project = projectHierarchy.GetExtensibilityObjectOrDefault(VSConstants.VSITEMID_ROOT) as EnvDTE.Project;
                    if (project == null)
                        continue;

                    string projectName = project.Name;

                    projectHierarchy.EnumHierarchyItems(VSConstants.VSITEMID_ROOT, 0, false, true, true,
                        (node, itemId, level) =>
                        {
                            try
                            {
                                EnvDTE.ProjectItem pi = node.GetExtensibilityObjectOrDefault(itemId) as EnvDTE.ProjectItem;
                                if (pi != null && pi.ContainingProject == project)
                                {
                                    for (short i = 1; i <= pi.FileCount; i++)
                                    {
                                        string filename = pi.get_FileNames(i);
                                        if (!System.IO.File.Exists(filename))
                                            continue;

                                        ProjectItemInfo itemInfo = new ProjectItemInfo()
                                        {
                                            FileName = System.IO.Path.GetFileName(filename),
                                            Project = projectName,
                                            Path = filename
                                        };
                                        this._items.Add(itemInfo);
                                    }
                                }
                            }
                            catch (Exception e)
                            {
                                // VS.Php likes to throw NotImplementedException
                                if (ErrorHandler.IsCriticalException(e))
                                    throw;
                            }
                        });
                }
            }
            catch (Exception ex)
            {
                if (ErrorHandler.IsCriticalException(ex))
                    throw;
            }
        }

        private void UpdateList()
        {
            if (Thread.CurrentThread != lstResults.Dispatcher.Thread)
            {
                lstResults.Dispatcher.Invoke((Action)UpdateList);
                return;
            }

            try
            {
                bool caseSensitive = this.CaseSensitive;
                string filter = this.Filter;

                Regex rx = null;
                try
                {
                    if (UseRegex)
                        rx = new Regex(filter, caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
                }
                catch (ArgumentException)
                {
                    lstResults.Items.Clear();
                    txtQuery.Background = Brushes.LightSalmon;
                    return;
                }

                var filtered = this._items.FindAll(
                    item =>
                    {
                        if (item == null || item.FileName == null)
                            return false;

                        if (rx == null)
                        {
                            if (!caseSensitive)
                                return item.FileName.ToLower().Contains(filter.ToLower());
                            else
                                return item.FileName.Contains(filter);
                        }

                        return rx.IsMatch(item.FileName);
                    });

                lstResults.Items.Clear();
                foreach (var item in filtered)
                    lstResults.Items.Add(item);

                txtQuery.Background = Brushes.White;

                // TODO: default selection
            }
            catch (Exception ex)
            {
                if (ErrorHandler.IsCriticalException(ex))
                    throw;

                try
                {
                    Action setColor = () => txtQuery.Background = Brushes.LightSalmon;
                    txtQuery.Dispatcher.Invoke(setColor);
                }
                catch (Exception ex2)
                {
                    if (ErrorHandler.IsCriticalException(ex2))
                        throw;
                }
            }
        }

        private void OnRefreshClick(object sender, RoutedEventArgs e)
        {
            UpdateFiles();
            UpdateList();
        }

        private void OnFilterChanged(object sender, RoutedEventArgs e)
        {
            if (Thread.CurrentThread != txtQuery.Dispatcher.Thread)
            {
                Action action = () => OnFilterChanged(sender, e);
                txtQuery.Dispatcher.Invoke(action);
                return;
            }

            if (txtQuery.Text.StartsWith("/") && !chkRegex.IsChecked.HasValue || !chkRegex.IsChecked.Value)
            {
                chkRegex.IsChecked = true;
                chkRegex.IsEnabled = false;
            }
            else if (!chkRegex.IsEnabled)
            {
                chkRegex.IsChecked = false;
                chkRegex.IsEnabled = true;
            }

            UpdateList();
        }

        private void OnOpenSelectedFiles(object sender, RoutedEventArgs e)
        {
            OpenSelectedFiles(lstResults.SelectedItems.OfType<ProjectItemInfo>());
        }

        private void OpenSelectedFiles(IEnumerable<ProjectItemInfo> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            Contract.EndContractBlock();

            IServiceProvider serviceProvider = _provider.ServiceProvider;
            IVsUIShellOpenDocument service = serviceProvider.GetService<IVsUIShellOpenDocument>();
            IVsRunningDocumentTable table = serviceProvider.GetService<IVsRunningDocumentTable>();

            foreach (var item in items)
            {
                string path = item.Path;
                if (string.IsNullOrEmpty(path) || !File.Exists(path))
                    continue;

                IVsUIHierarchy uiHierarchy = null;
                uint itemId = uint.MaxValue;
                IVsWindowFrame frame = null;
                Guid logicalView = Guid.Empty;
                bool isOpen = false;

                if (service != null && table != null)
                {
                    IntPtr handle = IntPtr.Zero;
                    try
                    {
                        IVsHierarchy hierarchy;
                        uint cookie;
                        int hr = table.FindAndLockDocument(0, path, out hierarchy, out itemId, out handle, out cookie);
                        ErrorHandler.ThrowOnFailure(hr);
                        uint[] pitemidOpen = new uint[1];
                        uint ido = (logicalView == Guid.Empty) ? 2U : 0U;
                        int open;
                        hr = service.IsDocumentOpen((IVsUIHierarchy)hierarchy, itemId, path, ref logicalView, ido, out uiHierarchy, pitemidOpen, out frame, out open);
                        ErrorHandler.ThrowOnFailure(hr);
                        if (frame != null)
                        {
                            itemId = pitemidOpen[0];
                            isOpen = open != 0;
                        }
                    }
                    finally
                    {
                        if (handle != IntPtr.Zero)
                            Marshal.Release(handle);
                    }
                }

                if (!isOpen)
                {
                    if (service != null)
                    {
                        IOleServiceProvider oleServiceProvider;
                        int hr = service.OpenDocumentViaProject(path, ref logicalView, out oleServiceProvider, out uiHierarchy, out itemId, out frame);
                        ErrorHandler.ThrowOnFailure(hr);
                    }
                }

                if (frame != null)
                {
                    int hr = frame.Show();
                    ErrorHandler.ThrowOnFailure(hr);
                }
            }
        }
    }

    public class ProjectItemInfo
    {
        private static readonly ConcurrentDictionary<string, ImageSource> _fileIcons =
            new ConcurrentDictionary<string, ImageSource>(StringComparer.OrdinalIgnoreCase);

        public string FileName
        {
            get;
            set;
        }

        public string Project
        {
            get;
            set;
        }

        public string Path
        {
            get;
            set;
        }

        public ImageSource Icon
        {
            get
            {
                return _fileIcons.GetOrAdd(System.IO.Path.GetExtension(FileName), IconFactory);
            }
        }

        private ImageSource IconFactory(string extension)
        {
            return LoadIcon();
        }

        private ImageSource LoadIcon()
        {
            using (System.Drawing.Icon sysicon = System.Drawing.Icon.ExtractAssociatedIcon(Path))
            {
                // This new call in WPF finally allows us to read/display 32bit Windows file icons!
                ImageSource source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHIcon(
                  sysicon.Handle,
                  System.Windows.Int32Rect.Empty,
                  System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                return source;
            }
        }
    }
}
