namespace Tvl.Java.DebugHost.Interop
{
    using System;

    [Flags]
    public enum jvmtiClassStatus
    {
        None = 0,
        Verified = 1, // Class bytecodes have been verified  
        Prepared = 2, // Class preparation is complete  
        Initialized = 4, // Class initialization is complete. Static initializer has been run.  
        Error = 8, // Error during initialization makes class unusable  
        Array = 16, // Class is an array. If set, all other bits are zero.  
        Primitive = 32, // Class is a primitive class (for example, java.lang.Integer.TYPE). If set, all other bits are zero.  
    }
}
