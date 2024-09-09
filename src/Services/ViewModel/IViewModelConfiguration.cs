namespace DevExpress.Mvvm
{
    public interface IViewModelConfiguration
    {
        bool? IsInDebugModeOverride { get; }

        bool ThrowFinalizerException { get; }

        bool ThrowParentViewModelIsNullException { get; }

        bool ThrowAlreadyDisposedException { get; }

        bool ThrowAlreadyInitializedException { get; }

    }
}
