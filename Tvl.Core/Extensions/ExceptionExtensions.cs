namespace Tvl
{
    using System;
    using System.Reflection;
    using JetBrains.Annotations;

    public static class ExceptionExtensions
    {
        private static readonly Action<Exception> _internalPreserveStackTrace =
            (Action<Exception>)Delegate.CreateDelegate(
                typeof(Action<Exception>),
                typeof(Exception).GetMethod(
                    "InternalPreserveStackTrace",
                    BindingFlags.Instance | BindingFlags.NonPublic));

#pragma warning disable 618
        public static bool IsCritical([NotNull] this Exception e)
        {
            Requires.NotNull(e, nameof(e));

            if (e is AccessViolationException
                || e is StackOverflowException
                || e is ExecutionEngineException
                || e is OutOfMemoryException
                || e is BadImageFormatException
                || e is AppDomainUnloadedException)
            {
                return true;
            }

            return false;
        }
#pragma warning restore 618

        public static void PreserveStackTrace([NotNull] this Exception e)
        {
            Requires.NotNull(e, nameof(e));

            _internalPreserveStackTrace(e);
        }
    }
}
