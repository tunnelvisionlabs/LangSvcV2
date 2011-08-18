namespace Tvl.Java.DebugHost.Interop
{
    internal enum jvmtiPhase
    {
        Invalid = 0,
        OnLoad = 1,
        Primordial = 2,
        Start = 6,
        Live = 4,
        Dead = 8
    }
}
