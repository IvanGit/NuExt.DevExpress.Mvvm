using DevExpress.Mvvm;

namespace WpfAppSample.Interfaces.ViewModels
{
    public interface IMainWindowViewModel
    {
        IAsyncCommand? CloseMovieCommand { get; }
        IAsyncCommand? OpenMovieCommand { get; }
    }
}
