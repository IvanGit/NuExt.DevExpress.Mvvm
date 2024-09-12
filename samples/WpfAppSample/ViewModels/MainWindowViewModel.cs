using DevExpress.Mvvm;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Windows.Input;
using WpfAppSample.Models;
using WpfAppSample.Services;

namespace WpfAppSample.ViewModels
{
    internal class MainWindowViewModel : WindowViewModel
    {

        #region Properties

        public ITabViewModel? ActiveTab
        {
            get => GetProperty(() => ActiveTab);
            set { SetProperty(() => ActiveTab, value, OnActiveTabChanged); }
        }

        public MainWindowSettings Settings
        {
            get { return GetProperty(() => Settings); }
            set { SetProperty(() => Settings, value); }
        }

        public ObservableCollection<ITabViewModel> Tabs { get; } = new();

        #endregion

        #region Commands

        public ICommand ForceCloseCommand
        {
            get { return GetProperty(() => ForceCloseCommand); }
            set { SetProperty(() => ForceCloseCommand, value); }
        }

        #endregion

        #region Services

        private ISettingsService? SettingsService => GetService<ISettingsService>();

        #endregion

        #region Event Handlers

        private void OnActiveTabChanged()
        {
            UpdateTitle();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();
            LoadSettings();
        }

        #endregion

        #region Methods

        private void CreateSettings()
        {
            Settings = new MainWindowSettings()
            {
            };
            Settings.Initialize();
            Settings.SuspendChanges();
        }

        private bool LoadSettings()
        {
            Debug.Assert(IsInitialized);
            Debug.Assert(SettingsService != null, $"{nameof(SettingsService)} is null");
            if (Settings.IsSuspended)
            {
                Settings.ResumeChanges();
                Debug.Assert(Settings.IsSuspended == false);
                using (Settings.SuspendDirty())
                {
                    return SettingsService?.LoadSettings(Settings) == true;
                }
            }
            return false;
        }

        protected override async ValueTask OnDisposeAsync()
        {
            SaveSettings();
            await Tabs.DisposeAndClearAsync(true);
            await base.OnDisposeAsync();
        }

        protected override void OnInitializeInRuntime()
        {
            base.OnInitializeInRuntime();
            UpdateTitle();

            CreateSettings();

            ForceCloseCommand = RegisterCommand(async () =>
            {
                await OpenWindowsService!.DisposeAsync();
            });
        }

        protected override async ValueTask OnInitializeAsync(CancellationToken cancellationToken)
        {
            Debug.Assert(EnvironmentService != null, $"{nameof(EnvironmentService)} is null");
            for (int i = 0; i < 5; i++)
            {
                Tabs.Add(new TabViewModel() { DisplayName = $"Tab {i + 1}", Header = $"Tab {i + 1}" });
            }
            Tabs.ForEach(tab => tab.SetParentViewModel(this));
            await Task.WhenAll(Tabs.Select(tab => tab.InitializeAsync(cancellationToken).AsTask()));
        }

        private void SaveSettings()
        {
            Debug.Assert(SettingsService != null, $"{nameof(SettingsService)} is null");
            if (Settings.IsDirty)
            {
                if (SettingsService?.SaveSettings(Settings) == true)
                {
                    Settings.ResetDirty();
                }
            }
        }

        private void UpdateTitle()
        {
            var sb = new ValueStringBuilder(260);
            if (ActiveTab != null)
            {
                sb.Append($"{ActiveTab?.DisplayName} - ");
            }
            sb.Append($"{AssemblyInfo.Product} v{AssemblyInfo.Version?.ToString(3)}");
            Title = sb.ToString();
        }

        #endregion
    }
}
