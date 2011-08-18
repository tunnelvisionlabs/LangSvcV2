namespace Tvl.Java.DebugHost
{
    public enum JvmThreadStatus
    {
        Unknown = -1,
        Zombie = 0,
        Running = 1,
        Sleeping = 2,
        Monitor = 3,
        Wait = 4,
        NotStarted = 5,
    }
}
