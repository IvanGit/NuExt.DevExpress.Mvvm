using DevExpress.Mvvm;
using DevExpress.Mvvm.UI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Threading;
using WpfAppSample.Services;
using WpfAppSample.ViewModels;
using ILogger = Microsoft.Extensions.Logging.ILogger;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace WpfAppSample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application, IDispatcher
    {
        private readonly CancellationTokenSource _cts = new();
        private readonly bool _createdNew;
        private readonly EventWaitHandle _ewh;
        private readonly Lifetime _lifetime = new();

        public App()
        {
            _ewh = new EventWaitHandle(false, EventResetMode.AutoReset, $"{GetType().FullName}", out _createdNew);
            _lifetime.AddDisposable(_ewh);
            _lifetime.Add(_cts.Cancel);
        }

        #region Properties

        public PerformanceMonitor? PerformanceMonitor { get; private set; }

        public Thread Thread => Dispatcher.Thread;

        #endregion

        #region Event Handlers

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            var logger = ServiceContainer.Default.GetService<ILogger>();
            logger?.LogError(e.Exception, "Dispatcher Unhandled Exception: {Exception}.", e.Exception.Message);
            e.Handled = true;
        }

        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            _lifetime.Dispose();

            var logger = ServiceContainer.Default.GetService<ILogger>();
            var windowsService = ServiceContainer.Default.GetService<IOpenWindowsService>();
            try
            {
                if (windowsService != null)
                {
                    await windowsService.DisposeAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                logger?.LogError(ex, "IOpenWindowsService disposing Exception: {Exception}.", ex.Message);
            }

            logger?.LogInformation("Application exited with code {ExitCode}.", e.ApplicationExitCode);
            LogManager.Shutdown();
        }

        private async void Application_SessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            if (e.ReasonSessionEnding != ReasonSessionEnding.Shutdown)
            {
                return;
            }
            var logger = ServiceContainer.Default.GetService<ILogger>();
            Debug.Assert(logger != null);
            var windowsService = ServiceContainer.Default.GetService<IOpenWindowsService>();
            try
            {
                await windowsService.DisposeAsync();
            }
            catch (Exception ex)
            {
                logger?.LogError(ex, "Application SessionEnding Exception: {Exception}.", ex.Message);
            }
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            if (!_createdNew)
            {
                _ewh.Set();
                Shutdown();
                return;
            }

            PresentationTraceSources.Refresh();
            PresentationTraceSources.DataBindingSource.Switch.Level = SourceLevels.Warning;
            PresentationTraceSources.DataBindingSource.Listeners.Add(new BindingErrorTraceListener());

            var environmentService = new EnvironmentService(AppDomain.CurrentDomain.BaseDirectory, e.Args);
            ServiceContainer.Default.RegisterService(environmentService);
            //https://docs.devexpress.com/WPF/17444/mvvm-framework/services/getting-started
            ServiceContainer.Default.RegisterService(new DispatcherService() { Name = "AppDispatcherService" });
            ServiceContainer.Default.RegisterService(this);

            ConfigureLogging(environmentService);

            var logger = ServiceContainer.Default.GetService<ILogger>();
            Debug.Assert(logger != null);
            logger?.LogInformation("Application started.");

            PerformanceMonitor = new PerformanceMonitor(Process.GetCurrentProcess(), CultureInfo.InvariantCulture)
            {
                ShowManagedMemory = true,
                ShowThreads = true
            };

            var viewModel = new MainWindowViewModel() { };
            try
            {
                await viewModel.SetParentViewModel(this).InitializeAsync(viewModel.CancellationTokenSource.Token);
                new MainWindow { DataContext = viewModel }.Show();
            }
            catch (Exception ex)
            {
                Debug.Assert(false, ex.Message);
                logger?.LogError(ex, "Error while initialization");
                await viewModel.DisposeAsync();
                Shutdown();
                return;
            }

            _ = Task.Run(() => WaitForNotifyAsync(_cts.Token), _cts.Token);
            _ = Task.Run(() => PerformanceMonitor.RunAsync(_cts.Token), _cts.Token);
        }

        #endregion

        #region Methods

        private static void ConfigureLogging(EnvironmentService environmentService)
        {
            Debug.Assert(IOUtils.NormalizedPathEquals(environmentService.BaseDirectory, Directory.GetCurrentDirectory()));
            var configFile = Path.Combine("Config", "nlog.config.json");
            Debug.Assert(File.Exists(configFile), $"Configuration file not found: {configFile}");
            var config = new ConfigurationBuilder()
                .SetBasePath(environmentService.BaseDirectory)
                .AddJsonFile(configFile, optional: false, reloadOnChange: true)
                .Build();

            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.ClearProviders();
#if DEBUG
                builder.SetMinimumLevel(LogLevel.Debug);
#else
                builder.SetMinimumLevel(LogLevel.Information);
#endif
                builder.AddNLog(config);
            });
            LogManager.Configuration.Variables["basedir"] = environmentService.LogsDir;
            ServiceContainer.Default.RegisterService(loggerFactory);

            var logger = loggerFactory.CreateLogger("App");
            Debug.Assert(logger.IsEnabled(LogLevel.Debug));
            ServiceContainer.Default.RegisterService(logger);
        }

        private async Task WaitForNotifyAsync(CancellationToken cancellationToken)
        {
            using var awaiter = new AsyncWaitHandle(_ewh);
            try
            {
                while (await awaiter.WaitOneAsync(cancellationToken))//_ewh is set
                {
                    await Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (cancellationToken.IsCancellationRequested)
                        {
                            return;
                        }
                        Current.MainWindow?.BringToFront();
                    }));
                }
            }
            catch (OperationCanceledException)
            {
                //do nothing
            }
            catch (Exception ex)
            {
                var logger = ServiceContainer.Default.GetService<ILogger>();
                logger?.LogError(ex, "Unexpected error");
            }
        }

        #endregion
    }
}
