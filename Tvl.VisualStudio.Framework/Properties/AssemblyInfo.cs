using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Tvl.VisualStudio.Framework")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Tunnel Vision Laboratories, LLC")]
[assembly: AssemblyProduct("Tvl.VisualStudio.Framework")]
[assembly: AssemblyCopyright("Copyright © Sam Harwell 2010")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: Guid("8A032C32-C117-4938-87CB-FA01748C8426")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("1.2.0.0")]
[assembly: AssemblyFileVersion("1.2.0.0")]
[assembly: AssemblyInformationalVersion("1.2.0")]

[assembly: ProvideCodeBase(AssemblyName = "Tvl.Core")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Language")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Language.Implementation")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Language.Interfaces")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Shell")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Shell.Implementation")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Shell.Interfaces")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Text")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Text.Implementation")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Text.Interfaces")]

[assembly: ProvideBindingRedirection(
    AssemblyName = "Tvl.Core",
    OldVersionLowerBound = "1.0.2.0",
    OldVersionUpperBound = "1.2.0.0",
    NewVersion = "1.2.0.0")]
[assembly: ProvideBindingRedirection(
    AssemblyName = "Tvl.VisualStudio.Language.Interfaces",
    OldVersionLowerBound = "1.0.2.0",
    OldVersionUpperBound = "1.2.0.0",
    NewVersion = "1.2.0.0")]
[assembly: ProvideBindingRedirection(
    AssemblyName = "Tvl.VisualStudio.Shell.Interfaces",
    OldVersionLowerBound = "1.0.2.0",
    OldVersionUpperBound = "1.2.0.0",
    NewVersion = "1.2.0.0")]
[assembly: ProvideBindingRedirection(
    AssemblyName = "Tvl.VisualStudio.Text.Interfaces",
    OldVersionLowerBound = "1.0.2.0",
    OldVersionUpperBound = "1.2.0.0",
    NewVersion = "1.2.0.0")]
