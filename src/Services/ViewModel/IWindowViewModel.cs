namespace DevExpress.Mvvm
{
    public interface IWindowViewModel: IControlViewModel
    {
        object? Title { get; set; }

        ValueTask CloseForcedAsync(bool forceClose = true);
    }
}
