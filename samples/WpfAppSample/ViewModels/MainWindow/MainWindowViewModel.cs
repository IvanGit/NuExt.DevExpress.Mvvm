using DevExpress.Mvvm;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using WpfAppSample.Interfaces.Services;
using WpfAppSample.Interfaces.ViewModels;
using WpfAppSample.Models;
using WpfAppSample.Views;

namespace WpfAppSample.ViewModels
{
    internal sealed partial class MainWindowViewModel : WindowViewModel, IMainWindowViewModel
    {
        #region Properties

        public IAsyncDocument? ActiveDocument
        {
            get => GetProperty(() => ActiveDocument);
            set { SetProperty(() => ActiveDocument, value, OnActiveDocumentChanged); }
        }

        public ObservableCollection<IMenuItemViewModel> MenuItems { get; } = new();

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

        public async ValueTask CloseMovieAsync(MovieModel movie, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var doc = DocumentManagerService!.FindDocumentById(new MovieDocument(movie));
            if (doc == null) return;
            await doc.CloseAsync();
        }

        private ValueTask LoadMenuAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            MenuItems.Clear();
            var menuItems = new IMenuItemViewModel[]
            {
                new MenuItemViewModel()
                {
                    Header = "File",
                    SubMenuItems=new ObservableCollection<IMenuItemViewModel?>(new IMenuItemViewModel?[]
                    {
                        new MenuItemViewModel() { Header = "Movies", Command = ShowMoviesCommand },
                        null,
                        new MenuItemViewModel() { Header = "Exit", Command = CloseCommand }
                    })
                },
                new MenuItemViewModel()
                {
                    Header = "View",
                    SubMenuItems=new ObservableCollection<IMenuItemViewModel?>(new IMenuItemViewModel?[]
                    {
                        new MenuItemViewModel() { Header = "Hide Active Document", CommandParameter = false, Command = ShowHideActiveDocumentCommand },
                        new MenuItemViewModel() { Header = "Show Active Document", CommandParameter = true, Command = ShowHideActiveDocumentCommand },
                        new MenuItemViewModel() { Header = "Close Active Document", Command = CloseActiveDocumentCommand }
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
            MessageBoxService?.ShowMessage($"An error has occurred in {callerName}:{Environment.NewLine}{ex.Message}", "Error", MessageButton.OK, MessageIcon.Error);
        }

        protected override async ValueTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            Debug.Assert(EnvironmentService != null, $"{nameof(EnvironmentService)} is null");
            Debug.Assert(MoviesService != null, $"{nameof(MoviesService)} is null");

            cancellationToken.ThrowIfCancellationRequested();
            await LoadMenuAsync(cancellationToken);
        }

        public async ValueTask OpenMovieAsync(MovieModel movie, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var document = await DocumentManagerService!.FindDocumentByIdOrCreateAsync(new MovieDocument(movie), async x =>
            {
                var vm = new MovieViewModel();
                var doc = x.CreateDocument(nameof(MovieView), vm, movie, this);
                doc.DestroyOnClose = true;
                try
                {
                    await vm.InitializeAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    Debug.Assert(ex is OperationCanceledException, ex.Message);
                    //await vm.DisposeAsync();
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
