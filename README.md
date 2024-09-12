# NuExt.DevExpress.Mvvm

`NuExt.DevExpress.Mvvm` is a NuGet package that offers a suite of extensions and utilities for the [DevExpress MVVM Framework](https://github.com/DevExpress/DevExpress.Mvvm.Free). The focus of this package is on **asynchronous operations**, enhancing the core capabilities of the DevExpress MVVM framework and simplifying the implementation of the Model-View-ViewModel (MVVM) pattern in WPF applications. It provides developers with tools to efficiently handle async tasks, improve application responsiveness, and reduce routine work.

### Commonly Used Types

- **`DevExpress.Mvvm.Bindable`**: Base class for creating bindable objects.
- **`DevExpress.Mvvm.ViewModel`**: Base class for ViewModels designed for asynchronous initialization and disposal.
- **`DevExpress.Mvvm.ControlViewModel`**: Base class for control-specific ViewModels.
- **`DevExpress.Mvvm.DocumentContentViewModelBase`**: Base class for ViewModels that represent document content.
- **`DevExpress.Mvvm.WindowViewModel`**: Base class for window-specific ViewModels.
- **`DevExpress.Mvvm.WindowPlacementService`**: Saves and restores window placement between runs.
- **`DevExpress.Mvvm.OpenWindowsService`**: Manages open window ViewModels within the application.
- **`DevExpress.Mvvm.SettingsService`**: Facilitates saving and loading settings.
- **`DevExpress.Mvvm.IAsyncDialogService`**: Displays dialog windows asynchronously.
- **`DevExpress.Mvvm.IAsyncDocument`**: Asynchronous document created with `IAsyncDocumentManagerService`.
- **`DevExpress.Mvvm.IAsyncDocumentManagerService`**: Manages asynchronous documents.
- **`DevExpress.Mvvm.AsyncCommandManager`**: Manages instances of `IAsyncCommand`.

### Usage Examples

For comprehensive examples of how to use the package, refer to the [samples](samples) directory in this repository and the [NuExt.DevExpress.Mvvm.MahApps.Metro](https://github.com/IvanGit/NuExt.DevExpress.Mvvm.MahApps.Metro) repository. These samples illustrate best practices for using DevExpress MVVM with these extensions.

### Contributing

Contributions are welcome! Feel free to submit issues, fork the repository, and send pull requests. Your feedback and suggestions for improvement are highly appreciated.

### License

Licensed under the MIT License. See the LICENSE file for details.