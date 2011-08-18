namespace Tvl.Java.DebugHost
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Collections.ObjectModel;

    public class DisposableObjectCollection<T> : Collection<T>
        where T : IDisposable
    {
    }
}
