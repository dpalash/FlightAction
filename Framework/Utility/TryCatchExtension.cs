using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace Framework.Utility
{
    public class TryCatchExtensionResult<TResult>
    {
        public bool RethrowException { get; set; }
        public TResult DefaultResult { get; set; }
        public Action? AdditionalAction { get; set; }
    }

    public static class TryCatchExtension
    {
        public static T IgnoreException<T>(this Func<T> action) where T : class
        {
            try
            {
                return action();
            }
            catch (Exception)
            {
                // ignored
                return null;
            }
        }

        public static void IgnoreException(this Action action)
        {
            try
            {
                action();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static void IgnoreException(this Action code, Action<Exception> error)
        {
            try
            {
                code();
            }
            catch (Exception iox)
            {
                error(iox);
            }
        }

        public static async Task IgnoreExceptionAsync(this Task taskToPerform, bool configureAwait = false)
        {
            try
            {
                if (taskToPerform != null)
                    await taskToPerform.ConfigureAwait(configureAwait);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public static async Task<TResult> IgnoreExceptionAsync<TResult>(this Task<TResult> taskToPerform, bool configureAwait = false)
        {
            TResult result = default(TResult);
            try
            {
                if (taskToPerform != null)
                {
                    result = await taskToPerform.ConfigureAwait(configureAwait);
                    return result;
                }
            }
            catch (Exception)
            {
                // ignored
            }

            return result;
        }

        public static async Task ExecuteAndIgnoreErrorAsync(this Func<Task> actionAsync)
        {
            try
            {
                await actionAsync().ConfigureAwait(false);
            }
            catch (Exception)
            {
                // Intentionally ignored
            }
        }

        public static async Task ExecuteAndHandleErrorAsync(this Func<Task> actionAsync, Func<Exception, bool> errorHandlerAsync)
        {
            try
            {
                await actionAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                bool needsThrow = errorHandlerAsync(ex);
                if (needsThrow)
                {
                    throw;
                }
            }
        }

        public static async Task ExecuteAndHandleErrorAsync(this Func<Task> actionAsync, Func<Exception, bool> errorHandlerAsync, Action finallyBlockActionAsync)
        {
            try
            {
                await actionAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                bool needsThrow = errorHandlerAsync(ex);
                if (needsThrow)
                {
                    throw;
                }
            }
            finally
            {
                finallyBlockActionAsync();
            }
        }

        public static async Task ExecuteAndHandleErrorAsync(this Func<Task> actionAsync, Func<Exception, Task<bool>> errorHandlerAsync)
        {
            ExceptionDispatchInfo capturedException = null;
            try
            {
                await actionAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                capturedException = ExceptionDispatchInfo.Capture(ex);
            }

            if (capturedException != null)
            {
                bool needsThrow = await errorHandlerAsync(capturedException.SourceException).ConfigureAwait(false);
                if (needsThrow)
                {
                    capturedException.Throw();
                }
            }
        }

        public static async Task ExecuteAndHandleErrorAsync(Func<Task> actionAsync, Func<Exception, Task<TryCatchExtensionResult<Task>>> errorHandlerAsync)
        {
            ExceptionDispatchInfo capturedException = null;
            try
            {
                await actionAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                capturedException = ExceptionDispatchInfo.Capture(ex);
            }

            if (capturedException != null)
            {
                var errorResult = await errorHandlerAsync(capturedException.SourceException).ConfigureAwait(false);
                if (errorResult.RethrowException)
                {
                    capturedException.Throw();
                }
            }
        }

        public static async Task<TResult> ExecuteAndHandleErrorAsync<TResult>(this Func<Task<TResult>> actionAsync, Func<Exception, TryCatchExtensionResult<TResult>> errorHandler)
        {
            ExceptionDispatchInfo capturedException;
            try
            {
                var result = await actionAsync().ConfigureAwait(false);
                return result;
            }
            catch (Exception ex)
            {
                capturedException = ExceptionDispatchInfo.Capture(ex);
            }

            var errorResult = errorHandler(capturedException.SourceException);
            errorResult.AdditionalAction?.Invoke();
            if (errorResult.RethrowException)
            {
                capturedException.Throw();
            }

            return errorResult.DefaultResult;
        }
    }
}
