namespace Tvl.VisualStudio.Shell
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using JetBrains.Annotations;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

    public static class ServiceProviderExtensions
    {
        [NotNull]
        public static SVsServiceProvider AsVsServiceProvider([NotNull] this IServiceProvider sp)
        {
            Requires.NotNull(sp, nameof(sp));

            return new VsServiceProviderWrapper(sp);
        }

        public static TService GetService<TService>([NotNull] this IServiceProvider sp)
        {
            Debug.Assert(sp != null);

            return GetService<TService, TService>(sp);
        }

        public static TServiceInterface GetService<TServiceClass, TServiceInterface>([NotNull] this IServiceProvider sp)
        {
            Requires.NotNull(sp, nameof(sp));

            return (TServiceInterface)sp.GetService(typeof(TServiceClass));
        }

        public static IOleServiceProvider TryGetOleServiceProvider([NotNull] this IServiceProvider sp)
        {
            Requires.NotNull(sp, nameof(sp));

            return (IOleServiceProvider)sp.GetService(typeof(IOleServiceProvider));
        }

        public static TServiceInterface TryGetGlobalService<TServiceClass, TServiceInterface>([NotNull] this IOleServiceProvider sp)
            where TServiceInterface : class
        {
            Requires.NotNull(sp, nameof(sp));

            Guid guidService = typeof(TServiceClass).GUID;
            Guid riid = typeof(TServiceClass).GUID;
            IntPtr obj = IntPtr.Zero;
            int result = ErrorHandler.CallWithCOMConvention(() => sp.QueryService(ref guidService, ref riid, out obj));
            if (ErrorHandler.Failed(result) || obj == IntPtr.Zero)
                return null;

            try
            {
                TServiceInterface service = (TServiceInterface)Marshal.GetObjectForIUnknown(obj);
                return service;
            }
            finally
            {
                Marshal.Release(obj);
            }
        }
    }
}
