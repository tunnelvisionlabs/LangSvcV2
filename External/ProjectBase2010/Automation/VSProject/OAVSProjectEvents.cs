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
    using System.Runtime.InteropServices;
    using VSLangProj;
    using VSLangProj80;

    /// <summary>
    /// Provides access to language-specific project events
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "OAVS")]
    [CLSCompliant(false)]
    [ComVisible(true)]
    public class OAVSProjectEvents : VSProjectEvents, VSProjectEvents2
    {
        private readonly OAVSProject _vsProject;

        public OAVSProjectEvents(OAVSProject vsProject)
        {
            Contract.Requires<ArgumentNullException>(vsProject != null, "vsProject");
            this._vsProject = vsProject;
        }

        #region VSProjectEvents Members

        public virtual BuildManagerEvents BuildManagerEvents
        {
            get
            {
                return _vsProject.BuildManager as BuildManagerEvents;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
        public virtual ImportsEvents ImportsEvents
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public virtual ReferencesEvents ReferencesEvents
        {
            get
            {
                // this can't return null or a NullReferenceException in Microsoft.VisualStudio.Xaml will take down the IDE (VS2010)
                ReferencesEvents events = _vsProject.References as ReferencesEvents;
                return events ?? EmptyReferencesEvents.Instance;
            }
        }

        #endregion

        #region VSProjectEvents2 Members

        public virtual VSLangProjWebReferencesEvents VSLangProjWebReferencesEvents
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
