namespace Tvl.VisualStudio.Language.Java.Project
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using Microsoft.VisualStudio.Project;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;
    using File = System.IO.File;
    using VSCOMPONENTSELECTORDATA = Microsoft.VisualStudio.Shell.Interop.VSCOMPONENTSELECTORDATA;
    using System.Collections.ObjectModel;
    using Path = System.IO.Path;

    [ComVisible(true)]
    public class JavaReferenceContainerNode : ReferenceContainerNode
    {
        private static ReadOnlyCollection<string> _supportedReferenceTypes =
            new ReadOnlyCollection<string>(new string[]
                {
                    ProjectFileConstants.ProjectReference,
                    JavaProjectFileConstants.JarReference,
                    JavaProjectFileConstants.MavenReference,
                });

        public JavaReferenceContainerNode(JavaProjectNode root)
            : base(root)
        {
            Contract.Requires<ArgumentNullException>(root != null, "root");
        }

        protected override ReadOnlyCollection<string> SupportedReferenceTypes
        {
            get
            {
                
                return base.SupportedReferenceTypes;
            }
        }

        protected override ReferenceNode CreateFileComponent(VSCOMPONENTSELECTORDATA selectorData, string wrapperTool = null)
        {
            if (File.Exists(selectorData.bstrFile))
            {
                if (string.Equals(Path.GetExtension(selectorData.bstrFile), ".jar", StringComparison.OrdinalIgnoreCase))
                {
                    return CreateJarReferenceNode(selectorData.bstrFile);
                }
                else
                {
                    throw new InvalidOperationException("Cannot add a file reference to a non-jar file.");
                }
            }

            return base.CreateFileComponent(selectorData, wrapperTool);
        }

        protected override ReferenceNode CreateReferenceNode(string referenceType, ProjectElement element)
        {
            switch (referenceType)
            {
            case ProjectFileConstants.ProjectReference:
                return CreateProjectReferenceNode(element);

            case JavaProjectFileConstants.JarReference:
                return CreateJarReferenceNode(element);

            case JavaProjectFileConstants.MavenReference:
                return CreateMavenReferenceNode(element);

            default:
                return null;
            }
        }

        protected virtual ReferenceNode CreateJarReferenceNode(ProjectElement element)
        {
            return new JarReferenceNode(ProjectManager, element);
        }

        protected virtual ReferenceNode CreateMavenReferenceNode(ProjectElement element)
        {
            throw new NotImplementedException();
        }

        protected virtual ReferenceNode CreateJarReferenceNode(string fileName)
        {
            return new JarReferenceNode(ProjectManager, fileName);
        }

        protected virtual ReferenceNode CreateMavenReferenceNode(string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
