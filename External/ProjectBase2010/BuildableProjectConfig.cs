/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
//#define ConfigTrace
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Project
{
    //=============================================================================
    // NOTE: advises on out of proc build execution to maximize
    // future cross-platform targeting capabilities of the VS tools.

    [CLSCompliant(false)]
    [ComVisible(true)]
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Buildable")]
    public class BuildableProjectConfig : IVsBuildableProjectCfg
    {
        #region fields
        ProjectConfig config = null;
        EventSinkCollection callbacks = new EventSinkCollection();
        #endregion

        #region ctors
        public BuildableProjectConfig(ProjectConfig config)
        {
            this.config = config;
        }
        #endregion

        #region IVsBuildableProjectCfg methods

        public virtual int AdviseBuildStatusCallback(IVsBuildStatusCallback callback, out uint cookie)
        {
            CCITracing.TraceCall();

            cookie = callbacks.Add(callback);
            return VSConstants.S_OK;
        }

        public virtual int get_ProjectCfg(out IVsProjectCfg p)
        {
            CCITracing.TraceCall();

            p = config;
            return VSConstants.S_OK;
        }

        public virtual int QueryStartBuild(uint options, int[] supported, int[] ready)
        {
            CCITracing.TraceCall();
            if(supported != null && supported.Length > 0)
                supported[0] = 1;
            if(ready != null && ready.Length > 0)
                ready[0] = (this.config.ProjectManager.BuildInProgress) ? 0 : 1;
            return VSConstants.S_OK;
        }

        public virtual int QueryStartClean(uint options, int[] supported, int[] ready)
        {
            CCITracing.TraceCall();
            config.PrepareBuild(false);
            if(supported != null && supported.Length > 0)
                supported[0] = 1;
            if(ready != null && ready.Length > 0)
                ready[0] = (this.config.ProjectManager.BuildInProgress) ? 0 : 1;
            return VSConstants.S_OK;
        }

        public virtual int QueryStartUpToDateCheck(uint options, int[] supported, int[] ready)
        {
            CCITracing.TraceCall();
            config.PrepareBuild(false);
            if(supported != null && supported.Length > 0)
                supported[0] = 0; // TODO:
            if(ready != null && ready.Length > 0)
                ready[0] = (this.config.ProjectManager.BuildInProgress) ? 0 : 1;
            return VSConstants.S_OK;
        }

        public virtual int QueryStatus(out int done)
        {
            CCITracing.TraceCall();

            done = (this.config.ProjectManager.BuildInProgress) ? 0 : 1;
            return VSConstants.S_OK;
        }

        public virtual int StartBuild(IVsOutputWindowPane pane, uint options)
        {
            CCITracing.TraceCall();
            config.PrepareBuild(false);

            // Current version of MSBuild wish to be called in an STA
            uint flags = VSConstants.VS_BUILDABLEPROJECTCFGOPTS_REBUILD;

            // If we are not asked for a rebuild, then we build the default target (by passing null)
            this.Build(options, pane, ((options & flags) != 0) ? MsBuildTarget.Rebuild : null);

            return VSConstants.S_OK;
        }

        public virtual int StartClean(IVsOutputWindowPane pane, uint options)
        {
            CCITracing.TraceCall();
            config.PrepareBuild(true);
            // Current version of MSBuild wish to be called in an STA
            this.Build(options, pane, MsBuildTarget.Clean);
            return VSConstants.S_OK;
        }

        public virtual int StartUpToDateCheck(IVsOutputWindowPane pane, uint options)
        {
            CCITracing.TraceCall();

            return VSConstants.E_NOTIMPL;
        }

        public virtual int Stop(int fsync)
        {
            CCITracing.TraceCall();

            return VSConstants.S_OK;
        }

        public virtual int UnadviseBuildStatusCallback(uint cookie)
        {
            CCITracing.TraceCall();


            callbacks.RemoveAt(cookie);
            return VSConstants.S_OK;
        }

        public virtual int Wait(uint ms, int fTickWhenMessageQNotEmpty)
        {
            CCITracing.TraceCall();

            return VSConstants.E_NOTIMPL;
        }
        #endregion

        #region helpers

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private bool NotifyBuildBegin()
        {
            int shouldContinue = 1;
            foreach (IVsBuildStatusCallback cb in callbacks)
            {
                try
                {
                    ErrorHandler.ThrowOnFailure(cb.BuildBegin(ref shouldContinue));
                    if (shouldContinue == 0)
                    {
                        return false;
                    }
                }
                catch (Exception e)
                {
                    // If those who ask for status have bugs in their code it should not prevent the build/notification from happening
                    Debug.Fail(String.Format(CultureInfo.CurrentCulture, SR.GetString(SR.BuildEventError, CultureInfo.CurrentUICulture), e.Message));
                }
            }

            return true;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private void NotifyBuildEnd(MSBuildResult result, string buildTarget)
        {
            int success = ((result == MSBuildResult.Successful) ? 1 : 0);

            foreach (IVsBuildStatusCallback cb in callbacks)
            {
                try
                {
                    ErrorHandler.ThrowOnFailure(cb.BuildEnd(success));
                }
                catch (Exception e)
                {
                    // If those who ask for status have bugs in their code it should not prevent the build/notification from happening
                    Debug.Fail(String.Format(CultureInfo.CurrentCulture, SR.GetString(SR.BuildEventError, CultureInfo.CurrentUICulture), e.Message));
                }
                finally
                {
                    // We want to refresh the references if we are building with the Build or Rebuild target or if the project was opened for browsing only.
                    bool shouldRepaintReferences = (buildTarget == null || buildTarget == MsBuildTarget.Build || buildTarget == MsBuildTarget.Rebuild);

                    // Now repaint references if that is needed. 
                    // We hardly rely here on the fact the ResolveAssemblyReferences target has been run as part of the build.
                    // One scenario to think at is when an assembly reference is renamed on disk thus becomming unresolvable, 
                    // but msbuild can actually resolve it.
                    // Another one if the project was opened only for browsing and now the user chooses to build or rebuild.
                    if (shouldRepaintReferences && (result == MSBuildResult.Successful))
                    {
                        this.RefreshReferences();
                    }
                }
            }
        }

        private void Build(uint options, IVsOutputWindowPane output, string target)
        {
            if (!this.NotifyBuildBegin())
            {
                return;
            }

            try
            {
                config.ProjectManager.BuildAsync(options, this.config.ConfigName, this.config.Platform, output, target, (result, buildTarget) => this.NotifyBuildEnd(result, buildTarget));
            }
            catch(Exception e)
            {
                Trace.WriteLine("Exception : " + e.Message);
                ErrorHandler.ThrowOnFailure(output.OutputStringThreadSafe("Unhandled Exception:" + e.Message + "\n"));
                this.NotifyBuildEnd(MSBuildResult.Failed, target);
                throw;
            }
            finally
            {              
                ErrorHandler.ThrowOnFailure(output.FlushToTaskList());               
            }
        }

        /// <summary>
        /// Refreshes references and redraws them correctly.
        /// </summary>
        private void RefreshReferences()
        {
            // Refresh the reference container node for assemblies that could be resolved.
            IReferenceContainer referenceContainer = this.config.ProjectManager.GetReferenceContainer();
            if (referenceContainer == null)
                return;

            foreach(ReferenceNode referenceNode in referenceContainer.EnumReferences())
            {
                referenceNode.RefreshReference();
            }
        }
        #endregion
    }
}
