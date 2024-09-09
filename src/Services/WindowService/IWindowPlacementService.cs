using System.IO;

namespace DevExpress.Mvvm
{
    public interface IWindowPlacementService
    {
        event ErrorEventHandler? Error;
        event EventHandler? Restored;
        event EventHandler? Saved;

        void SavePlacement();
    }
}
