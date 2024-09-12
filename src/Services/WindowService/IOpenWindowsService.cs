﻿namespace DevExpress.Mvvm
{
    /// <summary>
    /// Interface providing methods to manage open windows in the application.
    /// Includes methods for registering and unregistering window view models,
    /// as well as a property to retrieve the list of currently registered windows.
    /// </summary>
    public interface IOpenWindowsService : IAsyncDisposable
    {
        /// <summary>
        /// Gets the collection of registered window view models.
        /// </summary>
        IEnumerable<IWindowViewModel> ViewModels { get; }

        /// <summary>
        /// Registers a window view model.
        /// </summary>
        /// <param name="viewModel">The view model to register.</param>
        void Register(IWindowViewModel viewModel);

        /// <summary>
        /// Unregisters a window view model.
        /// </summary>
        /// <param name="viewModel">The view model to unregister.</param>
        void Unregister(IWindowViewModel viewModel);
    }
}
