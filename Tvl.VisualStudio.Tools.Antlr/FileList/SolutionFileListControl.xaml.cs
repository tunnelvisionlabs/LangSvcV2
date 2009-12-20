namespace Tvl.VisualStudio.Tools.FileList
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Collections.Concurrent;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio;
    using Tvl.VisualStudio.Shell.Extensions;
    using System.Runtime.InteropServices;

    public partial class SolutionFileListControl : UserControl
    {
        private static readonly ConcurrentDictionary<string, ImageSource> _fileIcons =
            new ConcurrentDictionary<string, ImageSource>(StringComparer.OrdinalIgnoreCase);

        private SolutionFileListProvider _provider;

        internal SolutionFileListControl(SolutionFileListProvider provider)
        {
            this._provider = provider;
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

#if false
        private delegate void ProcessHierarchyNode(IVsHierarchy hierarchyNode, uint itemid, int recursionLevel);

        private static void EnumHierarchyItems(IVsHierarchy hierarchy, uint itemid, int recursionLevel, bool isSolution, bool visibleOnly, ProcessHierarchyNode processNode)
        {
            EnumHierarchyItems(hierarchy, itemid, recursionLevel, isSolution, visibleOnly, false, processNode);
        }

        private static void EnumHierarchyItems(IVsHierarchy hierarchy, uint itemid, int recursionLevel, bool isSolution, bool visibleOnly, bool nothrow, ProcessHierarchyNode processNode)
        {
            int hr;
            IntPtr nestedHierarchyObj;
            uint nestedItemId;
            Guid hierGuid = typeof(IVsHierarchy).GUID;

            hr = hierarchy.GetNestedHierarchy(itemid, ref hierGuid, out nestedHierarchyObj, out nestedItemId);
            if (hr == VSConstants.S_OK && nestedHierarchyObj != IntPtr.Zero)
            {
                IVsHierarchy nestedHierarchy = Marshal.GetObjectForIUnknown(nestedHierarchyObj) as IVsHierarchy;
                Marshal.Release(nestedHierarchyObj);
                if (nestedHierarchy != null)
                {
                    EnumHierarchyItems(nestedHierarchy, nestedItemId, recursionLevel, false, visibleOnly, nothrow, processNode);
                }
            }
            else
            {
                object pVar;

                processNode(hierarchy, itemid, recursionLevel);

                recursionLevel++;

                hr = hierarchy.GetProperty(itemid, ((visibleOnly || (isSolution && recursionLevel == 1)) ? (int)__VSHPROPID.VSHPROPID_FirstVisibleChild : (int)__VSHPROPID.VSHPROPID_FirstChild), out pVar);
                if (!nothrow)
                    ErrorHandler.ThrowOnFailure(hr);

                if (hr == VSConstants.S_OK)
                {
                    uint childId = GetItemId(pVar);
                    while (childId != VSConstants.VSITEMID_NIL)
                    {
                        EnumHierarchyItems(hierarchy, childId, recursionLevel, false, visibleOnly, nothrow, processNode);

                        hr = hierarchy.GetProperty(childId, ((visibleOnly || (isSolution && recursionLevel == 1)) ? (int)__VSHPROPID.VSHPROPID_NextVisibleSibling : (int)__VSHPROPID.VSHPROPID_NextSibling), out pVar);
                        if (!nothrow)
                            ErrorHandler.ThrowOnFailure(hr);

                        if (hr == VSConstants.S_OK)
                        {
                            childId = GetItemId(pVar);
                        }
                        else
                        {
                            childId = VSConstants.VSITEMID_NIL;
                        }
                    }
                }
            }
        }

        private static uint GetItemId(object pvar)
        {
            if (pvar == null)
                return VSConstants.VSITEMID_NIL;
            if (pvar is int)
                return (uint)(int)pvar;
            if (pvar is uint)
                return (uint)pvar;
            if (pvar is short)
                return (uint)(short)pvar;
            if (pvar is ushort)
                return (uint)(ushort)pvar;
            if (pvar is long)
                return (uint)(long)pvar;
            return VSConstants.VSITEMID_NIL;
        }

        private void UpdateFiles()
        {
            try
            {
                IVsSolution solution = (IVsSolution)_provider.ServiceProvider.GetService<SVsSolution>();
                if (solution == null)
                    return;

                List<ProjectItemInfo> items = new List<ProjectItemInfo>();
                foreach (IVsHierarchy projectHierarchy in GetProjectHierarchies(solution))
                {
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
                                            filename = System.IO.Path.GetFileName(filename),
                                            project = projectName,
                                            path = filename
                                        };
                                        _items.Add(itemInfo);
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

                //var extensions = proj
            }
            catch
            {
            }
        }

        private void UpdateList()
        {
        }
#endif

        public class ProjectItemInfo
        {
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
                    Func<string, ImageSource> valueFactory = null;
                    return _fileIcons.GetOrAdd(System.IO.Path.GetExtension(FileName), valueFactory);
                }
            }
        }
    }
}
