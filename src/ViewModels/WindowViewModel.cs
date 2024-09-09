using System.ComponentModel;
using System.Diagnostics;

namespace DevExpress.Mvvm
{
    /// <summary>
    /// Represents a ViewModel for a window, providing properties and methods for managing the window's state,
    /// services for handling various window-related operations, and commands for interacting with the UI.
    /// </summary>
    public abstract partial class WindowViewModel: ControlViewModel, IWindowViewModel
    {
        #region Properties

        /// <summary>
        /// Gets the <see cref="CancellationTokenSource"/> used for managing cancellation of asynchronous operations.
        /// </summary>
        public CancellationTokenSource CancellationTokenSource { get; } = new();

        /// <summary>
        /// Gets or sets the title of the window.
        /// </summary>
        public object? Title
        {
            get => GetProperty(() => Title);
            set { SetProperty(() => Title, value); }
        }

        #endregion

        #region Services

        /// <summary>
        /// Gets the service responsible for managing the current window.
        /// </summary>
        protected ICurrentWindowService? CurrentWindowService => GetService<ICurrentWindowService>();

        /// <summary>
        /// Gets the service responsible for managing open windows.
        /// </summary>
        protected IOpenWindowsService? OpenWindowsService => GetService<IOpenWindowsService>();

        /// <summary>
        /// Gets the service responsible for managing window placement.
        /// </summary>
        protected IWindowPlacementService? WindowPlacementService => GetService<IWindowPlacementService>();

        #endregion

        #region Methods

        /// <summary>
        /// Closes the window asynchronously, optionally forcing closure.
        /// </summary>
        /// <param name="forceClose">If true, forces the window to close.</param>
        /// <returns>A task representing the asynchronous close operation.</returns>
        public async ValueTask CloseForcedAsync(bool forceClose = true)
        {
            Debug.Assert(CheckAccess());
            Debug.Assert(IsDisposed == false);

            Debug.Assert(CancellationTokenSource.IsCancellationRequested || forceClose);
            if (forceClose)
            {
#if NET8_0_OR_GREATER
                await CancellationTokenSource.CancelAsync();
#else
                CancellationTokenSource.Cancel();
#endif
                WindowPlacementService?.SavePlacement();//TODO check
            }

            try
            {
                await DisposeAsync();
            }
            catch (Exception ex)
            {
                //TODO logging
                if (forceClose == false)
                {
                    MessageBoxService?.ShowMessage(ex.Message, "Error while closing");
                }
            }

            //await Task.Delay(1000);

            Debug.Assert(CheckAccess());
            Debug.Assert(CurrentWindowService != null, $"{nameof(CurrentWindowService)} is null");
            CurrentWindowService?.Close();
            CancellationTokenSource.Dispose();
        }

        /// <summary>
        /// Called when the content of the window is rendered.
        /// Allows for additional initialization or setup that depends on the window's content being ready.
        /// </summary>
        /// <param name="cancellationToken">A token for cancelling the operation.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        protected virtual ValueTask OnContentRenderedAsync(CancellationToken cancellationToken)
        {
            Debug.Assert(CurrentWindowService != null, $"{nameof(CurrentWindowService)} is null");
            Debug.Assert(DispatcherService != null, $"{nameof(DispatcherService)} is null");
            Debug.Assert(EnvironmentService != null, $"{nameof(EnvironmentService)} is null");
            Debug.Assert(MessageBoxService != null, $"{nameof(MessageBoxService)} is null");
            Debug.Assert(OpenWindowsService != null, $"{nameof(OpenWindowsService)} is null");
            //Debug.Assert(SettingsService != null, $"{nameof(SettingsService)} is null");
            Debug.Assert(WindowPlacementService != null, $"{nameof(WindowPlacementService)} is null");

            return default;
        }

        /// <summary>
        /// Initializes the ViewModel at runtime, setting up commands and other necessary components.
        /// </summary>
        protected override void OnInitializeInRuntime()
        {
            base.OnInitializeInRuntime();
            ContentRenderedCommand = RegisterAsyncCommand(ContentRenderedAsync);
            CloseCommand = RegisterCommand(Close, CanClose);
            ClosingCommand = new DelegateCommand<CancelEventArgs>(Closing);
            PlacementRestoredCommand = RegisterCommand(OnPlacementRestored);
            PlacementSavedCommand = RegisterCommand(OnPlacementSaved);
        }

        #endregion
    }
}
