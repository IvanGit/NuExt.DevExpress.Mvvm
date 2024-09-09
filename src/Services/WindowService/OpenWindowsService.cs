using DevExpress.Mvvm.UI;
using System.Diagnostics;

namespace DevExpress.Mvvm
{
    /// <summary>
    /// Provides services for managing open windows within the application.
    /// This service maintains a list of currently open window view models and offers functionality to register,
    /// unregister, and force-close all registered windows asynchronously. It ensures thread safety using an asynchronous lock.
    /// </summary>
    public sealed class OpenWindowsService : ServiceBase, IOpenWindowsService
    {
        private readonly List<IWindowViewModel> _windows = new();
        private readonly AsyncLock _lock = new();

        /// <summary>
        /// Asynchronously disposes the service, force-closing all registered windows.
        /// </summary>
        /// <returns>A ValueTask representing the asynchronous operation.</returns>
        public async ValueTask DisposeAsync()
        {
            if (_lock.IsDisposed)
            {
                return;
            }
            await _lock.EnterAsync();
            try
            {
                List<Exception>? exceptions = null;
                for (int i = _windows.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        await _windows[i].CloseForcedAsync();
                    }
                    catch (Exception ex)
                    {
                        exceptions ??= new List<Exception>();
                        exceptions.Add(ex);
                    }
                }
                if (exceptions is not null)
                {
                    throw new AggregateException(exceptions);
                }
                Debug.Assert(_windows.Count == 0);
                _windows.Clear();
            }
            finally
            {
                _lock.Exit();
            }
            _lock.Dispose();
        }

        /// <summary>
        /// Registers a window view model with the service.
        /// </summary>
        /// <param name="viewModel">The window view model to register.</param>
        public void Register(IWindowViewModel viewModel)
        {
            _lock.Enter();
            try
            {
                _windows.Add(viewModel);
            }
            finally
            {
                _lock.Exit();
            }
        }

        /// <summary>
        /// Unregisters a window view model from the service.
        /// </summary>
        /// <param name="viewModel">The window view model to unregister.</param>
        public void Unregister(IWindowViewModel viewModel)
        {
            _lock.Enter();
            try
            {
                bool flag = _windows.Remove(viewModel);
                Debug.Assert(flag);
            }
            finally
            {
                _lock.Exit();
            }
        }
    }
}
