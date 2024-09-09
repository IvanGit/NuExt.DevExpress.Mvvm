using DevExpress.Mvvm;

namespace WpfAppSample.Models
{
    public class MainWindowSettings : BindableSettings
    {
        public int SelectedIndex
        {
            get { return GetProperty(() => SelectedIndex); }
            set { SetProperty(() => SelectedIndex, value); }
        }
    }
}
