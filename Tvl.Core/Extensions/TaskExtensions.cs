namespace Tvl
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using JetBrains.Annotations;

    public static class TaskExtensions
    {
        [NotNull]
        public static T HandleNonCriticalExceptions<T>([NotNull] this T task)
            where T : Task
        {
            Requires.NotNull(task, nameof(task));

            task.ContinueWith(HandleNonCriticalExceptionsCore, TaskContinuationOptions.OnlyOnFaulted | TaskContinuationOptions.ExecuteSynchronously);
            return task;
        }

        private static void HandleNonCriticalExceptionsCore([NotNull] Task task)
        {
            Requires.NotNull(task, nameof(task));

            AggregateException exception = task.Exception;
            if (HasCriticalException(exception))
                throw exception;
        }

        private static bool HasCriticalException([NotNull] Exception exception)
        {
            Requires.NotNull(exception, nameof(exception));

            AggregateException aggregate = exception as AggregateException;
            if (aggregate != null)
                return aggregate.InnerExceptions != null && aggregate.InnerExceptions.Any(HasCriticalException);

            return exception.IsCritical();
        }
    }
}
