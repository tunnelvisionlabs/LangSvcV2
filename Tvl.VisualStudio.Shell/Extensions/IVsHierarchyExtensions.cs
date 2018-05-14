namespace Tvl.VisualStudio.Shell
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;

    public static class IVsHierarchyExtensions
    {
        public delegate void ProcessHierarchyNode(IVsHierarchy hierarchyNode, uint itemid, int recursionLevel);

        public static object GetExtensibilityObject([NotNull] this IVsHierarchy hierarchy, uint itemId, bool nothrow)
        {
            Requires.NotNull(hierarchy, nameof(hierarchy));

            try
            {
                object obj;

                int hr = hierarchy.GetProperty(itemId, (int)__VSHPROPID.VSHPROPID_ExtObject, out obj);
                if (!nothrow)
                    ErrorHandler.ThrowOnFailure(hr);

                if (ErrorHandler.Succeeded(hr))
                    return obj;
            }
            catch (Exception ex) when (nothrow && !ErrorHandler.IsCriticalException(ex))
            {
            }

            return null;
        }

        public static object GetExtensibilityObject([NotNull] this IVsHierarchy hierarchy, uint itemId)
        {
            Debug.Assert(hierarchy != null);
            return GetExtensibilityObject(hierarchy, itemId, false);
        }

        public static object GetExtensibilityObjectOrDefault([NotNull] this IVsHierarchy hierarchy, uint itemId)
        {
            Debug.Assert(hierarchy != null);
            return GetExtensibilityObject(hierarchy, itemId, true);
        }

        public static object GetExtensibilityObject([NotNull] this IVsHierarchy hierarchy)
        {
            Debug.Assert(hierarchy != null);
            return GetExtensibilityObject(hierarchy, VSConstants.VSITEMID_ROOT, true);
        }

        public static void EnumHierarchyItems([NotNull] this IVsHierarchy hierarchy, uint itemid, int recursionLevel, bool isSolution, bool visibleOnly, [NotNull] ProcessHierarchyNode processNode)
        {
            Debug.Assert(hierarchy != null);
            Debug.Assert(processNode != null);
            EnumHierarchyItems(hierarchy, itemid, recursionLevel, isSolution, visibleOnly, false, processNode);
        }

        public static void EnumHierarchyItems([NotNull] this IVsHierarchy hierarchy, uint itemid, int recursionLevel, bool isSolution, bool visibleOnly, bool nothrow, [NotNull] ProcessHierarchyNode processNode)
        {
            Requires.NotNull(hierarchy, nameof(hierarchy));
            Requires.NotNull(processNode, nameof(processNode));

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
    }
}
