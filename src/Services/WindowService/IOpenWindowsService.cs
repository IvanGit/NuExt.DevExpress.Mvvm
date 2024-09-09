namespace DevExpress.Mvvm
{
    public interface IOpenWindowsService : IAsyncDisposable
    {
        void Register(IWindowViewModel viewModel);
        void Unregister(IWindowViewModel viewModel);
    }
}
