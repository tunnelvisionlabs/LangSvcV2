using System;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;

[assembly: Guid("C57F0A40-287C-41B0-B21C-E218398F1EEE")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]
[assembly: CLSCompliant(false)]

[assembly: ProvideCodeBase(AssemblyName = "Antlr3.Runtime")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.Core")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Antlr")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Language")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Language.Interfaces")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.OutputWindow.Interfaces")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Shell")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Text")]
[assembly: ProvideCodeBase(AssemblyName = "Tvl.VisualStudio.Text.Interfaces")]
