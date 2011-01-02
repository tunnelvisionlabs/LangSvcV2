namespace Tvl.VisualStudio.Shell.Extensions
{
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio;
    using IOleServiceProvider = Microsoft.VisualStudio.OLE.Interop.IServiceProvider;

    public static class ServiceProviderExtensions
    {
        public static TService GetService<TService>(this IServiceProvider sp)
        {
            if (sp == null)
                throw new ArgumentNullException("sp");
            Contract.EndContractBlock();

            return (TService)sp.GetService(typeof(TService));
        }

        public static IOleServiceProvider TryGetOleServiceProvider(this IServiceProvider sp)
        {
            if (sp == null)
                throw new ArgumentNullException("sp");
            Contract.EndContractBlock();

            return (IOleServiceProvider)sp.GetService(typeof(IOleServiceProvider));
        }

        public static TServiceInterface TryGetGlobalService<TServiceClass, TServiceInterface>(this IOleServiceProvider sp)
            where TServiceInterface : class
        {
            if (sp == null)
                throw new ArgumentNullException("sp");
            Contract.EndContractBlock();

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
