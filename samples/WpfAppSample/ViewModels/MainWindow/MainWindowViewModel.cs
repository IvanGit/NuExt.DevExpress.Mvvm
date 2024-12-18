﻿using DevExpress.Mvvm;
using MovieWpfApp.Interfaces.Services;
using MovieWpfApp.Interfaces.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace MovieWpfApp.ViewModels
{
    internal sealed partial class MainWindowViewModel : WindowViewModel, IMainWindowViewModel
    {
        #region Properties

        public IAsyncDocument? ActiveDocument
        {
            get => GetProperty(() => ActiveDocument);
            set { SetProperty(() => ActiveDocument, value, OnActiveDocumentChanged); }
        }

        public ObservableCollection<IMenuItemViewModel> MenuItems { get; } = [];

        #endregion

        #region Services

        public IAsyncDocumentManagerService? DocumentManagerService => GetService<IAsyncDocumentManagerService>("Documents");

        public IEnvironmentService EnvironmentService => GetService<IEnvironmentService>()!;

        private IMessageBoxService? MessageBoxService => GetService<IMessageBoxService>();

        private IMoviesService MoviesService => GetService<IMoviesService>()!;

        private ISettingsService? SettingsService => GetService<ISettingsService>();

        #endregion

        #region Event Handlers

        private void OnActiveDocumentChanged(IAsyncDocument? oldActiveDocument)
        {
        }

        #endregion

        #region Methods

        private ValueTask LoadMenuAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            MenuItems.Clear();
            var menuItems = new IMenuItemViewModel[]
            {
                new MenuItemViewModel()
                {
                    Header = Loc.File,
                    SubMenuItems=new ObservableCollection<IMenuItemViewModel?>(new IMenuItemViewModel?[]
                    {
                        new MenuItemViewModel() { Header = Loc.Movies, Command = ShowMoviesCommand },
                        null,
                        new MenuItemViewModel() { Header = Loc.Exit, Command = CloseCommand }
                    })
                },
                new MenuItemViewModel()
                {
                    Header = Loc.View,
                    SubMenuItems=new ObservableCollection<IMenuItemViewModel?>(new IMenuItemViewModel?[]
                    {
                        new MenuItemViewModel() { Header = Loc.Hide_Active_Document, CommandParameter = false, Command = ShowHideActiveDocumentCommand },
                        new MenuItemViewModel() { Header = Loc.Show_Active_Document, CommandParameter = true, Command = ShowHideActiveDocumentCommand },
                        new MenuItemViewModel() { Header = Loc.Close_Active_Document, Command = CloseActiveDocumentCommand }
                    })
                }
            };
            menuItems.ForEach(MenuItems.Add);
            return default;
        }

        protected override async ValueTask OnDisposeAsync()
        {
            var doc = DocumentManagerService?.FindDocumentById(default(Movies));
            Settings!.MoviesOpened = doc is not null;

            await base.OnDisposeAsync();
        }

        protected override void OnError(Exception ex, [CallerMemberName] string? callerName = null)
        {
            base.OnError(ex, callerName);
            MessageBoxService?.ShowMessage(string.Format(Loc.An_error_has_occurred_in_Arg0_Arg1, callerName, ex.Message), Loc.Error, MessageButton.OK, MessageIcon.Error);
        }

        protected override ValueTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            Debug.Assert(DocumentManagerService != null, $"{nameof(DocumentManagerService)} is null");
            Debug.Assert(EnvironmentService != null, $"{nameof(EnvironmentService)} is null");
            Debug.Assert(MessageBoxService != null, $"{nameof(MessageBoxService)} is null");
            Debug.Assert(MoviesService != null, $"{nameof(MoviesService)} is null");
            Debug.Assert(SettingsService != null, $"{nameof(SettingsService)} is null");

            if (DocumentManagerService is IAsyncDisposable asyncDisposable)
            {
                Lifetime.AddAsyncDisposable(asyncDisposable);
            }

            cancellationToken.ThrowIfCancellationRequested();
            return default;
        }

        private void UpdateTitle()
        {
            var sb = new ValueStringBuilder();
            var doc = ActiveDocument;
            if (doc != null)
            {
                sb.Append($"{doc.Title} - ");
            }
            sb.Append($"{AssemblyInfo.Product} v{AssemblyInfo.Version?.ToString(3)}");
            Title = sb.ToString();
        }

        #endregion
    }
}
