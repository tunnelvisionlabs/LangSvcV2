/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

/* This file provides a basefunctionallity for IVsCfgProvider2.
   Instead of using the IVsProjectCfgEventsHelper object we have our own little sink and call our own helper methods
   similiar to the interface. But there is no real benefit in inheriting from the interface in the first place. 
   Using the helper object seems to be:  
    a) undocumented
    b) not really wise in the managed world
*/
namespace Microsoft.VisualStudio.Project
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using Microsoft.VisualStudio.Shell.Interop;
    using MSBuild = Microsoft.Build.Evaluation;
    using Microsoft.Build.Construction;
    using System.Diagnostics.Contracts;
    using System.Linq;

    [CLSCompliant(false)]
    [ComVisible(true)]
    public class ConfigProvider : IVsCfgProvider2, IVsProjectCfgProvider, IVsExtensibleObject
    {
        #region fields
        public const string AnyCPUPlatform = "Any CPU";
        public const string x86Platform = "x86";

        private readonly ProjectNode project;
        private EventSinkCollection cfgEventSinks = new EventSinkCollection();
        private List<KeyValuePair<KeyValuePair<string, string>, string>> newCfgProps = new List<KeyValuePair<KeyValuePair<string, string>, string>>();
        private Dictionary<string, ProjectConfig> configurationsList = new Dictionary<string, ProjectConfig>();
        #endregion

        #region Properties
        /// <summary>
        /// The associated project.
        /// </summary>
        protected ProjectNode ProjectManager
        {
            get
            {
                return this.project;
            }
        }
        /// <summary>
        /// If the project system wants to add custom properties to the property group then 
        /// they provide us with this data.
        /// Returns/sets the [(<propName, propCondition>) <propValue>] collection
        /// </summary>
        [SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual List<KeyValuePair<KeyValuePair<string, string>, string>> NewConfigProperties
        {
            get
            {
                return newCfgProps;
            }

            set
            {
                newCfgProps = value;
            }
        }

        #endregion

        #region ctors
        public ConfigProvider(ProjectNode manager)
        {
            Contract.Requires<ArgumentNullException>(manager != null, "manager");

            this.project = manager;
        }
        #endregion

        #region methods

        public virtual string GetConfigurationCondition(string configurationName)
        {
            Contract.Ensures(Contract.Result<string>() != null);

            const string configString = " '$(Configuration)' == '{0}' ";
            return string.Format(CultureInfo.InvariantCulture, configString, configurationName);
        }

        public virtual string GetPlatformCondition(string platformName)
        {
            Contract.Ensures(Contract.Result<string>() != null);

            const string platformString = " '$(Platform)' == '{0}' ";
            return string.Format(CultureInfo.InvariantCulture, platformString, GetPlatformPropertyFromPlatformName(platformName));
        }

        public virtual string GetConfigurationPlatformCondition(string configurationName, string platformName)
        {
            Contract.Ensures(Contract.Result<string>() != null);

            const string configPlatformString = " '$(Configuration)|$(Platform)' == '{0}|{1}' ";
            return string.Format(CultureInfo.InvariantCulture, configPlatformString, configurationName, GetPlatformPropertyFromPlatformName(platformName));
        }

        /// <summary>
        /// Creates new Project Configuartion objects based on the configuration name.
        /// </summary>
        /// <param name="configName">The name of the configuration</param>
        /// <returns>An instance of a ProjectConfig object.</returns>
        protected ProjectConfig GetProjectConfiguration(string configName, string platform)
        {
            Contract.Requires<ArgumentNullException>(configName != null, "configName");
            Contract.Requires<ArgumentNullException>(platform != null, "platform");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(configName));
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(platform));

            string key = string.Format("{0}|{1}", configName, platform);

            // if we already created it, return the cached one
            ProjectConfig requestedConfiguration;
            if (configurationsList.TryGetValue(key, out requestedConfiguration))
                return requestedConfiguration;

            requestedConfiguration = CreateProjectConfiguration(configName, platform);
            configurationsList.Add(key, requestedConfiguration);
            return requestedConfiguration;
        }

        protected virtual ProjectConfig CreateProjectConfiguration(string configName, string platform)
        {
            Contract.Requires<ArgumentNullException>(configName != null, "configName");
            Contract.Requires<ArgumentNullException>(platform != null, "platform");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(configName));
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(platform));

            return new ProjectConfig(this.project, configName, platform);
        }

        #endregion

        #region IVsProjectCfgProvider methods
        /// <summary>
        /// Provides access to the IVsProjectCfg interface implemented on a project's configuration object. 
        /// </summary>
        /// <param name="projectCfgCanonicalName">The canonical name of the configuration to access.</param>
        /// <param name="projectCfg">The IVsProjectCfg interface of the configuration identified by szProjectCfgCanonicalName.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code. </returns>
        public virtual int OpenProjectCfg(string projectCfgCanonicalName, out IVsProjectCfg projectCfg)
        {
            Contract.Assert(!string.IsNullOrEmpty(projectCfgCanonicalName));

            projectCfg = null;

            // Be robust in release
            if(projectCfgCanonicalName == null)
            {
                return VSConstants.E_INVALIDARG;
            }

            Contract.Assert(this.project != null && this.project.BuildProject != null);

            string[] configs = GetPropertiesConditionedOn(ProjectFileConstants.Configuration);
            string[] platforms = GetPlatformsFromProject();

            foreach (string config in configs)
            {
                foreach (string platform in platforms)
                {
                    if (string.Equals(string.Format("{0}|{1}", config, platform), projectCfgCanonicalName, StringComparison.OrdinalIgnoreCase))
                    {
                        projectCfg = this.GetProjectConfiguration(config, platform);
                        if (projectCfg != null)
                            return VSConstants.S_OK;
                        else
                            return VSConstants.E_FAIL;
                    }
                }
            }

            return VSConstants.E_INVALIDARG;
        }

        /// <summary>
        /// Checks whether or not this configuration provider uses independent configurations. 
        /// </summary>
        /// <param name="usesIndependentConfigurations">true if independent configurations are used, false if they are not used. By default returns true.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public virtual int get_UsesIndependentConfigurations(out int usesIndependentConfigurations)
        {
            usesIndependentConfigurations = 1;
            return VSConstants.S_OK;
        }
        #endregion

        #region IVsCfgProvider2 methods
        /// <summary>
        /// Copies an existing configuration name or creates a new one. 
        /// </summary>
        /// <param name="name">The name of the new configuration.</param>
        /// <param name="cloneName">the name of the configuration to copy, or a null reference, indicating that AddCfgsOfCfgName should create a new configuration.</param>
        /// <param name="fPrivate">Flag indicating whether or not the new configuration is private. If fPrivate is set to true, the configuration is private. If set to false, the configuration is public. This flag can be ignored.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code. </returns>
        public virtual int AddCfgsOfCfgName(string name, string cloneName, int fPrivate)
        {
            string[] platforms = GetPlatformsFromProject();
            foreach (var platform in platforms)
            {
                CreateConfiguration(name, platform, cloneName, !string.IsNullOrEmpty(cloneName) ? platform : null);
            }

            NotifyOnCfgNameAdded(name);
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Copies an existing platform name or creates a new one. 
        /// </summary>
        /// <param name="platformName">The name of the new platform.</param>
        /// <param name="clonePlatformName">The name of the platform to copy, or a null reference, indicating that AddCfgsOfPlatformName should create a new platform.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public virtual int AddCfgsOfPlatformName(string platformName, string clonePlatformName)
        {
            string[] configurations = GetPropertiesConditionedOn(ProjectFileConstants.Configuration);
            foreach (var configuration in configurations)
            {
                CreateConfiguration(configuration, platformName, !string.IsNullOrEmpty(clonePlatformName) ? configuration : null, clonePlatformName);
            }

            NotifyOnPlatformNameAdded(platformName);
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Deletes a specified configuration name. 
        /// </summary>
        /// <param name="name">The name of the configuration to be deleted.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code. </returns>
        public virtual int DeleteCfgsOfCfgName(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException();

            // We need to QE/QS the project file
            if(!this.ProjectManager.QueryEditProjectFile(false))
            {
                throw Marshal.GetExceptionForHR(VSConstants.OLE_E_PROMPTSAVECANCELLED);
            }

            // Verify that this config exist
            string[] configs = GetPropertiesConditionedOn(ProjectFileConstants.Configuration);
            if (!configs.Contains(name, StringComparer.OrdinalIgnoreCase))
                return VSConstants.S_OK;

            string[] platforms = GetPlatformsFromProject();

            foreach (string config in configs)
            {
                if (!string.Equals(config, name, StringComparison.OrdinalIgnoreCase))
                    continue;

                foreach (string platform in platforms)
                {
                    // Create condition of config to remove
                    string configCondition = GetConfigurationCondition(config).Trim();
                    string configPlatformCondition = GetConfigurationPlatformCondition(config, platform).Trim();

                    foreach (ProjectPropertyGroupElement element in GetBuildProjectXmlPropertyGroups().ToArray())
                    {
                        if (element.Condition == null)
                            continue;

                        if (string.Equals(element.Condition.Trim(), configCondition, StringComparison.OrdinalIgnoreCase)
                            || string.Equals(element.Condition.Trim(), configPlatformCondition, StringComparison.OrdinalIgnoreCase))
                        {
                            element.Parent.RemoveChild(element);
                        }
                    }
                }
            }

            NotifyOnCfgNameDeleted(name);
            return VSConstants.S_OK;
        }

        protected virtual IEnumerable<ProjectPropertyGroupElement> GetBuildProjectXmlPropertyGroups(bool includeUserBuildProjects = true)
        {
            return project.BuildProject.Xml.PropertyGroups;
        }

        /// <summary>
        /// Deletes a specified platform name. 
        /// </summary>
        /// <param name="platName">The platform name to delete.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public virtual int DeleteCfgsOfPlatformName(string platName)
        {
            if (platName == null)
                throw new ArgumentNullException("platName");
            if (string.IsNullOrEmpty(platName))
                throw new ArgumentException();

            // We need to QE/QS the project file
            if (!this.ProjectManager.QueryEditProjectFile(false))
            {
                throw Marshal.GetExceptionForHR(VSConstants.OLE_E_PROMPTSAVECANCELLED);
            }

            // Verify that this config exist
            string[] platforms = GetPlatformsFromProject();
            if (!platforms.Contains(platName, StringComparer.OrdinalIgnoreCase))
                return VSConstants.S_OK;

            string[] configs = GetPropertiesConditionedOn(ProjectFileConstants.Configuration);

            foreach (string platform in platforms)
            {
                if (!string.Equals(platform, platName, StringComparison.OrdinalIgnoreCase))
                    continue;

                foreach (string config in configs)
                {
                    // Create condition of config to remove
                    string platformCondition = GetPlatformCondition(platform).Trim();
                    string configPlatformCondition = GetConfigurationPlatformCondition(config, platform).Trim();

                    foreach (ProjectPropertyGroupElement element in GetBuildProjectXmlPropertyGroups().ToArray())
                    {
                        if (element.Condition == null)
                            continue;

                        if (string.Equals(element.Condition.Trim(), platformCondition, StringComparison.OrdinalIgnoreCase)
                            || string.Equals(element.Condition.Trim(), configPlatformCondition, StringComparison.OrdinalIgnoreCase))
                        {
                            element.Parent.RemoveChild(element);
                        }
                    }
                }
            }

            NotifyOnPlatformNameDeleted(platName);
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Returns the existing configurations stored in the project file.
        /// </summary>
        /// <param name="celt">Specifies the requested number of property names. If this number is unknown, celt can be zero.</param>
        /// <param name="names">On input, an allocated array to hold the number of configuration property names specified by celt. This parameter can also be a null reference if the celt parameter is zero. 
        /// On output, names contains configuration property names.</param>
        /// <param name="actual">The actual number of property names returned.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public virtual int GetCfgNames(uint celt, string[] names, uint[] actual)
        {
            // get's called twice, once for allocation, then for retrieval            
            int i = 0;

            string[] configList = GetPropertiesConditionedOn(ProjectFileConstants.Configuration);

            if(names != null)
            {
                foreach(string config in configList)
                {
                    names[i++] = config;
                    if(i == celt)
                        break;
                }
            }
            else
                i = configList.Length;

            if(actual != null)
            {
                actual[0] = (uint)i;
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Returns the configuration associated with a specified configuration or platform name. 
        /// </summary>
        /// <param name="name">The name of the configuration to be returned.</param>
        /// <param name="platName">The name of the platform for the configuration to be returned.</param>
        /// <param name="cfg">The implementation of the IVsCfg interface.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public virtual int GetCfgOfName(string name, string platName, out IVsCfg cfg)
        {
            cfg = null;
            cfg = this.GetProjectConfiguration(name, platName);

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Returns a specified configuration property. 
        /// </summary>
        /// <param name="propid">Specifies the property identifier for the property to return. For valid propid values, see __VSCFGPROPID.</param>
        /// <param name="var">The value of the property.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public virtual int GetCfgProviderProperty(int propid, out object var)
        {
            switch ((__VSCFGPROPID)propid)
            {
            // If true, indicates that AddCfgsOfCfgName can be called on this object.
            case __VSCFGPROPID.VSCFGPROPID_SupportsCfgAdd:
                var = true;
                break;

            // If true, indicates that DeleteCfgsOfCfgName can be called on this object.
            case __VSCFGPROPID.VSCFGPROPID_SupportsCfgDelete:
                var = true;
                break;

            // If true, indicates that RenameCfgsOfCfgName can be called on this object.
            case __VSCFGPROPID.VSCFGPROPID_SupportsCfgRename:
                var = true;
                break;

            // If true, indicates that AddCfgsOfPlatformName can be called on this object.
            case __VSCFGPROPID.VSCFGPROPID_SupportsPlatformAdd:
                var = false;
                break;

            // If true, indicates that DeleteCfgsOfPlatformName can be called on this object.
            case __VSCFGPROPID.VSCFGPROPID_SupportsPlatformDelete:
                var = false;
                break;

            // Establishes the basis for automation extenders to make the configuration automation assignment extensible.
            case __VSCFGPROPID.VSCFGPROPID_IntrinsicExtenderCATID:
                var = null;
                return VSConstants.E_NOTIMPL;

            // Configurations will be hidden when this project is the active selected project in the selection context.
            case (__VSCFGPROPID)__VSCFGPROPID2.VSCFGPROPID_HideConfigurations:
                var = false;
                break;

            default:
                var = null;
                return VSConstants.E_NOTIMPL;
            }

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Returns the per-configuration objects for this object. 
        /// </summary>
        /// <param name="celt">Number of configuration objects to be returned or zero, indicating a request for an unknown number of objects.</param>
        /// <param name="a">On input, pointer to an interface array or a null reference. On output, this parameter points to an array of IVsCfg interfaces belonging to the requested configuration objects.</param>
        /// <param name="actual">The number of configuration objects actually returned or a null reference, if this information is not necessary.</param>
        /// <param name="flags">Flags that specify settings for project configurations, or a null reference (Nothing in Visual Basic) if no additional flag settings are required. For valid prgrFlags values, see __VSCFGFLAGS.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public virtual int GetCfgs(uint celt, IVsCfg[] a, uint[] actual, uint[] flags)
        {
            if(flags != null)
                flags[0] = 0;

            int i = 0;
            string[] configList = GetPropertiesConditionedOn(ProjectFileConstants.Configuration);
            string[] platformList = GetPlatformsFromProject();

            if (a != null)
            {
                foreach (string configName in configList)
                {
                    foreach (string platform in platformList)
                    {
                        a[i] = this.GetProjectConfiguration(configName, platform);

                        i++;
                        if (i == celt)
                            break;
                    }

                    if (i == celt)
                        break;
                }
            }
            else
            {
                i = configList.Length * platformList.Length;
            }

            if(actual != null)
                actual[0] = (uint)i;

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Returns one or more platform names. 
        /// </summary>
        /// <param name="celt">Specifies the requested number of platform names. If this number is unknown, celt can be zero.</param>
        /// <param name="names">On input, an allocated array to hold the number of platform names specified by celt. This parameter can also be a null reference if the celt parameter is zero. On output, names contains platform names.</param>
        /// <param name="actual">The actual number of platform names returned.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public virtual int GetPlatformNames(uint celt, string[] names, uint[] actual)
        {
            string[] platforms = this.GetPlatformsFromProject();
            return GetPlatforms(celt, names, actual, platforms);
        }

        /// <summary>
        /// Returns the set of platforms that are installed on the user's machine. 
        /// </summary>
        /// <param name="celt">Specifies the requested number of supported platform names. If this number is unknown, celt can be zero.</param>
        /// <param name="names">On input, an allocated array to hold the number of names specified by celt. This parameter can also be a null reference (Nothing in Visual Basic)if the celt parameter is zero. On output, names contains the names of supported platforms</param>
        /// <param name="actual">The actual number of platform names returned.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public virtual int GetSupportedPlatformNames(uint celt, string[] names, uint[] actual)
        {
            string[] platforms = this.GetSupportedPlatformsFromProject();
            return GetPlatforms(celt, names, actual, platforms);
        }

        /// <summary>
        /// Assigns a new name to a configuration. 
        /// </summary>
        /// <param name="old">The old name of the target configuration.</param>
        /// <param name="newname">The new name of the target configuration.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public virtual int RenameCfgsOfCfgName(string old, string newname)
        {
            // First create the condition that represent the configuration we want to rename
            string condition = GetConfigurationCondition(old).Trim();
            string[] platforms = GetPlatformsFromProject();

            foreach (ProjectPropertyGroupElement config in GetBuildProjectXmlPropertyGroups().ToArray())
            {
                // Only care about conditional property groups
                if(config.Condition == null || config.Condition.Length == 0)
                    continue;

                if (string.Equals(config.Condition.Trim(), condition, StringComparison.OrdinalIgnoreCase))
                {
                    // Change the name 
                    config.Condition = GetConfigurationCondition(newname);
                }

                foreach (var platform in platforms)
                {
                    string platformCondition = GetConfigurationPlatformCondition(old, platform).Trim();
                    if (string.Equals(config.Condition.Trim(), platformCondition, StringComparison.OrdinalIgnoreCase))
                    {
                        // Change the name 
                        config.Condition = GetConfigurationPlatformCondition(newname, platform);
                    }
                }
            }

            foreach (var platform in platforms)
            {
                string oldKey = string.Format("{0}|{1}", old, platform);
                string newKey = string.Format("{0}|{1}", newname, platform);

                // Update the name in our config list
                ProjectConfig configuration;
                if (configurationsList.TryGetValue(oldKey, out configuration))
                {
                    configurationsList.Remove(oldKey);
                    configurationsList.Add(newKey, configuration);
                    // notify the configuration of its new name
                    configuration.ConfigName = newname;
                }
            }

            NotifyOnCfgNameRenamed(old, newname);

            return VSConstants.S_OK;
        }

        /// <summary>
        /// Cancels a registration for configuration event notification. 
        /// </summary>
        /// <param name="cookie">The cookie used for registration.</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public virtual int UnadviseCfgProviderEvents(uint cookie)
        {
            this.cfgEventSinks.RemoveAt(cookie);
            return VSConstants.S_OK;
        }

        /// <summary>
        /// Registers the caller for configuration event notification. 
        /// </summary>
        /// <param name="sink">Reference to the IVsCfgProviderEvents interface to be called to provide notification of configuration events.</param>
        /// <param name="cookie">Reference to a token representing the completed registration</param>
        /// <returns>If the method succeeds, it returns S_OK. If it fails, it returns an error code.</returns>
        public virtual int AdviseCfgProviderEvents(IVsCfgProviderEvents sink, out uint cookie)
        {
            cookie = this.cfgEventSinks.Add(sink);
            return VSConstants.S_OK;
        }
        #endregion

        protected virtual int CreateConfiguration(string configurationName, string platformName, string copyFromConfigurationName, string copyFromPlatformName)
        {
            Contract.Requires<ArgumentNullException>(configurationName != null, "configurationName");
            Contract.Requires<ArgumentNullException>(platformName != null, "platformName");
            Contract.Requires<ArgumentException>(string.IsNullOrEmpty(copyFromConfigurationName) || !string.IsNullOrEmpty(copyFromPlatformName));
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(copyFromConfigurationName) || string.IsNullOrEmpty(copyFromPlatformName));

            // We need to QE/QS the project file
            if (!this.ProjectManager.QueryEditProjectFile(false))
            {
                throw Marshal.GetExceptionForHR(VSConstants.OLE_E_PROMPTSAVECANCELLED);
            }

            // First create the condition that represent the configuration we want to clone
            string condition = GetConfigurationPlatformCondition(copyFromConfigurationName, copyFromPlatformName).Trim();

            // Get all configs
            List<ProjectPropertyGroupElement> configGroup = new List<ProjectPropertyGroupElement>(GetBuildProjectXmlPropertyGroups());
            ProjectPropertyGroupElement configToClone = null;

            if (!string.IsNullOrEmpty(copyFromConfigurationName))
            {
                // Find the configuration to clone
                foreach (ProjectPropertyGroupElement currentConfig in configGroup)
                {
                    // Only care about conditional property groups
                    if (string.IsNullOrEmpty(currentConfig.Condition))
                        continue;

                    // Skip if it isn't the group we want
                    if (!string.Equals(currentConfig.Condition.Trim(), condition, StringComparison.OrdinalIgnoreCase))
                        continue;

                    configToClone = currentConfig;
                    break;
                }
            }

            ProjectPropertyGroupElement newConfig = null;
            if (configToClone != null)
            {
                // Clone the configuration settings
                newConfig = this.project.ClonePropertyGroup(configToClone);

                //Will be added later with the new values to the path
                foreach (ProjectPropertyElement property in newConfig.Properties.ToArray())
                {
                    if (property.Name.Equals("OutputPath", StringComparison.OrdinalIgnoreCase))
                    {
                        property.Parent.RemoveChild(property);
                    }
                }
            }
            else
            {
                // no source to clone from, lets just create a new empty config
                newConfig = this.project.BuildProject.Xml.AddPropertyGroup();
                // Get the list of property name, condition value from the config provider
                IList<KeyValuePair<KeyValuePair<string, string>, string>> propVals = this.NewConfigProperties;
                foreach (KeyValuePair<KeyValuePair<string, string>, string> data in propVals)
                {
                    KeyValuePair<string, string> propData = data.Key;
                    string value = data.Value;
                    ProjectPropertyElement newProperty = newConfig.AddProperty(propData.Key, value);
                    if (!string.IsNullOrEmpty(propData.Value))
                        newProperty.Condition = propData.Value;
                }
            }

            //add the output path
            string outputBasePath = this.ProjectManager.OutputBaseRelativePath;
            if (outputBasePath.EndsWith(Path.DirectorySeparatorChar.ToString(), StringComparison.Ordinal))
                outputBasePath = Path.GetDirectoryName(outputBasePath);
            newConfig.AddProperty("OutputPath", Path.Combine(outputBasePath, configurationName) + Path.DirectorySeparatorChar.ToString());

            // Set the condition that will define the new configuration
            string newCondition = GetConfigurationPlatformCondition(configurationName, platformName);
            newConfig.Condition = newCondition;
            return VSConstants.S_OK;
        }

        #region IVsExtensibleObject Members

        /// <summary>
        /// Proved access to an IDispatchable object being a list of configuration properties
        /// </summary>
        /// <param name="configurationName">Combined Name and Platform for the configuration requested</param>
        /// <param name="configurationProperties">The IDispatchcable object</param>
        /// <returns>S_OK if successful</returns>
        public virtual int GetAutomationObject(string configurationName, out object configurationProperties)
        {
            //Init out param
            configurationProperties = null;

            string name, platform;
            if(!ProjectConfig.TrySplitConfigurationCanonicalName(configurationName, out name, out platform))
            {
                return VSConstants.E_INVALIDARG;
            }

            // Get the configuration
            IVsCfg cfg;
            ErrorHandler.ThrowOnFailure(this.GetCfgOfName(name, platform, out cfg));

            // Get the properties of the configuration
            configurationProperties = ((ProjectConfig)cfg).ConfigurationProperties;

            return VSConstants.S_OK;

        }
        #endregion

        #region helper methods
        /// <summary>
        /// Called when a new configuration name was added.
        /// </summary>
        /// <param name="name">The name of configuration just added.</param>
        private void NotifyOnCfgNameAdded(string name)
        {
            foreach(IVsCfgProviderEvents sink in this.cfgEventSinks)
            {
                ErrorHandler.ThrowOnFailure(sink.OnCfgNameAdded(name));
            }
        }

        /// <summary>
        /// Called when a config name was deleted.
        /// </summary>
        /// <param name="name">The name of the configuration.</param>
        private void NotifyOnCfgNameDeleted(string name)
        {
            foreach(IVsCfgProviderEvents sink in this.cfgEventSinks)
            {
                ErrorHandler.ThrowOnFailure(sink.OnCfgNameDeleted(name));
            }
        }

        /// <summary>
        /// Called when a config name was renamed
        /// </summary>
        /// <param name="oldName">Old configuration name</param>
        /// <param name="newName">New configuration name</param>
        private void NotifyOnCfgNameRenamed(string oldName, string newName)
        {
            foreach(IVsCfgProviderEvents sink in this.cfgEventSinks)
            {
                ErrorHandler.ThrowOnFailure(sink.OnCfgNameRenamed(oldName, newName));
            }
        }

        /// <summary>
        /// Called when a platform name was added
        /// </summary>
        /// <param name="platformName">The name of the platform.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void NotifyOnPlatformNameAdded(string platformName)
        {
            foreach(IVsCfgProviderEvents sink in this.cfgEventSinks)
            {
                ErrorHandler.ThrowOnFailure(sink.OnPlatformNameAdded(platformName));
            }
        }

        /// <summary>
        /// Called when a platform name was deleted
        /// </summary>
        /// <param name="platformName">The name of the platform.</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void NotifyOnPlatformNameDeleted(string platformName)
        {
            foreach(IVsCfgProviderEvents sink in this.cfgEventSinks)
            {
                ErrorHandler.ThrowOnFailure(sink.OnPlatformNameDeleted(platformName));
            }
        }

        /// <summary>
        /// Gets all the platforms defined in the project
        /// </summary>
        /// <returns>An array of platform names.</returns>
        protected virtual string[] GetPlatformsFromProject()
        {
            string[] platforms = GetPropertiesConditionedOn(ProjectFileConstants.Platform);

            if (platforms == null || platforms.Length == 0)
            {
                return GetDefaultPlatforms();
            }

            for (int i = 0; i < platforms.Length; i++)
            {
                platforms[i] = GetPlatformNameFromPlatformProperty(platforms[i]);
            }

            return platforms;
        }

        protected virtual string[] GetDefaultPlatforms()
        {
            return new string[] { AnyCPUPlatform };
        }

        /// <summary>
        /// Return the supported platform names.
        /// </summary>
        /// <returns>An array of supported platform names.</returns>
        protected virtual string[] GetSupportedPlatformsFromProject()
        {
            string platformsString = this.ProjectManager.BuildProject.GetPropertyValue(ProjectFileConstants.AvailablePlatforms);

            if (platformsString == null)
            {
                return new string[0];
            }

            string[] platforms = platformsString.Split(',', ';');
            for (int i = 0; i < platforms.Length; i++)
            {
                platforms[i] = GetPlatformNameFromPlatformProperty(platforms[i]);
            }

            return platforms;
        }

        /// <summary>
        /// Helper function to convert AnyCPU to Any CPU.
        /// </summary>
        /// <param name="oldName">The oldname.</param>
        /// <returns>The new name.</returns>
        public virtual string GetPlatformNameFromPlatformProperty(string platformProperty)
        {
            if(String.Equals(platformProperty, ProjectFileValues.AnyCPU, StringComparison.OrdinalIgnoreCase))
            {
                return AnyCPUPlatform;
            }

            return platformProperty;
        }

        /// <summary>
        /// Helper function to convert Any CPU to AnyCPU.
        /// </summary>
        /// <param name="oldName">The oldname.</param>
        /// <returns>The new name.</returns>
        public virtual string GetPlatformPropertyFromPlatformName(string platformName)
        {
            if (String.Equals(platformName, AnyCPUPlatform, StringComparison.OrdinalIgnoreCase))
            {
                return ProjectFileValues.AnyCPU;
            }

            return platformName;
        }

        /// <summary>
        /// Common method for handling platform names.
        /// </summary>
        /// <param name="celt">Specifies the requested number of platform names. If this number is unknown, celt can be zero.</param>
        /// <param name="names">On input, an allocated array to hold the number of platform names specified by celt. This parameter can also be null if the celt parameter is zero. On output, names contains platform names</param>
        /// <param name="actual">A count of the actual number of platform names returned.</param>
        /// <param name="platforms">An array of available platform names</param>
        /// <returns>A count of the actual number of platform names returned.</returns>
        /// <devremark>The platforms array is never null. It is assured by the callers.</devremark>
        private static int GetPlatforms(uint celt, string[] names, uint[] actual, string[] platforms)
        {
            Debug.Assert(platforms != null, "The plaforms array should never be null");
            if(names == null)
            {
                if(actual == null || actual.Length == 0)
                {
                    throw new ArgumentException(SR.GetString(SR.InvalidParameter, CultureInfo.CurrentUICulture), "actual");
                }

                actual[0] = (uint)platforms.Length;
                return VSConstants.S_OK;
            }

            //Degenarate case
            if(celt == 0)
            {
                if(actual != null && actual.Length != 0)
                {
                    actual[0] = (uint)platforms.Length;
                }

                return VSConstants.S_OK;
            }

            uint returned = 0;
            for(int i = 0; i < platforms.Length && names.Length > returned; i++)
            {
                names[returned] = platforms[i];
                returned++;
            }

            if(actual != null && actual.Length != 0)
            {
                actual[0] = returned;
            }

            if(celt > returned)
            {
                return VSConstants.S_FALSE;
            }

            return VSConstants.S_OK;
        }
        #endregion

        /// <summary>
        /// Get all the configurations in the project.
        /// </summary>
        protected virtual string[] GetPropertiesConditionedOn(string constant)
        {
            List<string> configurations = null;
            this.project.BuildProject.ReevaluateIfNecessary();
            this.project.BuildProject.ConditionedProperties.TryGetValue(constant, out configurations);

            return (configurations == null) ? new string[] { } : configurations.ToArray();
        }

    }
}
