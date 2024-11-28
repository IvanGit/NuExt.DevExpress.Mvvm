﻿using ControlzEx.Theming;
using DevExpress.Mvvm;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using WpfAppSample.Views;

namespace WpfAppSample.ViewModels
{
    partial class MainWindowViewModel
    {
        #region Commands

        public ICommand? ActiveDocumentChangedCommand
        {
            get => GetProperty(() => ActiveDocumentChangedCommand);
            private set { SetProperty(() => ActiveDocumentChangedCommand, value); }
        }

        public ICommand? CloseActiveDocumentCommand
        {
            get => GetProperty(() => CloseActiveDocumentCommand);
            private set { SetProperty(() => CloseActiveDocumentCommand, value); }
        }

        public ICommand? ShowHideActiveDocumentCommand
        {
            get => GetProperty(() => ShowHideActiveDocumentCommand);
            private set { SetProperty(() => ShowHideActiveDocumentCommand, value); }
        }

        public ICommand? ShowMoviesCommand
        {
            get => GetProperty(() => ShowMoviesCommand);
            private set { SetProperty(() => ShowMoviesCommand, value); }
        }

        #endregion

        #region Command Methods

        private bool CanCloseActiveDocument()
        {
            return IsUsable && ActiveDocument != null;
        }

        private async Task CloseActiveDocumentAsync()
        {
            await ActiveDocument!.CloseAsync();
        }

        private bool CanShowHideActiveDocument(bool show)
        {
            return IsUsable && ActiveDocument != null;
        }

        private void ShowHideActiveDocument(bool show)
        {
            if (show)
            {
                ActiveDocument?.Show();
            }
            else
            {
                ActiveDocument?.Hide();
            }
        }

        private bool CanShowMovies()
        {
            return IsUsable && DocumentManagerService != null;
        }

        private async Task ShowMoviesAsync()
        {
            var cancellationToken = GetCurrentCancellationToken();

            var document = await DocumentManagerService!.FindDocumentByIdOrCreateAsync(default(Movies),
                async x =>
                {
                    var vm = new MoviesViewModel();
                    var doc = x.CreateDocument(nameof(MoviesView), vm, null, this);
                    doc.DestroyOnClose = true;
                    doc.Title = "Movies";
                    try
                    {
                        await vm.InitializeAsync(cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        Debug.Assert(ex is OperationCanceledException, ex.Message);
                        if (doc is IAsyncDisposable asyncDisposable)
                        {
                            await asyncDisposable.DisposeAsync();
                        }
                        throw;
                    }
                    return doc;
                });
            document.Show();
        }

        #endregion

        #region Methods

        protected override void CreateCommands()
        {
            base.CreateCommands();
            ActiveDocumentChangedCommand = RegisterCommand(UpdateTitle);
            ShowMoviesCommand = RegisterAsyncCommand(ShowMoviesAsync, CanShowMovies);
            ShowHideActiveDocumentCommand = RegisterCommand<bool>(ShowHideActiveDocument, CanShowHideActiveDocument);
            CloseActiveDocumentCommand = RegisterAsyncCommand(CloseActiveDocumentAsync, CanCloseActiveDocument);
        }

        protected override async ValueTask OnContentRenderedAsync(CancellationToken cancellationToken)
        {
            await base.OnContentRenderedAsync(cancellationToken);

            Debug.Assert(CheckAccess());
            cancellationToken.ThrowIfCancellationRequested();

            await LoadMenuAsync(cancellationToken);

            await MoviesService.InitializeAsync(cancellationToken);

            Debug.Assert(Settings!.IsSuspended == false);
            if (Settings.MoviesOpened)
            {
                ShowMoviesCommand?.Execute(null);
            }
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            CreateSettings();
            UpdateTitle();
        }

        #endregion
    }
}
