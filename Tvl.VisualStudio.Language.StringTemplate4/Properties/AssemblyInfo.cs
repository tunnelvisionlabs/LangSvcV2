using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

[assembly: CLSCompliant(false)]

[assembly: Guid("D3A5E10A-C1B1-44E0-A4D5-4B6119BC05CA")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

[assembly: ProvideCodeBase(AssemblyName = "Antlr3.Runtime")]
[assembly: ProvideCodeBase(AssemblyName = "Antlr4.Runtime")]
[assembly: ProvideCodeBase(AssemblyName = "Antlr4.StringTemplate")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.Core")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Antlr")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Language")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Language.Interfaces")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.OutputWindow.Interfaces")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Shell")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Text")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Text.Interfaces")]
