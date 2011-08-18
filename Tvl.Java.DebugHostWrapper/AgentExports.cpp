using namespace System;
using namespace System::IO;
//using namespace Tvl::Java::DebugHost;

namespace
{
	System::Reflection::MethodInfo^ GetAgentExportMethod(String^ name)
	{
		String^ assemblyFile = Path::Combine(Path::GetDirectoryName(System::Reflection::Assembly::GetExecutingAssembly()->Location), "Tvl.Java.DebugHost.dll");
		System::Reflection::Assembly^ assembly = System::Reflection::Assembly::LoadFrom(assemblyFile);
		Type^ type = assembly->GetType("Tvl.Java.DebugHost.Interop.AgentExports");
		return type->GetMethod(name, System::Reflection::BindingFlags::Public | System::Reflection::BindingFlags::Static);
	}
}

__declspec(dllexport)
int __cdecl Agent_OnLoad(void* vm, char* options, void* reserved)
{
	System::Reflection::MethodInfo^ method = GetAgentExportMethod("OnLoad");
	array<Object^>^ args = gcnew array<Object^>(3);
	args[0] = IntPtr(vm);
	args[1] = IntPtr(options);
	args[2] = IntPtr(reserved);
	return (int)method->Invoke(nullptr, args);
}

__declspec(dllexport)
int __cdecl Agent_OnAttach(void* vm, char* options, void* reserved)
{
	System::Reflection::MethodInfo^ method = GetAgentExportMethod("OnAttach");
	array<Object^>^ args = gcnew array<Object^>(3);
	args[0] = IntPtr(vm);
	args[1] = IntPtr(options);
	args[2] = IntPtr(reserved);
	return (int)method->Invoke(nullptr, args);
}

__declspec(dllexport)
void __cdecl Agent_OnUnload(void* vm)
{
	System::Reflection::MethodInfo^ method = GetAgentExportMethod("OnUnload");
	array<Object^>^ args = gcnew array<Object^>(1);
	args[0] = IntPtr(vm);
	method->Invoke(nullptr, args);
}
