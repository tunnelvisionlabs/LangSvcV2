namespace Microsoft.VisualStudio.Project
{
    using System;
    using System.Runtime.InteropServices;
    using EnvDTE;
    using EnvDTE80;

    [Guid("90D38BBD-F214-4111-8477-340EB6D043E7")]
    [ComVisible(true)]
    public class SingleFileGeneratorNodeExtenderProvider : IInternalExtenderProvider, IExtenderProvider
    {
        public static readonly string Name = "Single File Generator Node Extender";

        public bool CanExtend(string ExtenderCATID, string ExtenderName, object ExtendeeObject)
        {
            if (ExtenderName != Name)
                return false;

            FileNodeProperties fileNodeProperties = ExtendeeObject as FileNodeProperties;
            if (fileNodeProperties == null)
                return false;

            Guid extenderGuid;
            if (!Guid.TryParse(ExtenderCATID, out extenderGuid) || extenderGuid != typeof(FileNodeProperties).GUID)
                return false;

            FileNode fileNode = fileNodeProperties.Node as FileNode;
            if (fileNode == null)
                return false;

            if (fileNode.CreateSingleFileGenerator() == null)
                return false;

            return true;
        }

        public object GetExtender(string ExtenderCATID, string ExtenderName, object ExtendeeObject, IExtenderSite ExtenderSite, int Cookie)
        {
            SingleFileGeneratorNodeExtenderProperties extender = null;

            if (CanExtend(ExtenderCATID, ExtenderName, ExtendeeObject))
            {
                FileNodeProperties fileNodeProperties = (FileNodeProperties)ExtendeeObject;
                FileNode fileNode = (FileNode)fileNodeProperties.Node;
                extender = new SingleFileGeneratorNodeExtenderProperties(fileNode);
            }

            return extender;
        }

        public object GetExtenderNames(string ExtenderCATID, object ExtendeeObject)
        {
            return new string[] { Name };
        }
    }
}
