using DevExpress.Mvvm;
using System.Diagnostics;
using WpfAppSample.Services;

namespace WpfAppSample.ViewModels
{
    internal class TabViewModel: ControlViewModel, ITabViewModel
    {
        #region Properties
        public string? Content
        {
            get => GetProperty(() => Content);
            set { SetProperty(() => Content, value); }
        }

        public string? Header
        {
            get => GetProperty(() => Header);
            set { SetProperty(() => Header, value); }
        }

        #endregion

        #region Command Methods

        protected override void OnLoaded()
        {
            base.OnLoaded();
            Header = $"{DisplayName} (Loaded)";
        }

        protected override void OnUnloaded()
        {
            Header = $"{DisplayName} (Unloaded)";
            base.OnUnloaded();
        }

        #endregion

        #region Methods

        protected override async ValueTask OnDisposeAsync()
        {
            Debug.Assert(CheckAccess());
            await Task.Delay(100);
            await base.OnDisposeAsync();
        }

        protected override async ValueTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            Debug.Assert(CheckAccess());
            await Task.Delay(100, cancellationToken);
            Content = DisplayName;
        }

        #endregion
    }
}
