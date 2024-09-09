using DevExpress.Mvvm;

namespace WpfAppSample.Services
{
    public interface ITabViewModel: IControlViewModel
    {
        string? Header { get; }
        string? Content { get; set; }
    }
}
