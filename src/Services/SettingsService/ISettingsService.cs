using System.IO;

namespace DevExpress.Mvvm
{
    public interface ISettingsService
    {
        event ErrorEventHandler? Error;

        bool LoadSettings(IBindable settings, string name = "Settings");
        bool SaveSettings(IBindable settings, string name = "Settings");
    }
}
