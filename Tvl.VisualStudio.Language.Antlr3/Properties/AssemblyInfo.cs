using System;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Tvl.VisualStudio.Language.Antlr3")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Tunnel Vision Laboratories, LLC")]
[assembly: AssemblyProduct("Tvl.VisualStudio.Language.Antlr3")]
[assembly: AssemblyCopyright("Copyright © Sam Harwell 2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: Guid("F784B181-5267-402F-BF43-6D2807DCEF21")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]
[assembly: CLSCompliant(false)]

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
[assembly: AssemblyVersion("1.4.0.0")]
[assembly: AssemblyFileVersion("1.4.0.0")]
[assembly: AssemblyInformationalVersion("1.4.0")]

[assembly: ProvideCodeBase(CodeBase = "Antlr3.exe")]
[assembly: ProvideCodeBase(AssemblyName = "Antlr3.Runtime")]
[assembly: ProvideCodeBase(AssemblyName = "Antlr4.Runtime")]
[assembly: ProvideCodeBase(AssemblyName = "Antlr4.StringTemplate")]
[assembly: ProvideCodeBase(AssemblyName = "Antlr4.StringTemplate.Visualizer")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.Core")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Antlr")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Language")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Language.Interfaces")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.OutputWindow.Interfaces")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Shell")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Text")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Text.Interfaces")]
