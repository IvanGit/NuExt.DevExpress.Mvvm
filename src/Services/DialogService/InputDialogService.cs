﻿using System.Windows;

namespace DevExpress.Mvvm.UI
{
    /// <summary>
    /// Provides asynchronous methods to show and manage modal dialogs.
    /// Extends ViewServiceBase and implements IAsyncDialogService interface.
    /// </summary>
    public class InputDialogService : ViewServiceBase, IAsyncDialogService
    {
        #region Methods

        /// <summary>
        /// Attempts to retrieve the Window associated with the current object.
        /// </summary>
        /// <returns>
        /// The Window instance associated with the current object if available; otherwise, null.
        /// </returns>
        protected Window? GetWindow()
        {
            return AssociatedObject != null ? AssociatedObject as Window ?? Window.GetWindow(AssociatedObject) : null;
        }

        /// <summary>
        /// Displays a dialog asynchronously with the specified parameters.
        /// </summary>
        /// <param name="dialogCommands">A collection of UICommand objects representing the buttons available in the dialog.</param>
        /// <param name="title">The title of the dialog.</param>
        /// <param name="documentType">The type of the view to display within the dialog.</param>
        /// <param name="viewModel">The ViewModel associated with the view.</param>
        /// <param name="parameter">Additional parameters for initializing the view.</param>
        /// <param name="parentViewModel">The parent ViewModel for context.</param>
        /// <param name="cancellationToken">A token to cancel the dialog operation if needed.</param>
        /// <returns>A ValueTask&lt;UICommand?&gt; representing the command selected by the user.</returns>
        public ValueTask<UICommand?> ShowDialogAsync(IEnumerable<UICommand> dialogCommands, string? title, string? documentType, object? viewModel, object? parameter, object? parentViewModel, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var view = CreateAndInitializeView(documentType, viewModel, parameter, parentViewModel);

            var dialog = new InputDialog
            {
                CommandsSource = dialogCommands,
                Content = view,
                Owner = GetWindow(),
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            if (string.IsNullOrEmpty(title))
            {
                DocumentUIServiceBase.SetTitleBinding(dialog.Content, Window.TitleProperty, dialog, true);
            }
            else
            {
                dialog.Title = title;
            }

            return new ValueTask<UICommand?>(dialog.ShowDialog(cancellationToken));
        }

        #endregion
    }
}
