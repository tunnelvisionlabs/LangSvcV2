namespace Tvl.VisualStudio.Language.Antlr3.Project
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Runtime.InteropServices;
    using IExtenderSite = EnvDTE.IExtenderSite;
    using IExtenderProvider = EnvDTE.IExtenderProvider;
    using IInternalExtenderProvider = EnvDTE80.IInternalExtenderProvider;
    using PropertyDescriptorCollection = System.ComponentModel.PropertyDescriptorCollection;
    using PropertyDescriptor = System.ComponentModel.PropertyDescriptor;
    using TypeDescriptor = System.ComponentModel.TypeDescriptor;
    using Microsoft.VisualStudio.Shell.Interop;
    using Microsoft.VisualStudio;

    [Guid("2FBDD105-8744-4642-BBFB-679C4611A442")]
    [ComVisible(true)]
    public class GrammarFileObjectExtenderProvider : IExtenderProvider, IInternalExtenderProvider
    {
        public static readonly string Name = "AntlrGrammarFileObjectExtender";

        #region IExtenderProvider Members

        public bool CanExtend(string ExtenderCATID, string ExtenderName, object ExtendeeObject)
        {
            if (ExtenderName != Name)
                return false;

            try
            {
                PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(ExtendeeObject);
                if (properties == null)
                    return false;

                PropertyDescriptor property = properties["ItemType"];
                if (property == null)
                    return false;

                object value = property.GetValue(ExtendeeObject);
                if (value == null)
                    return false;

                string itemType = value.ToString();
                if (!string.Equals(itemType, "Antlr3", StringComparison.OrdinalIgnoreCase))
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                if (ex.IsCritical())
                    throw;

                return false;
            }
        }

        public object GetExtender(string ExtenderCATID, string ExtenderName, object ExtendeeObject, IExtenderSite ExtenderSite, int Cookie)
        {
            GrammarFileObjectExtenderProperties extender = null;

            if (CanExtend(ExtenderCATID, ExtenderName, ExtendeeObject))
            {
                IVsBrowseObject browseObject = ExtendeeObject as IVsBrowseObject;
                if (browseObject == null)
                    return null;

                IVsHierarchy hierarchy;
                uint itemId;
                if (ErrorHandler.Failed(browseObject.GetProjectItem(out hierarchy, out itemId)))
                    return null;

                IVsBuildPropertyStorage buildPropertyStorage = hierarchy as IVsBuildPropertyStorage;
                if (buildPropertyStorage == null)
                    return null;

                extender = new GrammarFileObjectExtenderProperties(buildPropertyStorage, itemId);
            }

            return extender;
        }

        #endregion

        #region IInternalExtenderProvider Members

        public object GetExtenderNames(string ExtenderCATID, object ExtendeeObject)
        {
            return new string[] { Name };
        }

        #endregion
    }
}
