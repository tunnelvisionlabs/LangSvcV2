using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Microsoft.Build.Execution;
using Microsoft.Build.Framework;

namespace Microsoft.VisualStudio.Project
{
	partial class DesignTimeAssemblyResolution
	{
		/// <summary>
		/// Accesssor for RAR related properties in the projectInstance.
		/// See ResolveAssemblyReferennce task msdn docs for member descriptions
		/// </summary>
		private class RarInputs
		{
			#region private fields

			// RAR related property/item names etc
			private const string TargetFrameworkDirectory = "TargetFrameworkDirectory";
			private const string RegistrySearchPathFormat = "Registry:{0},{1},{2}{3}";
			private const string FrameworkRegistryBase = "FrameworkRegistryBase";
			private const string TargetFrameworkVersionName = "TargetFrameworkVersion";
			private const string AssemblyFoldersSuffix = "AssemblyFoldersSuffix";
			private const string AssemblyFoldersExConditions = "AssemblyFoldersExConditions";
			private const string AllowedReferenceAssemblyFileExtensions = "AllowedReferenceAssemblyFileExtensions";
			private const string ProcessorArchitecture = "ProcessorArchitecture";
			private const string TargetFrameworkMonikerName = "TargetFrameworkMoniker";
			private const string TargetFrameworkMonikerDisplayNameName = "TargetFrameworkMonikerDisplayName";
			private const string TargetedRuntimeVersionName = "TargetedRuntimeVersion";
			private const string FullFrameworkReferenceAssemblyPaths = "_FullFrameworkReferenceAssemblyPaths";
			private const string TargetFrameworkProfile = "TargetFrameworkProfile";

			private const string ProjectDesignTimeAssemblyResolutionSearchPaths = "ProjectDesignTimeAssemblyResolutionSearchPaths";
			private const string Content = "Content";
			private const string None = "None";
			private const string RARResolvedReferencePath = "ReferencePath";
			private const string IntermediateOutputPath = "IntermediateOutputPath";
			private const string InstalledAssemblySubsetTablesName = "InstalledAssemblySubsetTables";
			private const string IgnoreInstalledAssemblySubsetTables = "IgnoreInstalledAssemblySubsetTables";
			private const string ReferenceInstalledAssemblySubsets = "_ReferenceInstalledAssemblySubsets";
			private const string FullReferenceAssemblyNames = "FullReferenceAssemblyNames";
			private const string LatestTargetFrameworkDirectoriesName = "LatestTargetFrameworkDirectories";
			private const string FullFrameworkAssemblyTablesName = "FullFrameworkAssemblyTables";
			private const string MSBuildProjectDirectory = "MSBuildProjectDirectory";

			#endregion //private fields

			public string[] TargetFrameworkDirectories { get; private set; }
			public string[] AllowedAssemblyExtensions { get; private set; }
			public string TargetProcessorArchitecture { get; private set; }
			public string TargetFrameworkVersion { get; private set; }
			public string TargetFrameworkMoniker { get; private set; }
			public string TargetFrameworkMonikerDisplayName { get; private set; }
			public string TargetedRuntimeVersion { get; private set; }
			public string[] FullFrameworkFolders { get; private set; }
			public string ProfileName { get; private set; }
			public string[] PdtarSearchPaths { get; private set; }
			public string[] CandidateAssemblyFiles { get; private set; }
			public string StateFile { get; private set; }
			public ITaskItem[] InstalledAssemblySubsetTables { get; private set; }
			public bool IgnoreDefaultInstalledAssemblySubsetTables { get; private set; }
			public string[] TargetFrameworkSubsets { get; private set; }
			public string[] FullTargetFrameworkSubsetNames { get; private set; }
			public ITaskItem[] FullFrameworkAssemblyTables { get; private set; }
			public string[] LatestTargetFrameworkDirectories { get; private set; }

			#region constructors
			public RarInputs(ProjectInstance projectInstance)
			{
				// Run through all of the entries we want to extract from the project instance before we discard it to save memory
				TargetFrameworkDirectories = GetTargetFrameworkDirectories(projectInstance);
				AllowedAssemblyExtensions = GetAllowedAssemblyExtensions(projectInstance);
				TargetProcessorArchitecture = GetTargetProcessorArchitecture(projectInstance);
				TargetFrameworkVersion = GetTargetFrameworkVersion(projectInstance);
				TargetFrameworkMoniker = GetTargetFrameworkMoniker(projectInstance);
				TargetFrameworkMonikerDisplayName = GetTargetFrameworkMonikerDisplayName(projectInstance);
				TargetedRuntimeVersion = GetTargetedRuntimeVersion(projectInstance);
				FullFrameworkFolders = GetFullFrameworkFolders(projectInstance);
				LatestTargetFrameworkDirectories = GetLatestTargetFrameworkDirectories(projectInstance);
				FullTargetFrameworkSubsetNames = GetFullTargetFrameworkSubsetNames(projectInstance);
				FullFrameworkAssemblyTables = GetFullFrameworkAssemblyTables(projectInstance);
				IgnoreDefaultInstalledAssemblySubsetTables = GetIgnoreDefaultInstalledAssemblySubsetTables(projectInstance);
				ProfileName = GetProfileName(projectInstance);

				/*               
				 * rar.CandidateAssemblyFiles = rarInputs.CandidateAssemblyFiles;
				   rar.StateFile = rarInputs.StateFile;
				   rar.InstalledAssemblySubsetTables = rarInputs.InstalledAssemblySubsetTables;
				   rar.TargetFrameworkSubsets = rarInputs.TargetFrameworkSubsets;
				 */

				// This set needs to be kept in sync with the set of project instance data that
				// is passed into Rar
				PdtarSearchPaths = GetPdtarSearchPaths(projectInstance);

				CandidateAssemblyFiles = GetCandidateAssemblyFiles(projectInstance);
				StateFile = GetStateFile(projectInstance);
				InstalledAssemblySubsetTables = GetInstalledAssemblySubsetTables(projectInstance);
				TargetFrameworkSubsets = GetTargetFrameworkSubsets(projectInstance);
			}
			#endregion // constructors

			#region public properties

			#region common properties/items

			private string[] GetTargetFrameworkDirectories(ProjectInstance projectInstance)
			{
				if (TargetFrameworkDirectories == null)
				{
					string val = projectInstance.GetPropertyValue(TargetFrameworkDirectory).Trim();

					TargetFrameworkDirectories = val.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
						.Select(s => s.Trim())
						.Where(s => s.Length > 0)
						.ToArray();
				}

				return TargetFrameworkDirectories;
			}

			private static string[] GetAllowedAssemblyExtensions(ProjectInstance projectInstance)
			{
				string[] allowedAssemblyExtensions;

				string val = projectInstance.GetPropertyValue(AllowedReferenceAssemblyFileExtensions).Trim();

				allowedAssemblyExtensions = val.Split(';').Select(s => s.Trim()).ToArray();

				return allowedAssemblyExtensions;
			}

			private static string GetTargetProcessorArchitecture(ProjectInstance projectInstance)
			{
				string val = projectInstance.GetPropertyValue(ProcessorArchitecture).Trim();

				return val;
			}

			private static string GetTargetFrameworkVersion(ProjectInstance projectInstance)
			{
				string val = projectInstance.GetPropertyValue(TargetFrameworkVersionName).Trim();

				return val;
			}

			private static string GetTargetFrameworkMoniker(ProjectInstance projectInstance)
			{
				string val = projectInstance.GetPropertyValue(TargetFrameworkMonikerName).Trim();

				return val;
			}

			private static string GetTargetFrameworkMonikerDisplayName(ProjectInstance projectInstance)
			{
				string val = projectInstance.GetPropertyValue(TargetFrameworkMonikerDisplayNameName).Trim();

				return val;
			}

			private static string GetTargetedRuntimeVersion(ProjectInstance projectInstance)
			{
				string val = projectInstance.GetPropertyValue(TargetedRuntimeVersionName).Trim();

				return val;
			}

			private static string[] GetFullFrameworkFolders(ProjectInstance projectInstance)
			{
				string val = projectInstance.GetPropertyValue(FullFrameworkReferenceAssemblyPaths).Trim();

				string[] _fullFrameworkFolders = val.Split(';').Select(s => s.Trim()).ToArray();

				return _fullFrameworkFolders;
			}

			private static string[] GetLatestTargetFrameworkDirectories(ProjectInstance projectInstance)
			{
				IEnumerable<ITaskItem> taskItems = projectInstance.GetItems(LatestTargetFrameworkDirectoriesName);

				string[] latestTargetFrameworkDirectory = (taskItems.Select((Func<ITaskItem, string>)((item) => { return item.ItemSpec.Trim(); }))).ToArray();

				return latestTargetFrameworkDirectory;
			}

			private static string GetProfileName(ProjectInstance projectInstance)
			{
				string val = projectInstance.GetPropertyValue(TargetFrameworkProfile).Trim();

				return val;
			}
			#endregion //common properties/items

			#region project dtar specific properties/items

			private static string[] GetPdtarSearchPaths(ProjectInstance projectInstance)
			{
				string val = projectInstance.GetPropertyValue(ProjectDesignTimeAssemblyResolutionSearchPaths).Trim();

				string[] _pdtarSearchPaths = val.Split(';').Select(s => s.Trim()).ToArray();

				return _pdtarSearchPaths;
			}

			private static string[] GetCandidateAssemblyFiles(ProjectInstance projectInstance)
			{
				var candidateAssemblyFilesList = new List<ProjectItemInstance>();

				candidateAssemblyFilesList.AddRange(projectInstance.GetItems(Content));
				candidateAssemblyFilesList.AddRange(projectInstance.GetItems(None));
				candidateAssemblyFilesList.AddRange(projectInstance.GetItems(RARResolvedReferencePath));

				string[] candidateAssemblyFiles = candidateAssemblyFilesList.Select((Func<ProjectItemInstance, string>)((item) => { return item.GetMetadataValue("FullPath").Trim(); })).ToArray();

				return candidateAssemblyFiles;
			}

			private static string GetStateFile(ProjectInstance projectInstance)
			{
				string intermediatePath = projectInstance.GetPropertyValue(IntermediateOutputPath).Trim();

				intermediatePath = GetFullPathInProjectContext(projectInstance, intermediatePath);

				string stateFile = Path.Combine(intermediatePath, "DesignTimeResolveAssemblyReferences.cache");

				return stateFile;
			}

			private static ITaskItem[] GetInstalledAssemblySubsetTables(ProjectInstance projectInstance)
			{
				return projectInstance.GetItems(InstalledAssemblySubsetTablesName).ToArray();
			}

			private static bool GetIgnoreDefaultInstalledAssemblySubsetTables(ProjectInstance projectInstance)
			{
				bool ignoreDefaultInstalledAssemblySubsetTables = false;

				string val = projectInstance.GetPropertyValue(IgnoreInstalledAssemblySubsetTables).Trim();

				if (!String.IsNullOrEmpty(val))
				{
					if (val == Boolean.TrueString || val == Boolean.FalseString)
					{
						ignoreDefaultInstalledAssemblySubsetTables = Convert.ToBoolean(val, CultureInfo.InvariantCulture);
					}
				}

				return ignoreDefaultInstalledAssemblySubsetTables;
			}

			private static string[] GetTargetFrameworkSubsets(ProjectInstance projectInstance)
			{
				IEnumerable<ITaskItem> taskItems = projectInstance.GetItems(ReferenceInstalledAssemblySubsets);

				string[] targetFrameworkSubsets = (taskItems.Select((Func<ITaskItem, string>)((item) => { return item.ItemSpec.Trim(); }))).ToArray();

				return targetFrameworkSubsets;
			}

			private static string[] GetFullTargetFrameworkSubsetNames(ProjectInstance projectInstance)
			{
				string val = projectInstance.GetPropertyValue(FullReferenceAssemblyNames).Trim();

				string[] fullTargetFrameworkSubsetNames = val.Split(';').Select(s => s.Trim()).ToArray();

				return fullTargetFrameworkSubsetNames;
			}

			private static ITaskItem[] GetFullFrameworkAssemblyTables(ProjectInstance projectInstance)
			{
				return projectInstance.GetItems(FullFrameworkAssemblyTablesName).ToArray();
			}

			#endregion //project dtar specific properties/items

			#endregion // public properties

			#region private methods
			static string GetFullPathInProjectContext(ProjectInstance projectInstance, string path)
			{
				string fullPath = path;

				if (!Path.IsPathRooted(path))
				{
					string projectDir = projectInstance.GetPropertyValue(MSBuildProjectDirectory).Trim();

					fullPath = Path.Combine(projectDir, path);

					fullPath = Path.GetFullPath(fullPath);
				}

				return fullPath;
			}
			#endregion // private methods
		}
	}
}
