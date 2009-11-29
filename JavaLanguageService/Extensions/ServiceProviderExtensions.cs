namespace JavaLanguageService.Extensions
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

    public static class ServiceProviderExtensions
    {
        public static IOleServiceProvider TryGetOleServiceProvider(this IServiceProvider sp)
        {
            return (IOleServiceProvider)sp.GetService(typeof(IOleServiceProvider));
        }

        public static TServiceInterface TryGetGlobalService<TServiceClass, TServiceInterface>(this IOleServiceProvider sp)
            where TServiceInterface : class
        {
            Contract.Requires<NullReferenceException>(sp != null);

            Guid guidService = typeof(TServiceInterface).GUID;
            Guid riid = typeof(TServiceInterface).GUID;
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
