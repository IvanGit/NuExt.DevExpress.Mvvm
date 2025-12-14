using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace DevExpress.Mvvm
{
    partial class ViewModel
    {
        #region Properties

        /// <summary>
        /// Gets the dispatcher associated with the UI thread.
        /// </summary>
        public Dispatcher Dispatcher { get; } = Dispatcher.CurrentDispatcher;

        /// <summary>
        /// Gets the thread on which the current instance was created.
        /// </summary>
        public Thread Thread => Dispatcher.Thread;

        #endregion

        #region Methods

        /// <summary>
        /// Checks if the current thread is the same as the thread on which this instance was created.
        /// </summary>
        /// <returns>True if the current thread is the same as the creation thread; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool CheckAccess()
        {
            return Dispatcher.CheckAccess();
        }

        /// <summary>
        /// Executes the specified delegate synchronously on the thread the <see cref="Dispatcher"/> is associated with.
        /// </summary>
        /// <param name="method">A delegate to a method that takes parameters of the same number and type that are contained in the args parameter.</param>
        /// <param name="args">An array of objects to pass as arguments to the given method. This can be null if no arguments are needed.</param>
        /// <returns>The return value from the delegate being invoked, or null if the delegate has no return value.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="method"/> parameter is null.</exception>
        public object? Invoke(Delegate method, params object?[] args)
        {
            ArgumentNullException.ThrowIfNull(method);

            if (!Dispatcher.CheckAccess())
            {
                return Dispatcher.Invoke(method, args);
            }

            return method.Call(args);
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> synchronously on the thread the <see cref="Dispatcher"/> is associated with.
        /// </summary>
        /// <param name="callback">A delegate to invoke through the dispatcher.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="callback"/> parameter is null.</exception>
        public void Invoke(Action callback)
        {
            ArgumentNullException.ThrowIfNull(callback);

            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(callback);
                return;
            }

            callback();
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> synchronously on the thread that the <see cref="Dispatcher"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The type of the callback return value.</typeparam>
        /// <param name="callback">A <see cref="Func{TResult}"/> delegate to invoke through the dispatcher.</param>
        /// <returns>The return value from the delegate being invoked.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="callback"/> parameter is null.</exception>
        public TResult Invoke<TResult>(Func<TResult> callback)
        {
            ArgumentNullException.ThrowIfNull(callback);

            if (!Dispatcher.CheckAccess())
            {
                return Dispatcher.Invoke(callback);
            }

            return callback();
        }

        /// <summary>
        /// Executes the specified <see cref="Action"/> asynchronously on the thread that the <see cref="Dispatcher"/> is associated with.
        /// </summary>
        /// <param name="callback">An <see cref="Action"/> delegate to invoke through the dispatcher.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="callback"/> parameter is null.</exception>
        public Task InvokeAsync(Action callback)
        {
            ArgumentNullException.ThrowIfNull(callback);

            if (!Dispatcher.CheckAccess())
            {
                return Dispatcher.InvokeAsync(callback).Task;
            }

            callback();
            return Task.CompletedTask;
        }

        /// <summary>
        /// Executes the specified <see cref="Func{TResult}"/> asynchronously on the thread that the <see cref="Dispatcher"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The type of the callback return value.</typeparam>
        /// <param name="callback">A <see cref="Func{TResult}"/> delegate to invoke through the dispatcher.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="callback"/> parameter is null.</exception>
        public Task<TResult> InvokeAsync<TResult>(Func<TResult> callback)
        {
            ArgumentNullException.ThrowIfNull(callback);

            if (!Dispatcher.CheckAccess())
            {
                return Dispatcher.InvokeAsync(callback).Task;
            }

            return Task.FromResult(callback());
        }

        /// <summary>
        /// Executes the specified Func&lt;Task&gt; asynchronously on the thread that the <see cref="Dispatcher"/> is associated with.
        /// </summary>
        /// <param name="callback">A Func&lt;Task&gt; delegate to invoke through the dispatcher.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="callback"/> parameter is null.</exception>
        public Task InvokeAsync(Func<Task> callback)
        {
            ArgumentNullException.ThrowIfNull(callback);

            if (!Dispatcher.CheckAccess())
            {
                return Dispatcher.InvokeAsync(callback).Task.Unwrap();
            }

            try
            {
                Task result = callback();
                return result;
            }
            catch (Exception ex)
            {
                return Task.FromException(ex);
            }
        }

        /// <summary>
        /// Executes the specified Func&lt;Task&lt;TResult&gt;&gt; asynchronously on the thread that the <see cref="Dispatcher"/> is associated with.
        /// </summary>
        /// <typeparam name="TResult">The type of the callback return value.</typeparam>
        /// <param name="callback">A Func&lt;Task&lt;TResult&gt;&gt; delegate to invoke through the dispatcher.</param>
        /// <returns>A Task representing the asynchronous operation.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="callback"/> parameter is null.</exception>
        public Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> callback)
        {
            ArgumentNullException.ThrowIfNull(callback);

            if (!Dispatcher.CheckAccess())
            {
                return Dispatcher.InvokeAsync(callback).Task.Unwrap();
            }

            try
            {
                Task<TResult> result = callback();
                return result;
            }
            catch (Exception ex)
            {
                return Task.FromException<TResult>(ex);
            }
        }

        /// <summary>
        /// Checks if the current thread is the same as the thread on which this instance was created and throws an <see cref="InvalidOperationException"/> if not.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the current thread is not the same as the thread on which this instance was created.</exception>
        void IDispatcherObject.VerifyAccess()
        {
            if (!CheckAccess())
            {
                ThrowVerifyAccess();
            }
        }

        private void ThrowVerifyAccess()
        {
            var message = $"{GetType().FullName} ({DisplayName ?? "Unnamed"}) ({GetHashCode()}): method was called from an invalid thread.";
            Trace.WriteLine(message);
            Debug.Fail(message);
            Throw.InvalidOperationException(message);
        }

        #endregion
    }
}
