/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

namespace Microsoft.VisualStudio.Project.Automation
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using VSLangProj;

    public class OABuildManager : ConnectionPointContainer,
                                    IEventSource<_dispBuildManagerEvents>,
                                    BuildManager,
                                    BuildManagerEvents
    {
        private readonly ProjectNode _projectManager;

        public OABuildManager(ProjectNode project)
        {
            Contract.Requires<ArgumentNullException>(project != null, "project");

            _projectManager = project;
            AddEventSource<_dispBuildManagerEvents>(this as IEventSource<_dispBuildManagerEvents>);
        }

        #region BuildManager Members

        public virtual string BuildDesignTimeOutput(string bstrOutputMoniker)
        {
            throw new NotImplementedException();
        }

        public virtual EnvDTE.Project ContainingProject
        {
            get
            {
                return _projectManager.GetAutomationObject() as EnvDTE.Project;
            }
        }

        public virtual EnvDTE.DTE DTE
        {
            get
            {
                return _projectManager.Site.GetService(typeof(EnvDTE.DTE)) as EnvDTE.DTE;
            }
        }

        /// <summary>
        /// Gets the temporary portable executable (PE) monikers for a project.
        /// </summary>
        /// <remarks>
        /// The design-time, output monikers are the names of all the PEs that can be generated from
        /// the project. The project system assigns PE monikers based on the project hierarchy. For
        /// example, if there is an XML Designer file, SomeData.xsd, in Folder1 of Project1 that
        /// generates output, SomeData.cs, then the moniker would be <c>"Project1\Folder1\SomeData.cs"</c>.
        /// Whitespace characters in the project name remain as spaces in the moniker.
        /// </remarks>
        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public virtual object DesignTimeOutputMonikers
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public virtual object Parent
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region _dispBuildManagerEvents_Event Members

        public event _dispBuildManagerEvents_DesignTimeOutputDeletedEventHandler DesignTimeOutputDeleted;

        public event _dispBuildManagerEvents_DesignTimeOutputDirtyEventHandler DesignTimeOutputDirty;

        #endregion

        #region IEventSource<_dispBuildManagerEvents> Members

        void IEventSource<_dispBuildManagerEvents>.OnSinkAdded(_dispBuildManagerEvents sink)
        {
            DesignTimeOutputDeleted += new _dispBuildManagerEvents_DesignTimeOutputDeletedEventHandler(sink.DesignTimeOutputDeleted);
            DesignTimeOutputDirty += new _dispBuildManagerEvents_DesignTimeOutputDirtyEventHandler(sink.DesignTimeOutputDirty);
        }

        void IEventSource<_dispBuildManagerEvents>.OnSinkRemoved(_dispBuildManagerEvents sink)
        {
            DesignTimeOutputDeleted -= new _dispBuildManagerEvents_DesignTimeOutputDeletedEventHandler(sink.DesignTimeOutputDeleted);
            DesignTimeOutputDirty -= new _dispBuildManagerEvents_DesignTimeOutputDirtyEventHandler(sink.DesignTimeOutputDirty);
        }

        #endregion

        protected virtual void OnDesignTimeOutputDeleted(string outputMoniker)
        {
            var handlers = this.DesignTimeOutputDeleted;
            if (handlers != null)
            {
                handlers(outputMoniker);
            }
        }

        protected virtual void OnDesignTimeOutputDirty(string outputMoniker)
        {
            var handlers = this.DesignTimeOutputDirty;
            if (handlers != null)
            {
                handlers(outputMoniker);
            }
        }
    }
}
