using System.ComponentModel;

namespace DevExpress.Mvvm
{
    public interface IBindable: INotifyPropertyChanged
    {
        bool IsInitialized { get; }

        Type? GetPropertyType(string propertyName);
        void Initialize();
        bool SetProperty(string propertyName, object? value);
    }
}
